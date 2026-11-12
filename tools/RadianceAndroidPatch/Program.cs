using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Radiance.ThorCompat;

if (args is ["self-test"])
{
    const string present = "Optional.Integration";
    if (ExactTypeLookup.FindInAssemblies(present, new[] { new LookupAssembly(false) }) != typeof(LookupAssembly)) throw new Exception("Present integration lost");
    if (ExactTypeLookup.FindInAssemblies("Absent", new[] { new LookupAssembly(false) }) != null) throw new Exception("Absent integration invented");
    if (ExactTypeLookup.FindInAssemblies(present, new[] { new LookupAssembly(true), new LookupAssembly(false) }) != typeof(LookupAssembly)) throw new Exception("Broken unrelated assembly blocked lookup");
    if (ExactTypeLookup.FindInAssemblies("Absent", new[] { new LookupAssembly(true), new LookupAssembly(false) }) != null) throw new Exception("Missing integration did not fail closed");
    if (!ExactTypeLookup.IsAssignableFrom(typeof(object), typeof(string))) throw new Exception("Valid subtype lost");
    if (ExactTypeLookup.IsAssignableFrom(typeof(string), typeof(object))) throw new Exception("Unrelated type admitted");
    if (ExactTypeLookup.IsAssignableFrom(new UnresolvedBaseType(), typeof(object))) throw new Exception("Unresolved type admitted");
    Console.WriteLine("PASS safe assignability");
    Console.WriteLine("PASS present, absent, unrelated missing dependency, no type enumeration");
    return;
}
if (args.Length != 3) throw new ArgumentException("Usage: ORIGINAL_ZIP NEW_ZIP PRIVATE_REFERENCE_DIRECTORY or self-test");
var input = Path.GetFullPath(args[0]); var output = Path.GetFullPath(args[1]);
if (File.Exists(output) || input.Equals(output, StringComparison.OrdinalIgnoreCase)) throw new IOException("Output must be new.");
if (Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(input))).ToLowerInvariant() != "7e1e29c2ddc64ca0e13e6243dcac1b22fb62b6be2c2e342f29bc846e3420e3d0") throw new InvalidDataException("Not the exact original Radiance2.4.8 archive.");
using var zip = ZipFile.OpenRead(input);
var dll = zip.Entries.Single(e => e.FullName.EndsWith("/SDV-Radiance.dll"));
string folder = dll.FullName[..^"SDV-Radiance.dll".Length];
using var stream = new MemoryStream(); using (var source = dll.Open()) source.CopyTo(stream); stream.Position = 0;
using var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(Path.GetFullPath(args[2]));
using var module = ModuleDefinition.ReadModule(stream, new ReaderParameters { AssemblyResolver = resolver });
using var helper = ModuleDefinition.ReadModule(typeof(ExactTypeLookup).Assembly.Location);
var replacement = module.ImportReference(helper.GetType("Radiance.ThorCompat.ExactTypeLookup").Methods.Single(m => m.Name == "Find"));
foreach (var (type, method) in new[] { ("SDVRadiance.ScaleUpArt", "Find"), ("SDVRadiance.ShadowSuppression", "LeanDynamicShaderCanopies") })
{
    var target = module.GetType(type).Methods.Single(m => m.Name == method);
    var call = target.Body.Instructions.Single(i => i.Operand is MethodReference m && m.DeclaringType.FullName == "HarmonyLib.AccessTools" && m.Name == "TypeByName");
    call.Operand = replacement;
}
var assignable = module.ImportReference(helper.GetType("Radiance.ThorCompat.ExactTypeLookup").Methods.Single(m => m.Name == "IsAssignableFrom"));
var iterator = module.GetType("SDVRadiance.LocationDrawHook").NestedTypes.Single(t => t.Name.StartsWith("<LocationTypesThatDrawThemselves>"));
var scanMethods = new[] {
    iterator.Methods.Single(m => m.Name == "MoveNext"),
    module.GetType("SDVRadiance.ShadowSuppression").Methods.Single(m => m.Name == "PatchSelfDrawnCharacters"),
    module.GetType("SDVRadiance.HarmonyPatcher").Methods.Single(m => m.Name == "HoldCharactersWhileFrozen")
};
foreach (var scan in scanMethods)
{
    var call = scan.Body.Instructions.Single(i => i.Operand is MethodReference m && m.DeclaringType.FullName == "System.Type" && m.Name == "IsAssignableFrom");
    call.OpCode = OpCodes.Call; call.Operand = assignable;
}
using var patched = new MemoryStream(); module.Write(patched);
var manifestEntry = zip.GetEntry(folder + "manifest.json")!;
using var reader = new StreamReader(manifestEntry.Open()); var manifest = JsonNode.Parse(reader.ReadToEnd())!;
manifest["Version"] = "2.4.8-android.2";
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
using (var result = ZipFile.Open(output, ZipArchiveMode.Create))
{
    foreach (var entry in zip.Entries)
    {
        var next = result.CreateEntry(entry.FullName, CompressionLevel.Optimal); next.LastWriteTime = entry.LastWriteTime;
        using var target = next.Open();
        if (entry.FullName == dll.FullName) target.Write(patched.ToArray());
        else if (entry.FullName == manifestEntry.FullName) { using var writer = new StreamWriter(target); writer.Write(manifest.ToJsonString(new() { WriteIndented = true })); }
        else { using var source = entry.Open(); source.CopyTo(target); }
    }
    var support = result.CreateEntry(folder + "Radiance.ThorCompat.dll", CompressionLevel.Optimal);
    support.LastWriteTime = dll.LastWriteTime;
    using (var supportStream = support.Open()) supportStream.Write(File.ReadAllBytes(typeof(ExactTypeLookup).Assembly.Location));
    var license = result.CreateEntry(folder + "ThorCompatibility-LICENSE", CompressionLevel.Optimal);
    license.LastWriteTime = dll.LastWriteTime;
    using var licenseTarget = license.Open();
    using var licenseSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ThorCompatibility.LICENSE")!;
    licenseSource.CopyTo(licenseTarget);
}
Console.WriteLine(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(output))).ToLowerInvariant() + " " + output);

sealed class LookupAssembly(bool broken) : Assembly
{
    public override Type? GetType(string name, bool throwOnError) => broken ? throw new FileNotFoundException("Unrelated missing dependency") : name == "Optional.Integration" ? typeof(LookupAssembly) : null;
    public override Type[] GetTypes() => throw new Exception("Enumerating unrelated types is forbidden");
}

sealed class UnresolvedBaseType() : TypeDelegator(typeof(object))
{
    public override bool IsAssignableFrom(Type? candidate) => throw new TypeLoadException("Unrelated type base assembly is unavailable");
}
