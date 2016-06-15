namespace BankApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using BankApp.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<BankApp.Data.BankContextNew>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BankApp.Data.BankContextNew context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            CustomerDB andrew = new CustomerDB("8807015011", "Andrew", "Peters");
            CustomerDB brice = new CustomerDB("9011244013", "Brice", "Lambson");
            CustomerDB rowan = new CustomerDB("8603123014", "Rowan", "Miller");

            context.Customers.AddOrUpdate(
              c => c.SocialSecurityNr,
              andrew,
              brice,
              rowan
            );

            AccountDB a1 = new AccountDB(Repository.GetInstance().GetAccountNumber(),
                1.01m, 0, "Savings", andrew);
            AccountDB a2 = new AccountDB(Repository.GetInstance().GetAccountNumber(),
                1.005m, 0, "Credit", andrew);

            AccountDB b1 = new AccountDB(Repository.GetInstance().GetAccountNumber(),
                1.01m, 0, "Savings", brice);
            AccountDB b2 = new AccountDB(Repository.GetInstance().GetAccountNumber(),
               1.005m, 0, "Credit", brice);

            AccountDB r1 = new AccountDB(Repository.GetInstance().GetAccountNumber(),
               1.01m, 0, "Savings", rowan);
            AccountDB r2 = new AccountDB(Repository.GetInstance().GetAccountNumber(),
               1.005m, 0, "Credit", rowan);

            context.Accounts.AddOrUpdate(
              a => a.AccountNumber,
              a1,
              a2,
              b1,
              b2,
              r1,
              r2
            );

            TransactionDB ta11 = new TransactionDB(new DateTime(2015, 08, 10), TransactionType.Deposit,
                0, 5000, a1);
            TransactionDB ta12 = new TransactionDB(new DateTime(2015, 08, 10), TransactionType.Withdraw,
               0, 4300, a1);

            TransactionDB ta21 = new TransactionDB(new DateTime(2015, 09, 10), TransactionType.Deposit,
               0, 3350, a2);
            TransactionDB ta22 = new TransactionDB(new DateTime(2015, 10, 10), TransactionType.Withdraw,
               0, 750, a2);

            TransactionDB tb11 = new TransactionDB(new DateTime(2015, 08, 10), TransactionType.Deposit,
               0, 2100, b1);
            TransactionDB tb12 = new TransactionDB(new DateTime(2015, 08, 10), TransactionType.Withdraw,
               0, 2000, b1);

            TransactionDB tb21 = new TransactionDB(new DateTime(2015, 11, 10), TransactionType.Deposit,
               0, 7000, b2);
            TransactionDB tb22 = new TransactionDB(new DateTime(2015, 12, 10), TransactionType.Withdraw,
               0, 300, b2);

            TransactionDB tr11 = new TransactionDB(new DateTime(2015, 06, 10), TransactionType.Deposit,
               0, 1100, r1);
            TransactionDB tr12 = new TransactionDB(new DateTime(2015, 09, 10), TransactionType.Withdraw,
               0, 900, r1);

            TransactionDB tr21 = new TransactionDB(new DateTime(2015, 06, 10), TransactionType.Deposit,
               0, 1300, r2);
            TransactionDB tr22 = new TransactionDB(new DateTime(2015, 10, 10), TransactionType.Withdraw,
               0, 150, r2);

            context.Transactions.AddOrUpdate(
              t => t.TransactionId,
              ta11, ta12, ta21, ta22, tb11, tb12, tb21, tb22, tr11, tr12, tr21, tr22
            );
        }
    }
}
