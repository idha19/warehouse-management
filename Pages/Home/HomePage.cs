using OpenQA.Selenium;
using warehouse.Helpers;
using static warehouse.Helpers.TestDataSource;

namespace warehouse.Pages.Home
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        private readonly ExtentReportsHelper _extentReportsHelper;
        private uint TimeoutInSeconds;
        private int Sleep;

        public HomePage()
        {
            _driver = null;
            _extentReportsHelper = null;

            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        public HomePage(IWebDriver driver, ExtentReportsHelper reportsHelper)
        {
            _driver = driver;
            _extentReportsHelper = reportsHelper;


            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        // LOCATORS
        private By BasketManagementTitle => By.XPath("//h2[contains(text(),'BASKET MANAGEMENT')]");
        private By BasketManagementHeader => By.XPath("//h1[contains(text(),'BASKET MANAGEMENT')]");
        private By AddBasketButton => By.XPath("//button[contains(.,'Add Basket')]");

        // METHODSS =====
        public void ClickBasketManagement()
        {
            _driver.ClickWrapper(BasketManagementTitle, _extentReportsHelper, "Click Basket Management", TimeoutInSeconds);
        }

        public bool VerifyBasketManagementPage()
        {
            return _driver.ControlDisplayed(BasketManagementHeader, _extentReportsHelper, "Basket Management page header visible", true, TimeoutInSeconds);
        }
        
        public void ClickAddBasketButton()
        {
            _driver.ClickWrapper(AddBasketButton, _extentReportsHelper, "Click Add Basket", TimeoutInSeconds);
        }
    }
}