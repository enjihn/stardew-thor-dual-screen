using System.IO.Compression;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using Mono.Cecil;
using Mono.Cecil.Cil;

const string OriginalZipSha = "a12fe600e338a5ebabcccfd5a3f585dbd0346fa4357f4e4e5aaa6a23a5e8e6d0";
const string MobileMenu = "StardewValley.Menus.MobileCustomizer";
const string PatchType = "FashionSense.Framework.Patches.Menus.CharacterCustomizationPatch";

if (args is ["self-test"] or ["self-test-unpatched"])
{
    bool patch = args[0] == "self-test";
    foreach (bool mobile in new[] { false, true })
    {
        using var module = ModuleDefinition.CreateModule("GuardTest" + mobile + Guid.NewGuid().ToString("N"), ModuleKind.Dll);
        var access = new TypeDefinition("HarmonyLib", "AccessTools", TypeAttributes.Public, module.TypeSystem.Object);
        module.Types.Add(access);
        var lookup = new MethodDefinition("TypeByName", MethodAttributes.Public | MethodAttributes.Static,
            module.ImportReference(typeof(Type)));
        lookup.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));
        access.Methods.Add(lookup);
        var lookupIl = lookup.Body.GetILProcessor();
        lookupIl.Emit(OpCodes.Ldarg_0);
        lookupIl.Emit(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetType", new[] { typeof(string) })!));
        lookupIl.Emit(OpCodes.Ret);
        if (mobile) module.Types.Add(new TypeDefinition("StardewValley.Menus", "MobileCustomizer", TypeAttributes.Public, module.TypeSystem.Object));
        var target = new TypeDefinition("Fixture", "Patch", TypeAttributes.Public, module.TypeSystem.Object);
        module.Types.Add(target);
        var counter = new FieldDefinition("Calls", FieldAttributes.Public | FieldAttributes.Static, module.TypeSystem.Int32);
        target.Fields.Add(counter);
        var apply = new MethodDefinition("Apply", MethodAttributes.Public | MethodAttributes.Static, module.TypeSystem.Void);
        target.Methods.Add(apply);
        var il = apply.Body.GetILProcessor();
        il.Emit(OpCodes.Ldsfld, counter); il.Emit(OpCodes.Ldc_I4_1); il.Emit(OpCodes.Add); il.Emit(OpCodes.Stsfld, counter);
        il.Emit(OpCodes.Ldstr, MobileMenu); il.Emit(OpCodes.Call, lookup);
        var end = il.Create(OpCodes.Ret); il.Emit(OpCodes.Brfalse, end);
        il.Emit(OpCodes.Newobj, module.ImportReference(typeof(MissingMethodException).GetConstructor(Type.EmptyTypes)!));
        il.Emit(OpCodes.Throw); il.Append(end);
        if (patch) AddAndroidGuard(apply, lookup);
        using var bytes = new MemoryStream(); module.Write(bytes); bytes.Position = 0;
        var context = new AssemblyLoadContext(module.Name, isCollectible: true);
        var assembly = context.LoadFromStream(bytes);
        var type = assembly.GetType("Fixture.Patch", throwOnError: true)!;
        type.GetMethod("Apply")!.Invoke(null, null);
        int calls = (int)type.GetField("Calls")!.GetValue(null)!;
        if (calls != (mobile ? 0 : 1)) throw new Exception($"Wrong platform behavior: mobile={mobile}, calls={calls}");
        context.Unload();
        Console.WriteLine($"PASS {(mobile ? "Android bypasses only desktop checkbox hook" : "Desktop still executes original hook")}");
    }
    return;
}
if (args.Length != 2) throw new ArgumentException("Usage: INPUT_ORIGINAL_ZIP OUTPUT_NEW_ZIP, or self-test");
string input = Path.GetFullPath(args[0]), output = Path.GetFullPath(args[1]);
if (File.Exists(output) || input.Equals(output, StringComparison.OrdinalIgnoreCase))
    throw new IOException("Choose a new output path; originals are never overwritten.");
if (Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(input))).ToLowerInvariant() != OriginalZipSha)
    throw new InvalidDataException("Input is not the tested original Fashion Sense7.5.0 archive.");
using var original = ZipFile.OpenRead(input);
var dllEntry = original.GetEntry("FashionSense/FashionSense.dll") ?? throw new InvalidDataException("Missing original DLL.");
using var dllBytes = new MemoryStream(); using (var stream = dllEntry.Open()) stream.CopyTo(dllBytes);
dllBytes.Position = 0;
using var mod = ModuleDefinition.ReadModule(dllBytes);
var method = mod.GetType(PatchType)?.Methods.Single(m => m.Name == "Apply") ?? throw new InvalidDataException("Missing exact patch group.");
var harmonyMethod = method.Body.Instructions.Select(i => i.Operand).OfType<MethodReference>()
    .First(m => m.DeclaringType.FullName == "HarmonyLib.AccessTools" && m.Name == "Method");
if (method.Body.Instructions.Count(i => i.OpCode == OpCodes.Ldstr && Equals(i.Operand, "ResetComponents")) != 1)
    throw new InvalidDataException("Unexpected desktop hook implementation.");
var typeReference = harmonyMethod.Parameters[0].ParameterType;
var typeByName = new MethodReference("TypeByName", typeReference, harmonyMethod.DeclaringType) { HasThis = false };
typeByName.Parameters.Add(new ParameterDefinition(mod.TypeSystem.String));
AddAndroidGuard(method, typeByName);
using var patchedBytes = new MemoryStream(); mod.Write(patchedBytes);
var manifest = JsonNode.Parse(ReadEntry(original.GetEntry("FashionSense/manifest.json")!))!;
manifest["Version"] = "7.5.0-android.1";
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
using (var result = ZipFile.Open(output, ZipArchiveMode.Create))
{
    foreach (var entry in original.Entries)
    {
        var next = result.CreateEntry(entry.FullName, CompressionLevel.Optimal);
        next.LastWriteTime = entry.LastWriteTime;
        using var target = next.Open();
        if (entry.FullName == dllEntry.FullName) target.Write(patchedBytes.ToArray());
        else if (entry.FullName == "FashionSense/manifest.json")
        {
            using var writer = new StreamWriter(target); writer.Write(manifest.ToJsonString(new() { WriteIndented = true }));
        }
        else { using var source = entry.Open(); source.CopyTo(target); }
    }
}
Console.WriteLine(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(output))).ToLowerInvariant() + " " + output);

static string ReadEntry(ZipArchiveEntry entry) { using var reader = new StreamReader(entry.Open()); return reader.ReadToEnd(); }

static void AddAndroidGuard(MethodDefinition method, MethodReference typeByName)
{
    // Android uses MobileCustomizer instead of the desktop character creation UI.
    // Leave the desktop hook intact and skip only this optional checkbox group.
    var il = method.Body.GetILProcessor();
    var originalFirst = method.Body.Instructions[0];
    il.InsertBefore(originalFirst, il.Create(OpCodes.Ldstr, MobileMenu));
    il.InsertBefore(originalFirst, il.Create(OpCodes.Call, typeByName));
    il.InsertBefore(originalFirst, il.Create(OpCodes.Brfalse, originalFirst));
    il.InsertBefore(originalFirst, il.Create(OpCodes.Ret));
    method.Body.MaxStackSize = Math.Max(method.Body.MaxStackSize, 1);
}
