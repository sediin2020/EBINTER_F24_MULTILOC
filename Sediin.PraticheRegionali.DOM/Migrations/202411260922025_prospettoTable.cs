namespace Sediin.PraticheRegionali.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prospettoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prospetto",
                c => new
                    {
                        ProspettoId = c.Int(nullable: false, identity: true),
                        Anno = c.String(nullable: false, maxLength: 4),
                        Mese = c.String(nullable: false, maxLength: 2),
                        Descrizione = c.String(nullable: false, maxLength: 175),
                        Data_Inserimento = c.DateTime(nullable: false),
                        FileName = c.String(),
                        Numero_Quote = c.Int(),
                        Importo_Totale = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProspettoId);
            
            CreateTable(
                "dbo.Quote",
                c => new
                    {
                        QuoteId = c.Int(nullable: false, identity: true),
                        ProspettoId = c.Int(nullable: false),
                        EbtId = c.Int(nullable: false),
                        Iban = c.String(),
                        Saldo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Data_Riferimento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.QuoteId)
                .ForeignKey("dbo.Ebt", t => t.EbtId)
                .ForeignKey("dbo.Prospetto", t => t.ProspettoId)
                .Index(t => t.ProspettoId)
                .Index(t => t.EbtId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quote", "ProspettoId", "dbo.Prospetto");
            DropForeignKey("dbo.Quote", "EbtId", "dbo.Ebt");
            DropIndex("dbo.Quote", new[] { "EbtId" });
            DropIndex("dbo.Quote", new[] { "ProspettoId" });
            DropTable("dbo.Quote");
            DropTable("dbo.Prospetto");
        }
    }
}
