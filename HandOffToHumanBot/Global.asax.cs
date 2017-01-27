namespace HandOffToHumanBot
{
    using System.Web.Http;
    using Autofac;
    using MessageRouting;
    using Microsoft.Bot.Builder.Dialogs;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<MessageRouterModule>();

            builder.Update(Conversation.Container);
        }
    }
}
