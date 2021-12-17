using System;
using System.Collections.Immutable;
using System.Linq;

using MoonWai.Shared.Definitions;

using Newtonsoft.Json;

namespace MoonWai.Api.Resources
{
    public class Translations
    {   
        public LanguageId                           LanguageId { get; init; }
        public ImmutableDictionary<ErrorId, string> Errors     { get; init; }
        
        public string GetErrorMsg(ErrorId errorId, params object[] args)
        {
            var translationStr = Errors[errorId];

            return string.Format(translationStr, args);
        }

        public static Translations Load(LanguageId languageId)
        {
            var languageIdStr = languageId switch
            {
                LanguageId.English => "en",
                _ => "ru"
            };

            var json = EmbeddedResources.GetResourceStr(languageIdStr + ".json");
            var translations = JsonConvert.DeserializeObject<Translations>(json);

            var errorCount = Enum.GetValues<ErrorId>().Count();
            if (translations.Errors.Keys.Count() != errorCount)
                throw new Exception($"Translation list for {languageId} is not complete.");

            return translations;
        }
    }
}
