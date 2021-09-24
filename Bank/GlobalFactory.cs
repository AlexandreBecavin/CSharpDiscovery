using System;
using System.Collections.Generic;
using Bank.Account;
using Bank.Audit;
using Bank.DateService;
using Bank.LockDown;

namespace Bank
{
	public enum AccountType
	{
		Current = 0,
		Saving = 1
	}

	/// <summary>
	/// This class creates instance of other class. The instance is guaranteed to stay the same for the duration of the test
	/// </summary>
	/// <remarks>
	/// You MUST modify this class as you see fit
	/// </remarks>
	public class GlobalFactory : IGlobalFactory
	{
		private LockDownManager _lockDownManager;
		private TransactionAudit _transactionAudit;
		private readonly Dictionary<int, AccountBase> _accounts = new Dictionary<int, AccountBase>();

		public IAccount GetAccount(AccountType type, int accountNumber)
		{
			if(_accounts.ContainsKey(accountNumber))
				return _accounts[accountNumber];

			AccountBase account = null;
			if(type == AccountType.Current)
				account = new CurrentAccount(accountNumber, GetAudit());
			else
				account = new SavingAccount(accountNumber, GetAudit());

			var ldm = GetLockDownManager();

			ldm.LockDownStarted += account.OnLockDownStarted;
			ldm.LockDownEnded += account.OnLockDownEnded;

			return _accounts[accountNumber] = account;
		}

		public ITransactionAudit GetAudit()
		{
			if(_transactionAudit == null) _transactionAudit = new TransactionAudit(GetDateService());
			return _transactionAudit;
		}

		public ILockDownManager GetLockDownManager()
		{
			if(_lockDownManager == null)
			{
				_lockDownManager = new LockDownManager();
			}

			return _lockDownManager;
		}
		public IDateService GetDateService()
		{
			return new Bank.DateService.DateService();
		}
	}
}