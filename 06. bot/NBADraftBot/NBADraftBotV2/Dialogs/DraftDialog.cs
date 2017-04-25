using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using NBADraftBotV2.Database;
using Microsoft.Bot.Builder.Luis.Models;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using NBADraftBotV2.Models;
using NBADraftBotV2.Services;

namespace NBADraftBotV2.Dialogs
{
    [LuisModel("a1e3be1b-c32f-48ea-a013-a3dc4cecdece", "a723b92d5e7c4ac0bf258ff698ed0f58")]
    [Serializable]
    public class DraftDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task Unknown(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand! Available intents are {string.Join(", ", result.Intents.Select(i => i.Intent))}";
            await PostMessage(context, message);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            string message = $"How you doing, Fantasy owner? Let me know the best way to help you ;)";
            await PostMessage(context, message);
        }

        [LuisIntent("DraftPlayer")]
        public async Task DraftPlayer(IDialogContext context, LuisResult result)
        {
            DBConnector _dbconnector = new DBConnector();

            string message = "";

            if (result.Entities.Count > 0)
            {
                string playerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.Entities.Where(e => e.Type == "Player").FirstOrDefault().Entity);
                Predictions playerInfo = await _dbconnector.GetPlayerInfoByName(playerName);
                if (string.IsNullOrEmpty(playerName) || playerInfo == null)
                {
                    message = "I'm afraid I don't know this player...could you please try again?";
                }
                else
                {
                    bool available = await _dbconnector.CheckPlayerAvailability(playerName);
                    if (available)
                    {
                        await _dbconnector.MarkPlayerAsDraftedByName(playerName);
                        message = $"{playerName} is available! I mean...he was! Drafted right away :)";
                    }
                    else
                    {
                        message = $"Sorry, but {playerName} has been already drafted...why don't you try with other player?";
                    }
                }
            }
            else
            {
                message = "I'm afraid I don't know this player...could you please try again?";
            }

            await PostMessage(context, message);
        }

        [LuisIntent("TopPlayersAvailable")]
        public async Task TopPlayersAvailable(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Let me take a look...");
            DBConnector _dbconnector = new DBConnector();
            BingImageSearchService _imageSearchService = new BingImageSearchService();

            string message = "";

            if (result.Entities.Count > 0)
            {
                int number = 0;
                int.TryParse(result.Entities.Where(e => e.Type == "builtin.number").FirstOrDefault().Entity, out number);

                List<Predictions> predictions = new List<Predictions>();

                if (number > 0)
                {
                    predictions = await _dbconnector.GetAvailableTopPlayersInfoPerDescending(number);
                    await context.PostAsync($"These are the top {number} players available:");
                }
                else
                {
                    predictions = await _dbconnector.GetAvailablePlayersInfoPerDescending();
                    await context.PostAsync($"These are all the players available:");
                }

                List<Attachment> attachments = new List<Attachment>();
                foreach (Predictions p in predictions)
                {
                    string picture = await _imageSearchService.GetPlayerPhotoByName(p.Player);
                    Attachment attachment = CreateThumbnailCard(picture, p, true);
                    attachments.Add(attachment);
                }
                await ReplyWithCards(context, attachments);
            }
            else
            {
                message = "Something went wrong...could you please try again?";
                await PostMessage(context, message);
            }
        }

        [LuisIntent("TopPlayersPositionAvailable")]
        public async Task TopPlayersPositionAvailable(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Let me take a look...");
            DBConnector _dbconnector = new DBConnector();
            BingImageSearchService _imageSearchService = new BingImageSearchService();

            string message = "";

            if (result.Entities.Count > 0)
            {
                string position = result.Entities.Where(e => e.Type == "Position").FirstOrDefault().Entity.ToUpper();
                int number = 0;
                int.TryParse(result.Entities.Where(e => e.Type == "builtin.number").FirstOrDefault().Entity, out number);
                if (string.IsNullOrEmpty(position))
                {
                    message = "Didn't get the position...could you please try again?";
                    await PostMessage(context, message);
                }
                else
                {
                    List<Predictions> predictions = new List<Predictions>();
                    if (number > 0)
                    {
                        predictions = await _dbconnector.GetAvailableTopPlayersByPosition(position, number);
                        await context.PostAsync($"These are the top {number} {position}s available:");
                    }
                    else
                    {
                        predictions = await _dbconnector.GetAvailablePlayersByPosition(position);
                        message = $"These are all the {position}s available:\n";
                    }

                    List<Attachment> attachments = new List<Attachment>();
                    foreach (Predictions p in predictions)
                    {
                        string picture = await _imageSearchService.GetPlayerPhotoByName(p.Player);
                        Attachment attachment = CreateThumbnailCard(picture, p, false);
                        attachments.Add(attachment);
                    }
                    await ReplyWithCards(context, attachments);
                }
            }
            else
            {
                message = "Something went wrong...could you please try again?";
                await PostMessage(context, message);
            }
        }

        [LuisIntent("PlayerOverview")]
        public async Task PlayerOverview(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Looking for him, give me a sec...");
            DBConnector _dbconnector = new DBConnector();
            BingImageSearchService _imageSearchService = new BingImageSearchService();
            BingNewsSearchService _newsSearchService = new BingNewsSearchService();

            string message = "";
            if (result.Entities.Count > 0)
            {
                string playerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.Entities.Where(e => e.Type == "Player").FirstOrDefault().Entity);
                Predictions playerInfo = await _dbconnector.GetPlayerInfoByName(playerName);

                if (string.IsNullOrEmpty(playerName) || playerInfo == null)
                {
                    message = "I'm afraid I don't know this player...could you please try again?";
                    await PostMessage(context, message);
                }
                else
                {
                    var picture = await _imageSearchService.GetPlayerPhotoByName(playerInfo.Player);
                    List<NewsValue> news = await _newsSearchService.GetPlayerNewsByName(playerInfo.Player);
                    Attachment card = CreateOverviewCard(picture, playerInfo);
                    List<Attachment> newsCarousel = CreateNewsCarousel(news);

                    await context.PostAsync("Got it!");
                    await ReplyWithCardAndNews(context, card, newsCarousel);
                }
            }
            else
            {
                message = "I'm afraid I don't know this player...could you please try again?";
                await PostMessage(context, message);
            }
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            string message = $"Happy to help! Go get that Fantasy League ring!!";
            await PostMessage(context, message);
        }

        private async Task PostMessage(IDialogContext context, string message)
        {
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        private Attachment CreateOverviewCard(string pictureUrl, Predictions playerInfo)
        {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: pictureUrl));

            HeroCard plCard = new HeroCard()
            {
                Title = $"{playerInfo.Player} ({playerInfo.Age}) - {playerInfo.Position} on {playerInfo.Team}",
                Subtitle = $"{playerInfo.Season} projected PER: {Math.Round(playerInfo.PER.Value, 2)}",
                Images = cardImages
            };

            Attachment attachment = plCard.ToAttachment();
            return attachment;
        }

        private Attachment CreateThumbnailCard(string pictureUrl, Predictions playerInfo, bool includePosition)
        {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: pictureUrl));

            string title = "";
            if (includePosition)
            {
                title = $"{playerInfo.Player} ({playerInfo.Age}) - {playerInfo.Position} on {playerInfo.Team}";
            }
            else
            {
                title = $"{playerInfo.Player} ({playerInfo.Age}) - {playerInfo.Team}";
            }

            ThumbnailCard plCard = new ThumbnailCard()
            {
                Title = title,
                Subtitle = $"{playerInfo.Season} projected PER: {Math.Round(playerInfo.PER.Value, 2)}",
                Images = cardImages
            };

            Attachment attachment = plCard.ToAttachment();
            return attachment;
        }

        private List<Attachment> CreateNewsCarousel(List<NewsValue> news)
        {
            List<Attachment> attachments = new List<Attachment>();
            foreach (var n in news)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: n.image.thumbnail.contentUrl));

                CardAction action = new CardAction()
                {
                    Title = n.name,
                    Type = "openUrl",
                    Value = n.url
                };

                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = n.name,
                    Subtitle = n.description,
                    Images = cardImages,
                    Tap = action
                };
                attachments.Add(plCard.ToAttachment());
            }
            return attachments;
        }

        private async Task ReplyWithCards(IDialogContext context, List<Attachment> attachments)
        {
            var message = context.MakeMessage();
            message.Type = "message";
            message.Attachments = attachments;
            if (attachments.Count > 1)
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        private async Task ReplyWithCardAndNews(IDialogContext context, Attachment card, List<Attachment> attachments)
        {
            var message = context.MakeMessage();
            message.Type = "message";

            message.Attachments.Clear();
            message.Attachments.Add(card);
            await context.PostAsync(message);

            await context.PostAsync("Here are also some news about this player:");
            message.Attachments.Clear();
            message.Attachments = attachments;
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            await context.PostAsync(message);

            context.Wait(MessageReceived);
        }
    }
}