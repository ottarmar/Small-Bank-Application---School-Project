using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Data
{
    public class BankContextNew : DbContext
    {
        public DbSet<CustomerDB> Customers { get; set; }
        public DbSet<AccountDB> Accounts { get; set; }
        public DbSet<TransactionDB> Transactions { get; set; }
    }
}
