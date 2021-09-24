using System;
using System.Linq;
using System.Threading.Tasks;

using Bank.Audit;
using Bank.Exceptions;

namespace Bank.Account
{
    public abstract class AccountBase : IAccount
    {
        private bool _isLocked;
        private ITransactionAudit _transactionAudit;

        public int AccountNumber { get; } = 0;

        public AccountBase(int accountNumber, ITransactionAudit transactionAudit)
        {
            if ( transactionAudit == null) throw new ArgumentNullException(nameof(transactionAudit));

            AccountNumber = accountNumber;
            _transactionAudit = transactionAudit;
        }
		
		public virtual decimal OverdraftLimit { get; set; }
		
        public decimal Balance 
        { 
            get
            {
                var transactions =  _transactionAudit.GetAccountTransactionsAsync(AccountNumber).Result;
                return transactions.Sum(t => t.Amount * (t.TransactionType == TransactionType.Deposit ? 1 : -1 ));
            }
        }
		
        public async Task DepositAsync(decimal amount)
        {
            if (_isLocked) throw new UnauthorizedAccountOperationException();
			if (amount < 0) throw new UnauthorizedAccountOperationException();

            var transaction = new Transaction()
            {
                AccountNumber = AccountNumber,
                TransactionType = TransactionType.Deposit,
                Amount = amount
            };

            _transactionAudit.WriteTransactionAsync(transaction);
        }

		public async Task WithdrawAsync(decimal amount)
        {
            if (_isLocked) throw new UnauthorizedAccountOperationException();
            if (!VerifyWithdrawAmount(amount)) throw new UnauthorizedAccountOperationException();

            var transaction = new Transaction()
            {
                AccountNumber = AccountNumber,
                TransactionType = TransactionType.Withdraw,
                Amount = amount
            };

            _transactionAudit.WriteTransactionAsync(transaction);
        }

        public void OnLockDownStarted(object sender, EventArgs e)
        {
            _isLocked = true;
        }

        public void OnLockDownEnded(object sender, EventArgs e)
        {
            _isLocked = false;
        }

        protected abstract bool VerifyWithdrawAmount(decimal amount);
    }
}