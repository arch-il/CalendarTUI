using CalendarTUI.Miscellaneous;
using CalendarTUI.TerminalGraphics;

namespace CalendarTUI.Modules;

public static class MainCalendar
{
	// list of events
	public static List<EventNode> events = new List<EventNode>(); // list of events

	// minute increment
	private static double increment = 0;

	// enum for View Mode
	public enum View
	{
		Day,
		FourDays,
		Week
	}

	// View details
	public static View currentView = View.Week; // current View
	public static int segmentCount = 7;         // number of segments
	public static int cursorPosition;           // location of selected cell
	public static int selectedEvent;            // selected event by cursor
	public static DateTime startDate;           // view start date
	public static DateTime endDate;             // view end date

	// cursor details
	public static int cursorX = 0; // cursor position
	public static int cursorY = 0;
	public static ConsoleColor cursorForegroundColor = ConsoleColor.DarkRed; // cursor colors
	public static ConsoleColor cursorBackgroundColor = ConsoleColor.Black;

	// window size during previous draw call
	public static int prevWidth = 0;
	public static int prevHeight = 0;

	// list of things to redraw
	public static List<Action> drawCalls = new List<Action>();
	

	// function for initializing data
	public static void Initialize()
	{
		// set start date to today
		startDate = DateTime.Today;
		// set to monday
		startDate = startDate.AddDays((int)DayOfWeek.Monday - (int)startDate.DayOfWeek);
		// set end date a week later
		endDate = startDate.AddDays(7);
		
		// set cursor position to today
		cursorPosition = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;
		// check if its sunday
		if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
		{
			// move selection by a week
			startDate = startDate.AddDays(-7);
			endDate = endDate.AddDays(-7);
			// update cursor position
			cursorPosition = 6;
		}
		
		// update time margins
		for (int i = 0; i < events.Count; i++)
			events[i].timingOptions.UpdateTimeMargins(startDate, endDate);
		// update selected event positon
		UpdateSelectedEvent();
		// update increment
		UpdateIncrement();
		// update previous window size
		prevWidth = GraphicsManager.width;
		prevHeight = GraphicsManager.height;

		// add every function to draw calls
		QueueEveryDraw();
	}
	
	// update input
	public static bool Update(ConsoleKey key)
	{
		switch(key)
		{
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
				// add required draws to draw calls
				drawCalls.Add(DrawDaySegments);
				drawCalls.Add(DrawEvents);
				drawCalls.Add(() => DrawCursorFrame(-1));
				drawCalls.Add(() => DrawCursor(-1));
				drawCalls.Add(DrawCurrentTime);
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
				// add required draws to draw calls
				drawCalls.Add(DrawDaySegments);
				drawCalls.Add(DrawEvents);
				drawCalls.Add(() => DrawCursorFrame(-1));
				drawCalls.Add(() => DrawCursor(-1));
				drawCalls.Add(DrawCurrentTime);
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
				// add required draws to draw calls
				drawCalls.Add(DrawDaySegments);
				drawCalls.Add(DrawEvents);
				drawCalls.Add(() => DrawCursorFrame(-1));
				drawCalls.Add(() => DrawCursor(-1));
				drawCalls.Add(DrawCurrentTime);
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
				// check if its sunday
				if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
				{
					// move selection by a week
					startDate = startDate.AddDays(-7);
					endDate = endDate.AddDays(-7);
					// update cursor position
					cursorPosition = 6;
				}
				// add required draws to draw calls
				drawCalls.Add(DrawDaySegments);
				drawCalls.Add(DrawEvents);
				drawCalls.Add(() => DrawCursorFrame(-1));
				drawCalls.Add(() => DrawCursor(-1));
				drawCalls.Add(DrawCurrentTime);
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
						// save previous selected event
						int temp = selectedEvent;
						// update selected event
						selectedEvent = i;
						// add required draw call
						drawCalls.Add(() => DrawCursor(temp));
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
						// save previous selected event
						int temp = selectedEvent;
						// update selected event
						selectedEvent = i;
						// add required draw call
						drawCalls.Add(() => DrawCursor(temp));
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
				{
					cursorPosition--;
					// add required draw calls
					drawCalls.Add(DrawDaySegments);
					drawCalls.Add(() => DrawCursorFrame(cursorPosition + 1));
					drawCalls.Add(() => DrawCursor(-1));
				}
				else
				{
					switch (currentView)
					{
						case View.Day:
							// move dates by 1
							startDate = startDate.AddDays(-1);
							endDate = endDate.AddDays(-1);
							break;
						case View.FourDays:
							// move dates by 4
							startDate = startDate.AddDays(-4);
							endDate = endDate.AddDays(-4);
							// update cursor position
							cursorPosition = segmentCount-1;
							break;
						case View.Week:
							// move dates by 7
							startDate = startDate.AddDays(-7);
							endDate = endDate.AddDays(-7);
							// update cursor position
							cursorPosition = segmentCount-1;
							break;
					}
					// add required draw calls
					drawCalls.Add(DrawDaySegments);
					drawCalls.Add(DrawEvents);
					drawCalls.Add(() => DrawCursorFrame(-1));
					drawCalls.Add(() => DrawCursor(-1));
					drawCalls.Add(DrawCurrentTime);
				}
				break;
			}
			// move right
			case ConsoleKey.RightArrow:
			case ConsoleKey.L:
			{
				// check if cursor is already at the edge
				if (cursorPosition < segmentCount-1)
				{
					cursorPosition++;
					// add required draw calls
					drawCalls.Add(DrawDaySegments);
					drawCalls.Add(() => DrawCursorFrame(cursorPosition - 1));
					drawCalls.Add(() => DrawCursor(-1));
				}
				else
				{
					switch (currentView)
					{
						case View.Day:
							// move dates by 1
							startDate = startDate.AddDays(1);
							endDate = endDate.AddDays(1);
							break;
						case View.FourDays:
							// move dates by 4
							startDate = startDate.AddDays(4);
							endDate = endDate.AddDays(4);
							// update cursor position
							cursorPosition = 0;
							break;
						case View.Week:
							// move dates by 7
							startDate = startDate.AddDays(7);
							endDate = endDate.AddDays(7);
							// update cursor position
							cursorPosition = 0;
							break;
					}
					// add required draw calls
					drawCalls.Add(DrawDaySegments);
					drawCalls.Add(DrawEvents);
					drawCalls.Add(() => DrawCursorFrame(-1));
					drawCalls.Add(() => DrawCursor(-1));
					drawCalls.Add(DrawCurrentTime);
				}
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
				// add required draw calls
				drawCalls.Add(DrawDaySegments);
				drawCalls.Add(DrawEvents);
				drawCalls.Add(() => DrawCursorFrame(-1));
				drawCalls.Add(() => DrawCursor(-1));
				drawCalls.Add(DrawCurrentTime);
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
				// add required draw calls
				drawCalls.Add(DrawDaySegments);
				drawCalls.Add(DrawEvents);
				drawCalls.Add(() => DrawCursorFrame(-1));
				drawCalls.Add(() => DrawCursor(-1));
				drawCalls.Add(DrawCurrentTime);
				break;
			}
		}

		// update time margins
		for (int i = 0; i < events.Count; i++)
			events[i].timingOptions.UpdateTimeMargins(startDate, endDate);
		// update selected event
		UpdateSelectedEvent();
		// return update status true
		return true;
	
	}

	// function for drawing main calendar
 	public static void Draw()
	{
		// draw each segment that is in draw call
		foreach (var drawCall in drawCalls)
			drawCall();
		// clear draw call list
		drawCalls.Clear();

		// update previous window size
		prevWidth = GraphicsManager.width;
		prevHeight = GraphicsManager.height;
	}

	// redraw everything on screen
	public static void QueueEveryDraw()
	{
		// clear list
		drawCalls.Clear();
		// add everything
		drawCalls.Add(DrawLogo);
		drawCalls.Add(DrawFrame);
		drawCalls.Add(DrawTimes);
		drawCalls.Add(DrawEvents);
		drawCalls.Add(DrawDaySegments);
		drawCalls.Add(() => DrawCursorFrame(-1));
		drawCalls.Add(() => DrawCursor(-1));
		drawCalls.Add(DrawCurrentTime);
	}

	// draw logo
	public static void DrawLogo()
	{
		// draw logo
		GraphicsManager.DrawText(" ^_^ ", ConsoleColor.White, ConsoleColor.Black, 0, 0);
		// draw frame around logo
		GraphicsManager.DrawText("|", ConsoleColor.DarkGray, ConsoleColor.Black, 5, 0);
		GraphicsManager.DrawText("-----", ConsoleColor.DarkGray, ConsoleColor.Black, 0, 1);
	}

	// draw frame
	public static void DrawFrame()
	{
		// draw frame
		GraphicsManager.DrawRectFrame(
			Calendar.borderLeft,
			Calendar.borderTop,
			GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight,
			GraphicsManager.height - Calendar.borderTop - Calendar.borderBottom,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);
	}

	// draw times on the left side of the screen
	public static void DrawTimes()
	{
		// draw times
		// temp date
		DateTime tempTime = new DateTime(1, 1, 1, 0, 0, 0);
		// cycle thru dates
		for (int j = 0; j < GraphicsManager.height - Calendar.borderTop - Calendar.borderBottom - 1; j++)
		{
			if (j == 2*DateTime.Now.Hour + DateTime.Now.Minute/30)
			{
				// save cursor y
				cursorY = Calendar.borderTop + 1 + j;
				// draw time one the side of the screen
				GraphicsManager.DrawText(
					tempTime.AddMinutes(increment*j).ToString("HH:mm"),
					ConsoleColor.Gray,
					ConsoleColor.DarkRed,
					0,
					Calendar.borderTop + 1 + j
				);
			}
			else
			{
				// draw time one the side of the screen
				GraphicsManager.DrawText(
					tempTime.AddMinutes(increment*j).ToString("HH:mm"),
					ConsoleColor.DarkGray,
					ConsoleColor.Black,
					0,
					Calendar.borderTop + 1 + j
				);
			}
		}
	}

	// draw day segments
	public static void DrawDaySegments()
	{
		// clear previously drawn area
		GraphicsManager.DrawText(
			new string(' ', prevWidth - Calendar.borderLeft - Calendar.borderRight - 2),
			ConsoleColor.Black,
			ConsoleColor.Black,
			Calendar.borderLeft + 1,
			0
		);

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
				cursorX = Calendar.borderLeft + i * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount +1;
			// draw date
			GraphicsManager.DrawText(
				cellDate.ToString("ddd dd.MM.yy").ToUpper(),
				dateForegroundColor,
				(cellDate == DateTime.Today ? ConsoleColor.DarkRed : ConsoleColor.Black),
				Calendar.borderLeft + (2*i+1) * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount / 2 - 6,
				0
			);
		}
	}

	// draw frame around selected date and cursor
	public static void DrawCursorFrame(int prevCursorPosition = -1)
	{
		// check if previous cursor position has been supplied and is valid
		if (prevCursorPosition >= 0)
		{
			// clear previous frame
			GraphicsManager.DrawRectFrame(
				Calendar.borderLeft + prevCursorPosition * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount,
				Calendar.borderTop,
				(GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount, 
				GraphicsManager.height - Calendar.borderTop - Calendar.borderBottom,
				ConsoleColor.Black,
				ConsoleColor.Black
			);
			}

		// redraw module frame
		DrawFrame();

		// draw cursor frame
		GraphicsManager.DrawRectFrame(
			Calendar.borderLeft + cursorPosition * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount,
			Calendar.borderTop,
			(GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount, 
			GraphicsManager.height - Calendar.borderTop - Calendar.borderBottom,
			ConsoleColor.Black,
			ConsoleColor.DarkGreen
		);
	}

	// draw each event occurence
	public static void DrawEvents()
	{
		// clear area inside frame
		GraphicsManager.DrawRect(
			Calendar.borderLeft + 1,
			Calendar.borderTop + 1,
			GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight - 2,
			GraphicsManager.height - Calendar.borderTop - Calendar.borderBottom - 2,
			ConsoleColor.Black
		);

		// draw each event occurences
		foreach (var tempEvent in events)
		{
			// draw time margins
			foreach (var timeMargin in tempEvent.timingOptions.timeMargins)
			{
				// save temp numbers
				int x = Calendar.borderLeft + 1 + (timeMargin.Item1 - startDate).Days * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount;
				int y = Calendar.borderTop + 1 + (int)(timeMargin.Item1.TimeOfDay.TotalMinutes / increment);
				int width = (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount - 2;
				int height = (int)Math.Round((timeMargin.Item2.TimeOfDay - timeMargin.Item1.TimeOfDay).TotalMinutes / increment);
				
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
	}

	// draw cursor arrows
	public static void DrawCursor(int prevSelectedEvent = -1)
	{
		// check if previous selected event has been supplied and is valid
		if (prevSelectedEvent >= 0)
		{
			// get which event is selected
			var timeMargin = events[prevSelectedEvent].timingOptions.timeMargins.Find(x => x.Item1.Date == startDate.AddDays(cursorPosition));

			// save temp numbers
			int x = Calendar.borderLeft + 1 + (timeMargin.Item1 - startDate).Days * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount;
			int y = Calendar.borderTop + 1 + (int)(timeMargin.Item1.TimeOfDay.TotalMinutes / increment);
			int width = (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount - 2;
			int height = (int)((timeMargin.Item2.TimeOfDay - timeMargin.Item1.TimeOfDay).TotalMinutes / increment);
				
			// draw arrows on sides
			for (int i = 0; i < height; i++)
			{
				// draw arrows on left
				GraphicsManager.DrawText(
					"|",
					ConsoleColor.DarkGreen,
					ConsoleColor.Black,
					x - 1,
					y + i
				);

				// draw arrows on right
				GraphicsManager.DrawText(
					"|",
					ConsoleColor.DarkGreen,
					ConsoleColor.Black,
					x + width,
					y + i 
				);
			}
		}

		// check if any even it selected
		if (selectedEvent >= 0)
		{
			// get which event is selected
			var timeMargin = events[selectedEvent].timingOptions.timeMargins.Find(x => x.Item1.Date == startDate.AddDays(cursorPosition));

			// save temp numbers
			int x = Calendar.borderLeft + 1 + (timeMargin.Item1 - startDate).Days * (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount;
			int y = Calendar.borderTop + 1 + (int)(timeMargin.Item1.TimeOfDay.TotalMinutes / increment);
			int width = (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount - 2;
			int height = (int)((timeMargin.Item2.TimeOfDay - timeMargin.Item1.TimeOfDay).TotalMinutes / increment);
				
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
	}

	// draw current time indicator
	public static void DrawCurrentTime()
	{
		// check if current time is on screen
		if (DateTime.Today >= startDate && DateTime.Today < endDate)
		{
			// draw cursor on screen
			GraphicsManager.DrawText(
				">" + new string('-', (GraphicsManager.width - Calendar.borderLeft - Calendar.borderRight)/segmentCount - 3),
				cursorForegroundColor,
				cursorBackgroundColor,
				cursorX,
				cursorY
			);
		}
	}

	// function for updating current selected event
	public static void UpdateSelectedEvent()
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

	// update minute increment
	public static void UpdateIncrement()
	{
		// update minute increment
		increment = 1440.0 / (GraphicsManager.height - Calendar.borderTop - Calendar.borderBottom - 2);
	}
}