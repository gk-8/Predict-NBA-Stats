using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using NBADraftBot.Database;
using NBADraftBot.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NBADraftBot.Dialogs
{
    [LuisModel("<<YOUR-LUIS-ID>>", "<<YOUR-LUIS-SUBSCRIPTION-KEY>>")]
    [Serializable]
    public class DraftDialog : LuisDialog<object>
    {
        private DBConnector _dbconnector = new DBConnector();
        private Activity _activity;
        private ConnectorClient _connector;

        public DraftDialog(Activity activity)
        {
            _activity = activity;
            _connector = new ConnectorClient(new Uri(_activity.ServiceUrl));
        }

        // We don't recognize the intention of the user
        [LuisIntent("")]
        public async Task Unknown(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand! Available intents are {string.Join(", ", result.Intents.Select(i => i.Intent))}";
            await PostMessage(message);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            string message = $"How you doing, Fantasy owner? Let me know the best way to help you ;)";
            await PostMessage(message);
        }

        [LuisIntent("DraftPlayer")]
        public async Task DraftPlayer(IDialogContext context, LuisResult result)
        {
            string message = "";
            string playerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.Entities.Where(e => e.Type == "Player").FirstOrDefault().Entity.ToString().ToLower());
            if (string.IsNullOrEmpty(playerName) || _dbconnector.GetPlayerInfoByName(playerName) == null)
            {
                message = "I'm afraid I don't know this player...could you please try again?";
            }
            else
            {
                if (_dbconnector.CheckPlayerAvailability(playerName))
                {
                    _dbconnector.MarkPlayerAsDraftedByName(playerName);
                    message = $"{playerName} is available! I mean...he was! Drafted right away :)";
                }
                else
                {
                    message = $"Sorry, but {playerName} has been already drafted...why don't you try with other player?";
                }
            }
            await PostMessage(message);
        }

        [LuisIntent("TopPlayersAvailable")]
        public async Task TopPlayersAvailable(IDialogContext context, LuisResult result)
        {
            string message = "";
            int number = 0;
            int.TryParse(result.Entities.Where(e => e.Type == "builtin.number").FirstOrDefault().Entity.ToString(), out number);

            if (number > 0)
            {
                List<Predictions> predictions = _dbconnector.GetAvailableTopPlayersInfoPerDescending(number);
                message = $"These are the top {number} players available:\n";
                foreach (Predictions p in predictions)
                {
                    message += $"\n{p.Player} - {p.Position} - {p.Team} - {p.PER}\n";
                }
            }
            else
            {
                List<Predictions> predictions = _dbconnector.GetAvailablePlayersInfoPerDescending();
                message = $"These are all the players available:\n";
                foreach (Predictions p in predictions)
                {
                    message += $"\n{p.Player} - {p.Position} - {p.Team} - {p.PER}\n";
                }
            }

            await PostMessage(message);
        }

        [LuisIntent("TopPlayersPositionAvailable")]
        public async Task TopPlayersPositionAvailable(IDialogContext context, LuisResult result)
        {
            string message = "";
            string position = result.Entities.Where(e => e.Type == "Position").FirstOrDefault().Entity.ToString().ToUpper();
            int number = 0;
            int.TryParse(result.Entities.Where(e => e.Type == "builtin.number").FirstOrDefault().Entity.ToString(), out number);
            if (string.IsNullOrEmpty(position))
            {
                message = "Didn't get the position...could you please try again?";
            }
            else
            {
                if (number > 0)
                {
                    List<Predictions> predictions = _dbconnector.GetAvailableTopPlayersByPosition(position, number);
                    message = $"These are the top {number} {position.ToUpper()}s available:\n";
                    foreach (Predictions p in predictions)
                    {
                        message += $"\n{p.Player} - {p.Team} - {p.PER}\n";
                    }
                }
                else
                {
                    List<Predictions> predictions = _dbconnector.GetAvailablePlayersByPosition(position);
                    message = $"These are all the {position.ToUpper()}s available:\n";
                    foreach (Predictions p in predictions)
                    {
                        message += $"\n{p.Player} - {p.Team} - {p.PER}\n";
                    }
                }
            }
            await PostMessage(message);
        }

        [LuisIntent("PlayerOverview")]
        public async Task PlayerOverview(IDialogContext context, LuisResult result)
        {
            string message = "";
            string playerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.Entities.Where(e => e.Type == "Player").FirstOrDefault().Entity.ToString().ToLower());
            if (string.IsNullOrEmpty(playerName) || _dbconnector.GetPlayerInfoByName(playerName) == null)
            {
                message = "I'm afraid I don't know this player...could you please try again?";
            }
            else
            {
                Predictions playerInfo = _dbconnector.GetPlayerInfoByName(playerName);
                message = $"Got it! {playerInfo.Player} ({playerInfo.Age}), {playerInfo.Position} from {playerInfo.Team} has a projected PER of {playerInfo.PER} for the {playerInfo.Season} season";
            }
            await PostMessage(message);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            string message = $"Happy to help! Go get that Fantasy League ring!!";
            await PostMessage(message);
        }

        private async Task PostMessage(string message)
        {
            Activity reply = _activity.CreateReply(message);
            await _connector.Conversations.ReplyToActivityAsync(reply);
        }
    }
}