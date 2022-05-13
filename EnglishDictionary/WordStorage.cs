using System;
using System.Collections.Generic;
using System.IO;

namespace EnglishTreiner
{
    public class WordStorage
    {
        private const string _path = "D:\\1MEDIA\\Исходники\\Visual Studio 2022\\EnglishDictionaryChatBot\\EnglishDictionary\\bin\\Debug\\Dict.txt";
        private const string _pathKnow = "D:\\1MEDIA\\Исходники\\Visual Studio 2022\\EnglishDictionaryChatBot\\EnglishDictionary\\bin\\Debug\\KnowWord.txt";

        public WordStorage()
        {   //Если файла не существует создаем
            if(!File.Exists(_path))
            {
                File.Create(_path);
            }
            if(!File.Exists(_pathKnow))
            {
                File.Create(_pathKnow);
            }
        }
        public Dictionary<string, string> GetAllWords(bool howDict)
        {   
            var dic=new Dictionary<string, string>();
            try
            {
                if (File.Exists(_path))
                {   //проходим по файлу
                    foreach (var line in File.ReadAllLines(_path))
                    {   //разделяем строку на массив из двух значений
                        var words = line.Split('|');
                        if (words.Length == 2)
                        {       //  Создаем агло-рус и рус-англ словарь возвращаем какой нужно
                                if (howDict)
                                    dic.Add(words[0], words[1]);
                                else
                                    dic.Add(words[1], words[0]);
                        }
                    }
                }
                return dic;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Словарь не найден!");
                Console.WriteLine(ex.Message);
                return new Dictionary<string, string>();
            }

        }
        public void AddWord(string english, string transl)
        {
            try
            {
                using (var sw = new StreamWriter(_path, true))
                {   //добавляем строку с разделителем между словами
                    sw.WriteLine($"{english.ToLower()}|{transl.ToLower()}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Не удалось добавить слово {english} в словарь!");
                Console.WriteLine(ex.Message);
            }
        }

        public void AddKnowWord(string knowWord)
        {
            try
            {
                using(var sw = new StreamWriter(_pathKnow, true))
                {
                    sw.WriteLine(knowWord);   
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Не удалось записать слово {knowWord} в файл, возможно он не создан.");
                Console.WriteLine(ex.Message);
            }
        }

        public List<string> GetKnowWord()
        {
            var list = new List<string>();
            try
            {
                 if(File.Exists(_pathKnow))
                {
                    foreach(var line in File.ReadAllLines(_pathKnow))
                    {
                        list.Add(line);
                    }
                }
                return list;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Не удалось считать выученые слова из файла");
                Console.WriteLine(ex.Message);
                return list;
            }
        }
    }
}
