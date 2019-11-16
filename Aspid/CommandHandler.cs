using System;
using System.Reflection;
using System.Threading.Tasks;
using Aspid.Modules;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace Aspid
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), services: null); 
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage lmao)
        {
            var msg = lmao as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);

            if (context.User.IsBot) return;

            int argPos = 0;

            if (msg.Content.Contains("аспид") || msg.Content.Contains("Аспид"))
            {
                var emote = Emote.Parse("<:Aspid:567801319197245448>");
                await context.Message.AddReactionAsync(emote);
            }

            if((msg.Content.Contains('f') || msg.Content.Contains('F')) && msg.Content.Length == 1)
            {
                await msg.DeleteAsync();
                RestUserMessage newer = await context.Channel.SendMessageAsync("🇫", false);
                var emote = Emote.Parse("<:F_:575051447717330955>");
                await newer.AddReactionAsync(emote); 
            }

            if (msg.HasStringPrefix(Config.bot.prefix, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, services: null);
                if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
