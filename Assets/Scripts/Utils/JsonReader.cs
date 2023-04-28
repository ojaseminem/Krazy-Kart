using UnityEngine;
using System.IO;

namespace Utils
{
    public static class JsonReader
    {
        [System.Serializable]
        public class SentenceBank
        {
            public string[] sentences;
        }

        public static string[] GetSentenceBank()
        {
            var filePath = Application.dataPath + "/StreamingAssets/SentenceBank.json";
            var jsonString = File.ReadAllText(filePath);
            var jsonData = JsonUtility.FromJson<SentenceBank>(jsonString);
            string[] sentenceBank = jsonData.sentences;

            return sentenceBank;
        }
    }
}