namespace Sediin.PraticheRegionali.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EBT_tableCreate1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Ebt", "ComuneId", "dbo.Comuni");
            DropForeignKey("dbo.Ebt", "LocalitaId", "dbo.Localita");
            DropIndex("dbo.Ebt", new[] { "ComuneId" });
            DropIndex("dbo.Ebt", new[] { "LocalitaId" });
            AlterColumn("dbo.Ebt", "Iban_Transitorio", c => c.String(nullable: false, maxLength: 27));
            AlterColumn("dbo.Ebt", "Iban_Operativo", c => c.String(nullable: false, maxLength: 27));
            DropColumn("dbo.Ebt", "ComuneId");
            DropColumn("dbo.Ebt", "LocalitaId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Ebt", "LocalitaId", c => c.Int(nullable: false));
            AddColumn("dbo.Ebt", "ComuneId", c => c.Int(nullable: false));
            AlterColumn("dbo.Ebt", "Iban_Operativo", c => c.String(maxLength: 27));
            AlterColumn("dbo.Ebt", "Iban_Transitorio", c => c.String(maxLength: 27));
            CreateIndex("dbo.Ebt", "LocalitaId");
            CreateIndex("dbo.Ebt", "ComuneId");
            AddForeignKey("dbo.Ebt", "LocalitaId", "dbo.Localita", "LocalitaId");
            AddForeignKey("dbo.Ebt", "ComuneId", "dbo.Comuni", "ComuneId");
        }
    }
}
