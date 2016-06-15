using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Data
{
    public class AccountDB
    {
        [Key]
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Balance { get; set; }
        public string AccType { get; set; }
        public DateTime? FreeWithdrawDate { get; set; }
        public CustomerDB CustomerFK { get; set; }

        public AccountDB() { }

        public AccountDB(int accountNumber, decimal interestRate, decimal balance, string accType, CustomerDB customerId)
        {
            AccountNumber = accountNumber;
            InterestRate = interestRate;
            Balance = balance;
            AccType = accType;
            CustomerFK = customerId;
        }
    }
}
