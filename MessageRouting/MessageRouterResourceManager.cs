namespace MessageRouting
{
    using System;
    using System.Reflection;
    using System.Resources;
    using Resources;

    [Serializable]
    public class MessageRouterResourceManager
    {
        private readonly ResourceManager resourceManager;

        public MessageRouterResourceManager() :
           this(null, null)
        {
        }

        internal MessageRouterResourceManager(Assembly resourceAssembly = null, string resourceName = null)
        {
            if (resourceAssembly == null || resourceName == null)
            {
                resourceAssembly = typeof(MessageRouter).Assembly;
                resourceName = typeof(Strings).FullName;
            }

            this.resourceManager = new ResourceManager(resourceName, resourceAssembly);
        }

        public virtual string EndOfConversationMessageForUser => this.GetResource(nameof(Strings.EndOfConversationMessageForUser));

        public virtual string EndOfConversationCommand => this.GetResource(nameof(Strings.EndOfConversationCommand));

        public virtual string RegisterCommand => this.GetResource(nameof(Strings.RegisterCommand));

        public virtual string AgentNotBusyMessage => this.GetResource(nameof(Strings.AgentNotBusyMessage));

        public virtual string EndOfConversationMessageForAgent(string agent, string user)
        {
            return this.Format(this.GetResource(nameof(Strings.EndOfConversationMessageForAgent)), new[] { agent, user });
        }

        public virtual string UserToAgentMessage(string user, string message)
        {
            return this.Format(this.GetResource(nameof(Strings.UserToAgentMessage)), new[] { user, message });
        }

        public virtual string AgentRegisteredMessage(string agent)
        {
            return this.Format(this.GetResource(nameof(Strings.AgentRegisteredMessage)), new[] { agent });
        }

        public virtual string AgentConnectedWithUserMessage(string agent, string user, string message)
        {
            return this.Format(this.GetResource(nameof(Strings.AgentConnectedWithUserMessage)), new[] { agent, user, message, this.EndOfConversationCommand });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1127:AvoidStringFormatUseStringInterpolation", Justification = "Reviewed. Formatting Resource string.")]
        private string Format(string format, object[] args)
        {
            return string.Format(format, args);
        }

        private string GetResource(string name)
        {
            return this.resourceManager.GetString(name) ?? Strings.ResourceManager.GetString(name);
        }
    }
}