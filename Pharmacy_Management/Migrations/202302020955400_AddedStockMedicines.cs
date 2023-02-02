﻿namespace Pharmacy_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStockMedicines : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Medicines", "Stock", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Medicines", "Stock");
        }
    }
}
