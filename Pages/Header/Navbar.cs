using OpenQA.Selenium;
using warehouse.Helpers;
using static warehouse.Helpers.TestDataSource;

namespace warehouse.Pages.Header
{
    public class Navbar
    {
        private readonly IWebDriver _driver;
        private readonly ExtentReportsHelper _extentReportsHelper;
        private uint TimeoutInSeconds;
        private int Sleep;

        public Navbar()
        {
            _driver = null;
            _extentReportsHelper = null;

            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        public Navbar(IWebDriver driver, ExtentReportsHelper reportsHelper)
        {
            _driver = driver;
            _extentReportsHelper = reportsHelper;

            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        // LOCAORS
        private By ImgLogo => By.CssSelector("img[alt='logo ut']");
        private By TxtUserName => By.XPath("//h1[contains(@class,'font-Inter') and contains(text(),'Aria Gusti Panjalu')]");

        // METHODS

        /// <summary>
        /// Mengecek apakah navbar (logo + nama user) tampil di halaman.
        /// </summary>
        public bool IsNavbarVisible()
        {
            bool logoVisible = _driver.ControlDisplayed(ImgLogo, _extentReportsHelper, "Logo UT visible", true, TimeoutInSeconds);
            bool userVisible = _driver.ControlDisplayed(TxtUserName, _extentReportsHelper, "User name visible", true, TimeoutInSeconds);
            return logoVisible && userVisible;
        }

        /// <summary>
        /// Klik logo UT di header (biasanya menuju dashboard).
        /// </summary>
        public void ClickLogo()
        {
            _driver.ClickWrapper(ImgLogo, _extentReportsHelper, "Click Logo UT", TimeoutInSeconds);
            Thread.Sleep(Sleep);
        }

        /// <summary>
        /// Validasi bahwa nama user di header sesuai yang diharapkan.
        /// </summary>
        public void ValidateUserName(string expectedName)
        {
            _driver.ValidateElementTextContains(TxtUserName, _extentReportsHelper, expectedName, "Validasi nama user di navbar", true, TimeoutInSeconds);
        }
    }
}