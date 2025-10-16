using System;
using warehouse.Helpers;
using warehouse.Pages.Login;
using Dynamitey.DynamicObjects;
using SeleniumExtras.PageObjects;
using warehouse.Pages.Header;
using warehouse.Pages.Home;
using warehouse.Pages.BasketManagement;

namespace warehouse.PageAssembly
{
    /// <summary>
    /// BasePages
    /// </summary>
    public class BasePages
    {
        /// <summary>
        /// Konstruktor BasePages
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="extentReportsHelper"></param>
        public BasePages(Browsers? browser, ExtentReportsHelper extentReportsHelper)
        {
            Browser = browser;
            ExtentReportsHelper = extentReportsHelper;
        }

        /// <summary>
        /// Browser
        /// </summary>
        protected Browsers? Browser { get; }

        /// <summary>
        /// ExtentReportsHelper
        /// </summary>
        protected ExtentReportsHelper? ExtentReportsHelper { get; }

        /// <summary>
        /// Generic untuk inisialisasi page object
        /// </summary>
        private T GetPages<T>() where T : new()
        {
            var page = (T)Activator.CreateInstance(typeof(T), Browser?.GetDriver, ExtentReportsHelper)!;
            if (Browser?.GetDriver != null) 
                PageFactory.InitElements(Browser.GetDriver, page);
            return page;
        }

        /// <summary>
        /// ProductsPage instance
        /// </summary>
        public LoginPage LoginPage => GetPages<LoginPage>();
        public Navbar Navbar => GetPages<Navbar>();
        public HomePage HomePage => GetPages<HomePage>();
        public AddBasket AddBasket => GetPages<AddBasket>();
    }
}
