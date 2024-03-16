using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.WebSocket;
using System.Text;

namespace DiscordBot
{
	public class LLMResponse
	{
		[JsonPropertyName("model")]
 		public string Model { get; set; }

	    [JsonPropertyName("created_at")]
 		public DateTimeOffset CreatedAt { get; set; }

	    [JsonPropertyName("response")]
	    public string Response { get; set; }

	    [JsonPropertyName("done")]
	    public bool Done { get; set; }

	    [JsonPropertyName("context")]
	    public List<int> Context { get; set; }

	    [JsonPropertyName("total_duration")]
	    public long TotalDuration { get; set; }

	    [JsonPropertyName("load_duration")]
	    public long LoadDuration { get; set; }

	    [JsonPropertyName("prompt_eval_count")]
	    public int PromptEvalCount { get; set; }

	    [JsonPropertyName("prompt_eval_duration")]
	    public long PromptEvalDuration { get; set; }

	    [JsonPropertyName("eval_count")]
	    public int EvalCount { get; set; }

	    [JsonPropertyName("eval_duration")]
	    public long EvalDuration { get; set; }

		public string Endpoint { get; set; }

		public string Payload { get; set; }

		public LLMResponse(string model, string endpoint)
		{
			this.Model = model;
			this.Endpoint = endpoint;
		}

		public void SetPayLoad(string message)
		{
			this.Payload =$"{{\"model\": \"{this.Model}\", \"prompt\": \"{message}\", \"stream\": false}}"; 
		}

        public static LLMResponse FromJson(string jsonString)
        {
            return JsonSerializer.Deserialize<LLMResponse>(jsonString);
        }

		public void PrintDetails()
        {
            Console.WriteLine($"Model: {Model}");
            Console.WriteLine($"Created At: {CreatedAt}");
            Console.WriteLine($"Response: {Response}");
            Console.WriteLine($"Done: {Done}");
            Console.WriteLine($"Context: [{string.Join(", ", Context)}]");
            Console.WriteLine($"Total Duration: {TotalDuration}");
            Console.WriteLine($"Load Duration: {LoadDuration}");
            Console.WriteLine($"Prompt Eval Count: {PromptEvalCount}");
            Console.WriteLine($"Prompt Eval Duration: {PromptEvalDuration}");
            Console.WriteLine($"Eval Count: {EvalCount}");
            Console.WriteLine($"Eval Duration: {EvalDuration}");
        }


		public async Task SendMessageAsync(SocketUserMessage message)
		{
			var channel = message.Channel;
			await channel.SendMessageAsync(this.Response);
		}

		public async Task<LLMResponse> SendRequestAsync(string message)
		{
			using (var httpClient = new HttpClient())
			try
			{
				// Configuration de l'en-tête Content-Type
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
		        
				this.SetPayLoad(message);
				// Création du contenu de la requête
                var content = new StringContent(this.Payload, Encoding.UTF8, "application/json");
				
				// Envoi de la requête POST
                var response = await httpClient.PostAsync(Endpoint, content);
				string responseBody = await response.Content.ReadAsStringAsync();

				var llmResponse = FromJson(responseBody);
				if (llmResponse.Done == true)
				{
					return llmResponse;
				}
				else
                {
                    // Affichage du code d'erreur en cas d'échec de la requête
                    Console.WriteLine("La requête a échoué. Code d'erreur : " + response.StatusCode);
                    return null;
            	}
			}
			catch (Exception ex)
			{
            	// Gestion des erreurs
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
                return null;
			}
		}
	}
}