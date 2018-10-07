namespace Stwalkerster.Bot.MediaWikiLib.Startup
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Stwalkerster.Bot.MediaWikiLib.Services;

    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().InSameNamespaceAs<WebServiceClient>().WithServiceAllInterfaces()
            );
        }
    }
}