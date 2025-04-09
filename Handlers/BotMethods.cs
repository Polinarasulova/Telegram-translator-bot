using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Handlers
{
    public static class BotMethods
    {
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

        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { Text: { } messageText } || update.Message.From is null)
                return;

            var chatId = update.Message.Chat.Id;
            var username = update.Message.From.Username ?? "солнышко";

            try
            {
               if (messageText == "/start")
                {
                    await SendMessage(botClient, chatId, $"Привет, @{username}! Этот бот переводит текст с русского на английский и турецкий.))");
                    return;
                }

                if (messageText == "/author")
                {
                    await SendMessage(botClient, chatId, "Автор бота: @polinkarva😊");
                    return;
                }

                if (Dictionary.TryGetValue(messageText.Trim(), out var translation))
                {
                    await SendMessage(botClient, chatId, $"Перевод с:\n Английского: {translation.English}\n Турецкого: {translation.Turkish}");
                }
                else
                {
                    var englishTranslation = await TranslateTextAsync(messageText, "ru", "en");
                    var turkishTranslation = await TranslateTextAsync(messageText, "ru", "tr");

                    await SendMessage(botClient, chatId, $"Перевод с Google Translate:\n Английский: {englishTranslation}\n Турецкий: {turkishTranslation}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await SendMessage(botClient, chatId, "ОШИБКА");
            }
        }

        private static async Task SendMessage(ITelegramBotClient botClient, long chatId, string text)
        {
            await botClient.SendTextMessageAsync(chatId, text, cancellationToken: CancellationToken.None);
        }

        private static async Task<string> TranslateTextAsync(string text, string sourceLang, string targetLang)
        {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={Uri.EscapeDataString(text)}";
            var response = await httpClient.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<List<object>>(response);
            return result?[0]?.ToString()?.Split('"')[1] ?? "Ошибка: Нет перевода";
        }
    }
}