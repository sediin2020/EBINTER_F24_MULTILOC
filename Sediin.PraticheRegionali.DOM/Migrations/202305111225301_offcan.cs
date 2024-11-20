namespace Sediin.PraticheRegionali.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class offcan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Azioni", "SuccessModalOffcanvas", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Azioni", "SuccessModalOffcanvas");
        }
    }
}
