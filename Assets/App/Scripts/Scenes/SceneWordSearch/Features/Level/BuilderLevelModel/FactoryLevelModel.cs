using System.Collections.Generic;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        private Dictionary<char, int> allUniqChars = new Dictionary<char, int>();
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {           
            List<char> listChars = new List<char>();

            foreach (var word in words)
            {
                Dictionary<char,int> wordUniqChars = word.GroupBy(letter => letter)
                .ToDictionary(letter => letter.Key, letter => letter.Count());

                foreach (var charKeyValueCount in wordUniqChars)
                {
                    allUniqChars = TryAddUniqCharsToDict(charKeyValueCount);
                }
            }

            return ConvertDictCharsToList(allUniqChars);
        }

        private Dictionary<char, int> TryAddUniqCharsToDict(KeyValuePair<char,int> charKeyValueCount)
        {           
            bool isAddedUniqChar = allUniqChars.TryAdd(charKeyValueCount.Key, charKeyValueCount.Value);

            if (!isAddedUniqChar)
            {
                if (charKeyValueCount.Value > allUniqChars[charKeyValueCount.Key])
                {
                    allUniqChars[charKeyValueCount.Key] = charKeyValueCount.Value;
                }
            }
            return allUniqChars;
        }

        private List<char> ConvertDictCharsToList(Dictionary<char,int> uniqueChars)
        {
            List<char> listChars = new List<char>();

            foreach (var uniqChar in uniqueChars)
            {
                for (int sameCharCounter = 0; sameCharCounter < uniqChar.Value; sameCharCounter++)
                {
                    listChars.Add(uniqChar.Key);
                }
            }
            return listChars;
        }
    }
}