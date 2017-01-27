namespace HandOffToHumanBot
{
    using System;
    using System.Threading.Tasks;
    using MessageRouting;
    using MessageRouting.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string UserSymptomsMessageKey = "UserSymptomsMessage";

        private readonly MessageRouter messageRouter;

        public RootDialog(MessageRouter messageRouter)
        {
            this.messageRouter = messageRouter;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // Check if the message should be routed to an agent or an user
            bool routed = await this.messageRouter.RouteAsync(context, message, this.MessageReceivedAsync);

            if (!routed)
            {
                PromptDialog.Text(context, this.AfterSymptomsDescribed, "Hi there. Please describe your symptoms.");
            }
        }

        private async Task AfterSymptomsDescribed(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("Thanks for describing your symptoms. I'm looking in my knowledge base to try to help you...");

            var symptoms = await result;

            if (symptoms.ToLowerInvariant().Contains("leg"))
            {
                await context.PostAsync($"I'm sorry, I couldn't find anything about your symptoms: \"{symptoms}\"");

                context.PrivateConversationData.SetValue(UserSymptomsMessageKey, symptoms);

                PromptDialog.Confirm(context, this.ResumeAfterConfirm, "Do you want to me to share your symptoms with a real doctor and connect you with him?");
            }
            else
            {
                var escapedSymptoms = Uri.EscapeUriString(symptoms);

                await context.PostAsync($@"This is what I found about your symptoms: http://www.bing.com/search?q={escapedSymptoms}");
            }
        }

        private async Task ResumeAfterConfirm(IDialogContext context, IAwaitable<bool> result)
        {
            var connectWithAgent = await result;

            if (connectWithAgent)
            {
                await context.PostAsync("Perfect. Let me try to find a doctor available for you....");

                var symptoms = context.PrivateConversationData.Get<string>(UserSymptomsMessageKey);

                var user = new User(Address.FromActivity(context.Activity), context.Activity.From.Name);

                // Check if there is an agent available to rout the user message
                if (await this.messageRouter.TryRoutingToAvailableAgent(symptoms, user))
                {
                    context.PrivateConversationData.RemoveValue(UserSymptomsMessageKey);

                    await context.PostAsync("OK. You are now connected with a real doctor.");
                }
                else
                {
                    await context.PostAsync("I'm sorry. All our doctors are busy right now. Please come back later");
                }
            }
            else
            {
                await context.PostAsync("OK, nevermind! It wasn't so urgent then :)");
            }

            context.Wait(this.MessageReceivedAsync);
        }
    }
}