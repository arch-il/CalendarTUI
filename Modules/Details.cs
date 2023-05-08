using CalendarTUI.TerminalGraphics;
using CalendarTUI.Miscellaneous;

namespace CalendarTUI.Modules;

public static class Details
{
	// list of things to redraw
	private static List<Action> drawCalls = new List<Action>();

	// save selected event
	private static EventNode savedEvent = new EventNode() { title = "" };

	// function to initialize data
	public static void Initialize()
	{
		// force call one update
		Update();

		// queue every draw
		QueueEveryDraw();
	}

	// function to update saved event when selected event changes
	public static void Update()
	{
		// check if any event is selected
		if (MainCalendar.selectedEvent < 0)
		{
			// check is saved selected event already cleared
			if (savedEvent.title != "")
			{
				// clear saved selected event
				savedEvent = new EventNode() { title = "" };

				// queue draw call
				drawCalls.Add(DrawEventDetails);
			}
			return;
		}
		// check if selected event has changed
		if (savedEvent != MainCalendar.events[MainCalendar.selectedEvent])
		{
			// update saved event
			savedEvent = MainCalendar.events[MainCalendar.selectedEvent];

			// queue draw call
			drawCalls.Add(DrawEventDetails);
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
		drawCalls.Add(DrawEventDetails);
	}

	// draw frame
	public static void DrawFrame()
	{
		// draw frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - Calendar.borderRight + 1,
			Calendar.borderTop + 11,
			Calendar.borderRight,
			25, //?
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);
	}

	// draw frame label
	public static void DrawFrameLabel()
	{
		// draw details label
		GraphicsManager.DrawText(
			"Details:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			11
		);
	}	

	// draw details of event
	public static void DrawEventDetails()
	{
		// clear section of the screen needed
		GraphicsManager.DrawRect(
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 12,
			Calendar.borderRight - 2,
			23, //?
			ConsoleColor.Black
		);

		// check if there is any event to draw
		if (savedEvent.title == "")
			return;
		
		// number of lines taken
		int lineCounter = 1; 

		// draw title
		GraphicsManager.DrawText(
			$"Title: {savedEvent.title}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// draw start time
		GraphicsManager.DrawText(
			$"From: {savedEvent.timingOptions.eventStartDate.ToString("HH:mm")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// draw end time
		GraphicsManager.DrawText(
			$"To:   {savedEvent.timingOptions.eventEndDate.ToString("HH:mm")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// draw duration
		// temp message to draw
		string tempMessage = "Duration: ";
		// temporarily save time difference
		TimeSpan diff = savedEvent.timingOptions.eventEndDate - savedEvent.timingOptions.eventStartDate;
		// check if there is hours to display
		if (diff.Hours > 0) 
		{
			// check if there is more than one hour
			if (diff.Hours > 1) tempMessage += $"{diff.Hours} hrs ";
			else tempMessage += $"{diff.Hours} hr ";
		}
		// check if there is minute ti display
		if (diff.Minutes > 0) 
		{
			// check if there is more than one minute
			if (diff.Minutes > 1) tempMessage += $"{diff.Minutes} mins";
			else tempMessage += $"{diff.Minutes} min";
		}
		// draw duration
		GraphicsManager.DrawText(
			tempMessage,
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// increment line counter again for empty space
		lineCounter++;

		// draw start date
		GraphicsManager.DrawText(
			$"Start Date: {savedEvent.timingOptions.eventStartDate.ToString("ddd dd.MM.yy").ToUpper()}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// draw end date
		GraphicsManager.DrawText(
			$"End Date:   {savedEvent.timingOptions.eventEndDate.ToString("ddd dd.MM.yy").ToUpper()}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// draw repetition type
		GraphicsManager.DrawText(
			$"Repeats: {Enum.GetName(typeof(TimingOptions.RepeatType), savedEvent.timingOptions.repeatType)}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			Calendar.borderTop + 11 + lineCounter
		);
		// increment line counter
		lineCounter++;

		// draw repetiton days //todo: T T is same tue thu
		// get temporary message
		tempMessage = "";
		switch (savedEvent.timingOptions.repeatType)
		{
			case TimingOptions.RepeatType.Weekly:
			{
				// cycle thru dates
				foreach (var date in savedEvent.timingOptions.selectedDates)
					tempMessage += Enum.GetName(typeof(DayOfWeek), date.DayOfWeek)[0] + ", ";
				// remove last 2 characters
				tempMessage = tempMessage.Substring(0, tempMessage.Length-2);
				break;
			}
			case TimingOptions.RepeatType.Monthly:
			{
				// cycle thru dates
				foreach (var date in savedEvent.timingOptions.selectedDates)
					tempMessage += date.Day + ", ";
				// remove last 2 characters
				tempMessage = tempMessage.Substring(0, tempMessage.Length-2);
				break;
			}
			case TimingOptions.RepeatType.Annualy:
			{
				// cycle thru dates
				foreach (var date in savedEvent.timingOptions.selectedDates)
					tempMessage += date.ToString("dd.MM") + ", ";
				// remove last 2 characters
				tempMessage = tempMessage.Substring(0, tempMessage.Length-2);
				break;
			}
			// if there are no repeat options clear message
			default:
				tempMessage = "";
				break;
		}
		if (tempMessage != "")
		{
			// draw desription
			List<string> lines = ("Every: " + tempMessage).MakePassage(Calendar.borderRight - 2);
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
						Calendar.borderTop + 11 + lineCounter
					);

					// increment line counter
					lineCounter++;
				}
			}
		}


		// draw description
		List<string> descriptionLines = ("Description: " + savedEvent.description).MakePassage(Calendar.borderRight - 2);
		// check if there is passage to write
		if (descriptionLines.Count > 1 || descriptionLines[0] != "Description: ")
		{
			// increment line counter
			lineCounter++;
			// draw line by line
			foreach (var line in descriptionLines)
			{
				GraphicsManager.DrawText(
					line,
					ConsoleColor.Gray,
					ConsoleColor.Black,
					GraphicsManager.width - Calendar.borderRight + 2,
					Calendar.borderTop  + 11 + lineCounter
				);

				// increment line counter
				lineCounter++;
			}
		}
	}
}