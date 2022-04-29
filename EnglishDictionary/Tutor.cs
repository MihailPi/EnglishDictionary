using System;
using System.Collections.Generic;

namespace EnglishTreiner
{
    internal class Tutor
    {
        private Dictionary<string, string> _dict;
        private WordStorage _storage = new WordStorage();
        private Random _random = new Random();
        public Tutor()
        {   //Заполняем список уже имеющимися парами слов
            _dict = _storage.GetAllWords();
        }
        public bool AddWord(string englishWord, string translate)
        {   //если еще нет такого слова
            if (!_dict.ContainsKey(englishWord))
            {   //добавляем в текущий список и в файл
                _dict.Add(englishWord, translate);
                _storage.AddWord(englishWord, translate);
                return true;
            }
            else
                return false;
              
        }

        public string Translate(string englishWord)
        {

            if (_dict.ContainsKey(englishWord.ToLower()))
                return _dict[englishWord.ToLower()];
            else
                return "Такого слова еще нет в словаре!";
        }
        public bool CheckWord(string englishWord, string checkWord)
        {
            if (!_dict.ContainsKey(englishWord))
                return false;
            else
            {
                var answer = _dict[englishWord];
                return answer.ToLower() == checkWord.ToLower();
            }
                
        }

        public string GetRandomEngWord()
        {
            var r = _random.Next(0, _dict.Count);
            var keys=new List<string>(_dict.Keys);
            return keys[r];
        }
    }
}
