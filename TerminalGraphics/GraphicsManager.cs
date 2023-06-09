using CalendarTUI.Miscellaneous;
using CalendarTUI.Modules;

namespace CalendarTUI.TerminalGraphics;

public static class GraphicsManager
{
	// window size
	public static int width  = 0;
	public static int height = 0;

	// function for initializing graphics
	public static void InitializeGraphics()
	{
		// hide cursor
		Console.CursorVisible = false;

		// update dimensions
		width = Console.WindowWidth-2;
		height = Console.WindowHeight;

		// update window size 
		Update();
	}


	// update window size
	public static void Update()
	{
		// check that window is big enough
		if (Console.WindowWidth < 133 || Console.WindowHeight < 51)
		{
			// reset width and height to trigger other if
			width = height = 0;

			// clear screen
			Console.Clear();
			// write error message
			DrawText(
				"Window size needs to be at least 133x51",
				ConsoleColor.Gray,
				ConsoleColor.Black,
				0,
				0);
			
			// dont exit before window is appropriate size
			while (Console.WindowWidth < 133 || Console.WindowHeight < 51);
		}

		if (Console.WindowWidth-2 != width || Console.WindowHeight != height)
		{
			// update dimesions
			width = Console.WindowWidth-2;
			height = Console.WindowHeight;
			
			// clear screen
			Console.Clear();

			// notify program that size has changed
			MainCalendar.QueueEveryDraw();
			MainCalendar.UpdateSize();
			MonthCalendar.QueueEveryDraw();
			CurrentTime.QueueEveryDraw();
			CurrentEvent.QueueEveryDraw();
			MainCalendar.UpdateIncrement();
			Details.Initialize();
		}
	}

	// function for drawing text using color and position
	public static void DrawText(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor, int x = -1, int y = -1)
	{
		// set cursor position
		if (x >= 0 && y >= 0)
			Console.SetCursorPosition(x, y);

		// change colors
		Console.ForegroundColor = foregroundColor;
		Console.BackgroundColor = backgroundColor;

		// draw text
		Console.Write(text);
	}

	// function for drawing rectangle using position size and color
	public static void DrawRect(int x, int y, int width, int height, ConsoleColor backgroundColor, ConsoleColor foregroundColor = ConsoleColor.White, char texture = ' ')
	{
		// draw it line by line
		for (int i = 0; i < height; i++)
			DrawText(new string(texture, width), ConsoleColor.White, backgroundColor, x, y + i);
	}

	// function for drawing rectangle frame using position size and color
	public static void DrawRectFrame(int x, int y, int width, int height, ConsoleColor backgroundColor, ConsoleColor foregroundColor = ConsoleColor.White)
	{
		// draw horizontal lines
		// top line
		DrawText('+' + new string('-', width-2) + '+', foregroundColor, backgroundColor, x, y);
		// bottom line
		DrawText('+' + new string('-', width-2) + '+', foregroundColor, backgroundColor, x, y + height - 1);

		// draw vertical lines
		for (int i = 1; i < height-1; i++)
		{
			// draw left line
			DrawText($"{'|'}", foregroundColor, backgroundColor, x,           y + i);
			// draw right line
			DrawText($"{'|'}", foregroundColor, backgroundColor, x + width-1, y + i);
		}
	}
}