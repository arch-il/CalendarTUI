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

	// empty constructor
	public EventNode() { /* Empty */ }

	
	// custom equals operator
	public static bool operator ==(EventNode a, EventNode b)
	{
		return a.title == b.title && 
			   a.backgroundColor == b.backgroundColor &&
			   a.foregroundColor == b.foregroundColor &&
			   a.description == b.description &&
			   a.timingOptions.eventStartDate == b.timingOptions.eventEndDate &&
			   a.timingOptions.eventEndDate == b.timingOptions.eventEndDate &&
			   a.timingOptions.repeatType == b.timingOptions.repeatType;
	}

	// custom does not equal operator
	public static bool operator !=(EventNode a, EventNode b)
	{
		return a.title != b.title ||
			   a.backgroundColor != b.backgroundColor ||
			   a.foregroundColor != b.foregroundColor ||
			   a.description != b.description ||
			   a.timingOptions.eventStartDate != b.timingOptions.eventEndDate ||
			   a.timingOptions.eventEndDate != b.timingOptions.eventEndDate ||
			   a.timingOptions.repeatType != b.timingOptions.repeatType;
	}
}