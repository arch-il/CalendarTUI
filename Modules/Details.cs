using CalendarTUI.TerminalGraphics;
using CalendarTUI.Miscellaneous;

namespace CalendarTUI.Modules;

public static class Details
{
	//?
	public static void Update()
	{

	}

	// function for drawing details
	public static void Draw()
	{
		return;
		// draw details frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - Calendar.borderRight + 1,
			Calendar.borderTop,
			Calendar.borderRight,
			GraphicsManager.height - Calendar.borderTop - 11,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);

		// draw details label
		GraphicsManager.DrawText(
			"Details:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			0
		);

		// check if any event is selected
		if (MainCalendar.selectedEvent < 0)
			return;

		// number of lines taken
		int lineCounter = 1; 

		// draw title
		GraphicsManager.DrawText(
			$"Title: {MainCalendar.events[MainCalendar.selectedEvent].title}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw start time
		GraphicsManager.DrawText(
			$"From: {MainCalendar.events[MainCalendar.selectedEvent].timingOptions.eventStartDate.ToString("HH:mm")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw end time
		GraphicsManager.DrawText(
			$"To:   {MainCalendar.events[MainCalendar.selectedEvent].timingOptions.eventEndDate.ToString("HH:mm")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// get duration
		TimeSpan duration = (MainCalendar.events[MainCalendar.selectedEvent].timingOptions.eventEndDate.TimeOfDay - MainCalendar.events[MainCalendar.selectedEvent].timingOptions.eventStartDate.TimeOfDay);
		// draw duration
		GraphicsManager.DrawText(
			$"Duration: {duration.Hours} hrs {duration.Minutes} mins",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw desription
		List<string> lines = ("Description: " + MainCalendar.events[MainCalendar.selectedEvent].description).MakePassage(Calendar.borderRight - 2);
		// check if there is passage to write
		if (lines.Count > 1 || lines[0] != "Description: ")
		{
			// increment line counter
			lineCounter++;
			// draw line by line
			foreach (var line in lines)
			{
				GraphicsManager.DrawText(
					line,
					ConsoleColor.Gray,
					ConsoleColor.Black,
					GraphicsManager.width - Calendar.borderRight + 2,
					Calendar.borderTop + lineCounter
				);

				// increment line counter
				lineCounter++;
			}
		}
		
		// increment line counter
		lineCounter++;

		// draw start date
		GraphicsManager.DrawText(
			$"Start Date: {MainCalendar.events[MainCalendar.selectedEvent].timingOptions.eventStartDate.ToString("ddd dd.MM.yy")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw end date
		GraphicsManager.DrawText(
			$"End Date:   {MainCalendar.events[MainCalendar.selectedEvent].timingOptions.eventEndDate.ToString("ddd dd.MM.yy")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw full day status
		GraphicsManager.DrawText(
			$"Full Day: {(MainCalendar.events[MainCalendar.selectedEvent].timingOptions.isFullDay ? "Yes" : "No")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw repetition type
		GraphicsManager.DrawText(
			$"Repeats: {Enum.GetName(typeof(TimingOptions.RepeatType), MainCalendar.events[MainCalendar.selectedEvent].timingOptions.repeatType)}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw repetiton days
		// get temporary message
		string message = "";
		switch (MainCalendar.events[MainCalendar.selectedEvent].timingOptions.repeatType)
		{
			case TimingOptions.RepeatType.Weekly:
			{
				// cycle thru dates
				foreach (var date in MainCalendar.events[MainCalendar.selectedEvent].timingOptions.selectedDates)
					message += Enum.GetName(typeof(DayOfWeek), date.DayOfWeek)[0] + ", ";
				// remove last 2 characters
				message = message.Substring(0, message.Length-2);
				break;
			}
			case TimingOptions.RepeatType.Monthly:
			{
				// cycle thru dates
				foreach (var date in MainCalendar.events[MainCalendar.selectedEvent].timingOptions.selectedDates)
					message += date.Day + ", ";
				// remove last 2 characters
				message = message.Substring(0, message.Length-2);
				break;
			}
			case TimingOptions.RepeatType.Annualy:
			{
				// cycle thru dates
				foreach (var date in MainCalendar.events[MainCalendar.selectedEvent].timingOptions.selectedDates)
					message += date.ToString("dd.MM") + ", ";
				// remove last 2 characters
				message = message.Substring(0, message.Length-2);
				break;
			}
			// if there are no repeat options clear message
			default:
				message = "";
				break;
		}
		if (message != "")
		{
			// draw desription
			lines = ("Every: " + message).MakePassage(Calendar.borderRight - 2);
			// check if there is passage to write
			if (lines.Count > 1 || lines[0] != "Every: ")
			{
				// draw line by line
				foreach (var line in lines)
				{
					GraphicsManager.DrawText(
						line,
						ConsoleColor.Gray,
						ConsoleColor.Black,
						GraphicsManager.width - Calendar.borderRight + 2,
						Calendar.borderTop + lineCounter
					);

					// increment line counter
					lineCounter++;
				}
			}
		}
	}
}