using System.Runtime.InteropServices;

namespace CalendarTUI.Miscellaneous;

public static class FixWindowsSizeIssues
{
	// resize issues
	private const int MF_BYCOMMAND = 0x00000000;
	public const int SC_CLOSE = 0xF060;
	public const int SC_MINIMIZE = 0xF020;
	public const int SC_MAXIMIZE = 0xF030;
	public const int SC_SIZE = 0xF000; //resize

	[DllImport("user32.dll")]
	public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

	[DllImport("user32.dll")]
	private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

	[DllImport("kernel32.dll", ExactSpelling = true)]
	private static extern IntPtr GetConsoleWindow();

	public static void FixResizeIssues()
	{
		IntPtr handle = GetConsoleWindow();
		IntPtr sysMenu = GetSystemMenu(handle, false);

		if (handle != IntPtr.Zero)
		{
			// DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
			// DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
			DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
			DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);//resize
		}
	}



	// scroll bar issues
	[StructLayout(LayoutKind.Sequential)]
	private struct Coord
	{
		public short X, Y;

		public Coord(short x, short y)
		{
			X = x;
			Y = y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct SmallRect
	{
		public short Left, Top, Right, Bottom;

		public SmallRect(short width, short height)
		{
			Left = Top = 0;
			Right = width;
			Bottom = height;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct ConsoleScreenBufferInfoEx
	{
		public uint cbSize;
		public Coord dwSize;
		public Coord dwCursorPosition;
		public short wAttributes;
		public SmallRect srWindow;
		public Coord dwMaximumWindowSize;
		public ushort wPopupAttributes;
		public bool bFullscreenSupported;

		public Colorref black, darkBlue, darkGreen, darkCyan, darkRed, darkMagenta, darkYellow, gray, darkGray, blue, green, cyan, red, magenta, yellow, white;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct Colorref
	{
		public uint ColorDWORD;
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GetConsoleScreenBufferInfoEx(
		IntPtr hConsoleOutput,
		ref ConsoleScreenBufferInfoEx ConsoleScreenBufferInfo);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetConsoleScreenBufferInfoEx(
		IntPtr hConsoleOutput,
		ref ConsoleScreenBufferInfoEx ConsoleScreenBufferInfoEx);

	
	public static void FixScrollBarIssues()
	{

		IntPtr stdHandle = GetStdHandle(-11);
		ConsoleScreenBufferInfoEx bufferInfo = new ConsoleScreenBufferInfoEx();
		bufferInfo.cbSize = (uint)Marshal.SizeOf(bufferInfo);
		GetConsoleScreenBufferInfoEx(stdHandle, ref bufferInfo);
		++bufferInfo.srWindow.Right;
		++bufferInfo.srWindow.Bottom;
		SetConsoleScreenBufferInfoEx(stdHandle, ref bufferInfo);
	}

}
