using CalendarTUI.Miscellaneous;

namespace CalendarTUI.TerminalGraphics;

public static class GraphicsManager
{
	// window size
	public const int width  = 126 + 5 + 28;
		public const int height = 50 + 1;

	// function for initializing graphics
	public static void InitializeGraphics()
	{
		// set window size
		Console.SetWindowSize(width,   height);
		Console.SetBufferSize(width+2, height);
		// fix window issues
		FixWindowsSizeIssues.FixResizeIssues();
		FixWindowsSizeIssues.FixScrollBarIssues();

		// hide cursor
		Console.CursorVisible = false;
	}

	public static void Clear()
	{
		// draw rectangle size of entire screen
		DrawRect(0, 0, width, height, ConsoleColor.Black);
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