using CalendarTUI.TerminalGraphics;

namespace CalendarTUI.Modules;

public static class CurrentTime
{
	// saved drawn date
	private static DateTime savedDate;

	// list of things to redraw
	private static List<Action> drawCalls = new List<Action>();


	// initialize data
	public static void Initialize()
	{
		// save current time
		savedDate = DateTime.Now;

		// ask program to draw everything
		QueueEveryDraw();
	}

	// function for updating drawing state
	public static void Update()
	{
		// check if second has changed
		if (DateTime.Now.Second != savedDate.Second)
		{
			// update saved date
			savedDate = DateTime.Now;
			
			// queue required draw call
			drawCalls.Add(DrawTimeAndDate);
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
		drawCalls.Add(DrawTimeAndDate);
	}
	
	// draw frame
	public static void DrawFrame()
	{
		// draw frame
		GraphicsManager.DrawRectFrame(
			GraphicsManager.width - Calendar.borderRight + 1,
			1,
			Calendar.borderRight,
			3,
			ConsoleColor.Black,
			ConsoleColor.DarkGray
		);
	}

	// draw frame label
	public static void DrawFrameLabel()
	{
		// draw label
		GraphicsManager.DrawText(
			"Time And Date:",
			ConsoleColor.DarkGray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 2,
			0
		);
	}

	// draw current time and date
	public static void DrawTimeAndDate()
	{
		// clearing is not needed here

		// draw time
		GraphicsManager.DrawText(
			savedDate.ToString("HH:mm:ss"),
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - Calendar.borderRight + 3,
			2
		);

		// draw date
		GraphicsManager.DrawText(
			savedDate.ToString("ddd dd.MM.yy").ToUpper(),
			ConsoleColor.Gray,
			ConsoleColor.Black,
			GraphicsManager.width - 13,
			2
		);
	}
}