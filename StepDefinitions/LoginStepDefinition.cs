using warehouse.Helpers;
using warehouse.PageAssembly;
using TechTalk.SpecFlow;
using static warehouse.Helpers.TestDataSource;

namespace warehouse.StepDefinitions
{
    [Binding, Scope(Tag = "Login")]
    public class LoginStepDefinition
    {
        private BasePages? BasePage => BasePagesHelper.GetBasePage;

        // METHOD DI FEATURE===============
        [Given(@"verify that login page is visible successfully")]
        public void ThenVerifyThatLoginPageIsVisibleSuccessfully()
        {
            Assert.IsTrue(BasePage?.LoginPage.IsLoginPageVisible(), "Login page tidak tampil!");
        }

        // ===INPUT USERNAME AND PASSWORD=============
        [When(@"user enters valid username")]
        public void WhenUserEntersValidUsername()
        {
            BasePage?.LoginPage.EnterCredentials(Username, "");
        }

        [When(@"user enters valid password")]
        public void WhenUserEntersValidPassword()
        {
            BasePage?.LoginPage.EnterCredentials("", Password);
        }

        [When(@"user enters invalid username ""(.*)""")]
        public void WhenUserEntersInvalidUsername(string username)
        {
            BasePage?.LoginPage.EnterCredentials(username, "");
        }

        [When(@"user enters invalid password ""(.*)""")]
        public void WhenUserEntersInvalidPassword(string password)
        {
            BasePage?.LoginPage.EnterCredentials("", password);
        }

        [When(@"user leaves username and password fields empty")]
        public void WhenUserLeavesUsernameAndPasswordFieldsEmpty()
        {
            BasePage?.LoginPage.EnterCredentials("", "");
        }

        //==== BUTTON LOGIN===================
        [When(@"user clicks the login button")]
        public void WhenUserClicksTheLoginButton()
        {
            BasePage?.LoginPage.ClickLoginButton();
        }

        // ===== THEN: LOGIN SUCCESS =====
        [Then(@"user should be redirected to the dashboard page successfully")]
        public void ThenUserShouldBeRedirectedToTheDashboardPageSuccessfully()
        {
            bool isNavbarVisible = BasePage?.Navbar.IsNavbarVisible() ?? false;
            Assert.IsTrue(isNavbarVisible, "❌ Navbar tidak tampil — kemungkinan login gagal atau halaman belum termuat.");
        }

        // ===== THEN: LOGIN ERROR MESSAGE =====
        [Then(@"user should see an error message")]
        public void ThenUserShouldSeeAnErrorMessage()
        {
            bool isLoginFailedVisible = BasePage?.LoginPage.IsLoginFailedVisible() ?? false;
            Assert.IsTrue(isLoginFailedVisible, "❌ Login failed message is not visible!");
        }

        // ===== THEN: VALIDATION MESSAGE =====
        [Then(@"user should see a validation message")]
        public void ThenUserShouldSeeAValidationMessage()
        {
            bool usernameErrorVisible = BasePage?.LoginPage.IsUsernameErrorVisible() ?? false;
            bool passwordErrorVisible = BasePage?.LoginPage.IsPasswordErrorVisible() ?? false;

            Assert.IsTrue(usernameErrorVisible && passwordErrorVisible,
                "❌ Validasi 'Username is required' atau 'Password is required' tidak tampil!");
        }
    }
}