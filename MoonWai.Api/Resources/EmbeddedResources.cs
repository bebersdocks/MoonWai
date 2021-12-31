using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MoonWai.Api.Resources
{
    public static class EmbeddedResources
    {
        public static string GetResourceStr(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            var name = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(s => s.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase))
                ?? throw new FileNotFoundException($"Embedded resource {resourceName} not found in assembly {assembly.FullName}.", resourceName);

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}