using NBADraftBot.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NBADraftBot.Database
{
    public class NBABotContext : DbContext
    {
        public NBABotContext() : base("AzureSQLdb") { }

        public DbSet<Predictions> PlayersInfo { get; set; }
    }
}