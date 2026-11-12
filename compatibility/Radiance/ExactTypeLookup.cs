using System.Reflection;

namespace Radiance.ThorCompat;

public static class ExactTypeLookup
{
    public static bool IsAssignableFrom(Type target, Type candidate)
    {
        try { return target.IsAssignableFrom(candidate); }
        catch (Exception error) when (error is FileNotFoundException or FileLoadException
            or TypeLoadException or ReflectionTypeLoadException)
        {
            // Android can expose a type whose base assembly is resolved only here.
            return false;
        }
    }

    public static Type? Find(string name) => FindInAssemblies(name, AppDomain.CurrentDomain.GetAssemblies());

    public static Type? FindInAssemblies(string name, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            try
            {
                var found = assembly.GetType(name, throwOnError: false);
                if (found != null) return found;
            }
            catch (Exception error) when (error is FileNotFoundException or FileLoadException
                or TypeLoadException or ReflectionTypeLoadException)
            {
                // An unrelated assembly with unavailable dependencies must not stop
                // discovery of an optional integration in another loaded assembly.
            }
        }
        return null;
    }
}
