using NBADraftBotV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBADraftBotV2.Database
{
    public class DBConnector
    {
        private NBABotContext db;

        public DBConnector()
        {
            db = new NBABotContext();
        }

        public async Task<List<Predictions>> GetAvailablePlayersInfoPerDescending()
        {
            await db.Database.Connection.OpenAsync();
            List<Predictions> list = db.PlayersInfo.Where(p => p.Drafted == 0).OrderByDescending(p => p.PER).ToList();
            db.Database.Connection.Close();
            return list;
        }

        public async Task<List<Predictions>> GetAvailableTopPlayersInfoPerDescending(int number)
        {
            await db.Database.Connection.OpenAsync();
            List<Predictions> list = db.PlayersInfo.Where(p => p.Drafted == 0).OrderByDescending(p => p.PER).Take(number).ToList();
            db.Database.Connection.Close();
            return list;
        }

        public async Task<List<Predictions>> GetAvailableTopPlayersByPosition(string position, int number)
        {
            await db.Database.Connection.OpenAsync();
            List<Predictions> list = db.PlayersInfo.Where(p => p.Position == position && p.Drafted == 0).OrderByDescending(p => p.PER).Take(number).ToList();
            db.Database.Connection.Close();
            return list;
        }

        public async Task<List<Predictions>> GetAvailablePlayersByPosition(string position)
        {
            await db.Database.Connection.OpenAsync();
            List<Predictions> list = db.PlayersInfo.Where(p => p.Position == position && p.Drafted == 0).OrderByDescending(p => p.PER).ToList();
            db.Database.Connection.Close();
            return list;
        }

        public async Task<Predictions> GetPlayerInfoByName(string player)
        {
            await db.Database.Connection.OpenAsync();
            Predictions result = db.PlayersInfo.Where(p => p.Player == player).FirstOrDefault();
            db.Database.Connection.Close();
            return result;
        }

        public async Task<bool> CheckPlayerAvailability(string player)
        {
            Predictions playerInfo = await GetPlayerInfoByName(player);
            return playerInfo.Drafted == 0;
        }

        public async Task MarkPlayerAsDraftedByName(string player)
        {
            Predictions p = await GetPlayerInfoByName(player);
            if (p != null)
            {
                p.Drafted = 1;
                db.SaveChanges();
            }
        }
    }
}