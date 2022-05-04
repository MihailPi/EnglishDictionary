using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EnglishTreiner
{
    internal class Tutor
    {
        private Dictionary<string, string> _dictEng;
        private Dictionary<string, string> _dictRus;
        private WordStorage _storage = new WordStorage();
        private Random _random = new Random();
        private Regex _reg = new Regex("[a-z]");
        public Tutor()
        {   //Заполняем список уже имеющимися парами слов
            _dictEng = _storage.GetAllWords(true);
            _dictRus = _storage.GetAllWords(false);
        }
        public bool AddWord(string wordForTranslate, string translate)
        {   //если еще нет такого слова
            if (!_dictEng.ContainsKey(wordForTranslate) && !_dictRus.ContainsKey(translate))
            {   //добавляем в текущие списки и в файл
                _dictEng.Add(wordForTranslate, translate);
                _dictRus.Add(translate, wordForTranslate);
                _storage.AddWord(wordForTranslate, translate);
                return true;
            }
            else
                return false;
              
        }

        public string Translate(string wordForTranslate)
        {
            //  Проверка на язык
            if (_reg.Match(wordForTranslate.ToLower()).Success)
            {
                if (_dictEng.ContainsKey(wordForTranslate.ToLower()))
                    return _dictEng[wordForTranslate.ToLower()];
                else
                    return "Такого слова еще нет в словаре!";
            }
            else
            { 
                if (_dictRus.ContainsKey(wordForTranslate.ToLower()))
                    return _dictRus[wordForTranslate.ToLower()];
                else
                    return "Такого слова еще нет в словаре!";
            }
        }

        public bool CheckWord(string wordForCheck, string checkWord)
        {
            if (_reg.Match(wordForCheck.ToLower()).Success)
            {
                if (!_dictEng.ContainsKey(wordForCheck))
                    return false;
                else
                {
                    var answer = _dictEng[wordForCheck];
                    return answer.ToLower() == checkWord.ToLower();
                }
            }
            else
            {
                if (!_dictRus.ContainsKey(wordForCheck))
                    return false;
                else
                {
                    var answer = _dictRus[wordForCheck];
                    return answer.ToLower() == checkWord.ToLower();
                }
            }
                
                
        }

        public string GetRandomEngOrRusWord(bool lang)
        {
            var r = _random.Next(0, _dictEng.Count);
            if (lang)
            {
                var keys = new List<string>(_dictEng.Keys);
                return keys[r];
            }
            else
            {
                var keys = new List<string>(_dictRus.Keys);
                return keys[r];
            }
        }
    }
}
