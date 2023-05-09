using CalendarTUI.TerminalGraphics;

namespace CalendarTUI.Modules;

public static class MonthCalendar
{
	// culture info
	private static System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("de-DE"); // culture info

	// selected date
	private static DateTime selectedDate = new DateTime(); 

	// list of things to redraw
	private static List<Action> drawCalls = new List<Action>();

	
	// function to initialize data
	public static void Initialize()
	{
		// ask program to draw everything
		QueueEveryDraw();
	}


	// update using input
	public static void Update()
	{
		// check if date has been updated
		if (selectedDate != MainCalendar.startDate.AddDays(MainCalendar.cursorPosition))
		{
			// update date
			selectedDate = MainCalendar.startDate.AddDays(MainCalendar.cursorPosition);

			// queue required draws
			drawCalls.Add(DrawTopLabel);
			drawCalls.Add(DrawDays);
		}	
	}

	// function for drawing month calendar
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
		drawCalls.Add(DrawTopLabel);
		drawCalls.Add(DrawWeekLabels);
		drawCalls.Add(DrawDays);
	}

	// draw frame
	public static void DrawFrame()
	{
		// draw frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - Calendar.borderRight + 1,
			GraphicsManager.height - 10,
			Calendar.borderRight,
			10,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);
	}

	// draw calendar label
	public static void DrawFrameLabel()
	{
		// draw calendar label
		GraphicsManager.DrawText(
			"Calendar:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			GraphicsManager.height - 11
		);
	}

	// draw day month and year
	public static void DrawTopLabel()
	{
		// clear needed space
		GraphicsManager.DrawText(
			new string(' ', Calendar.borderRight - 4),
			ConsoleColor.Black,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			GraphicsManager.height - 9
		);

		// draw day of year
		GraphicsManager.DrawText(
			$"D:{cultureInfo.Calendar.GetDayOfYear(MainCalendar.startDate.AddDays(MainCalendar.cursorPosition))}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			GraphicsManager.height - 9
		);

		// draw current month
		GraphicsManager.DrawText(
			MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).ToString("MMMM").ToUpper(),
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 15 - MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).ToString("MMMM").Count()/2,
			GraphicsManager.height - 9
		);

		// draw current year
		GraphicsManager.DrawText(
			$"Y:{MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).ToString("yyyy")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - 7,
			GraphicsManager.height - 9
		);
	}

	// draw week labels
	public static void DrawWeekLabels()
	{
		// draw week labels
		GraphicsManager.DrawText(
			" #|  M  T  W  T  F  S  S",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			GraphicsManager.height - 8
		);
	}

	// draw days of month
	public static void DrawDays()
	{
		// clear needed space
		GraphicsManager.DrawRect(
			GraphicsManager.width - Calendar.borderRight + 3,
			GraphicsManager.height - 7,
			Calendar.borderRight - 3,
			6,
			ConsoleColor.Black
		);

		// draw days
		// set starting date to 1st of current month
		DateTime tempDate = new DateTime(MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).Year, MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).Month, 1);
		// set starting date to monday
		// check if its sunday
		if (tempDate.DayOfWeek == DayOfWeek.Sunday)
			tempDate = tempDate.AddDays(-6);
		else
			tempDate = tempDate.AddDays((int)DayOfWeek.Monday - (int)tempDate.DayOfWeek);
		
		// draw line by line
		for (int i = 0; i < 6; i++)
		{
			// get week number using culture info
			int weekNumber = cultureInfo.Calendar.GetWeekOfYear(
					tempDate,
					cultureInfo.DateTimeFormat.CalendarWeekRule,
					cultureInfo.DateTimeFormat.FirstDayOfWeek);
			// draw week number
			GraphicsManager.DrawText(
				$"{(weekNumber / 10 > 0 ? "" : " ")}{weekNumber}|",
				ConsoleColor.Gray,
				ConsoleColor.Black,
				GraphicsManager.width - Calendar.borderRight + 3,
				GraphicsManager.height - 7 + i
			);
			
			// draw day by day
			for (int j = 0; j < 7; j++)
			{
				// check if date is selected
				if (tempDate == MainCalendar.startDate.AddDays(MainCalendar.cursorPosition))
					// draw box around selected date
					GraphicsManager.DrawText(
						"[  ]",
						ConsoleColor.DarkGreen,
						ConsoleColor.Black,
						GraphicsManager.width - Calendar.borderRight + 6 + 3*j,
						GraphicsManager.height - 7 + i
					);
				
				// save colors to change them
				ConsoleColor dateForegroundColor = ConsoleColor.White;
				ConsoleColor dateBackgroundColor = ConsoleColor.Black;
				// check if date is not in current month
				if (tempDate.Month != MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).Month)
					dateForegroundColor = ConsoleColor.DarkGray;
				// check if date is today
				if (tempDate == DateTime.Today)
					dateBackgroundColor = ConsoleColor.DarkRed;

				// draw date
				GraphicsManager.DrawText(
					$"{tempDate.Day}",
					dateForegroundColor,
					dateBackgroundColor,
					GraphicsManager.width - Calendar.borderRight + 7 + 3*j + (tempDate.Day / 10 > 0 ? 0 : 1),
					GraphicsManager.height - 7 + i
				);

				// increment temp date
				tempDate = tempDate.AddDays(1);
			}
		}
	}
}