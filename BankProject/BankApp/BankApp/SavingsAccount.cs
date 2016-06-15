using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class SavingsAccount : Account
    {
        public decimal WithdrawCharge { get; }

        public SavingsAccount(int accNumber) : base(accNumber)
        {
            AccType = AccountType.Savings;
            InterestRate = 1.01m;
            WithdrawCharge = 1.02m;
        }

        public override bool Withdraw(decimal amountToWithdraw)
        {
            bool freeWithdrawUsed = FreeWithdrawDate.Year == DateTime.Now.Year;

            if (freeWithdrawUsed)
                amountToWithdraw *= WithdrawCharge;

            if (amountToWithdraw <= 0 || amountToWithdraw > Balance)
            {
                return false;
            }

            if(!freeWithdrawUsed)
                FreeWithdrawDate = DateTime.Now;

            Balance -= amountToWithdraw;
            return true;
        }
    }
}
