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
			GraphicsManager.Update();
			// check user input update
			Calendar.Update();
			// draw modules
			Calendar.Draw();
		}
	}
}


//? inprogress:
// todo: fix details module

// ! issues:
//! make current time indicator cursor moving
//! gap between events changes


// todo:
// todo: make drawing more efficient (move every calculation to update)

// todo: update comments

// todo: add settings.json
// todo: vim-mode

// todo: add tasks

// todo: add values to events (occupancy, exclude dates)
// todo: current event view under details

// todo: clock on top of calendar
// todo: moon-phase

// todo: full day events (above calendar displayed in list)
// todo: national holidays
