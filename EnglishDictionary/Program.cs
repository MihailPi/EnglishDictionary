using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Collections.Generic;

namespace EnglishTreiner
{
    internal class Program
    {
        //бот
        static TelegramBotClient Bot;
        // предложенное слово для конкретного пользователя
        static Dictionary<int, string> lastUserWord = new Dictionary<int, string>();
        //список команд
        const string COMMAND_LIST =
@"Список команд:
/add - Добавление нового слова (фразы) в словарь
/getEng - Получаем случайное английское слово (фразу) из словаря;
/getRus - Получаем случайное русское слово (фразу) из словаря;
/get - Получаем перевод введенного слова (фразы) из словаря;
/check - Проверка правильности перевода английского слова (фразы);
/howWord - Количество слов в словаре;
/command - Список команд.";
        //  Список поздравительных фраз
        static List<string> congratulation = new List<string>()
        {
            "Молодец!", "Орёл!", "Так держать!", "Гений!", "Боец!", "Круто!", "Хвалю!", "Молодцом!", "Cool!",
            "Супер!", "Решительно!", "Да ты талантище!", "Юное дарование!", "Только вперёд!", "Моё почтение!"
        };
        static Random ranCong = new Random();
        static Tutor tutor = new Tutor();
        static string nameBot;
        //  Флаги для поочередного ввода слов
        static bool transFlag = false;
        static bool enFlag = false;
        static bool checkFlag = false;
        static bool getFlag = false;
        static void Main(string[] args)
        {
 
            Bot = new TelegramBotClient("5334725873:AAFxwsLjCUdR04ptxvGGuCtr_cNtE3WCP-E");
            var cts = new CancellationTokenSource();
            // StartReceiving не блокирует вызывающий поток
            var receiverOptions = new ReceiverOptions {AllowedUpdates = { }};
            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = Bot.GetMeAsync();
            nameBot = me.Result.Username;
            Console.WriteLine($"Слушаем чат-бот @{nameBot}:");
            Console.ReadLine();
            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // обрабатывать только обновления сообщений и обробатывать только текстовые сообщения
            if (update.Type != UpdateType.Message || update.Message.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            
            var userId = (int)update.Message.From.Id;
            String textForMessage;
            //  Кнопки
            KeyboardButton[][] button = new KeyboardButton[][] 
            { 
                new KeyboardButton[]{ "/get", "/getRus", "/getEng" }, 
                new KeyboardButton[]{ "/check", "/add", "/command" } 
            };

            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(button);
            replyKeyboardMarkup.ResizeKeyboard = true;

            Console.WriteLine($"Получено сообщение: '{messageText}' из чата: {chatId}, от usera {update.Message.From.FirstName}");

            // обрабатываем ответ пользователя
            switch (messageText)
            {
                case "/start":
                    textForMessage = $"Привет, я бот {nameBot}, и вот что я умею.\n" + COMMAND_LIST;
                    break;
                case "/command":
                    textForMessage = COMMAND_LIST;
                    break;
                case "/add":
                    enFlag = true;
                    textForMessage = "Введите английское слово (фразу).";
                    break;
                case "/get":
                    getFlag = true;
                    textForMessage = "Введите слово (фразу) которое нужно найти в словаре.";    
                    break;
                case "/getEng":         //  Передаем в функцию true получим английское слово
                    textForMessage = GetRandomWord(userId, true);
                    break;
                case "/getRus":          //  Передаем false - получим русское
                    textForMessage = GetRandomWord(userId, false);
                    break;
                case "/check":
                    checkFlag = true;
                    textForMessage = "Введите английское слово (фразу).";
                    break;
                case "/howWord":
                    textForMessage = $"В словаре сейчас {tutor.GetHowWordInDict} слов (фраз)! Ого!";
                    break;
                default:
                    if (lastUserWord.ContainsKey(userId) && !enFlag && !transFlag && !checkFlag && !getFlag)
                    {
                        textForMessage = CheckWord(lastUserWord[userId], messageText);
                        lastUserWord.Remove(userId);
                    }
                    else if(enFlag)
                    {
                        lastUserWord[userId] = messageText;
                        textForMessage = "Введите перевод.";
                        enFlag = false;
                        transFlag = true;
                    }
                    else if(transFlag)
                    {
                        textForMessage = AddNewWords(lastUserWord[userId], messageText);
                        transFlag = false;
                        lastUserWord.Remove(userId);
                    }
                    else if(checkFlag)
                    {
                        lastUserWord[userId] = messageText;
                        textForMessage = "Введите перевод.";
                        checkFlag = false;
                    }
                    else if(getFlag)
                    {
                        textForMessage = tutor.Translate(messageText);
                        getFlag = false;
                    }
                    else
                        textForMessage = "Я еще тупенький... знаю только эти команды.\n" + COMMAND_LIST;
                    break;
            }

            await botClient.SendTextMessageAsync(
            chatId,
            text: textForMessage,
            replyMarkup:replyKeyboardMarkup,
            cancellationToken: cancellationToken);
        
        
        }

        //  Добавляем во временный словарь слово для конкретного юзера
        private static string GetRandomWord(int userId, bool isEnglish)
        {
            var ranWord = tutor.GetRandomEngOrRusWord(isEnglish);

            if (ranWord != String.Empty)
            {
                if (lastUserWord.ContainsKey(userId))
                    lastUserWord[userId] = ranWord;
                else
                    lastUserWord.Add(userId, ranWord);

                return $"Ваше слово (фраза):{ranWord}. Как это переводится?";
            }
            else
                return "Вы выучили ВСЕ слова и фразы из текущего словаря!";
            
        }

        // Проверка слов
        private static string CheckWord(string wordForTransl, string transl)
        {
            if (tutor.CheckWord(wordForTransl, transl))
            {   //  Добавляем в список с уже знакомыми словами, чтобы не повторялись
                tutor.AddToKnow(wordForTransl);
                //  Проверяем на кратность 10-ти
                var checkWinCountWord = tutor.GetHowKnowWord;
                if (checkWinCountWord % 10 == 0)
                {
                    var answer = congratulation[ranCong.Next(congratulation.Count)];
                    return $"Правильно!\nТы уже запомнил перевод {checkWinCountWord} слов (фраз)!\n{answer}";
                }
                else
                    return "Правильно!";
            }
            else
            {
                var correctAnswer = tutor.Translate(wordForTransl);
                return $"Перевод из словаря: {correctAnswer}";
            }
        }

        // Добавление нового слова в словарь
        private static string AddNewWords(string eng, string transl)
        {
            var check = tutor.AddWord(eng.ToLower(), transl.ToLower());
            if (check)
                return "Слово (фраза) добавленно в словарь!";
            else
                return "Такое слово (фраза) уже есть в словаре!";
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine("нет интернета)");
            return Task.CompletedTask;
        }
    }
}
