using System.Text.Json;
using CalendarTUI.Miscellaneous;
using CalendarTUI.TerminalGraphics;

namespace CalendarTUI;

public class Calendar
{
	public string filePath { get; set; } // json file where events are saved
	private System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("de-DE"); // culture info

	public int borderLeft = 5;   // extra space needed on the left side of the screen
	public int borderRight = 28; // extra space needed on the right side of the screen
	public int borderTop = 1;    // extra space needed on the top side of the screen
	public int borderBottom = 0; // extra space needed on the bottom side of the screen	


	private List<EventNode> events = new List<EventNode>(); // list of events

	// enum for View Mode
	private enum View
	{
		Day,
		FourDays,
		Week
	}

	// View details
	private View currentView = View.Week; // current View
	private int segmentCount = 7;         // number of segments
	private int cursorPosition;           // location of selected cell
	private int selectedEvent;            // selected event by cursor
	private DateTime startDate;           // view start date
	private DateTime endDate;             // view end date


	// simple constructor
	public Calendar(string filePath)
	{
		// set value
		this.filePath = filePath;
		
		// parse events from file
		ReadEventsFromFile();

		// set start date to today
		startDate = DateTime.Today;
		// set to monday
		startDate = startDate.AddDays((int)DayOfWeek.Monday - (int)startDate.DayOfWeek);
		// set end date a week later
		endDate = startDate.AddDays(7);
		// set cursor position to today
		cursorPosition = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;
		// update time margins
		for (int i = 0; i < events.Count; i++)
			events[i].timingOptions.UpdateTimeMargins(startDate, endDate);
		// update selected event positon
		UpdateSelectedEvent();
	}


	// function for taking input and updating state
	public bool Update()
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

				// refresh input
				case ConsoleKey.R:
					return true;

				// change view keys
				// day
				case ConsoleKey.D:
				{
					// check if view is already set to day
					if (currentView == View.Day)
						return false;
					// update current view and segment count
					currentView = View.Day;
					segmentCount = 1;
					// set end date a day later
					endDate = startDate.AddDays(1);
					// check if cursor is still on screen
					if (cursorPosition >= segmentCount-1)
						cursorPosition = 0;
					break;
				}
				// four days
				case ConsoleKey.X:
				{
					// check if view is already set to four days
					if (currentView == View.FourDays)
						return false;
					// update current view and segment count	
					currentView = View.FourDays;
					segmentCount = 4;
					// set end date 4 days later
					endDate = startDate.AddDays(4);
					// check if cursor is still on screen
					if (cursorPosition >= segmentCount-1)
						cursorPosition = 0;
					break;
				}
				// week
				case ConsoleKey.W:
				{
					// check if view is already set to week
					if (currentView == View.Week)
						return false;
					// update current view and segment count
					currentView = View.Week;
					segmentCount = 7;
					// set to monday
					startDate = startDate.AddDays((int)DayOfWeek.Monday - (int)startDate.DayOfWeek);
					// set end date a week later
					endDate = startDate.AddDays(7);
					// check if cursor is still on screen
					if (cursorPosition >= segmentCount-1)
						cursorPosition = 0;
					break;
				}

				// move to today
				case ConsoleKey.T:
				{
					// check if view and cursor is already on today
					if (startDate == DateTime.Today && (cursorPosition == (DateTime.Today - startDate.Date).Days))
						return false;
					// same thing but for week view
					if (currentView == View.Week && (cursorPosition == (DateTime.Today - startDate.Date).Days) &&
						DateTime.Today >= startDate && DateTime.Today < endDate)
						return false;
					// set start date to today
					startDate = DateTime.Today;
					// if view is week move to monday
					if (currentView == View.Week)
						startDate = startDate.AddDays((int)DayOfWeek.Monday - (int)startDate.DayOfWeek);
					// move end date by segment count
					endDate = startDate.AddDays(segmentCount);
					// update cursor positon
					cursorPosition = (DateTime.Today - startDate.Date).Days;
					break;
				}
				
				// event selection movements
				// move up
				case ConsoleKey.M:
				{
					// check if any events are today
					if (selectedEvent == -1)
						return false;
					// check above current selected event
					for (int i = selectedEvent-1; i >= 0; i--)
						// check if that event ocurs today
						if (events[i].timingOptions.timeMargins.Count(x => x.Item1.Date == startDate.AddDays(cursorPosition)) > 0)
						{
							// update selected event
							selectedEvent = i;
							// return update status true
							return true;
						}
					// return update status false
					return false;
				}
				// move down
				case ConsoleKey.N:
				{
					// check if any events are today
					if (selectedEvent == -1)
						return false;
					// check below current selected event
					for (int i = selectedEvent+1; i < events.Count; i++)
						// check if that event ocurs today
						if (events[i].timingOptions.timeMargins.Count(x => x.Item1.Date == startDate.AddDays(cursorPosition)) > 0)
						{
							// update selected event
							selectedEvent = i;
							// return update status true
							return true;
						}
					// return update status false
					return false;
				}
				
				// date movements
				// move left
				case ConsoleKey.LeftArrow:
				case ConsoleKey.H:
				{
					// check if cursor is already at the edge
					if (cursorPosition > 0)
						cursorPosition--;
					else
						return false;
					break;
				}
				// move right
				case ConsoleKey.RightArrow:
				case ConsoleKey.L:
				{
					// check if cursor is already at the edge
					if (cursorPosition < segmentCount-1)
						cursorPosition++;
					else
						return false;
					break;
				}
				// move up
				case ConsoleKey.UpArrow:
				case ConsoleKey.K:
				{
					switch (currentView)
					{
						// move by day
						case View.Day:
							startDate = startDate.AddDays(-1);
							endDate = endDate.AddDays(-1);
							break;
						// move by four days
						case View.FourDays:
							startDate = startDate.AddDays(-4);
							endDate = endDate.AddDays(-4);
							break;
						// move by week
						case View.Week:
							startDate = startDate.AddDays(-7);
							endDate = endDate.AddDays(-7);
							break;
					}
					break;
				}
				// move down
				case ConsoleKey.DownArrow:
				case ConsoleKey.J:
				{
					switch (currentView)
					{
						// move by day
						case View.Day:
							startDate = startDate.AddDays(1);
							endDate = endDate.AddDays(1);
							break;
						// move by four days
						case View.FourDays:
							startDate = startDate.AddDays(4);
							endDate = endDate.AddDays(4);
							break;
						// move by week
						case View.Week:
							startDate = startDate.AddDays(7);
							endDate = endDate.AddDays(7);
							break;
					}
					break;
				}

					// other keys
				default:
					// no input -> no update
					return false;
			}
			// update time margins
			for (int i = 0; i < events.Count; i++)
				events[i].timingOptions.UpdateTimeMargins(startDate, endDate);
			// update selected event
			UpdateSelectedEvent();
			
			// return update status true
			return true;
		}
		// return update status false
		return false;
	}


	// function for drawing calendar on screen
	public void Draw()
	{
		// clear screen
		GraphicsManager.Clear();

		// draw main calendar
		DrawCalendar();

		// draw details
		DrawDetails();

		// draw small calendar
		DrawMonthCalendar();
	}

	// function for drawing main calendar
	public void DrawCalendar()
	{
		// save current time cursor position
		int cursorX = 0;
		int cursorY = 0;
		// save current time cursor colors if it overlaps something
		ConsoleColor cursorForegroundColor = ConsoleColor.DarkRed;
		ConsoleColor cursorBackgroundColor = ConsoleColor.Black;

		// draw logo
		GraphicsManager.DrawText("^_^", ConsoleColor.White, ConsoleColor.Black, 1, 0);
		// draw frame around logo
		GraphicsManager.DrawText("|", ConsoleColor.DarkGray, ConsoleColor.Black, 5, 0);
		GraphicsManager.DrawText("-----", ConsoleColor.DarkGray, ConsoleColor.Black, 0, 1);
		// draw line at the end
		GraphicsManager.DrawText("-----", ConsoleColor.DarkGray, ConsoleColor.Black, 0, GraphicsManager.height-1);

		// draw frame
		GraphicsManager.DrawRectFrame(
			borderLeft,
			borderTop,
			GraphicsManager.width - borderLeft - borderRight,
			48+2,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);

		// draw times
		for (int j = 0; j < 48; j++)
		{
			if (j == 2*DateTime.Now.Hour + DateTime.Now.Minute/30)
			{
				// save cursor y
				cursorY = borderTop + 1 + j;
				// draw time one the side of the screen
				GraphicsManager.DrawText(
					$"{(j / 2 < 10 ? $"0{j/2}" : $"{j/2}")}:{(j % 2 == 0 ? "00" : "30")}",
					ConsoleColor.Gray,
					ConsoleColor.DarkRed,
					0,
					borderTop + 1 + j
				);
				continue;
			}
			// draw time one the side of the screen
			GraphicsManager.DrawText(
				$"{(j / 2 < 10 ? $"0{j/2}" : $"{j/2}")}:{(j % 2 == 0 ? "00" : "30")}",
				ConsoleColor.DarkGray,
				ConsoleColor.Black,
				0,
				borderTop + 1 + j
			);
		}

		// draw day segments
		for (int i = 0; i < segmentCount; i++)
		{
			// get temporary cell date
			DateTime cellDate = startDate.AddDays(i).Date;
			
			// create temporary date foreground color
			ConsoleColor dateForegroundColor = ConsoleColor.DarkGray;
			// check if this date is selected date
			if (cellDate == startDate.AddDays(cursorPosition))
				dateForegroundColor = ConsoleColor.Green;
			// check if this date is today
			else if (cellDate == DateTime.Today)
				dateForegroundColor = ConsoleColor.Gray;

			// check is cell date is today
			if (cellDate == DateTime.Today)
				// save cursorX
				cursorX = borderLeft + i * (GraphicsManager.width - borderLeft - borderRight)/segmentCount +1;
			// draw date
			GraphicsManager.DrawText(
				cellDate.ToString("ddd dd.MM.yy").ToUpper(),
				dateForegroundColor,
				(cellDate == DateTime.Today ? ConsoleColor.DarkRed : ConsoleColor.Black),
				borderLeft + (2*i+1) * (GraphicsManager.width - borderLeft - borderRight)/segmentCount / 2 - 6,
				0
			);

			if (i == cursorPosition)
				// draw cursor frame
				GraphicsManager.DrawRectFrame(
					borderLeft + i * (GraphicsManager.width - borderLeft - borderRight)/segmentCount,
					borderTop, 
					(GraphicsManager.width - borderLeft - borderRight)/segmentCount, 
					48 + 2,
					ConsoleColor.Black,
					ConsoleColor.DarkGreen
				);
		}

		// draw each event occurences
		foreach (var tempEvent in events)
		{
			// draw time margins
			foreach (var timeMargin in tempEvent.timingOptions.timeMargins)
			{
				// save temp numbers
				int x = borderLeft + 1 + (timeMargin.Item1 - startDate).Days * (GraphicsManager.width - borderLeft - borderRight)/segmentCount;
				int y = borderTop + 1 + 2*timeMargin.Item1.Hour + (int)Math.Round(timeMargin.Item1.Minute / 30.0);
				int width = (GraphicsManager.width - borderLeft - borderRight)/segmentCount - 2;
				int height = 2*timeMargin.Item2.Hour + (int)Math.Round(timeMargin.Item2.Minute/30.0) - 2*timeMargin.Item1.Hour - (int)Math.Round(timeMargin.Item1.Minute/30.0);
				
				// check if cursor is on top of this event
				if (cursorX >= x && cursorX < x + width &&
					cursorY >= y && cursorY < y + height)
				{
					// save colors
					cursorForegroundColor = tempEvent.foregroundColor;
					cursorBackgroundColor = tempEvent.backgroundColor;
				}
				
				// draw event rect
				GraphicsManager.DrawRect(
					x,
					y,
					width,
					height,
					tempEvent.backgroundColor
				);

				// draw event title
				GraphicsManager.DrawText(
					tempEvent.title,
					tempEvent.foregroundColor,
					tempEvent.backgroundColor,
					x,
					y
				);
				// draw event times
				GraphicsManager.DrawText(
					$"{tempEvent.timingOptions.eventStartDate.ToString("HH:mm")}-{tempEvent.timingOptions.eventEndDate.ToString("HH:mm")}",
					tempEvent.foregroundColor,
					tempEvent.backgroundColor,
					x,
					y+1
				);
			}
		}

		// draw cursor arrows
		// check if any even it selected
		if (selectedEvent >= 0)
		{
			// get which event is selected
			var timeMargin = events[selectedEvent].timingOptions.timeMargins.Find(x => x.Item1.Date == startDate.AddDays(cursorPosition));

				// save temp numbers
				int x = borderLeft + 1 + (timeMargin.Item1 - startDate).Days * (GraphicsManager.width - borderLeft - borderRight)/segmentCount;
				int y = borderTop + 1 + 2*timeMargin.Item1.Hour + (int)Math.Round(timeMargin.Item1.Minute / 30.0);
				int width = (GraphicsManager.width - borderLeft - borderRight)/segmentCount - 2;
				int height = 2*timeMargin.Item2.Hour + (int)Math.Round(timeMargin.Item2.Minute/30.0) - 2*timeMargin.Item1.Hour - (int)Math.Round(timeMargin.Item1.Minute/30.0);
				
			// draw arrows on sides
			for (int i = 0; i < height; i++)
			{
				// draw arrows on left
				GraphicsManager.DrawText(
					">",
					ConsoleColor.DarkGreen,
					ConsoleColor.Black,
					x - 1,
					y + i
				);

				// draw arrows on right
				GraphicsManager.DrawText(
					"<",
					ConsoleColor.DarkGreen,
					ConsoleColor.Black,
					x + width,
					y + i 
				);
			}
		}
	
		// draw cursor
		// check if current time is on screen
		if (DateTime.Today >= startDate && DateTime.Today < endDate)
		{
			// draw cursor on screen
			GraphicsManager.DrawText(
				">" + new string('-', (GraphicsManager.width - borderLeft - borderRight)/segmentCount - 3),
				cursorForegroundColor,
				cursorBackgroundColor,
				cursorX,
				cursorY
			);
		}
	}

	// function for drawing details
	private void DrawDetails()
	{
		// draw details frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - borderRight + 1,
			borderTop,
			borderRight,
			GraphicsManager.height - borderTop - 11,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);

		// draw details label
		GraphicsManager.DrawText(
			"Details:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			0
		);

		// check if any event is selected
		if (selectedEvent < 0)
			return;

		// number of lines taken
		int lineCounter = 1; 

		// draw title
		GraphicsManager.DrawText(
			$"Title: {events[selectedEvent].title}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw start time
		GraphicsManager.DrawText(
			$"From: {events[selectedEvent].timingOptions.eventStartDate.ToString("HH:mm")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw end time
		GraphicsManager.DrawText(
			$"To:   {events[selectedEvent].timingOptions.eventEndDate.ToString("HH:mm")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// get duration
		TimeSpan duration = (events[selectedEvent].timingOptions.eventEndDate.TimeOfDay - events[selectedEvent].timingOptions.eventStartDate.TimeOfDay);
		// draw duration
		GraphicsManager.DrawText(
			$"Duration: {duration.Hours} hrs {duration.Minutes} mins",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw desription
		List<string> lines = ("Description: " + events[selectedEvent].description).MakePassage(borderRight - 2);
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
					GraphicsManager.width - borderRight + 2,
					borderTop + lineCounter
				);

				// increment line counter
				lineCounter++;
			}
		}
		
		// increment line counter
		lineCounter++;

		// draw start date
		GraphicsManager.DrawText(
			$"Start Date: {events[selectedEvent].timingOptions.eventStartDate.ToString("ddd dd.MM.yy")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw end date
		GraphicsManager.DrawText(
			$"End Date:   {events[selectedEvent].timingOptions.eventEndDate.ToString("ddd dd.MM.yy")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw full day status
		GraphicsManager.DrawText(
			$"Full Day: {(events[selectedEvent].timingOptions.isFullDay ? "Yes" : "No")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw repetition type
		GraphicsManager.DrawText(
			$"Repeats: {Enum.GetName(typeof(TimingOptions.RepeatType), events[selectedEvent].timingOptions.repeatType)}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			borderTop + lineCounter
		);

		// increment line counter
		lineCounter++;

		// draw repetiton days
		// get temporary message
		string message = "";
		switch (events[selectedEvent].timingOptions.repeatType)
		{
			case TimingOptions.RepeatType.Weekly:
			{
				// cycle thru dates
				foreach (var date in events[selectedEvent].timingOptions.selectedDates)
					message += Enum.GetName(typeof(DayOfWeek), date.DayOfWeek)[0] + ", ";
				// remove last 2 characters
				message = message.Substring(0, message.Length-2);
				break;
			}
			case TimingOptions.RepeatType.Monthly:
			{
				// cycle thru dates
				foreach (var date in events[selectedEvent].timingOptions.selectedDates)
					message += date.Day + ", ";
				// remove last 2 characters
				message = message.Substring(0, message.Length-2);
				break;
			}
			case TimingOptions.RepeatType.Annualy:
			{
				// cycle thru dates
				foreach (var date in events[selectedEvent].timingOptions.selectedDates)
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
			lines = ("Every: " + message).MakePassage(borderRight - 2);
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
						GraphicsManager.width - borderRight + 2,
						borderTop + lineCounter
					);

					// increment line counter
					lineCounter++;
				}
			}
		}

	}

	// function for drawing small month calendar
	private void DrawMonthCalendar()
	{
		// draw frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - borderRight + 1,
			GraphicsManager.height - 10,
			borderRight,
			10,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);

		// draw calendar label
		GraphicsManager.DrawText(
			"Calendar:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 2,
			GraphicsManager.height - 11
		);

		// draw current month
		GraphicsManager.DrawText(
			startDate.AddDays(cursorPosition).ToString("MMMM").ToUpper(),
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 15 - startDate.AddDays(cursorPosition).ToString("MMMM").Count()/2,
			GraphicsManager.height - 9
		);

		// draw current year
		GraphicsManager.DrawText(
			$"Y:{startDate.AddDays(cursorPosition).ToString("yyyy")}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - 7,
			GraphicsManager.height - 9
		);

		// draw day of year
		GraphicsManager.DrawText(
			$"D:{cultureInfo.Calendar.GetDayOfYear(startDate.AddDays(cursorPosition))}",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 3,
			GraphicsManager.height - 9
		);

		// draw week labels
		GraphicsManager.DrawText(
			" #|  M  T  W  T  F  S  S",
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - borderRight + 3,
			GraphicsManager.height - 8
		);

		// draw days
		// set starting date to 1st of current month
		DateTime tempDate = new DateTime(startDate.AddDays(cursorPosition).Year, startDate.AddDays(cursorPosition).Month, 1);
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
				GraphicsManager.width - borderRight + 3,
				GraphicsManager.height - 7 + i
			);
			
			// draw day by day
			for (int j = 0; j < 7; j++)
			{
				// save colors to change them
				ConsoleColor dateForegroundColor = ConsoleColor.White;
				ConsoleColor dateBackgroundColor = ConsoleColor.Black;
				// check if date is not in current month
				if (tempDate.Month != startDate.AddDays(cursorPosition).Month)
					dateForegroundColor = ConsoleColor.DarkGray;
				// check if date is today
				if (tempDate == DateTime.Today)
					dateBackgroundColor = ConsoleColor.DarkRed;
				// check if date is selected
				 if (tempDate == startDate.AddDays(cursorPosition))
					dateForegroundColor = ConsoleColor.DarkGreen;

				// draw date
				GraphicsManager.DrawText(
					$"{tempDate.Day}",
					dateForegroundColor,
					dateBackgroundColor,
					GraphicsManager.width - borderRight + 7 + 3*j + (tempDate.Day / 10 > 0 ? 0 : 1),
					GraphicsManager.height - 7 + i
				);

				// increment temp date
				tempDate = tempDate.AddDays(1);
			}
		}
	}


	// function for updating current selected event
	public void UpdateSelectedEvent()
	{

		if (selectedEvent >= 0 && events[selectedEvent].timingOptions.timeMargins.Count(x => x.Item1.Date == startDate.AddDays(cursorPosition)) > 0)
			return;
		for (int i = 0; i < events.Count; i++)
			if (events[i].timingOptions.timeMargins.Count(x => x.Item1.Date == startDate.AddDays(cursorPosition)) > 0)
			{
				selectedEvent = i;
				return;
			}
		selectedEvent = -1;
	}

	// function for parsing events form json file
	public void ReadEventsFromFile()
	{
		// read events from file
		using (StreamReader sr = new StreamReader(filePath))
			events = JsonSerializer.Deserialize<List<EventNode>>(sr.ReadToEnd());
		// sort events using start time
		events.Sort((a, b) => a.timingOptions.eventStartDate.ToString("HH:mm").CompareTo(b.timingOptions.eventStartDate.ToString("HH:mm")));
	}

	// function for parsing events into json file
	public void WriteEventsToFile()
	{
		// sort events using start time
		events.Sort((a, b) => a.timingOptions.eventStartDate.CompareTo(b.timingOptions.eventStartDate));
		// write events to file
		using (StreamWriter sw = new StreamWriter(filePath, false))
			JsonSerializer.Serialize<List<EventNode>>(sw.BaseStream, events);
	}
}



//? inprogress:

// ! issues:

// todo:
// todo: add tasks
// todo: move to seperate classes
// todo: get lambda call of what to redraw
// todo: add [] around dates in calendar and main view
// todo: full day events (above calendar displayed in list)
// todo: add occupancy: (with color)
// todo: add exclude option and few others
// todo: add events
// todo: current event view under details
// todo: add contacts
// todo: clock on top of calendar
// todo: fix extra space on the right
// todo: check that min width is 131 and min height is 51
// todo: vim-mode
// todo: moon-phase
// todo: national holidays