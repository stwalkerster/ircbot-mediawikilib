namespace Stwalkerster.Bot.MediaWikiLib.Tests
{
    using Castle.Core.Logging;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.Bot.MediaWikiLib.Configuration;

    public class TestBase
    {
        protected ILogger LoggerMock { get; private set; }
        protected IMediaWikiConfiguration AppConfigMock { get; private set; }

        [SetUp]
        public void Setup()
        {
            this.LoggerMock = Substitute.For<ILogger>();
            this.AppConfigMock = Substitute.For<IMediaWikiConfiguration>();
        }
    }
}