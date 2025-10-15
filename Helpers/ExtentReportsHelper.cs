// -----------------------------------------------------------------------
// <copyright file="ExtentReportsHelper.cs" company="PT United Tractos Tbk.">
// Copyright (c) 2021 Ahmad Ilman Fadilah. All rights reserved.
// </copyright>
// <author>Ahmad Ilman Fadilah, ahmadilmanfadilah@gmail.com</author>
// -----------------------------------------------------------------------

using System;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using Reporter = AventStack.ExtentReports.ExtentReports;

namespace warehouse.Helpers
{
    /// <summary>
    /// ExtentReportsHelper
    /// </summary>
    public class ExtentReportsHelper
    {
        private Reporter Extent { get; }
        private ExtentHtmlReporter Reporter { get; }
        private ExtentTest? Test { get; set; }

        /// <summary>
        /// ExtentReportsHelper
        /// </summary>
        public ExtentReportsHelper()
        {
            Extent = new Reporter();
            var reportDirectory = DirectoryHelper.GetReportsDirectory();
            Reporter = new ExtentHtmlReporter(reportDirectory)
            {
                Config =
                {
                    DocumentTitle = $"Automation Testing Report",
                    ReportName = "Regression Testing",
                    Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard
                }
            };
            var appName = TestContext.Parameters.Get<string>("AppName", "PartOnlineTransaction");
            var appVersion = TestContext.Parameters.Get<string>("AppVersion", "123-1");
            Extent.AttachReporter(Reporter);
            Extent.AddSystemInfo("Application Under Test", $"{appName} {appVersion}");
            Extent.AddSystemInfo("Environment", $"QA, .NET {Environment.Version.ToString()}");
            Extent.AddSystemInfo("Machine", Environment.MachineName);
            Extent.AddSystemInfo("OS", Environment.OSVersion.VersionString);
        }

        /// <summary>
        /// CreateTest
        /// </summary>
        /// <param name="testName"></param>
        public void CreateTest(string testName)
        {
            Test = Extent.CreateTest(testName);
        }

        /// <summary>
        /// SetStepStatusPass
        /// </summary>
        /// <param name="stepDescription"></param>
        public void SetStepStatusPass(string stepDescription)
        {
            Test?.Log(Status.Pass, stepDescription);
        }

        /// <summary>
        /// SetStepStatusWarning
        /// </summary>
        /// <param name="stepDescription"></param>
        public void SetStepStatusWarning(string stepDescription)
        {
            Test?.Log(Status.Warning, stepDescription);
        }

        /// <summary>
        /// SetTestStatusPass
        /// </summary>
        public void SetTestStatusPass()
        {
            Test?.Pass("Test Executed Sucessfully!");
        }

        /// <summary>
        /// SetTestStatusFail
        /// </summary>
        /// <param name="message"></param>
        public void SetTestStatusFail(string? message = null)
        {
            var printMessage = "<p><b>Test FAILED!</b></p>";
            if (!string.IsNullOrEmpty(message))
            {
                printMessage += $"Message: <br>{message}<br>";
            }

            Test?.Fail(printMessage);
        }

        /// <summary>
        /// AddTestFailureScreenshot
        /// </summary>
        /// <param name="base64ScreenCapture"></param>
        public void AddTestFailureScreenshot(string? base64ScreenCapture)
        {
            Test?.AddScreenCaptureFromBase64String(base64ScreenCapture, "Screenshot on Error:");
        }

        /// <summary>
        /// AddTestPassScreenshot
        /// </summary>
        /// <param name="base64ScreenCapture"></param>
        public void AddTestPassScreenshot(string? base64ScreenCapture)
        {
            Test?.AddScreenCaptureFromBase64String(base64ScreenCapture, "Screenshot on pass:");
        }

        /// <summary>
        /// AddTestInfoScreenshot
        /// </summary>
        /// <param name="base64ScreenCapture"></param>
        public void AddTestInfoScreenshot(string? base64ScreenCapture)
        {
            Test?.AddScreenCaptureFromBase64String(base64ScreenCapture, "Screenshot on info:");
        }

        /// <summary>
        /// SetTestStatusSkipped
        /// </summary>
        public void SetTestStatusSkipped()
        {
            Test?.Skip("Test skipped!");
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            try
            {
                Extent.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }
        /// <summary>
        /// Log information (for step tracking)
        /// </summary>
        /// <param name="message"></param>
        public void LogInfo(string message)
        {
            Test?.Log(Status.Info, message);
        }

    }
}