using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private string pathToWordsList = Path.Combine($"{Application.dataPath}/App/Resources/Fillwords", "words_list.txt");

        private string pathToWordInfo = Path.Combine($"{Application.dataPath}/App/Resources/Fillwords", "pack_0.txt");

        private Dictionary<int, string> wordsInfoDict;

        private Dictionary<int, char> gridCharDict;

        private char[] charsToOutput;

        private List<string> words;

        public GridFillWords LoadModel(int index)
        {
            string levelString = ReadLevelFillWordInfoFromFile(pathToWordInfo, index);

            wordsInfoDict = new Dictionary<int,string>();
            LoadLevelInfoToDict(levelString);

            words = new List<string>();
            ReadWordsFromFile(pathToWordsList);

            gridCharDict = CharGrid();
            SortedDictionary<int, char> sortedGridChars = new SortedDictionary<int, char>(gridCharDict);

            charsToOutput = sortedGridChars.Values.ToArray();

            return CreateGridFillWords();
        }

        private string ReadLevelFillWordInfoFromFile(string fileName, int index)
        {
            IEnumerable<string> levels = File.ReadLines(fileName);

            return levels.Skip(index - 1).FirstOrDefault();
        }

        private void LoadLevelInfoToDict(string levelString)
        {
            List<string> wordNumber = levelString.Split(" ").ToList();

            for (int i = 0; i < wordNumber.Count; i += 2)
            {
                List<string> oneWordInfo = wordNumber.Skip(i).Take(2).ToList();
                wordsInfoDict.Add(Convert.ToInt32(oneWordInfo[0]), oneWordInfo[1]);
            }
        }

        private void ReadWordsFromFile(string pathToFile)
        {
            words.Clear();
            foreach (var wordInfo in wordsInfoDict)
            {
                string word = File.ReadLines(pathToFile).Skip(wordInfo.Key).FirstOrDefault();
                words.Add(word);
            }            
        }

        private int[] SeparateWordToCharArray(int currentWordIndex)
        {           
            string listOfCharIndexes = wordsInfoDict.ElementAt(currentWordIndex).Value;
            string[] charIndexesArray = listOfCharIndexes.Split(";").ToArray();
            int[] gridCharIndexes = new int[charIndexesArray.Length];

            for (int i = 0; i < charIndexesArray.Length; i++)
            {
                Int32.TryParse(charIndexesArray[i], out gridCharIndexes[i]);
            }            
            return gridCharIndexes;
        }
        
        private Dictionary<int,char> CharGrid()
        {
            Dictionary<int, char> gridChar = new Dictionary<int, char>();   
            
            for (int currentWordIndex = 0; currentWordIndex < words.Count; currentWordIndex++)
            {
                int[] gridCharIndexes = SeparateWordToCharArray(currentWordIndex);
                char[] wordChars = words[currentWordIndex].ToCharArray();               
                for (int currentCharIndex = 0; currentCharIndex < wordChars.Length; currentCharIndex++)
                {
                    gridChar.Add(gridCharIndexes[currentCharIndex], wordChars[currentCharIndex]);
                }
            }          
            
           return gridChar;
        }

        private Vector2Int CalculateSizeGrid(int size)
        {
            Vector2Int gridSize = new Vector2Int((int)Math.Sqrt(size), (int)Math.Sqrt(size));

            return gridSize;
        }

        private GridFillWords CreateGridFillWords()
        {
            Vector2Int size = CalculateSizeGrid(charsToOutput.Length);
            GridFillWords grid = new GridFillWords(size);

            SetDataGrid(grid, charsToOutput);

            return grid;
        }

        private GridFillWords SetDataGrid(GridFillWords grid, char[] chars)
        {
            int ArrayCounter = 0;

            for (int i = 0; i < grid.Size.y; i++)
            {
                for (int j = 0; j < grid.Size.x; j++)
                {
                    CharGridModel charToGridModel = new CharGridModel(chars[ArrayCounter]);
                    grid.Set(i, j, charToGridModel);
                    ArrayCounter++;
                }
            }
            ClearData();

            return grid;
        }

        private void ClearData()
        {
            words.Clear();
            wordsInfoDict.Clear();
            gridCharDict.Clear();
            charsToOutput = new char[0];
        }
    }
}