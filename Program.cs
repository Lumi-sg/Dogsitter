using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace Doghouse_Bot_Beta
{
    public class Program
    {
        static void Main(string[] args) => new Program()
            .RunBotAsync()
            .GetAwaiter()
            .GetResult();


        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            var config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(config);
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            var token = "<YOURTOKENGOESHERE>";

            _client.Log += _client_Log;
            await Dogsit();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task Dogsit()
        {
            _client.MessageReceived += MessageDeleter;
            _client.MessageUpdated += EditedMessageDeleter;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageDeleter(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            //var context = new SocketCommandContext(_client, message);
            var whitelistedWords = new List<string>();

            whitelistedWords.Add("woof");
            whitelistedWords.Add("bark");

            if (whitelistedWords.All(words => message.Content.ToLower() != (words)))
            {
                await message.DeleteAsync();
            }
        }

        private async Task EditedMessageDeleter(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = after as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            var whitelistedWords = new List<string>();

            whitelistedWords.Add("woof");
            whitelistedWords.Add("bark");

            if (whitelistedWords.All(words => message.Content.ToLower() != (words)))
            {
                await message.DeleteAsync();
            }
        }
    }
}
