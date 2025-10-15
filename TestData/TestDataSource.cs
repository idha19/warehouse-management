namespace warehouse.Helpers
{
    /// <summary>
    /// BasePagesHelper
    /// </summary>
    public static class TestDataSource
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        public static string Username = TestContext.Parameters.Get<string>("Username", "");
        public static string Password = TestContext.Parameters.Get<string>("Password", "");
        public static uint TimeoutInSeconds => TestContext.Parameters.Get<uint>("SeleniumTimeout", 30);
        public static int Sleep => TestContext.Parameters.Get<int>("SeleniumSleep", 3) * 1000;
    }
}