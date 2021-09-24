using System;

namespace Bank.Audit
{
	/// <summary>
	/// This class records a single transaction for a given account.
	/// </summary>
	/// <remarks>
	/// You CAN MODIFY this class as you see fit
	/// </remarks>
	public class Transaction
	{
		private static object _verrou = new object();
		private static int _currentId = 0;

		public Transaction()
		{
			lock (_verrou)
			{
				 Id = _currentId++;
			}
		}

		public int Id { get; private set; }

		public int AccountNumber { get; set; }

		public TransactionType TransactionType { get; set; }

		public DateTimeOffset TransactionDate { get; set; }

		public decimal Amount { get; set; }
	}
}