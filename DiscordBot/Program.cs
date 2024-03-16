using System;
using System.IO;

using System.Security.Principal;
using Discord.Rest;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;


namespace DiscordBot
{
	class Program {
		static async Task Main(){

			var bot = new Bot();
			await bot.RunBotAsync();
		}
	}
}
