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

		// initialize main calendar
		MainCalendar.Initialize();

		// initialize month calendar
		MonthCalendar.Initialize();

		// initialize current date
		CurrentTime.Initialize();

		// initialize current event
		CurrentEvent.Initialize();

		// initalize details
		Details.Initialize();
	}


	// function for taking input and updating state
	public static void Update()
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
					return;

				// refresh input
				case ConsoleKey.R:
					// clear screen
					Console.Clear();
					// ask program to redraw everything
					MainCalendar.QueueEveryDraw();
					MonthCalendar.QueueEveryDraw();
					CurrentTime.QueueEveryDraw();
					CurrentEvent.QueueEveryDraw();
					Details.QueueEveryDraw();
					return;
			}
			
			// call update functions
			MainCalendar.Update(key.Key);
			MonthCalendar.Update();
			CurrentTime.Update();
			CurrentEvent.Update();
			Details.Update();
			
			// exit
			return;
		}
		
		// these need to be updated anyways
		MainCalendar.Update();
		CurrentTime.Update();
		CurrentEvent.Update();
	}


	// function for drawing calendar on screen
	public static void Draw()
	{
		// draw main calendar
		MainCalendar.Draw();

		// draw small calendar
		MonthCalendar.Draw();

		// draw current time
		CurrentTime.Draw();

		// draw current event
		CurrentEvent.Draw();
		
		// draw details
		Details.Draw();
	}

	// function for parsing events form json file
	public static void ReadEventsFromFile()
	{
		try
		{
			// read events from file
			using (StreamReader sr = new StreamReader(filePath))
				MainCalendar.events = JsonSerializer.Deserialize<List<EventNode>>(sr.ReadToEnd());
			// sort events using start time
			MainCalendar.events.Sort((a, b) => a.timingOptions.eventStartDate.ToString("HH:mm").CompareTo(b.timingOptions.eventStartDate.ToString("HH:mm")));
		}
		catch (Exception ex)
		{
			// clear screen
			Console.Clear();

			// notify user that parsing json failed
			Console.SetCursorPosition(0, 0);
			Console.WriteLine($"Something went wrong wile reading from json file!");
			Console.WriteLine(ex.Message);

			// wait for user input
			Console.Write("Press enter for exit...");
			Console.ReadLine();

			// exit program
			Environment.Exit(1);
		}
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
