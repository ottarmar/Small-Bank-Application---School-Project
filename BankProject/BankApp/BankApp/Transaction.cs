using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class Transaction
    {
        public int AccountNumber { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransType { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }

        public Transaction(int accountNumber, DateTime date, TransactionType transType, decimal balance, decimal amount)
        {
            AccountNumber = accountNumber;
            Date = date;
            TransType = transType;
            Balance = balance;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"Account number: {AccountNumber}, Date: {Date}, Transaction type: {TransType}, Amount: {Amount}, Balance: {Balance}";
        }

        public string GetTransactionInfo()
        {
            return $"Date: {Date}, Transaction type: {TransType}, Amount: {Amount}, Balance: {Balance}";
        }
    }
}
