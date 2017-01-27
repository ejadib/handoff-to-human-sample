namespace MessageRouting
{
    using Autofac;
    using Microsoft.Bot.Builder.Internals.Fibers;

    public class MessageRouterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<MessageRouterResourceManager>()
               .Keyed<MessageRouterResourceManager>(FiberModule.Key_DoNotSerialize)
               .AsSelf()
               .SingleInstance();

            builder.RegisterType<MessageRouter>()
                .Keyed<MessageRouter>(FiberModule.Key_DoNotSerialize)
                .AsSelf()
                .SingleInstance();
        }
    }
}
