namespace BankApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountDBs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Int(nullable: false),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccType = c.String(),
                        FreeWithdrawDate = c.DateTime(),
                        CustomerFK_SocialSecurityNr = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerDBs", t => t.CustomerFK_SocialSecurityNr)
                .Index(t => t.CustomerFK_SocialSecurityNr);
            
            CreateTable(
                "dbo.CustomerDBs",
                c => new
                    {
                        SocialSecurityNr = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.SocialSecurityNr);
            
            CreateTable(
                "dbo.TransactionDBs",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        TransType = c.String(),
                        TransactionDateTime = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountFK_Id = c.Int(),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.AccountDBs", t => t.AccountFK_Id)
                .Index(t => t.AccountFK_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionDBs", "AccountFK_Id", "dbo.AccountDBs");
            DropForeignKey("dbo.AccountDBs", "CustomerFK_SocialSecurityNr", "dbo.CustomerDBs");
            DropIndex("dbo.TransactionDBs", new[] { "AccountFK_Id" });
            DropIndex("dbo.AccountDBs", new[] { "CustomerFK_SocialSecurityNr" });
            DropTable("dbo.TransactionDBs");
            DropTable("dbo.CustomerDBs");
            DropTable("dbo.AccountDBs");
        }
    }
}
