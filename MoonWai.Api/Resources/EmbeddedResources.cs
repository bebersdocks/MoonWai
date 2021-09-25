using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MoonWai.Api.Resources
{
    public static class EmbeddedResources
    {
        public async static Task<string> GetResourceStr(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            var name = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(i => i.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase))
                ?? throw new FileNotFoundException($"Embedded resource {resourceName} not found in assembly {assembly.FullName}", resourceName);

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}