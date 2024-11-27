namespace Sediin.PraticheRegionali.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EBT_tableCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ebt",
                c => new
                    {
                        EbtId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 175),
                        Pec = c.String(nullable: false, maxLength: 175),
                        Sap = c.String(nullable: false, maxLength: 175),
                        Iban_Transitorio = c.String(maxLength: 27),
                        Iban_Operativo = c.String(maxLength: 27),
                        Note = c.String(),
                        F24_Percentuale = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MultiLoc_Percentuale = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenteNome = c.String(nullable: false, maxLength: 175),
                        ReferenteCognome = c.String(nullable: false, maxLength: 175),
                        ReferenteEmail = c.String(nullable: false, maxLength: 175),
                        ReferentePec = c.String(nullable: false, maxLength: 175),
                        ReferenteCellulare = c.String(nullable: false, maxLength: 50),
                        RegioneId = c.Int(nullable: false),
                        ProvinciaId = c.Int(nullable: false),
                        ComuneId = c.Int(nullable: false),
                        LocalitaId = c.Int(nullable: false),
                        Data_Inserimento = c.DateTime(nullable: false),
                        Data_Modifica = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EbtId)
                .ForeignKey("dbo.Comuni", t => t.ComuneId)
                .ForeignKey("dbo.Localita", t => t.LocalitaId)
                .ForeignKey("dbo.Province", t => t.ProvinciaId)
                .ForeignKey("dbo.Regioni", t => t.RegioneId)
                .Index(t => t.RegioneId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.ComuneId)
                .Index(t => t.LocalitaId);
            
            CreateTable(
                "dbo.F24Percentuale",
                c => new
                    {
                        F24PercentualeId = c.Int(nullable: false, identity: true),
                        EbtId = c.Int(nullable: false),
                        DataInserimento = c.DateTime(nullable: false),
                        F24 = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.F24PercentualeId)
                .ForeignKey("dbo.Ebt", t => t.EbtId)
                .Index(t => t.EbtId);
            
            CreateTable(
                "dbo.IbanStorico",
                c => new
                    {
                        IbanStoricoId = c.Int(nullable: false, identity: true),
                        EbtId = c.Int(nullable: false),
                        DataInserimento = c.DateTime(nullable: false),
                        Iban_Operativo = c.String(),
                        Iban_transitorio = c.String(),
                    })
                .PrimaryKey(t => t.IbanStoricoId)
                .ForeignKey("dbo.Ebt", t => t.EbtId)
                .Index(t => t.EbtId);
            
            CreateTable(
                "dbo.MultiLocPercentuale",
                c => new
                    {
                        MultiLocPercentualeId = c.Int(nullable: false, identity: true),
                        EbtId = c.Int(nullable: false),
                        DataInserimento = c.DateTime(nullable: false),
                        MultiLoc = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.MultiLocPercentualeId)
                .ForeignKey("dbo.Ebt", t => t.EbtId)
                .Index(t => t.EbtId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ebt", "RegioneId", "dbo.Regioni");
            DropForeignKey("dbo.Ebt", "ProvinciaId", "dbo.Province");
            DropForeignKey("dbo.MultiLocPercentuale", "EbtId", "dbo.Ebt");
            DropForeignKey("dbo.Ebt", "LocalitaId", "dbo.Localita");
            DropForeignKey("dbo.IbanStorico", "EbtId", "dbo.Ebt");
            DropForeignKey("dbo.F24Percentuale", "EbtId", "dbo.Ebt");
            DropForeignKey("dbo.Ebt", "ComuneId", "dbo.Comuni");
            DropIndex("dbo.MultiLocPercentuale", new[] { "EbtId" });
            DropIndex("dbo.IbanStorico", new[] { "EbtId" });
            DropIndex("dbo.F24Percentuale", new[] { "EbtId" });
            DropIndex("dbo.Ebt", new[] { "LocalitaId" });
            DropIndex("dbo.Ebt", new[] { "ComuneId" });
            DropIndex("dbo.Ebt", new[] { "ProvinciaId" });
            DropIndex("dbo.Ebt", new[] { "RegioneId" });
            DropTable("dbo.MultiLocPercentuale");
            DropTable("dbo.IbanStorico");
            DropTable("dbo.F24Percentuale");
            DropTable("dbo.Ebt");
        }
    }
}
