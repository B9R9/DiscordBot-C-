using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordBot
{
	public class Bot
	{
		
		private DiscordSocketClient _client;
		private CommandService _commands;
		private IServiceProvider _services;
		private LLMResponse llm = new LLMResponse("gemma:2b", "http://localhost:11434/api/generate");

		public async Task RunBotAsync()
		{
			_client = new DiscordSocketClient();
			_commands = new CommandService();

			_client.Log += Log;
			_commands.Log += Log;

			_services = new ServiceCollection().BuildServiceProvider();

			await RegisterCommandsAsync();

			Api discord = new Api("DISCORD");
			string token = discord.GetToken(); // Obtenez le token du bot depuis la configuration


            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

			var feedNews = new FeedNewsHandler();
			Task databaseUpdateTask = Task.Run ( async ()=>
			{
				while (true)
				{
					Console.WriteLine("test 0.0");
					await feedNews.UpdateDataBase();
					await Task.Delay(TimeSpan.FromHours(12));
				}
			});
			
			Task publishingTask = Task.Run( async ()=>
			{
				await feedNews.PublishArticlesAsync(_client);
			});
            await Task.Delay(-1);
		}

		private async Task RegisterCommandsAsync()
		{
			_client.MessageReceived += HandleCommandsAsync;
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
		}

		private async Task HandleCommandsAsync(SocketMessage arg)
		{
			var message = arg as SocketUserMessage;
			var context = new SocketCommandContext(_client, message);

			if (message.Author.IsBot || message == null)
				return ;
			
			int argPos = 0;
			if (message.HasStringPrefix("$", ref argPos))
			{
				var result = await _commands.ExecuteAsync(context, argPos, _services);
				if (!result.IsSuccess)
				{
					Console.WriteLine(result.ErrorReason);
				}
			}
			else if (message.Content.ToLower() == "!stopbot")
			{
				await _client.LogoutAsync();
				await _client.StopAsync();
				await _client.DisposeAsync();

				Environment.Exit(0);
			}
			else
			{
				var llmResponse = await llm.SendRequestAsync(message.Content);
				if (llmResponse != null && llmResponse.Done )
				{
					llmResponse.SendMessageAsync(message);
				}
			}
		}
		private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
	}
} 