using System.Text.RegularExpressions;

namespace DiscordBot
{
	public static class Utils
	{
		public static int GetCharInPosition(char lookingFor, int position, string container)
		{
			int counter = 0;
			for (int i = 0; i < container.Length; i++)
			{
				if (container[i] == lookingFor)
				{
					counter += 1;
					if (counter == position)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static DateTime? ExtractDate(int index, string line)
		{
			string value = "";
			int start = GetCharInPosition(',', index, line) + 1;
			if (start < 0 || start >= line.Length || (start + 10)  >= line.Length)
  	 	 	{
   	    		throw new ArgumentException($"Invalid start or end index {start} || {line.Length}");
    		}
			value = line.Substring(start, 10);
			
			Regex regex = new Regex(@"\b\d{1,2}/\d{1,2}/\d{4}\b");

        	// Recherche de la date dans le texte
        	Match match = regex.Match(value);
			if (match.Success)
			{
				if(DateTime.TryParse(value, out DateTime date))
				{
					return date;
				}
			}
			return null;
		}
	}
}