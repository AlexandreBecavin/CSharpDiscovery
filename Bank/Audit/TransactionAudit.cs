using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Bank.DateService;

namespace Bank.Audit
{
    public class TransactionAudit : ITransactionAudit
    {
        private Task _lastTask = Task.CompletedTask;
        private readonly Dictionary<int, List<Transaction>> _transactions = new Dictionary<int, List<Transaction>>();
        private readonly IDateService _dateService;
        private static object _verrou = new object();

        public TransactionAudit(IDateService dateService)
        {
            if (dateService == null) throw new ArgumentNullException(nameof(dateService));
            _dateService = dateService;
        }

        /// <summary>
		/// Gets a list of transactions for the specified account
		/// </summary>
		public Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(int accountNumber)
        {
            return (Task<IEnumerable<Transaction>>)(_lastTask = _lastTask.ContinueWith<IEnumerable<Transaction>>(t => {
                if(!_transactions.ContainsKey(accountNumber)) return Enumerable.Empty<Transaction>();

                return _transactions[accountNumber];
            }));
        }

		/// <summary>
		/// Writes a transaction
		/// </summary>
		public Task WriteTransactionAsync(Transaction transaction)
        {
            return _lastTask = _lastTask.ContinueWith(t => {
                if (!_transactions.ContainsKey(transaction.AccountNumber))
                {
                    _transactions.Add(transaction.AccountNumber, new List<Transaction>());
                }

                transaction.TransactionDate = _dateService.GetCurrentDateTime();

                _transactions[transaction.AccountNumber].Add(transaction);
            });
        }
    }
}