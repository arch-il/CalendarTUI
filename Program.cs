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
//! updates return bools
// todo: remove magic numbers (add const numbers) 
//! event heights do not ailgn
//! make current time indicator cursor moving
// todo: update comments

// ! issues:

// todo:
// todo: add "next event in:" in current event 

// todo: make drawing more efficient (move every calculation to update)

// todo: add settings.json
// todo: vim-mode

// todo: add tasks

// todo: add exclude days to events

// todo: full day events (above calendar displayed in list)
// todo: national holidays
