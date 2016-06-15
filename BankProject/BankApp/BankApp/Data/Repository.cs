using BankApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankApp
{
    public class Repository
    {
        private static Repository instance;

        private int accountNumber;

        private Repository()
        {
            SetNewAccountNumber();
        }

        public static Repository GetInstance()
        {
            if (instance == null)
                instance = new Repository();
            return instance;
        }

        public int GetAccountNumber()
        {
            return accountNumber++;
        }

        private void SetNewAccountNumber()
        {
            accountNumber = 1001;

            //Check if there are already created accounts and update accountNumber if true
            using (var db = new BankContextNew())
            {
                var queryAcc = from a in db.Accounts
                               orderby a.AccountNumber descending
                               select a;

                if (queryAcc.Count() > 0)
                {
                    int newAccNr = queryAcc.First().AccountNumber + 1;

                    if (newAccNr > accountNumber)
                        accountNumber = newAccNr;
                }
            }
        }

        public bool UpdateWithdrawDate(SavingsAccount account)
        {
            using (var db = new BankContextNew())
            {
                int accID = 0;
                foreach (var item in db.Accounts)
                {
                    if (item.AccountNumber == account.AccountNumber)
                        accID = item.Id;
                }
                if (accID == 0)
                {
                    return false;
                }
                else
                {
                    var acc = db.Accounts.Find(accID);
                    acc.FreeWithdrawDate = account.FreeWithdrawDate;
                    db.SaveChanges();
                    return true;
                }
            }   
        }

        public int AddNewAccount(Account acc, string socialSecNr)
        {
            using (var db = new BankContextNew())
            {
                var queryAccount = from a in db.Accounts
                                   where a.AccountNumber == acc.AccountNumber
                                   select a;

                if(queryAccount.Count()==0)
                {
                    var queryCustomer = from c in db.Customers
                                        where c.SocialSecurityNr == socialSecNr
                                        select c;

                    if(queryCustomer.Count()>0)
                    {
                        db.Accounts.Add(new AccountDB(acc.AccountNumber, acc.InterestRate,
                                                        acc.Balance, acc.AccType.ToString().ToLower(), 
                                                        queryCustomer.First()));

                        db.SaveChanges();
                        return acc.AccountNumber;
                    }
                    return -1;
                }
                return -1;
            }
        }

        public void RemoveAccount(Account acc)
        {
            using (var db = new BankContextNew())
            {
                var queryAccount = from a in db.Accounts
                                   where a.AccountNumber == acc.AccountNumber
                                   select a;

                if (queryAccount.Count() == 1)
                {
                    RemoveTransactions(acc.AccountNumber);
                    db.Accounts.Remove(queryAccount.First());
                }

                db.SaveChanges();
            }
        }

        private void RemoveAllAccounts(string socialSecurity)
        {
            using (var db = new BankContextNew())
            {
                var queryAccounts = from a in db.Accounts
                                   where a.CustomerFK.SocialSecurityNr == socialSecurity
                                   select a;

                if (queryAccounts.Count() >0)
                {
                    foreach (var acc in queryAccounts)
                    {
                        RemoveTransactions(acc.AccountNumber);
                        db.Accounts.Remove(acc);
                    }

                    db.SaveChanges();
                }
            }   
        }

        private void RemoveTransactions(int accountNumber)
        {
            using (var db = new BankContextNew())
            {
                var queryTransactions = from t in db.Transactions
                                    where t.AccountFK.AccountNumber == accountNumber
                                    select t;

                if (queryTransactions.Count() > 0)
                {
                    foreach (var trans in queryTransactions)
                    {
                        db.Transactions.Remove(trans);
                    }

                    db.SaveChanges();
                }
            }
        }

        public List<Account> GetCustomerAccounts(string socialSecurityNr)
        {
            if (string.IsNullOrEmpty(socialSecurityNr))
                return null;

            List<Account> accounts = new List<Account>();

            using (var db = new BankContextNew())
            {
                var queryCustomer = from c in db.Customers
                                    where c.SocialSecurityNr == socialSecurityNr
                                    select c;

                if (queryCustomer.Count() > 0)
                {
                    string socNr = queryCustomer.First().SocialSecurityNr;

                    var queryAccounts = from a in db.Accounts
                                        where a.CustomerFK.SocialSecurityNr == socNr
                                        select a;

                    if (queryAccounts.Count() > 0)
                    {
                        int size = queryAccounts.Count();

                        foreach (var item in queryAccounts)
                        {
                            if (item.AccType.ToString().ToLower() == AccountType.Credit.ToString().ToLower())
                                accounts.Add(new CreditAccount(item.AccountNumber));
                            else if (item.AccType.ToString().ToLower() == AccountType.Savings.ToString().ToLower())
                                accounts.Add(new SavingsAccount(item.AccountNumber));
                        }

                        return accounts;
                    }
                    return null;
                }
                else
                    return null;
            }
        }

        public bool AddNewCustomer(string fName, string lName, string socialSecurityNr)
        {
            if (string.IsNullOrEmpty(fName) || string.IsNullOrEmpty(lName) || string.IsNullOrEmpty(socialSecurityNr))
                return false;

            using (var db = new BankContextNew())
            {
                var queryCustomer = from c in db.Customers
                                    where c.SocialSecurityNr == socialSecurityNr
                                    select c;

                if (queryCustomer.Count() > 0)
                {
                    return false;
                }
                else
                {
                    db.Customers.Add(new CustomerDB(socialSecurityNr, fName, lName));
                    db.SaveChanges();
                    return true;
                }
            }
        }

        public IList<string> GetCustomerList()
        {
            IList<string> customerInfo = new List<string>();

            using (var db = new BankContextNew())
            {
                var queryCustomer = from cs in db.Customers
                                    select cs;

                if (queryCustomer.Count() > 0)
                {
                    int i = queryCustomer.Count();

                    foreach (var item in queryCustomer)
                    {
                        Customer c = new Customer(item.SocialSecurityNr,
                                                    item.FirstName,
                                                    item.LastName);

                        customerInfo.Add(c.ToString());
                    }
                    return customerInfo;
                }
                else
                    return null;
            }
        }

        public Customer GetCustomer(int index)
        {
            List<CustomerDB> customers = new List<CustomerDB>();

            using (var db = new BankContextNew())
            {
                var queryCustomer = from cs in db.Customers
                                    select cs;

                if (queryCustomer.Count() > 0)
                {
                    foreach (var item in queryCustomer)
                    {
                        customers.Add(item);
                    }

                    Customer c = new Customer(customers[index].SocialSecurityNr,
                                                customers[index].FirstName,
                                                customers[index].LastName);

                    return c;
                }
                else
                    return null;
            }
        }

        public IList<string> RemoveCustomer(string socialSecurityNr)
        {
            IList<string> customerInfo = new List<string>();
            decimal totalBalance = 0, totalInterest = 0;

            using (var db = new BankContextNew())
            {
                var queryCustomer = from c in db.Customers
                                    where c.SocialSecurityNr == socialSecurityNr
                                    select c;

                if (queryCustomer.Count() > 0)
                {
                    CustomerDB cust = new CustomerDB(queryCustomer.First().SocialSecurityNr,
                                                        queryCustomer.First().FirstName,
                                                        queryCustomer.First().LastName);
                    
                    customerInfo.Add($"{cust.FirstName} {cust.LastName}\n{cust.SocialSecurityNr}\n");

                    var queryAccount = from a in db.Accounts
                                       where a.CustomerFK.SocialSecurityNr == socialSecurityNr
                                       select a;

                    if (queryAccount.Count() > 0)
                    {
                        foreach (var item in queryAccount)
                        {
                            if (item.AccType.ToString().ToLower() == AccountType.Credit.ToString().ToLower())
                            {
                                CreditAccount acc = new CreditAccount(item.AccountNumber);
                                customerInfo.Add($"{acc.GetAccountInfo()}\n{acc.CloseAccountInfo()}\n");

                                totalBalance += acc.Balance;
                                totalInterest += acc.Balance * acc.InterestRate - acc.Balance;
                            }
                            else if (item.AccType.ToString().ToLower() == AccountType.Savings.ToString().ToLower())
                            {
                                SavingsAccount acc = new SavingsAccount(item.AccountNumber);
                                customerInfo.Add($"{acc.GetAccountInfo()}\n{acc.CloseAccountInfo()}\n");

                                totalBalance += acc.Balance;
                                totalInterest += acc.Balance * acc.InterestRate - acc.Balance;
                            }
                        }
                        customerInfo.Add("Total saldo: " + totalBalance + " kr.");
                        customerInfo.Add("Räntan blev: " + totalInterest + " kr.");

                        RemoveAllAccounts(socialSecurityNr);
                    }
                    RemoveCustomerCheck(socialSecurityNr);

                    return customerInfo;
                }
                return null;
            }
        }

        private void RemoveCustomerCheck(string socialSecurityNr)
        {
            using (var db = new BankContextNew())
            {
                var queryCustomer = from c in db.Customers
                                    where c.SocialSecurityNr == socialSecurityNr
                                    select c;

                if (queryCustomer.Count()>0)
                {
                    db.Customers.Remove(queryCustomer.First());
                    db.SaveChanges();
                }
            }
        }

        public bool UpdateCustomerName(Customer customerToUpdate)
        {
            using (var db = new BankContextNew())
            {
                var queryCustomer = from c in db.Customers
                                    where c.SocialSecurityNr == customerToUpdate.SocialSecurityNr
                                    select c;

                if (queryCustomer.Count() > 0)
                {
                    queryCustomer.First().FirstName = customerToUpdate.FName;
                    queryCustomer.First().LastName = customerToUpdate.LName;
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
        }

        public IList<string> GetTransactions(string socialSecurityNr, int accountNumber)
        {
            if (string.IsNullOrEmpty(socialSecurityNr))// || !int.TryParse(accountNumber.ToString(), out accountNumber))
                return null;

            List<string> transactionInfo = new List<string>();

            using (var db = new BankContextNew())
            {
                var queryAccounts = from a in db.Accounts
                                    where a.AccountNumber == accountNumber
                                    select a;

                if (queryAccounts.Count() > 0)
                {
                    AccountDB acc = queryAccounts.First();

                    var queryTransaction = from t in db.Transactions
                                           where t.AccountFK.AccountNumber == acc.AccountNumber
                                           select t;

                    transactionInfo.Add($"Account number: {acc.AccountNumber} Balance: {acc.Balance} kr {acc.AccType.ToString().ToUpper()} ({acc.InterestRate})");

                    if (queryTransaction.Count() > 0)
                    {
                        foreach (var t in queryTransaction)
                        {
                            transactionInfo.Add($"{t.TransactionDateTime} {t.TransType.ToString()}: {t.Amount} {t.Balance} kr");
                        }
                    }
                    else
                        transactionInfo.Add("Inga transaktioner kan hittas.");
                }
                return transactionInfo;
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
                return;

            using (var db = new BankContextNew())
            {
                var queryAccounts = from a in db.Accounts
                                    where a.AccountNumber == transaction.AccountNumber
                                    select a;

                if (queryAccounts.Count() > 0)
                {
                    queryAccounts.First().Balance = transaction.Balance;

                    db.Transactions.Add(new TransactionDB(transaction.Date, transaction.TransType,
                                                            transaction.Balance, transaction.Amount,
                                                            queryAccounts.First()));
                    db.SaveChanges();
                }
            }
        }
    }
}
