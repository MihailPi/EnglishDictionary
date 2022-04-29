using System;
using System.Collections.Generic;
using System.IO;

namespace EnglishTreiner
{
    public class WordStorage
    {
        private const string _path = "D:\\1MEDIA\\Исходники\\Visual Studio 2022\\EnglishDictionaryChatBot\\EnglishDictionary\\bin\\Debug\\Dict.txt";

        public WordStorage()
        {   //Если файла не существует создаем
            if(!File.Exists(_path))
            {
                File.Create(_path);
            }
        }
        public Dictionary<string, string> GetAllWords()
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
                        {
                            dic.Add(words[0], words[1]);
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
                    sw.WriteLine($"{english}|{transl}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Не удалось добавить слово {english} в словарь!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
