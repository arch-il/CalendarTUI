using System.Text.Json;
using CalendarTUI.Miscellaneous;
using CalendarTUI.Modules;
using CalendarTUI.TerminalGraphics;

namespace CalendarTUI;

public static class Calendar
{
	public static string filePath { get; set; } // json file where events are saved
	
	public static int borderLeft = 5;   // extra space needed on the left side of the screen
	public static int borderRight = 28; // extra space needed on the right side of the screen
	public static int borderTop = 1;    // extra space needed on the top side of the screen
	public static int borderBottom = 0; // extra space needed on the bottom side of the screen	


	

	// simple constructor
	public static void Initialize(string filepath)
	{
		// set value
		filePath = filepath;
		
		// parse events from file
		ReadEventsFromFile();

		// initialize main claendar
		MainCalendar.Initialize();
	}


	// function for taking input and updating state
	public static bool Update()
	{
		// check if key is available
		if (Console.KeyAvailable)
		{
			// get input
			var key = Console.ReadKey(true);

			switch(key.Key)
			{
				// exit input
				case ConsoleKey.Escape:
					Environment.Exit(0);
					return false;

				// refresh input //todo:
				case ConsoleKey.R:
					MainCalendar.QueueEveryDraw();
					return true;
			}
			
			MainCalendar.Update(key.Key);
			MonthCalendar.Update();
			Details.Update();
	
			
			// return update status true
			return true;
		}
		// return update status false
		return false;
	}


	// function for drawing calendar on screen
	public static void Draw()
	{
		// draw main calendar
		MainCalendar.Draw();

		// draw details
		Details.Draw();

		// draw small calendar
		MonthCalendar.Draw();
	}

	// function for parsing events form json file
	public static void ReadEventsFromFile()
	{
		// read events from file
		using (StreamReader sr = new StreamReader(filePath))
			MainCalendar.events = JsonSerializer.Deserialize<List<EventNode>>(sr.ReadToEnd());
		// sort events using start time
		MainCalendar.events.Sort((a, b) => a.timingOptions.eventStartDate.ToString("HH:mm").CompareTo(b.timingOptions.eventStartDate.ToString("HH:mm")));
	}

	// function for parsing events into json file
	public static void WriteEventsToFile()
	{
		// sort events using start time
		MainCalendar.events.Sort((a, b) => a.timingOptions.eventStartDate.CompareTo(b.timingOptions.eventStartDate));
		// write events to file
		using (StreamWriter sw = new StreamWriter(filePath, false))
			JsonSerializer.Serialize<List<EventNode>>(sw.BaseStream, MainCalendar.events);
	}
}
