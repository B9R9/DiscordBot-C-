using Discord.WebSocket;
using Discord;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
	public class FeedNewsHandler
	{
		public Api NewsApi { get; set; }
		private CSVData dataBase = new CSVData("./database/news.csv", true);
		public string Endpoint { get; set; }
		private string Channel { get; set; }

		private int MaxArticles { get; set; }
		public FeedNewsHandler() {
			this.NewsApi = new Api("MEDIASTACK");
			this.Endpoint = "http://api.mediastack.com/v1/news";
			
			this.MaxArticles = 4;
		}

		public async Task UpdateDataBase()
		{
			using (var httpClient = new HttpClient())
			try
			{
				httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
				string param = $"access_key={NewsApi.GetToken()}&categories=technology&languages=en&sort=published_desc&limit=20&countries=us, fr, gb, jp, cn";

				HttpResponseMessage response = await httpClient.GetAsync($"{Endpoint}?{param}");
				string responseBody = await response.Content.ReadAsStringAsync();
// 				string responseBody = @"{
//     ""pagination"": {
//         ""limit"": 20,
//         ""offset"": 0,
//         ""count"": 20,
//         ""total"": 4664
//     },
//     ""data"": [
//         {
//             ""author"": ""Steve Dent"",
//             ""title"": ""Epic accuses Apple of flouting court order by charging for external links on iOS apps"",
//             ""description"": ""Epic Games has already accused Apple of 'malicious compliance' with the EU's new competition laws, and now it's making the same allegation stateside. In a new legal filing, it accused Apple of non-compliance with a 2021 ruling that allowed developers to bypass Apple's 30 percent cut of in-app payments and is asking the court to enforce the original injunction.Once the Supreme Court declined to hear an appeal of the ruling, Apple released revised guidelines, forcing developers to apply for an 'entitlement,' while still offering the option to purchase through Apple's own billing system. Moreover..."",
//             ""url"": ""https://www.engadget.com/epic-accuses-apple-of-flouting-court-order-by-charging-for-external-links-on-ios-apps-070036198.html?src=rss"",
//             ""source"": ""Engadget"",
//             ""image"": ""https://o.aolcdn.com/images/dims?image_uri=https%3A%2F%2Fs.yimg.com%2Fos%2Fcreatr-uploaded-images%2F2024-03%2F8a792240-e1c7-11ee-96f7-ed736ee0a0b0&resize=1400%2C933&client=19f2b5e49a271b2bde77&signature=77d0fd9e6a7a12033f9960e5d25691f5b690c5dd"",
//             ""category"": ""technology"",
//             ""language"": ""en"",
//             ""country"": ""us"",
//             ""published_at"": ""2024-03-14T09:00:36+02:00""
//         },
//         {
//             ""author"": ""Amy Skorheim"",
//             ""title"": ""The best docking stations for laptops in 2024"",
//             ""description"": ""Depending on how much stuff you need to plug in, your laptop may not have enough ports to support it all — particularly if you have more wired accessories than Bluetooth ones. Docking stations add different combinations of Ethernet, HDMI, DisplayPort, 3.5mm, memory card and USB connections and, unlike simple hubs, are often DC-powered. For those who switch up their working location regularly, a docking station can make it easier to swap between a fully-connected desk setup and a simple laptop, since just one port links your computer to the dock. Which docking station you should get depends in..."",
//             ""url"": ""https://www.engadget.com/best-docking-station-160041863.html?src=rss"",
//             ""source"": ""Engadget"",
//             ""image"": ""https://o.aolcdn.com/images/dims?image_uri=https%3A%2F%2Fs.yimg.com%2Fos%2Fcreatr-uploaded-images%2F2023-11%2F66b7f380-799a-11ee-9add-b4bc7d9cfb96&resize=1400%2C840&client=19f2b5e49a271b2bde77&signature=b60de64fb60ed55565cd9b1750e2a6d76d920ba2"",
//             ""category"": ""technology"",
//             ""language"": ""en"",
//             ""country"": ""us"",
//             ""published_at"": ""2024-03-14T09:00:36+02:00""
//         },
//         {
//             ""author"": ""Tage Kene-Okafor"",
//             ""title"": ""Nigeria’s Youverify raises $2.5M to enhance anti-money laundering compliance"",
//             ""description"": ""Youverify, a Nigerian provider of identity verification and anti-money laundering (AML) solutions for banks and startups, secured a $2.5 million investment from Elm, which specializes in offering ready-made and customized digital solutions to public and private institutions in Saudi Arabia. The pre-Series A investment from Elm also includes a strategic partnership to help Youverify streamline [&#8230;]© 2024 TechCrunch. All rights reserved. For personal use only."",
//             ""url"": ""https://tecihcrunch.com/2024/03/13/nigerias-youverify-raises-2-5m-to-enhance-anti-money-laundering-compliance/"",
//             ""source"": ""TechCrunch"",
//             ""image"": null,
//             ""category"": ""technology"",
//             ""language"": ""en"",
//             ""country"": ""us"",
//             ""published_at"": ""2024-03-14T08:58:38+02:00""
//         }
//     ]
// }";
				// if(true)
				if (response.IsSuccessStatusCode)
				{
					JObject jsonObject = JObject.Parse(responseBody);
					// Console.WriteLine($"JsonObject[\'data\'] -->\n{jsonObject["data"]}");
					dataBase.AddData((JArray)jsonObject["data"]);
					dataBase.DeleteData();				
				}
                else
                {
                    // Display error message if request was not successful
                    Console.WriteLine($"Request failed with status code {response.StatusCode}");
                    // Console.WriteLine($"Request failed with status code i");
                }
			}
            catch (HttpRequestException ex)
            {
                // Display error message if an exception occurred during the request
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
			dataBase.Updated = true;
		}

		private List<EmbedBuilder> CreateEmbedMessage(List<string> articles)
		{
			if (articles != null)
			{
				List<EmbedBuilder> embedArticles = new List<EmbedBuilder>();
				foreach(string article in articles)
				{
					string[] splitedArticleInfo = article.Split(',');
					string imageUrl = splitedArticleInfo.Length > 5 && splitedArticleInfo[5] != "none" ? splitedArticleInfo[5] : null;
					var embedBuilder = new EmbedBuilder()
					{
						Author = new EmbedAuthorBuilder{ Name = splitedArticleInfo[0].Length > 5 && splitedArticleInfo[0] != "none" ? splitedArticleInfo[0] : null },
						Title = splitedArticleInfo[1],
						Description = splitedArticleInfo[2],
						Url = splitedArticleInfo[3],
						Color = Color.Blue,
						ImageUrl = imageUrl,
						Timestamp = DateTimeOffset.Parse(splitedArticleInfo[6]),
					};
					embedArticles.Add(embedBuilder);
				}
				return embedArticles;
			}
			return null;

		}

		private async Task PublishEmbedArticles(List<EmbedBuilder> embedArticles, DiscordSocketClient _client)
		{
			var channelId = ulong.Parse(this.Channel);
			var channel = _client.GetChannel(channelId) as IMessageChannel;
			if (channel != null)
			{
				if (embedArticles != null)
				{
					foreach (var embed in embedArticles)
					{
						await channel.SendMessageAsync(embed: embed.Build());
					}
				}
				else
				{
					Console.WriteLine("Error EmbedArticles is null");
				}
			}
		}

		public async Task PublishArticlesAsync(DiscordSocketClient _client)
		{
			DateTime now = DateTime.Now;
			int hour = now.Hour;


			if (hour >= 8 && hour < 20)
			if (true)
			{
				string filePath = dataBase.GetFilePath();		
				string tempFilePath = "./database/tempnews.csv";
			
				while(true)
				{
					List<string>articles = new List<string>();
					using(StreamReader sr = new StreamReader(filePath))
					{
						using (StreamWriter sw = new StreamWriter(tempFilePath))
						{
							int i = 0;
							string line;
							while ((line = sr.ReadLine()) != null)
							{
								if (line.Contains("false") && i < this.MaxArticles)
								{
									line = line.Replace("false", "true");
									articles.Add(line);
									sw.WriteLine(line);
									i += 1;
								}
								else
								{
									sw.WriteLine(line);
								}
							}
						}
						sr.Close();
					}

					File.Delete(filePath);
					File.Move(tempFilePath, filePath);
					File.Delete(tempFilePath);

					List<EmbedBuilder> embedArticles = CreateEmbedMessage(articles);
					await PublishEmbedArticles(embedArticles, _client);
					
					articles.Clear();
					
					await Task.Delay(TimeSpan.FromHours(4));
				}
			}
		}
	}
}