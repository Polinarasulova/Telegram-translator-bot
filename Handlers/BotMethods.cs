using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Handlers
{
    public static class BotMethods
    {
        // Словарь с русского на англ и турецкий
        private static readonly Dictionary<string, (string English, string Turkish)> Dictionary = new(StringComparer.OrdinalIgnoreCase)
        {
            { "привет", ("hello", "merhaba") },
            { "мир", ("world", "dünya") },
            { "кот", ("cat", "kedi") },
            { "собака", ("dog", "köpek") },
            { "дом", ("house", "ev") },
            { "ухо", ("ear", "kulak") },
            { "зима", ("winter", "kış") },
            { "лето", ("summer", "yaz") },
            { "весна", ("spring", "bahar") },
            { "осень", ("autumn", "sonbahar") },
            { "програмист", ("programmer", "programcı") },
            { "любовь", ("love", "aşk") },
            { "Россия", ("Russia", "Rusya") },
            { "Санкт-Петербург", ("Saint-Petersburg", "Saint Petersburg") },
            { "фен", ("fan", "saç kurutma makinesi") },
            { "лампа", ("lamp", "lamba") },
            { "музыка", ("music", "müzik") },
            { "лицо", ("face", "kişi") },
            { "код", ("code", "kod") },
            { "пожалуйста", ("please", "lütfen") },
            { "спасибо", ("thanks", "teşekkürler") },
            { "пока", ("while", "kadar") }

        };

        /// <summary>
        /// Обработка входящих обновлений (сообщений).
        /// </summary>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { Text: { } messageText } || update.Message.From is null)
                return;

            var chatId = update.Message.Chat.Id;
            var username = update.Message.From.Username ?? "солнышко"; // @ отсутствует,

            try
            {
                if (messageText == "/start")
                {
                    await SendMessage(botClient, chatId, $"Привет, @{username}! Этот бот позволяет переводить сообщения с русского на англисский и турецкий.");
                    return;
                }

                if (messageText == "/author")
                {
                    await SendMessage(botClient, chatId, "Автор бота: @polinkarva😊");
                    return;
                }

                // Проверка слова в словаре
                if (Dictionary.TryGetValue(messageText.Trim(), out var translation))
                {
                    await SendMessage(botClient, chatId, $"Перевод с: \n Английского: {translation.English}\n Турецкого: {translation.Turkish}");
                }
                else
                {
                    await SendMessage(botClient, chatId, "Я не поняла, что ты написал, напиши по-другому.");
                }
            }
            catch (Exception ex)
            {
                await SendMessage(botClient, chatId, $"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправка сообщения пользователю.
        /// </summary>
        private static async Task SendMessage(ITelegramBotClient botClient, long chatId, string text)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                cancellationToken: CancellationToken.None
            );
        }
    }
}