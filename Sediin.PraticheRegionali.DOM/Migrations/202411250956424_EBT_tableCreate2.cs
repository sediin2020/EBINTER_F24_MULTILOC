namespace Sediin.PraticheRegionali.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EBT_tableCreate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ebt", "Accordo", c => c.DateTime());
            AddColumn("dbo.Ebt", "Sospeso", c => c.Boolean(nullable: false));
            AddColumn("dbo.Ebt", "Data_Sospensione", c => c.DateTime());
            AlterColumn("dbo.Ebt", "Sap", c => c.String(maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteNome", c => c.String(maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteCognome", c => c.String(maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteEmail", c => c.String(maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferentePec", c => c.String(maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteCellulare", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ebt", "ReferenteCellulare", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Ebt", "ReferentePec", c => c.String(nullable: false, maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteEmail", c => c.String(nullable: false, maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteCognome", c => c.String(nullable: false, maxLength: 175));
            AlterColumn("dbo.Ebt", "ReferenteNome", c => c.String(nullable: false, maxLength: 175));
            AlterColumn("dbo.Ebt", "Sap", c => c.String(nullable: false, maxLength: 175));
            DropColumn("dbo.Ebt", "Data_Sospensione");
            DropColumn("dbo.Ebt", "Sospeso");
            DropColumn("dbo.Ebt", "Accordo");
        }
    }
}
