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
		// todo: month calendar
		// todo: details

// ! issues:

// todo:
// todo: update comments
// todo: add settings.json
// todo: add tasks
// todo: get lambda call of what to redraw
// todo: add [] around dates in calendar and main view
// todo: full day events (above calendar displayed in list)
// todo: add occupancy: (with color)
// todo: add exclude option and few others
// todo: add events
// todo: current event view under details
// todo: add contacts
// todo: clock on top of calendar
// todo: fix extra space on the right
// todo: check that min width is 131 and min height is 51
// todo: vim-mode
// todo: moon-phase
// todo: national holidays
