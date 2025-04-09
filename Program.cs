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
        // Инициализация бота           
        botClient = new TelegramBotClient("7769306776:AAGW8k9A9MmFJSZp6N1Lpr-IQYVsGgj77sM");

        try
        {
            // информация о боте
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Информация о боте:");
            Console.WriteLine($"Название: {me.FirstName}");
            Console.WriteLine($"Username: @{me.Username}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении информации о боте: {ex.Message}");
        }

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        botClient.StartReceiving(
            updateHandler: BotMethods.HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: CancellationToken.None
        );

        Console.WriteLine("БОТ ЗАПУЩЕН(перейдите в тг)");
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