using warehouse.PageAssembly;

namespace warehouse.Helpers
{
    /// <summary>
    /// BasePagesHelper
    /// </summary>
    public static class BasePagesHelper
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        private static BasePages? BasePage;

        /// <summary>
        /// SetBasePage
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="_extend"></param>
        public static void SetBasePage(Browsers? browser, ExtentReportsHelper _extend)
        {
            BasePage = new BasePages(browser, _extend);
        }

        /// <summary>
        /// GetBasePage
        /// </summary>
        public static BasePages? GetBasePage => BasePage;
    }
}