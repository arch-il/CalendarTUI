using System;

namespace CalendarTUI.Miscellaneous;

public class EventNode
{
	// event details
	public string title { get; set; }       // event title
	public ConsoleColor backgroundColor { get; set; } // event background color
	public ConsoleColor foregroundColor { get; set; } // event background color
	public string description { get; set; } // event short description
	public TimingOptions timingOptions { get; set; }  // start date, end date and recurring order


	// simple constructor
	public EventNode(string title, ConsoleColor backgroundColor, ConsoleColor foregroundColor, string description, TimingOptions timingOptions)
	{
		// set data
		this.title = title;
		this.backgroundColor = backgroundColor;
		this.foregroundColor = foregroundColor;
		this.description = description;
		this.timingOptions = timingOptions;
	}
}