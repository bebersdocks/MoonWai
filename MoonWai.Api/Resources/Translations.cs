using System;
using System.Collections.Generic;
using System.Linq;

using MoonWai.Shared.Definitions;

using Newtonsoft.Json;

namespace MoonWai.Api.Resources
{
    public static class Translations
    {
        private static List<(LanguageId, string)> languageResources = new()
        {
            (LanguageId.English, "en"),
            (LanguageId.Russian, "ru")
        };

        private static Dictionary<LanguageId, Dictionary<TranslationId, string>> translations;

        public async static void Load()
        {
            if (translations != null)
                return;

            var translationsCounts = Enum.GetValues<TranslationId>().Count();

            translations = new();

            foreach (var (languageId, languageIdStr) in languageResources)
            {
                var json = await EmbeddedResources.GetResourceStr(languageIdStr + ".json");

                translations[languageId] = JsonConvert.DeserializeObject<Dictionary<TranslationId, string>>(json);

                if (translations[languageId].Keys.Count != translationsCounts)
                    throw new Exception($"Translation list for {languageId.ToString()} is not complete");
            }
        }

        public static string GetTranslation(LanguageId languageId, TranslationId translationId, params object[] args)
        {
            var translationStr = translations[languageId][translationId];

            return string.Format(translationStr, args);
        }
    }
}
