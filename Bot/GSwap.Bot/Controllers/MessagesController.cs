using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;
using System.Net;
using System.Configuration;
using QnABot.Models.MealPositions;
using GSwap.Services;
using System.Collections.Generic;

namespace Microsoft.Bot.Sample.QnABot
{
    //[BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        private IGetMeals _getMeals;
        private ISiteConfigService _siteConfigService;
        private IExternalService _externalService;

        public MessagesController(IGetMeals getMeals, ISiteConfigService siteConfigService, IExternalService externalService)
        {
            _getMeals = getMeals;
            _siteConfigService = siteConfigService;
            _externalService = externalService;
        }

        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            try
            {
                if (activity.GetActivityType() == ActivityTypes.Message)
                {
                    string appdId = "*APP ID HERE*";
                    string appPassword = "*APP PASSWORD HERE*";


                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl), appdId, appPassword);

                    MicrosoftAppCredentials.TrustServiceUrl(activity.ServiceUrl);

                    MicrosoftAppCredentials.IsTrustedServiceUrl(activity.ServiceUrl);
                    var channelId = activity.ChannelId;

                    await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog(channelId, _getMeals, _siteConfigService, _externalService));

                    return Request.CreateResponse(HttpStatusCode.OK);

                }
                else
                {
                    await HandleSystemMessageAsync(activity);
                }
                ;
            }
            catch (Exception e)
            {

                String s = e.ToString();
                throw;
            }



            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                IConversationUpdateActivity iConversationUpdated = message as IConversationUpdateActivity;
                if (iConversationUpdated != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));

                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then 
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            

                            Activity replyNow = ((Activity)iConversationUpdated).CreateReply();
                            List<CardImage> cardImages = new List<CardImage>();

                            string imageURL = "https://uproxx.files.wordpress.com/2016/03/robot-burger2-uproxx.jpg?quality=100&w=650";
                            cardImages.Add(new CardImage(url: imageURL));
                            string subtitle = "";
                            subtitle = @"Hi! I'm GSwap Bot. Say Hi if you'd like to chat.";
                            HeroCard plCard = new HeroCard()
                            {
                                Title = "Welcome!",
                                Subtitle = subtitle,
                                Images = cardImages,
                                Buttons = null
                            };

                            replyNow.Attachments.Add(plCard.ToAttachment());

                            await connector.Conversations.ReplyToActivityAsync(replyNow);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
