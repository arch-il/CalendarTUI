namespace CalendarTUI.Miscellaneous;

public class TimingOptions
{
	// enum for determining repeat type
	public enum RepeatType
	{
		No,
		Daily,
		Weekly,
		Monthly,
		Annualy
	}

	public List<Tuple<DateTime, DateTime>> timeMargins { get; set; } // time margins in selected time

	// time margins	
	public DateTime eventStartDate { get; set; } // start date
	public DateTime eventEndDate { get; set; }   // end date

	// repeat parameters
	public RepeatType repeatType { get; set; }        // repeat type
	public List<DateTime> selectedDates { get; set; } // list of selected dates


	// simple constructor
	public TimingOptions(DateTime eventStartDate, DateTime eventEndDate, RepeatType repeatType, List<DateTime> selectedDates)
	{
		this.eventStartDate = eventStartDate;
		this.eventEndDate = eventEndDate;
		this.repeatType = repeatType;
		this.selectedDates = selectedDates;
		timeMargins = new List<Tuple<DateTime, DateTime>>();
	}

	// function for getting list of occurences
	public List<Tuple<DateTime, DateTime>> GetTimeMargins(DateTime startDate, DateTime endDate)
	{
		// clear list
		List<Tuple<DateTime, DateTime>> tempTimeMargins = new List<Tuple<DateTime, DateTime>>();	
		
		// check if event occurs in given time span
		if (eventStartDate > endDate || eventEndDate < startDate)
			return tempTimeMargins;
		
		// temproray date time
		DateTime tempDate;

		// check which start point is bigger
		if (eventStartDate < startDate)
			tempDate = new DateTime(
				startDate.Year, startDate.Month, startDate.Day, 
				eventStartDate.Hour, eventStartDate.Minute, eventStartDate.Second
			);
		else
			tempDate = new DateTime(
				eventStartDate.Year, eventStartDate.Month, eventStartDate.Day, 
				eventStartDate.Hour, eventStartDate.Minute, eventStartDate.Second
			);
		// check which end time is smaller
		endDate = (endDate <= eventEndDate ? endDate : eventEndDate);

		// check repeat type
		switch (repeatType)
		{
			case RepeatType.No:
			{
				tempTimeMargins = new List<Tuple<DateTime, DateTime>>()
				{
					new Tuple<DateTime, DateTime>(
						(eventStartDate > startDate ? eventEndDate : startDate),
						(eventEndDate < endDate ? eventEndDate : endDate)
					)
				};
				break;
			}

			case RepeatType.Daily:
			{			
				// move between dates and add a day
				for (; tempDate.Date < endDate.Date; tempDate = tempDate.AddDays(1))
					// add new date pair to list
					tempTimeMargins.Add(new Tuple<DateTime, DateTime>(
						tempDate,
						new DateTime(
							tempDate.Year, tempDate.Month, tempDate.Day,
							eventEndDate.Hour, eventEndDate.Minute, eventEndDate.Second
						)
					));
				break;
			}

			case RepeatType.Weekly:
			{
				// check for each selected date
				foreach(var selectedDate in selectedDates)
				{
					// create starting point
					DateTime tempStartDate = new DateTime(
						tempDate.Year,
						tempDate.Month,
						tempDate.Day,
						eventStartDate.Hour,
						eventStartDate.Minute,
						eventStartDate.Second
					);

					// correct week day offset
					int diff = (int)selectedDate.DayOfWeek - (int)tempStartDate.DayOfWeek;
					tempStartDate = tempStartDate.AddDays(diff >= 0 ? diff : 7 + diff);

					// check if date is lower than start date
					if (tempStartDate < tempDate)
						tempStartDate = tempStartDate.AddDays(7);

					// move between dates and add a week
					for (; tempStartDate.Date < endDate.Date; tempStartDate = tempStartDate.AddDays(7))
						// add date pair in dates list
						tempTimeMargins.Add(new Tuple<DateTime, DateTime>(
							tempStartDate,
							new DateTime(
								tempStartDate.Year, tempStartDate.Month, tempStartDate.Day,
								eventEndDate.Hour, eventEndDate.Minute, eventEndDate.Second
							)
						));
				}
				break;
			}

			case RepeatType.Monthly:
			{
				// check for each selected date
				foreach(var selectedDate in selectedDates)
				{
					// create starting point
					DateTime tempStartDate = new DateTime(
						tempDate.Year,
						tempDate.Month,
						selectedDate.Day,
						eventStartDate.Hour,
						eventStartDate.Minute,
						eventStartDate.Second
					);

					// check if date is lower than start date
					if (tempStartDate < tempDate)
						tempStartDate = tempStartDate.AddMonths(1);

					// move between dates and add a month
					for (; tempStartDate < endDate; tempStartDate = tempStartDate.AddMonths(1))
						// add date pair in dates list
						tempTimeMargins.Add(new Tuple<DateTime, DateTime>(
							tempStartDate,
							new DateTime(
								tempStartDate.Year, tempStartDate.Month, tempStartDate.Day,
								eventEndDate.Hour, eventEndDate.Minute, eventEndDate.Second
							)
						));
				}
				break;
			}
			
			case RepeatType.Annualy:
			{
				// check for each selected date
				foreach(var selectedDate in selectedDates)
				{
					// create starting point
					DateTime tempStartDate = new DateTime(
						tempDate.Year,
						selectedDate.Month,
						selectedDate.Day,
						eventStartDate.Hour,
						eventStartDate.Minute,
						eventStartDate.Second
					);

					// check if date is lower than start date
					if (tempStartDate < tempDate)
						tempStartDate = tempStartDate.AddYears(1);

					// move between dates and add a year
					for (; tempStartDate < endDate; tempStartDate = tempStartDate.AddYears(1))
						// add date pair in dates list
						timeMargins.Add(new Tuple<DateTime, DateTime>(
							tempStartDate,
							new DateTime(
								tempStartDate.Year, tempStartDate.Month, tempStartDate.Day,
								eventEndDate.Hour, eventEndDate.Minute, eventEndDate.Second
							)
						));
				}
				break;
			}
		}

		// return value
		return tempTimeMargins;
	}
}