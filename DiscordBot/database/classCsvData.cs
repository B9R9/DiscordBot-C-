using Newtonsoft.Json.Linq;


namespace DiscordBot
{
	public class CSVData
	{
		private List<string> Header { get; set; }
		private string FilePath { get; set; }
		private bool AppendFile { get; set; }
		private StreamWriter Sw { get; set; }
		public bool Updated {get; set; }

		public CSVData(string filePath, bool appendFile)
		{
			this.FilePath = filePath;
			this.AppendFile = appendFile;
			this.ConfigHeader();
			this.Updated = false;
			using (this.Sw = new StreamWriter(this.FilePath, this.AppendFile))
			{
				if (LineCount(this.FilePath) < 1)
				{
					this.CreateDataBase(this.Sw);
				}
			}
		}

		static int LineCount(string filePath)
		{
			int count = 0;

			using(StreamReader reader = new StreamReader(filePath))
			{
				while (reader.ReadLine() != null)
				{
					count += 1;
				}
				return count;
			}
		}

		public string GetFilePath()
		{
			return this.FilePath;
		}
		public void CreateDataBase(StreamWriter sw)
		{

			try
			{
				for (int i = 0; i < this.Header.Count; i++)
				{
					bool isLastOne = (i == this.Header.Count - 1);
					sw.Write(this.Header[i]);
					if (!isLastOne)
					{
						sw.Write(",");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				throw;
			}
		}
		public void ConfigHeader()
		{
			this.Header = new List<string>();
			this.Header.Add("author");
			this.Header.Add("title");
			this.Header.Add("description");
			this.Header.Add("url");
			this.Header.Add("source");
			this.Header.Add("image");
			this.Header.Add("published_at");
			this.Header.Add("published");
        }
		public void AddData(JArray articles)
		{

			using (StreamWriter sw = new StreamWriter(this.FilePath, true))
			{
				if (articles != null)
				{
					foreach(JObject article in articles)
					{
						int i = 0;
						bool addPublished = false;
						foreach (var item in article)
						{
							string strValue;
							string key = item.Key;
							JToken value = item.Value;
							string line = value.ToString();
							if (string.IsNullOrEmpty(line))
							{
								strValue = "none";
							}
							else
							{
								strValue = line.Replace("," , " ");
							}
	d
							if (this.Header.Contains(key))
							{
								sw.Write(strValue + ",");
								addPublished = true;
							}
							i++;
						}
						if (addPublished)
						{
							sw.Write("false");
							sw.WriteLine();
						}
					}
				}
			}
		}
		public void DeleteData()
		{
			try
			{
				string tempFilePath = "./database/tempnews.csv";
				using(StreamReader sr = new StreamReader(this.FilePath))
				{
					using(StreamWriter sw = new StreamWriter(tempFilePath))
					{
						// this.CreateDataBase(sw);
						string line = sr.ReadLine();
						sw.WriteLine(line);
						while ((line = sr.ReadLine()) != null)
						{
							if (this.IsNotOutDated(line) & IsNotThere(line))
							{
								sw.WriteLine(line);
							}
						}
					}
					sr.Close();
				}
				File.Delete(this.FilePath);
				File.Move(tempFilePath, this.FilePath);
				File.Delete(tempFilePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occured: {ex.Message}");
			}
		}
		public bool IsNotThere(string line)
		{
			using (StreamReader sr = new StreamReader(this.FilePath))
			{
				string reference;

				while ((reference = sr.ReadLine()) != null)
				{
					if (reference.Contains(line))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsNotOutDated(string line)
		{
			DateTime? date = Utils.ExtractDate(6, line);

			if (date == null)
			{
				return false;
			}
			DateTime _date = (DateTime)date;


			DateTime today = DateTime.Today;
			TimeSpan diff = today - _date;

			if (diff.TotalDays > 7)
			{
				return false;
			}
			return true;
		}
	}
}