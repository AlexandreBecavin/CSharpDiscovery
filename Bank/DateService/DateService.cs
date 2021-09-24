using System;

namespace Bank.DateService
{
	public class DateService : IDateService
	{
		public DateTimeOffset GetCurrentDateTime()
        {
            return DateTimeOffset.Now;
        }
	}
}