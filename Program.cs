using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Handlers; 

class Program
{
    private static ITelegramBotClient? botClient;

    static async Task Main(string[] args)
    {
        // API
        botClient = new TelegramBotClient("8154294084:AAEaR9VKxSz3YKbhQqoO_2WTqxO9E74ZeCM");

        try
        {
            // информация о боте в теминал 
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Информация о боте:");
            Console.WriteLine($"Название: {me.FirstName}");
            Console.WriteLine($"Username: @{me.Username}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении информации о боте: {ex.Message}");
        }

        // настройка опций для получения обновлений
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() 
        };

        // Запуск получения обновлений
        botClient.StartReceiving(
            updateHandler: BotMethods.HandleUpdateAsync, // Используем метод из BotMethods
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: CancellationToken.None
        );

        Console.WriteLine("БОТ ЗАПУЩЕН");
        await Task.Delay(-1);
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiException => $"Ошибка Telegram API:\n[{apiException.ErrorCode}]\n{apiException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}