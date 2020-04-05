using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SecretNamesBackend.Utils
{
    public class WordProvider
    {
        private WordProvider() {
            var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SecretNamesBackend.Resources.WordList.txt"));
            _words = streamReader.ReadToEnd().Split("\n").ToList();
        }

        private List<string> _words;

        private static WordProvider _instance;

        public static WordProvider GetInstance()
        {
            if (_instance == null) _instance = new WordProvider();
            return _instance;
        }

        public string GetRandomWord()
        {
            return GetRandomWord(new Random());
        }

        public string GetRandomWord(Random random)
        {
            return _words[random.Next(0, _words.Count)];
        }
    }
}
