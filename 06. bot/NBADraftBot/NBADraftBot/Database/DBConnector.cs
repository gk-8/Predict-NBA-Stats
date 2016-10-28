using NBADraftBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBADraftBot.Database
{
    public class DBConnector
    {
        private NBABotContext db;

        public DBConnector()
        {
            db = new NBABotContext();
        }

        public List<Predictions> GetAvailablePlayersInfoPerDescending()
        {
            return db.PlayersInfo.Where(p => p.Drafted == 0).OrderByDescending(p => p.PER).ToList();
        }

        public List<Predictions> GetAvailableTopPlayersInfoPerDescending(int number)
        {
            return db.PlayersInfo.Where(p => p.Drafted == 0).OrderByDescending(p => p.PER).Take(number).ToList();
        }

        public List<Predictions> GetAvailableTopPlayersByPosition(string position, int number)
        {
            return db.PlayersInfo.Where(p => p.Position == position && p.Drafted == 0).OrderByDescending(p => p.PER).Take(number).ToList();
        }

        public List<Predictions> GetAvailablePlayersByPosition(string position)
        {
            return db.PlayersInfo.Where(p => p.Position == position && p.Drafted == 0).OrderByDescending(p => p.PER).ToList();
        }

        public Predictions GetPlayerInfoByName(string player)
        {
            return db.PlayersInfo.Where(p => p.Player == player).FirstOrDefault();
        }

        public bool CheckPlayerAvailability(string player) {
            return GetPlayerInfoByName(player).Drafted == 0;
        }

        public void MarkPlayerAsDraftedByName(string player) {
            Predictions p = GetPlayerInfoByName(player);
            if (p != null) {
                p.Drafted = 1;
                db.SaveChanges();
            }
        }
    }
}