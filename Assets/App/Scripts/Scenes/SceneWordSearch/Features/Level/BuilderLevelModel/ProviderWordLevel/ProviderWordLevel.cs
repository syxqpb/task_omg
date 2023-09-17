using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public string pathToLevelData = $"{Application.dataPath}/App/Resources/WordSearch/Levels/";
        public LevelInfo LoadLevelData(int levelIndex)
        {
            string currentLevelPath = Path.Combine($"{pathToLevelData}", $"{levelIndex}.json");

            Words levelWords = ReadLevelInfoFromFile(currentLevelPath);

            return LoadLevelInfo(levelWords);
        }

        private Words ReadLevelInfoFromFile(string fileName)
        {
            string json = File.ReadAllText(fileName);

            return JsonUtility.FromJson<Words>(json);
        }
        private LevelInfo LoadLevelInfo(Words levelWords)
        {
            return new LevelInfo() { words = levelWords.words };
        }
    }

    [Serializable]
    public class Words
    {
        public List<string> words;
    }
}