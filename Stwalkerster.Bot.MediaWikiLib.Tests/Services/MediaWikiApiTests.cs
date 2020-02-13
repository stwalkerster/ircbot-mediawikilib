namespace Stwalkerster.Bot.MediaWikiLib.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Moq;
    using NUnit.Framework;
    using Stwalkerster.Bot.MediaWikiLib.Exceptions;
    using Stwalkerster.Bot.MediaWikiLib.Model;
    using Stwalkerster.Bot.MediaWikiLib.Services;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    [TestFixture]
    public class MediaWikiApiTests : TestBase
    {
        private Mock<IWebServiceClient> wsClient;
        private MediaWikiApi mwApi;

        [SetUp]
        public void LocalSetup()
        {
            this.wsClient = new Mock<IWebServiceClient>();
            this.mwApi = new MediaWikiApi(this.LoggerMock.Object, this.wsClient.Object, this.AppConfigMock.Object);
        }

        [Test, TestCaseSource(typeof(MediaWikiApiTests), "GroupParseTestCases")]
        public List<string> ShouldParseGroupsCorrectly(string user, string input)
        {
            // arrange
            var memstream = new MemoryStream();
            var sw = new StreamWriter(memstream);
            sw.Write(input);
            sw.Flush();
            memstream.Position = 0;

            this.wsClient
                .Setup(x => x.DoApiCall(It.IsAny<NameValueCollection>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(memstream);

            // act
            return this.mwApi.GetUserGroups(user).ToList();
        }

        [Test]
        public void ShouldParseGroupsOfNonexistentUser()
        {
            var memStream = new MemoryStream();
            var sw = new StreamWriter(memStream);
            sw.Write("<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><users><user name=\"Stwnonexist\" missing=\"\" /></users></query></api>");
            sw.Flush();
            memStream.Position = 0;

            this.wsClient
                .Setup(x => x.DoApiCall(It.IsAny<NameValueCollection>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(memStream);

            Assert.Throws<MissingUserException>(() => this.mwApi.GetUserGroups("Stwnonexist"));
        }

        [Test, TestCaseSource(typeof(MediaWikiApiTests), "CategoryParseTestCases")]
        public bool ShouldParseCategoriesCorrectly(string input)
        {
            // arrange
            var memstream = new MemoryStream();
            var sw = new StreamWriter(memstream);
            sw.Write(input);
            sw.Flush();
            memstream.Position = 0;

            this.wsClient
                .Setup(x => x.DoApiCall(It.IsAny<NameValueCollection>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(memstream);

            // act
            return this.mwApi.PageIsInCategory(string.Empty, string.Empty);
        }

        [Test, TestCaseSource(typeof(MediaWikiApiTests), "PageInformationTestCases")]
        public PageInformation ShouldParsePageDataCorrectly(string input)
        {
            // arrange
            var memoryStream = new MemoryStream();
            var sw = new StreamWriter(memoryStream);
            sw.Write(input);
            sw.Flush();
            memoryStream.Position = 0;

            this.wsClient
                .Setup(x => x.DoApiCall(It.IsAny<NameValueCollection>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieContainer>(), It.IsAny<bool>()))
                .Returns(memoryStream);

            // act
            return this.mwApi.GetPageInformation(string.Empty);
        }

        [Test]
        public void ShouldRetrieveCategoriesCorrectly()
        {
            // arrange
            var title = "Draft:Al Silva";

            var return1 = "<api><continue clcontinue=\"58543806|AfC_submissions_declined_as_an_advertisement\" continue=\"||\" /><query><pages><page _idx=\"58543806\" pageid=\"58543806\" ns=\"118\" title=\"Draft:Al Silva\"><categories><cl ns=\"14\" title=\"Category:AfC submissions by date/21 September 2018\" sortkey=\"293f044d393f53290306293f044d393f5329011501dcc4dcc1dcc4dc08\" sortkeyprefix=\"Al Silva\" /><cl ns=\"14\" title=\"Category:AfC submissions declined as a non-notable biography\" sortkey=\"293f044d393f5329010c01dcc4dc08\" sortkeyprefix=\"\" /></categories></page></pages></query></api>";
            var stream1 = new MemoryStream();
            var sw = new StreamWriter(stream1);
            sw.Write(return1);
            sw.Flush();
            stream1.Position = 0;

            var return2 = "<api><query><pages><page _idx=\"58543806\" pageid=\"58543806\" ns=\"118\" title=\"Draft:Al Silva\"><categories><cl ns=\"14\" title=\"Category:AfC submissions declined as an advertisement\" sortkey=\"293f044d393f5329010c01dcc4dc08\" sortkeyprefix=\"\" /><cl ns=\"14\" title=\"Category:Declined AfC submissions\" sortkey=\"293f044d393f5329010c01dcc4dc08\" sortkeyprefix=\"\" /></categories></page></pages></query></api>";
            var stream2 = new MemoryStream();
            var sw2 = new StreamWriter(stream2);
            sw2.Write(return2);
            sw2.Flush();
            stream2.Position = 0;

            this.wsClient
                .SetupSequence(x => x.DoApiCall(It.IsAny<NameValueCollection>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stream1)
                .Returns(stream2);

            // act
            var categoriesOfPage = this.mwApi.GetCategoriesOfPage(title).ToList();

            // assert
            Assert.AreEqual(4, categoriesOfPage.Count);

            Assert.Contains("Category:Declined AfC submissions", categoriesOfPage.Select(x => x.Key).ToList());
            Assert.Contains("Category:AfC submissions by date/21 September 2018", categoriesOfPage.Select(x => x.Key).ToList());
        }

        [Test]
        public void ShouldParseBlocksCorrectly()
        {
            var return1 = "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><blocks><block id=\"9062654\" user=\"Od Mishehu\" by=\"BU Rob13\" timestamp=\"2019-06-05T22:52:39Z\" expiry=\"infinity\" reason=\"{{checkuserblock-account}}\" nocreate=\"\" autoblock=\"\" allowusertalk=\"\" /></blocks></query></api>";
            var stream1 = new MemoryStream();
            var sw = new StreamWriter(stream1);
            sw.Write(return1);
            sw.Flush();
            stream1.Position = 0;

            this.wsClient
                .Setup(x => x.DoApiCall(It.IsAny<NameValueCollection>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stream1);

            var blocks = this.mwApi.GetBlockInformation("foo");

            Assert.Pass();
        }

        public static IEnumerable<TestCaseData> GroupParseTestCases
        {
            get
            {
                yield return new TestCaseData(
                        "Stwalkerster",
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><users><user userid=\"851859\" name=\"Stwalkerster\"><groups><g>abusefilter</g><g>sysop</g><g>*</g><g>user</g><g>autoconfirmed</g></groups></user></users></query></api>")
                    .Returns(new List<string> {"abusefilter", "sysop", "*", "user", "autoconfirmed"});
                yield return new TestCaseData(
                        "127.0.0.1",
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><users><user name=\"127.0.0.1\" invalid=\"\" /></users></query></api>")
                    .Returns(new List<string> {"*"});
            }
        }

        public static IEnumerable<TestCaseData> CategoryParseTestCases
        {
            get
            {
                yield return new TestCaseData(
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><pages><page _idx=\"534366\" pageid=\"534366\" ns=\"0\" title=\"Barack Obama\"><categories><cl ns=\"14\" title=\"Category:Living people\" /></categories></page></pages></query></api>")
                    .Returns(true);
                yield return new TestCaseData(
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><pages><page _idx=\"534366\" pageid=\"534366\" ns=\"0\" title=\"Barack Obama\" /></pages></query></api>")
                    .Returns(false);
                yield return new TestCaseData(
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><pages><page _idx=\"-1\" ns=\"0\" title=\"Nonexisdyrnfdnkdkdkkfd\" missing=\"\" /></pages></query></api>")
                    .Returns(false);
            }
        }

        public static IEnumerable<TestCaseData> PageInformationTestCases
        {
            get
            {
                // A => B redirect
                yield return new TestCaseData(
                        "<api batchcomplete=\"\"><query><redirects><r from=\"Foo\" to=\"Foobar\" /></redirects><pages><page _idx=\"11178\" pageid=\"11178\" ns=\"0\" title=\"Foobar\" contentmodel=\"wikitext\" pagelanguage=\"en\" pagelanguagehtmlcode=\"en\" pagelanguagedir=\"ltr\" touched=\"2018-10-23T14:49:15Z\" lastrevid=\"864172043\" length=\"7598\"><revisions><rev user=\"Sam Sailor\" comment=\"Adding local [[Wikipedia:Short description|short description]]: &quot;Use of metasyntactic variables in computer programming&quot; ([[User:Galobtter/Shortdesc helper|Shortdesc helper]])\" /></revisions><protection /><restrictiontypes><rt>edit</rt><rt>move</rt></restrictiontypes></page></pages></query></api>")
                    .Returns(
                        new PageInformation(
                            new List<string> {"Foo"},
                            new List<PageProtection>(),
                            "Foobar",
                            7598,
                            "Adding local [[Wikipedia:Short description|short description]]: \"Use of metasyntactic variables in computer programming\" ([[User:Galobtter/Shortdesc helper|Shortdesc helper]])",
                            "Sam Sailor",
                            new DateTime(2018, 10, 23, 14, 49, 15)
                        ));

                // Normal article (edit-protected)
                yield return new TestCaseData(
                        "<api batchcomplete=\"\"><query><pages><page _idx=\"23501\" pageid=\"23501\" ns=\"0\" title=\"Potato\" contentmodel=\"wikitext\" pagelanguage=\"en\" pagelanguagehtmlcode=\"en\" pagelanguagedir=\"ltr\" touched=\"2018-10-23T21:24:52Z\" lastrevid=\"865424894\" length=\"93906\"><revisions><rev user=\"Northamerica1000\" comment=\"/* External links */ nav bar layout\" /></revisions><protection><pr type=\"edit\" level=\"autoconfirmed\" expiry=\"infinity\" /></protection><restrictiontypes><rt>edit</rt><rt>move</rt></restrictiontypes></page></pages></query></api>")
                    .Returns(
                        new PageInformation(
                            new List<string>(),
                            new List<PageProtection>
                            {
                                new PageProtection("edit", "autoconfirmed", null)
                            },
                            "Potato",
                            93906,
                            "/* External links */ nav bar layout",
                            "Northamerica1000",
                            new DateTime(2018, 10, 23, 21, 24, 52)
                        ));

                // A => B => C redirects
                yield return new TestCaseData(
                        "<api batchcomplete=\"\"><query><redirects><r from=\"User:Stwalkerster/Sandbox/r1\" to=\"User:Stwalkerster/Sandbox/r2\" /><r from=\"User:Stwalkerster/Sandbox/r2\" to=\"User:Stwalkerster/Sandbox\" /></redirects><pages><page _idx=\"31745629\" pageid=\"31745629\" ns=\"2\" title=\"User:Stwalkerster/Sandbox\" contentmodel=\"wikitext\" pagelanguage=\"en\" pagelanguagehtmlcode=\"en\" pagelanguagedir=\"ltr\" touched=\"2018-10-09T08:15:11Z\" lastrevid=\"851646323\" length=\"1280\"><revisions><rev user=\"Stwalkerster\" comment=\"/* Speedy deletion nomination of File:Sriti jha image.jpg */ tweak wording\" /></revisions><protection /><restrictiontypes><rt>edit</rt><rt>move</rt></restrictiontypes></page></pages></query></api>")
                    .Returns(
                        new PageInformation(
                            new List<string>{"User:Stwalkerster/Sandbox/r1", "User:Stwalkerster/Sandbox/r2"},
                            new List<PageProtection>(),
                            "User:Stwalkerster/Sandbox",
                            1280,
                            "/* Speedy deletion nomination of File:Sriti jha image.jpg */ tweak wording",
                            "Stwalkerster",
                            new DateTime(2018,10,9,8,15,11)
                        ));

                // A => A redirect
                yield return new TestCaseData(
                        "<api batchcomplete=\"\"><query><redirects><r from=\"User:Stwalkerster/Sandbox/r1\" to=\"User:Stwalkerster/Sandbox/r1\" /></redirects></query></api>")
                    .Returns(new PageInformation(new List<string> {"User:Stwalkerster/Sandbox/r1"}));

                // A => B => A redirect
                yield return new TestCaseData(
                        "<api batchcomplete=\"\"><query><redirects><r from=\"User:Stwalkerster/Sandbox/r1\" to=\"User:Stwalkerster/Sandbox/r2\" /><r from=\"User:Stwalkerster/Sandbox/r2\" to=\"User:Stwalkerster/Sandbox/r1\" /></redirects></query></api>")
                    .Returns(new PageInformation(new List<string> {"User:Stwalkerster/Sandbox/r1", "User:Stwalkerster/Sandbox/r2"}));

                // A => B (missing, create protected)
                yield return new TestCaseData(
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><redirects><r from=\"User:Stwalkerster/sandbox/r1\" to=\"User:Stwalkerster/sandbox/r2\" /></redirects><pages><page _idx=\"-1\" ns=\"2\" title=\"User:Stwalkerster/sandbox/r2\" missing=\"\" contentmodel=\"wikitext\" pagelanguage=\"en\" pagelanguagehtmlcode=\"en\" pagelanguagedir=\"ltr\"><protection><pr type=\"create\" level=\"autoconfirmed\" expiry=\"2018-10-25T02:00:38Z\" /></protection><restrictiontypes><rt>create</rt></restrictiontypes></page></pages></query></api>")
                    .Returns(
                        new PageInformation(
                            new List<string> {"User:Stwalkerster/sandbox/r1"},
                            new List<PageProtection>
                                {new PageProtection("create", "autoconfirmed", new DateTime(2018, 10, 25, 2, 0, 38))},
                            "User:Stwalkerster/sandbox/r2",
                            true
                        ));

                // Redirect protection does nothing to affect the result when &redirects=true, including (as expected) for an A => B => A configuration.

            }
        }
    }
}