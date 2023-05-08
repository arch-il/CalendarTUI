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
		height = Console.WindowHeight-1;

		// update window size 
		Update();
	}


	// update window size
	public static bool Update()
	{
		// check that window is big enough
		if (Console.WindowWidth < 133 || Console.WindowHeight < 51)
		{
			// clear screen
			Console.Clear();
			// write error message
			DrawText(
				"Window size needs to be at least 132x51",
				ConsoleColor.Gray,
				ConsoleColor.Black,
				0,
				0);
			
			// dont exit before window is appropriate size
			while (Console.WindowWidth < 133 || Console.WindowHeight < 51);
			
			// clear screen
			Console.Clear();

			// notify program that size has changed
			MainCalendar.QueueEveryDraw();
			MonthCalendar.QueueEveryDraw();
			MainCalendar.UpdateIncrement();

			// update dimesions
			width = Console.WindowWidth-2;
			height = Console.WindowHeight-1;

			// ask program to redraw calendar
			return true;
		}

		if (Console.WindowWidth-2 != width || Console.WindowHeight-1 != height)
		{
			// update dimesions
			width = Console.WindowWidth-1;
			height = Console.WindowHeight-1;
			
			// clear screen
			Console.Clear();

			// notify program that size has changed
			MainCalendar.QueueEveryDraw();
			MonthCalendar.QueueEveryDraw();
			MainCalendar.UpdateIncrement();

			// ask program to redraw calendar
			return true;
		}

		// dont redraw anything
		return false;
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