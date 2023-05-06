using CalendarTUI.TerminalGraphics;

namespace CalendarTUI;

public class Program
{
	static void Main(string[] args)
	{
		// initialize Graphics
		GraphicsManager.InitializeGraphics();

		// create calendar 
		Calendar calendar = new Calendar("events.json");

		// manually draw first time
		calendar.Draw();
		
		// game loop
		while (true)
		{
			// check if anything has been updated
			if (calendar.Update())
				// draw calendar
				calendar.Draw();
		}
	}
}
