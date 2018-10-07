namespace Stwalkerster.Bot.MediaWikiLib.Tests
{
    using Castle.Core.Logging;
    using Moq;
    using NUnit.Framework;
    using Stwalkerster.Bot.MediaWikiLib.Configuration;

    public class TestBase
    {
        protected Mock<ILogger> LoggerMock { get; private set; }
        protected Mock<IMediaWikiConfiguration> AppConfigMock { get; private set; }

        [SetUp]
        public void Setup()
        {
            this.LoggerMock = new Mock<ILogger>();
            this.AppConfigMock = new Mock<IMediaWikiConfiguration>();
        }
    }
}