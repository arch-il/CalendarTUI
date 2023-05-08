using CalendarTUI.TerminalGraphics;

namespace CalendarTUI.Modules;

public static class CurrentEvent
{
	// saved checked time
	private static DateTime savedTime;
	// event details
	private static string eventTitle = "";     // even title
	private static string eventStartTime = ""; // event start time
	private static string eventEndTime = "";   // event end time
	private static string timeLeft = "";       // time left before event is over


	// list of things to redraw
	private static List<Action> drawCalls = new List<Action>();

	
	// function for initializing data
	public static void Initialize()
	{
		// update saved time
		savedTime = new DateTime();

		// queue every draw
		QueueEveryDraw();
	}

	// function for updating drawing state
	public static void Update()
	{
		// check if minute has changed
		if (savedTime.Minute != DateTime.Now.Minute)
		{
			// update saved time
			savedTime = DateTime.Now;

			// set everything to zero
			eventTitle = "";
			eventStartTime = "";
			eventEndTime = "";
			timeLeft = "";

			// cycle thru each event
			foreach (var tempEvent in MainCalendar.events)
			{
				// get timeMargins
				var timeMargins = tempEvent.timingOptions.GetTimeMargins(DateTime.Today, DateTime.Today.AddDays(1));
				// cycle thru each time Margin
				foreach (var timeMargin in timeMargins)
					// check current time is between time margins
					if (timeMargin.Item1 <= DateTime.Now && timeMargin.Item2 >= DateTime.Now)
					{
						// update event information
						eventTitle = tempEvent.title;
						eventStartTime = timeMargin.Item1.ToString("HH:mm");
						eventEndTime = timeMargin.Item2.ToString("HH:mm");
						// temporarily save time difference
						TimeSpan diff = timeMargin.Item2 - DateTime.Now; 
						// check if there is hours to display
						if (diff.Hours > 0) 
						{
							// check if there is more than one hour
							if (diff.Hours > 1) timeLeft += $"{diff.Hours} hrs ";
							else timeLeft += $"{diff.Hours} hr ";
						}
						// check if there is minute ti display
						if (diff.Minutes > 0) 
						{
							// check if there is more than one minute
							if (diff.Minutes > 1) timeLeft += $"{diff.Minutes} mins";
							else timeLeft += $"{diff.Minutes} min";
						}

						break;
					}
			}

			// queue required draw call
			drawCalls.Add(DrawCurrentEvent);
		}
	}

	// function for drawing current time
	public static void Draw()
	{
		// draw each segment that is in draw call
		foreach (var drawCall in drawCalls)
			drawCall();
		// clear draw call list
		drawCalls.Clear();
	}

	// redraw everything on screen
	public static void QueueEveryDraw()
	{
		// clear draw call list
		drawCalls.Clear();
		// add everything
		drawCalls.Add(DrawFrame);
		drawCalls.Add(DrawFrameLabel);
		drawCalls.Add(DrawCurrentEvent);
	}

	// draw frame
	public static void DrawFrame()
	{
		// draw frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - Calendar.borderRight + 1,
			5,
			Calendar.borderRight,
			6,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);
	}

	// draw frame label
	public static void DrawFrameLabel()
	{
		// draw label
		GraphicsManager.DrawText(
			"Current Event:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			4
		);
	}

	// draw current event
	public static void DrawCurrentEvent()
	{
		// clear area
		GraphicsManager.DrawRect(
			GraphicsManager.width - Calendar.borderRight + 3,
			6,
			Calendar.borderRight - 4,
			4,
			ConsoleColor.Black
		);

		// check if no event is going right now
		if (eventTitle == "")
		{
			// draw message
			GraphicsManager.DrawText(
				$"You are free!",
				ConsoleColor.Gray,
				ConsoleColor.Black,
				GraphicsManager.width - Calendar.borderRight + 3,
				6
			);
			
			// exit function
			return;
		}

		// draw event title
		GraphicsManager.DrawText(
			$"Title: {eventTitle}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			6
		);

		// draw event start time
		GraphicsManager.DrawText(
			$"From: {eventStartTime}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			7
		);

		// draw event end time
		GraphicsManager.DrawText(
			$"To:   {eventEndTime}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			8
		);

		// draw time left
		GraphicsManager.DrawText(
			$"Time Left: {timeLeft}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			9
		);
	}
}