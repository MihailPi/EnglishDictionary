using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading;

namespace EnglishTreiner
{
    internal class Program
    {
        //бот
        static TelegramBotClient Bot;
        //список команд
        const string COMMAND_LIST = 
@"Список комманд:
/add <eng> <rus> - Добавление английского слова и его перевода в словарь;
/get - Получаем случайное английское слово из словаря;
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
            //Console.WriteLine($"Даров, я бот {me.Result.FirstName}");
            Console.WriteLine($"Слушаем чат-бот @{me.Result.Username}:");
            Console.ReadLine();
            // Send cancellation request to stop bot
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
                    textForMessage = "Ваше слово: " + tutor.GetRandomEngWord() + ". Как оно переводится?";
                    break;
                case "/check":
                    textForMessage = CheckWord(argsMessage);
                    break;
                default:
                    textForMessage = "Я еще тупенький... знаю только эти комманды.\n" + COMMAND_LIST;
                    break;
            }
            
            await botClient.SendTextMessageAsync(
            chatId,
            text: textForMessage,
            cancellationToken: cancellationToken);
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
            Console.WriteLine("hui=>pizda=>Gigurda (Походу нет интернета:)");
            return Task.CompletedTask;
        }
    }
}
