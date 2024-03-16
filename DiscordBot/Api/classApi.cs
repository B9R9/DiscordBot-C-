using System.Text.Json;
using System;

namespace DiscordBot
{
	public class Api{
		private string Name { get; set; } = "";
		private string Token { get; set; } = "";
		public Api(string name){
			try
			{
				using (StreamReader reader = new StreamReader("Configuration/configS.json"))
				{
					string jsonContent = reader.ReadToEnd();
					var configData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
					
					if (configData != null)
					{
                        Name = name;
                        Token = configData[name];
					}
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Le fichier de configuration configS.json n'a pas été trouvé.");
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Une erreur s'est produite lors de la lecture du fichier : {ex.Message}");
			}
			catch (JsonException ex)
            {
                Console.WriteLine($"Une erreur s'est produite lors de la désérialisation du JSON : {ex.Message}");
            }
		}
        public override string ToString() => $"You create {this.Name} API";

		public string GetToken(){
			return Token;
		}
		public string GetName(){
			return Name;
		}		
    }
}