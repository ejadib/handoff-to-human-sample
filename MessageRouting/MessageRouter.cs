namespace MessageRouting
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;

    [Serializable]
    public class MessageRouter
    {
        private readonly MessageRouterResourceManager resourceManager;

        // TODO: Extract this into an interface so it can be replaced with a real storage mechanism
        private ConcurrentBag<Agent> registeredAgents = new ConcurrentBag<Agent>();

        public MessageRouter(MessageRouterResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public async Task<bool> RouteAsync(IDialogContext context, IMessageActivity message, ResumeAfter<IMessageActivity> resume)
        {
            // Check if the incoming message is from a registered agent
            var agent = this.registeredAgents.SingleOrDefault(x => x.AddressInformation.UserId == message.From.Id);

            if (agent != null)
            {
                // If the agent is busy (has an user assigned), pass through his message to the user
                if (agent.IsBusy)
                {
                    var user = agent.AssignedUser;

                    // Check if the incoming message from the agent is the command to end the conversation with the user
                    if (message.Text.Equals(this.resourceManager.EndOfConversationCommand, StringComparison.InvariantCultureIgnoreCase))
                    {
                        await PassThroughMessageAsync(user.AddressInformation, this.resourceManager.EndOfConversationMessageForUser);

                        await context.PostAsync(this.resourceManager.EndOfConversationMessageForAgent(agent.Name, agent.AssignedUser.Name));

                        agent.SetAvailable();
                    }
                    else
                    {
                        // Pass-through agent message to user
                        await PassThroughMessageAsync(user.AddressInformation, message.Text);
                    }

                    context.Wait(resume);

                    return true;
                }
                else
                {
                    await context.PostAsync(this.resourceManager.AgentNotBusyMessage);

                    context.Wait(resume);

                    return true;
                }
            }

            // Check if the incoming message is from an user that is being attended by an agent
            var agentForUser = this.registeredAgents.SingleOrDefault(x => x.AssignedUser?.AddressInformation.UserId == message.From.Id);

            if (agentForUser != null)
            {
                // Pass-through user message to agent
                await PassThroughMessageAsync(agentForUser.AddressInformation, this.resourceManager.UserToAgentMessage(agentForUser.AssignedUser.Name, message.Text));

                context.Wait(resume);

                return true;
            }

            // Check if the incoming message is the command to register an agent
            // TODO: this is really approach for demo purposes. These should be extracted; so the logic to register agents can be replaced
            if (message.Text.Equals(this.resourceManager.RegisterCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                var newAgent = new Agent(Address.FromActivity(message), message.From.Name);

                this.registeredAgents.Add(newAgent);

                await context.PostAsync(this.resourceManager.AgentRegisteredMessage(newAgent.Name));

                context.Wait(resume);

                return true;
            }

            return false;
        }

        public async Task<bool> TryRoutingToAvailableAgent(string message, User user)
        {
            var availableAgent = this.registeredAgents.FirstOrDefault(x => !x.IsBusy);

            if (availableAgent != null)
            {
                availableAgent.AssignUser(user);

                await PassThroughMessageAsync(availableAgent.AddressInformation, this.resourceManager.AgentConnectedWithUserMessage(availableAgent.Name, user.Name, message));

                return true;
            }
            
            // TODO: If there is no agent availables; user should be put into a Waiting state 
            return false;
        }
        
        private static async Task PassThroughMessageAsync(Address address, string text)
        {
            var to = new ChannelAccount(address.UserId, string.Empty);
            var from = new ChannelAccount(address.BotId, "Bot");
            var connector = new ConnectorClient(new Uri(address.ServiceUrl));

            IMessageActivity message = Activity.CreateMessageActivity();

            message.From = from;
            message.Recipient = to;
            message.Conversation = new ConversationAccount(id: address.ConversationId);
            message.Text = text;
            message.Locale = "en-Us";
            message.ChannelId = address.ChannelId;
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
}
