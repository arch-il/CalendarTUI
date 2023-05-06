namespace CalendarTUI.Miscellaneous;

public static class PassageMaker
{
	public static List<string> MakePassage(this string message, int lineLimit)
	{
		//! add check if word is too big
		// create list to return
		List<string> result = new List<string>();
		result.Add("");
		// cycle thru words
		foreach (var word in message.Split(' '))
		{
			// check if next word will be too long for this line
			if ((result.Last() + " " + word).Count() > lineLimit)
			{
				// add new line
				result.Add(word);
			}
			else
				result[result.Count-1] += " " + word;
		}
		// check if there is extra space
		if (result[0][0] == ' ')
			result[0] = result[0].Substring(1, result[0].Length-1);
		// return result
		return result;
	}
}