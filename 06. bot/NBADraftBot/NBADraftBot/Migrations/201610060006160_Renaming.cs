namespace NBADraftBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Renaming : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PlayerInfoes", newName: "Predictions");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Predictions", newName: "PlayerInfoes");
        }
    }
}
