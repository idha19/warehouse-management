using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using warehouse.Helpers;
using warehouse.PageAssembly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.Extensions;

namespace warehouse.Support
{
    /// <summary>
    /// Hooks
    /// </summary>
    [Binding]
    public sealed class Hooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        protected Browsers? Browser;
        protected ExtentReportsHelper? _extend;
        protected ScenarioContext _scenarioContext;
        protected bool SameProcess;

        /// <summary>
        /// BeforeScenario
        /// </summary>
        [BeforeScenario]
        public void BeforeScenario()
        {
            // Example of filtering hooks using tags. (in this case, this 'before scenario' hook will execute if the feature/scenario contains the tag '@tag1')
            // See https://docs.specflow.org/projects/specflow/en/latest/Bindings/Hooks.html?highlight=hooks#tag-scoping

            //TODO: implement logic that has to run before executing each scenario

            _extend = new ExtentReportsHelper();
            SameProcess = false;
            if (SameProcess && Browser != null) return;
            Browser = new Browsers(); /// ini ada yang aku ubah
            Browser.Init();
            BasePagesHelper.SetBasePage(Browser, _extend);
        }

        /// <summary>
        /// AfterScenario
        /// </summary>
        /// <returns></returns>
        [AfterScenario]
        public async Task AfterScenario()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var stacktrace = TestContext.CurrentContext.Result.StackTrace;
                var errorMessage = "<pre>" + TestContext.CurrentContext.Result.Message + "</pre>";
                switch (status)
                {
                    case TestStatus.Failed:
                        _extend?.SetTestStatusFail($"<br>{errorMessage}<br>Stack Trace: <br>{stacktrace}<br>");
                        SaveScreenshot("Failed", TestContext.CurrentContext.Test.Name);
                        break;
                    case TestStatus.Skipped:
                        _extend?.SetTestStatusSkipped();
                        break;
                    case TestStatus.Inconclusive:
                        break;
                    case TestStatus.Passed:
                        SaveScreenshot("Passed", TestContext.CurrentContext.Test.Name);
                        break;
                    case TestStatus.Warning:
                        break;
                    default:
                        _extend?.SetTestStatusPass();
                        break;
                }
                await SaveDownloadedFile();
            }
            finally
            {
                if (!SameProcess)
                {
                    Browser?.Close();
                }
            }
        }

        /// <summary>
        /// SaveScreenshot
        /// </summary>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="delay"></param>
        protected void SaveScreenshot(string status, string name, int delay = 0)
        {
            Thread.Sleep(delay);
            var screenshot = Browser?.GetDriver.GetScreenshot();
            name = name + "_" + status;
            new List<string> { "\"", ",", "(", ")", "null" }.ForEach(m => name = name.Replace(m, ""));
            new List<string> { @"\" }.ForEach(m => name = name.Replace(m, "_"));
            new List<string> { "__" }.ForEach(m => name = name.Replace(m, "_"));
            var screenshotFile = DirectoryHelper.GetScreenshotDirectory($"{name}.png");
            var folderSavePath = Path.GetDirectoryName(screenshotFile);
            folderSavePath = string.IsNullOrEmpty(folderSavePath) ? "./" : folderSavePath;
 
            if (!Directory.Exists(folderSavePath))
            {
                Directory.CreateDirectory(folderSavePath);
            }
            screenshot?.SaveAsFile(screenshotFile);
            SaveAttachment(screenshotFile, $"{status} Test");
        }


        /// <summary>
        /// SaveAttachment
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="description"></param>
        private void SaveAttachment(string fileName, string description)
        {
            TestContext.AddTestAttachment(fileName, description);
        }

        /// <summary>
        /// SaveDownloadedFile
        /// </summary>
        /// <returns></returns>
        private async Task SaveDownloadedFile()
        {
            var sessionId = ((RemoteWebDriver?)Browser?.GetDriver)?.SessionId.ToString();
            var isNewArchitecture = TestContext.Parameters.Get<bool>("IsNewArchitecture", false);
            var url = TestContext.Parameters.Get<string>("SeleniumRemote", "10.2.41.10:4444");
            var baseAPI = URLHelper.GetTestServer(url) + "/download/";
            var client = new HttpClient();

            if (isNewArchitecture)
            {
                var username = TestContext.Parameters.Get<string>("SeleniumUsername", "admin");
                var password = TestContext.Parameters.Get<string>("SeleniumPassword", "Admin123");
                var authenticationString = URLHelper.GetServerAuth(username, password);
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }

            try
            {
                var getListDownloadedFileUrl = baseAPI + sessionId + "/?json";
                var getListDownloadedFileResponse = await client.GetAsync(getListDownloadedFileUrl);

                if (getListDownloadedFileResponse.IsSuccessStatusCode)
                {
                    var listDownloadedFileJson = await getListDownloadedFileResponse.Content.ReadAsStringAsync();
                    var listDownloadedFiles = JsonConvert.DeserializeObject<List<string>>(listDownloadedFileJson);

                    if (listDownloadedFiles.Count > 0)
                    {
                        foreach (var fileName in listDownloadedFiles)
                        {
                            string downloadedFile = DirectoryHelper.GetDownloadsDirectory(fileName);
                            string? folderSavePath = Path.GetDirectoryName(downloadedFile);
                            folderSavePath = string.IsNullOrEmpty(folderSavePath) ? "./" : folderSavePath;

                            if (!Directory.Exists(folderSavePath))
                            {
                                Directory.CreateDirectory(folderSavePath);
                            }

                            var downloadFileUrl = baseAPI + sessionId + "/" + fileName;
                            var downloadResponse = await client.GetAsync(downloadFileUrl);

                            using (var stream = await downloadResponse.Content.ReadAsStreamAsync())
                            {
                                using (FileStream destinationStream = File.Create(downloadedFile))
                                {
                                    await stream.CopyToAsync(destinationStream);
                                    SaveAttachment(downloadedFile, fileName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// GetLogs
        /// </summary>
        /// <returns></returns>
        private ReadOnlyCollection<LogEntry>? GetLogs()
        {
            return Browser?.GetDriver?.Manage().Logs.GetLog(LogType.Browser);
        }

        /// <summary>
        /// CloseAll
        /// </summary>
        [OneTimeTearDown]
        public void CloseAll()
        {
            Browser?.Close();
            _extend?.Close();
        }

        /// <summary>
        /// TestHelper
        /// </summary>
        public class TestHelper
        {
            /// <summary>
            /// GetBrowserName
            /// </summary>
            /// <returns></returns>
            public static string GetBrowserName()
            {
                return TestContext.Parameters.Get<string>("SeleniumBrowser", "firefox");
            }

            /// <summary>
            /// IsMobile
            /// </summary>
            /// <returns></returns>
            public static bool IsMobile()
            {
                return TestContext.Parameters.Get<bool>("IsMobile", false);
            }
        }

        /// <summary>
        /// DelayAfterTest
        /// </summary>
        public class DelayAfterTest : Attribute, ITestAction
        {
            /// <summary>
            /// DelayAfterTest
            /// </summary>
            /// <param name="v"></param>
            public DelayAfterTest(int v)
            {
                this.Delay = v;
            }

            /// <summary>
            /// Delay
            /// </summary>
            /// <value></value>
            public int Delay { get; set; }
            /// <summary>
            /// Targets
            /// </summary>
            /// <value></value>
            public ActionTargets Targets { get; private set; }

            /// <summary>
            /// AfterTest
            /// </summary>
            /// <param name="test"></param>
            public void AfterTest(ITest test)
            {
                Thread.Sleep(this.Delay);
            }

            /// <summary>
            /// BeforeTest
            /// </summary>
            /// <param name="test"></param>
            public void BeforeTest(ITest test)
            {
            }
        }

        /// <summary>
        /// RunIfBrowserIs
        /// </summary>
        public class RunIfBrowserIs : Attribute, ITestAction
        {
            /// <summary>
            /// RunIfBrowserIs
            /// </summary>
            /// <param name="v"></param>
            public RunIfBrowserIs(string v)
            {
                this.Browser = v;
            }

            /// <summary>
            /// Browser
            /// </summary>
            /// <value></value>
            public string Browser { get; set; }

            /// <summary>
            /// Targets
            /// </summary>
            /// <value></value>
            public ActionTargets Targets { get; private set; }

            /// <summary>
            /// AfterTest
            /// </summary>
            /// <param name="test"></param>
            public void AfterTest(ITest test)
            {
            }

            /// <summary>
            /// BeforeTest
            /// </summary>
            /// <param name="test"></param>
            public void BeforeTest(ITest test)
            {
                string b = TestHelper.GetBrowserName();
                bool isMobile = TestHelper.IsMobile();

                if (!b.ToLower().Equals(Browser.ToLower()) || isMobile)
                {
                    Assert.Ignore("Omitting {0}. Browser is not {1} or device is mobile", test.Name, b);
                }
            }
        }
    }
}