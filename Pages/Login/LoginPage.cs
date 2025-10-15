using OpenQA.Selenium;
using warehouse.Helpers;
using static warehouse.Helpers.TestDataSource;

namespace warehouse.Pages.Login
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly ExtentReportsHelper _extentReportsHelper;
        private uint TimeoutInSeconds;
        private int Sleep;

        public LoginPage()
        {
            _driver = null;
            _extentReportsHelper = null;
            
            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        public LoginPage(IWebDriver driver, ExtentReportsHelper reportsHelper)
        {
            _driver = driver;
            _extentReportsHelper = reportsHelper;

            
            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        // LOCAORS
        private By ImgLogo => By.CssSelector("img[alt='logo ut']");
        private By TitleWelcome => By.XPath("//h1[contains(text(),'Welcome to Smartwarehouse')]");
        private By InputUsername => By.CssSelector("input[name='username']");
        private By InputPassword => By.CssSelector("input[name='password']");
        private By BtnLogin => By.XPath("//button[contains(text(),'Log In')]");
        private By TxtErrorUsername => By.XPath("//p[contains(text(),'Username is required')]");
        private By TxtErrorPassword => By.XPath("//p[contains(text(),'Password is required')]");
        private By TxtLoginFailed => By.XPath("//h3[contains(.,'Login failed')]");

        // METHODS

        public void NavigateToLoginPage(string url)
        {
            _driver.Navigate().GoToUrl(url);
            _extentReportsHelper.LogInfo($"Navigated to Login Page: {url}");
            Thread.Sleep(Sleep);
        }

        public bool IsLoginPageVisible()
        {
            bool logoVisible = _driver.ControlDisplayed(ImgLogo, _extentReportsHelper, "Logo visible", true, TimeoutInSeconds);
            bool titleVisible = _driver.ControlDisplayed(TitleWelcome, _extentReportsHelper, "Title visible", true, TimeoutInSeconds);
            return logoVisible && titleVisible;
        }

        public void EnterCredentials(string username, string password)
        {
            _driver.SendKeysWrapper(InputUsername, _extentReportsHelper, username, "Input Username", TimeoutInSeconds);
            _driver.SendKeysWrapper(InputPassword, _extentReportsHelper, password, "Input Password", TimeoutInSeconds);
        }

        public void ClickLoginButton()
        {
            _driver.ClickWrapper(BtnLogin, _extentReportsHelper, "Click Login Button", TimeoutInSeconds);
            Thread.Sleep(Sleep);
        }

        public bool IsUsernameErrorVisible()
        {
            return _driver.ControlDisplayed(TxtErrorUsername, _extentReportsHelper, "Username is required error visible", true, TimeoutInSeconds);
        }

        public bool IsPasswordErrorVisible()
        {
            return _driver.ControlDisplayed(TxtErrorPassword, _extentReportsHelper, "Password is required error visible", true, TimeoutInSeconds);
        }

        public bool IsLoginFailedVisible()
        {
            return _driver.ControlDisplayed(TxtLoginFailed, _extentReportsHelper, "Login failed message", true, TimeoutInSeconds);
        }

        // public void LoginWithValidCredentials(string username, string password)
        // {
        //     EnterCredentials(username, password);
        //     ClickLoginButton();
        // }
    }
}