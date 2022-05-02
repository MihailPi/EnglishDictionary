using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
@"Список комманд:
/add <eng> <rus> - Добавление английского слова и его перевода в словарь;
/get - Получаем случайное английское слово из словаря;
/get <eng> - Получаем перевод введенного английского слова из словаря;
/check <eng> <rus> - Проверка правильности перевода английского слова.";

        static Tutor tutor = new Tutor();

        static void Main(string[] args)
        {
 
            Bot = new TelegramBotClient("5334725873:AAFxwsLjCUdR04ptxvGGuCtr_cNtE3WCP-E");
            var cts = new CancellationTokenSource();
            // StartReceiving не блокирует вызывающий поток. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions {AllowedUpdates = { }};
            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = Bot.GetMeAsync();
            Console.WriteLine($"Слушаем чат-бот @{me.Result.Username}:");
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
            //получаем массив из ответа пользователя
            var argsMessage = messageText.Split(' ');
            var userId = (int)update.Message.From.Id;
            String textForMessage;

            Console.WriteLine($"Получено сообщение: '{messageText}' из чата: {chatId}, от usera {update.Message.From.FirstName}");

            // обрабатываем ответ пользователя
            switch (argsMessage[0])
            {
                case "/start":
                    textForMessage = COMMAND_LIST;
                    break;
                case "/add":
                    textForMessage = AddNewWords(argsMessage);
                    break;
                case "/get":
                    if (argsMessage.Length > 1)
                    {
                        textForMessage = tutor.Translate(argsMessage[1]);
                    }
                    else
                    {
                        textForMessage = $"Ваше слово: {GetRandomEngWord(userId)}. Как оно переводится?";
                    }
                    break;
                case "/check":
                    textForMessage = CheckWord(argsMessage);
                    break;
                default:
                    if (lastUserWord.ContainsKey(userId))
                    {
                        textForMessage = CheckWord(lastUserWord[userId], argsMessage[0]);
                    }
                    else
                        textForMessage = "Я еще тупенький... знаю только эти команды.\n" + COMMAND_LIST;
                    break;
            }

            await botClient.SendTextMessageAsync(
            chatId,
            text: textForMessage,
            cancellationToken: cancellationToken);
        }

        //  Добавляем слово для конкретного юзера
        private static string GetRandomEngWord(int userId)
        {
            var ranWord = tutor.GetRandomEngWord();
            
            if(lastUserWord.ContainsKey(userId))
                lastUserWord[userId] = ranWord;
            else
                lastUserWord.Add(userId, ranWord);

            return ranWord;
        }

        // Проверка слов
        private static string CheckWord(string[] argsMessage)
        {
            if (argsMessage.Length != 3)
                return "Аргументов у /check должно быть 2! (английское слово и перевод)";
 
            else
            {
                if (tutor.CheckWord(argsMessage[1], argsMessage[2]))
                    return "Верно! Главный приз: Автомобииииль!!!";
                else
                {
                    var correctAnswer = tutor.Translate(argsMessage[1]);
                    return $"Перевод из словаря: {correctAnswer}";
                }
            }
        }

        private static string CheckWord(string engl, string transl)
        {
            if (tutor.CheckWord(engl, transl))
                return "Правильно!";
            else
            {
                var correctAnswer = tutor.Translate(engl);
                return $"Перевод из словаря: {correctAnswer}";
            }
        }

        // Добавление нового слова в словарь
        private static string AddNewWords(string[] argsMessage)
        {
            if (argsMessage.Length != 3)
                return "Аргументов у /add должно быть 2! (английское слово и перевод)";
            else
            {
                var check = tutor.AddWord(argsMessage[1], argsMessage[2]);
                if (check)
                    return "Слово добавленно в словарь!";
                else
                    return "Такое слово уже есть в словаре!";
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine("нет интернета)");
            return Task.CompletedTask;
        }
    }
}
