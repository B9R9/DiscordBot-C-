using Discord.Commands;


namespace DiscordBot
{
	public class CommandModule: ModuleBase<SocketCommandContext>
	{
		[Command("hello")]
		public async Task HellloCommand()
		{
			await ReplyAsync("Hello!");
		}
	}
}