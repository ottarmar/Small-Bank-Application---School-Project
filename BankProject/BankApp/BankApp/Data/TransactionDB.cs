using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Data
{
    public class TransactionDB
    {
        [Key]
        public int TransactionId { get; private set; }
        public string TransType { get; private set; }
        public DateTime TransactionDateTime { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }
        public AccountDB AccountFK { get; private set; }

        public TransactionDB() { }

        public TransactionDB(DateTime date, TransactionType transType, decimal balance, decimal amount, AccountDB foreignAccount)
        {
            TransactionDateTime = date;
            TransType = transType.ToString();
            Balance = balance;
            Amount = amount;
            AccountFK = foreignAccount;
        }
    }
}
