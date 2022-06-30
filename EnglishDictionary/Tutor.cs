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
        private List<string> _knowWord;
        private Random _random = new Random();
        private Regex _reg = new Regex("[a-z]");

        public Tutor()
        {   //Заполняем список уже имеющимися парами слов
            _dictEng = _storage.GetAllWords(true);
            _dictRus = _storage.GetAllWords(false);
            _knowWord = _storage.GetKnowWord();
        }
        public int GetHowKnowWord
        {
            get
            { return _knowWord.Count; }
        }

        public int GetHowWordInDict 
        { 
            get
            { return _dictEng.Count; }  
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
                    return "Такого слова (фразы) еще нет в словаре!";
            }
            else
            { 
                if (_dictRus.ContainsKey(wordForTranslate.ToLower()))
                    return _dictRus[wordForTranslate.ToLower()];
                else
                    return "Такого слова (фразы) еще нет в словаре!";
            }
        }

        public bool CheckWord(string wordForCheck, string checkWord)
        {
            if (_reg.Match(wordForCheck.ToLower()).Success)
            {
                if (!_dictEng.ContainsKey(wordForCheck.ToLower()))
                    return false;
                else
                {
                    var answer = _dictEng[wordForCheck.ToLower()];
                    return answer.ToLower() == checkWord.ToLower();
                }
            }
            else
            {
                if (!_dictRus.ContainsKey(wordForCheck.ToLower()))
                    return false;
                else
                {
                    var answer = _dictRus[wordForCheck.ToLower()];
                    return answer.ToLower() == checkWord.ToLower();
                }
            }
                
                
        }

        public string GetRandomEngOrRusWord(bool isEnglish)
        {
            //  Если нужно английское слово, работаем с английским словарем
            if (isEnglish)
                return GetRandomWord(_dictEng);
            //  Если русское, то с русским
            else
                return GetRandomWord(_dictRus);
        }

        private string GetRandomWord(Dictionary<string, string> dict)
        {
            var listUnknowWords = MakeListUnknowWords(dict);
            if (listUnknowWords.Count > 0)
            {
                var randIndex = _random.Next(0, listUnknowWords.Count-1);
                return listUnknowWords[randIndex];
            }
            else
                return String.Empty;
        }

        //  Создаем список с еще не выученными словами
        private List<string> MakeListUnknowWords(Dictionary<string, string> dict)
        {
            List<string> listUnknowWord = new List<string>(); 
            foreach (var word in dict)
            {
                if (!_knowWord.Contains(word.Key))
                    listUnknowWord.Add(word.Key);
            }
            return listUnknowWord;
        }

        //  Добавляем в список слова которые уже знаем
        public void AddToKnow(string knowWord)
        {
            _knowWord.Add(knowWord);
            _storage.AddKnowWord(knowWord);
        }
    }
}
