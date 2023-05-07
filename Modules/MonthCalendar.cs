using CalendarTUI.TerminalGraphics;

namespace CalendarTUI.Modules;

public static class MonthCalendar
{
	// culture info
	private static System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("de-DE"); // culture info


	//?
	public static void Update()
	{

	}

	// function for drawing small month calendar
	public static void Draw()
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

		// draw calendar label
		GraphicsManager.DrawText(
			"Calendar:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			GraphicsManager.height - 11
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

		// draw day of year
		GraphicsManager.DrawText(
			$"D:{cultureInfo.Calendar.GetDayOfYear(MainCalendar.startDate.AddDays(MainCalendar.cursorPosition))}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			GraphicsManager.height - 9
		);

		// draw week labels
		GraphicsManager.DrawText(
			" #|  M  T  W  T  F  S  S",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			GraphicsManager.height - 8
		);

		// draw days
		// set starting date to 1st of current month
		DateTime tempDate = new DateTime(MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).Year, MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).Month, 1);
		// set starting date to monday
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
				// save colors to change them
				ConsoleColor dateForegroundColor = ConsoleColor.White;
				ConsoleColor dateBackgroundColor = ConsoleColor.Black;
				// check if date is not in current month
				if (tempDate.Month != MainCalendar.startDate.AddDays(MainCalendar.cursorPosition).Month)
					dateForegroundColor = ConsoleColor.DarkGray;
				// check if date is today
				if (tempDate == DateTime.Today)
					dateBackgroundColor = ConsoleColor.DarkRed;
				// check if date is selected
				if (tempDate == MainCalendar.startDate.AddDays(MainCalendar.cursorPosition))
					dateForegroundColor = ConsoleColor.DarkGreen;

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