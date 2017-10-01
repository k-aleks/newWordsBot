using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using NLog.LayoutRenderers.Wrappers;

namespace NewWordsBot
{
    public class MacmillanGetItemResultParser
    {
        public bool TryParse(string jsonResult, out List<string> definitions, out PartOfSpeech partOfSpeech)
        {
            partOfSpeech = default(PartOfSpeech);
            definitions = new List<string>();
            
            var xmlContent = GetXmlContentFromJson(jsonResult);
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlContent)))
            {
                reader.ReadToFollowing("PART-OF-SPEECH");
                reader.Read();
                var partOfSpeechValue = reader.Value.Trim();
                partOfSpeech = GetPartOfSpeech(partOfSpeechValue);
                while (reader.ReadToFollowing("DEFINITION"))
                {
                    reader.MoveToContent();
                    reader.Read();
                    var def  = reader.Value.Trim();
                    definitions.Add(def);
                }
            }
            return true;
        }

        private PartOfSpeech GetPartOfSpeech(string partOfSpeechValue)
        {
            //TODO: complete part of speech
            switch (partOfSpeechValue.ToLower())
            {
                case "noun" : return PartOfSpeech.Noun;
                case "verb" : return PartOfSpeech.Verb;
                case "phrasal verb" : return PartOfSpeech.PhrasalVerb;
                case "adjective" : return PartOfSpeech.Adjective;
            }
            return PartOfSpeech.Unknown;
        }

        private string GetXmlContentFromJson(string jsonResult)
        {
            var definition = new
            {
                topics = new Object[]{},
                dictionaryCod = "",
                entryLabel = "",
                entryContent = "",
                format = "",
                entryId = ""
            };
            var json = JsonConvert.DeserializeAnonymousType(jsonResult, definition);
            return json.entryContent;
        }
    }
}