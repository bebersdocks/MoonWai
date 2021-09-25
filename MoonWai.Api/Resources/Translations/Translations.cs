using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Resources.Translations
{
    public static class Translations
    {
        private const string resourceName = "Translations.xml";

        private static Dictionary<LanguageId, Dictionary<TranslationId, string>> translations;

        public async static void Load()
        {
            if (translations != null)
                return;

            var translationsXml = await EmbeddedResources.GetResourceStr(resourceName);

            var doc = new XmlDocument();

            doc.LoadXml(translationsXml);

            translations = new();
            translations[LanguageId.English] = new();
            translations[LanguageId.Russian] = new();

            var translationIds = Enum.GetValues<TranslationId>().ToHashSet();

            foreach (XmlNode stringNode in doc.FirstChild.ChildNodes)
            {
                if (!Enum.TryParse(stringNode.Attributes["id"]?.Value ?? string.Empty, out TranslationId translationId))
                    continue;

                foreach (XmlNode translationNode in stringNode.ChildNodes)
                {
                    var languageIdStr = translationNode.Attributes["languageId"].Value;
                    var languageId = languageIdStr switch 
                    {
                        "en" => LanguageId.English,
                        "ru" => LanguageId.Russian,
                        _ => throw new Exception($"Unmatched language id: {languageIdStr}")
                    };

                    translations[languageId][translationId] = translationNode.InnerText;
                }

                translationIds.Remove(translationId);
            }

            if (translationIds.Any())
                throw new Exception("Translation list is not complete");

            if (translations[LanguageId.English].Count != translations[LanguageId.Russian].Count)
                throw new Exception("Translation counts for languages don't match");
        }

        public static string GetTranslation(LanguageId languageId, TranslationId translationId, params object[] args)
        {
            var translationStr = translations[languageId][translationId];

            return string.Format(translationStr, args);
        }
    }
}