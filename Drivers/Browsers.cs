using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Appium;

namespace warehouse.PageAssembly
{
    // RemoteWebDriver dengan logging support
    internal class RemoteWebDriverWithLogs : RemoteWebDriver, ISupportsLogs
    {
        public RemoteWebDriverWithLogs(Uri remoteAddress, DriverOptions options)
            : base(remoteAddress, options.ToCapabilities(), TimeSpan.FromMinutes(3)) // timeout 3 menit
        {
        }
    }

    public class Browsers
    {
        private readonly string _remoteWebDriverUrl;
        private readonly string _browser;
        private readonly string _baseUrl;
        private readonly bool _isSelenoid;
        private readonly bool _isMobile;
        private readonly string _logLevel;

        public Browsers()
        {
            _remoteWebDriverUrl = TestContext.Parameters.Get<string>("SeleniumRemote", "http://localhost:4444/wd/hub");
            _browser = TestContext.Parameters.Get<string>("SeleniumBrowser", "chrome");
            _baseUrl = TestContext.Parameters.Get<string>("AppBaseURL", "https://www.google.com/");
            _isSelenoid = TestContext.Parameters.Get<bool>("IsSelenoid", false);
            _isMobile = TestContext.Parameters.Get<bool>("IsMobile", false);
            _logLevel = TestContext.Parameters.Get<string>("LogLevel", "All");

            // Pastikan URL selalu valid
            if (!_remoteWebDriverUrl.EndsWith("/wd/hub"))
                _remoteWebDriverUrl = _remoteWebDriverUrl.TrimEnd('/') + "/wd/hub";

            Console.WriteLine($"Remote WebDriver URL = {_remoteWebDriverUrl}");
            Console.WriteLine($"Base URL = {_baseUrl}");
        }

        public IWebDriver? GetDriver { get; private set; }

        private DriverOptions DetectWebDriver()
        {
            switch (_browser.ToLower())
            {
                case "chrome":
                    return new ChromeOptions { AcceptInsecureCertificates = true };
                case "firefox":
                    return new FirefoxOptions { AcceptInsecureCertificates = true };
                case "safari":
                    if (_isMobile)
                    {
                        var appiumOptions = new AppiumOptions();
                        appiumOptions.PlatformName = "IOS";
                        appiumOptions.AddAdditionalAppiumOption("udid", TestContext.Parameters.Get<string>("SeleniumUdid", "xxxxxxxxx"));
                        appiumOptions.DeviceName = TestContext.Parameters.Get<string>("SeleniumDeviceName", "iPhone 13 Pro Max");
                        appiumOptions.PlatformVersion = TestContext.Parameters.Get<string>("SeleniumPlatformVersion", "15.4");
                        appiumOptions.BrowserName = "safari";
                        return appiumOptions;
                    }
                    else
                    {
                        return new SafariOptions { PlatformName = "MAC" };
                    }
                case "microsoftedge":
                    return new EdgeOptions();
                default:
                    throw new Exception($"Browser {_browser} is not supported");
            }
        }

        public void Init()
        {
            GetDriver = GetWebDriver();

            if (GetDriver is IAllowsFileDetection allowsDetection)
                allowsDetection.FileDetector = new LocalFileDetector();

            Goto("");
        }

        private IWebDriver GetWebDriver()
        {
            DriverOptions options = DetectWebDriver();

            // Set log level
            var logLevel = _logLevel.ToLower() switch
            {
                "all" => LogLevel.All,
                "info" => LogLevel.Info,
                "warning" => LogLevel.Warning,
                "severe" => LogLevel.Severe,
                "off" => LogLevel.Off,
                "debug" => LogLevel.Debug,
                _ => LogLevel.All
            };
            options.SetLoggingPreference(LogType.Browser, logLevel);

            // Inisialisasi RemoteWebDriver
            IWebDriver driver = new RemoteWebDriverWithLogs(new Uri(_remoteWebDriverUrl), options);

            if (!_isMobile)
                driver.Manage().Window.Maximize();

            return driver;
        }

        public void Goto(string path)
        {
            if (GetDriver == null) return;

            string url = Path.Combine(_baseUrl, path).Replace("\\", "/");
            GetDriver.Url = url;

            Console.WriteLine($"Navigated to URL: {GetDriver.Url}");
        }

        public void Close()
        {
            GetDriver?.Quit();
            GetDriver = null;
            Console.WriteLine("Browser closed.");
        }
    }
}
