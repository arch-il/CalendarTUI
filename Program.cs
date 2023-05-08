using CalendarTUI.TerminalGraphics;

namespace CalendarTUI;

public class Program
{
	static void Main(string[] args)
	{

		// initialize Graphics
		GraphicsManager.InitializeGraphics();

		// initialize calendar
		Calendar.Initialize("events.json");

		// clear screen before starting drawing
		Console.Clear();

		// manually draw first time
		Calendar.Draw();
		
		// game loop
		while (true)
		{
			// check if window has been updated
			if (GraphicsManager.Update())
			{
				// call calendar update and draw
				Calendar.Update();
				Calendar.Draw();
			}
			// check if input has been updated
			else if (Calendar.Update())
				// call calendar draw
				Calendar.Draw();
		}
	}
}


//? inprogress:
// todo: make drawing more efficient
	// todo: implement clearing section of screen
		// todo: details

// ! issues:

// todo:
// todo: update comments

// todo: add settings.json
// todo: vim-mode

// todo: add tasks

// todo: add values to events (occupancy, exclude date)
// todo: current event view under details

// todo: clock on top of calendar
// todo: moon-phase

// todo: full day events (above calendar displayed in list)
// todo: national holidays
