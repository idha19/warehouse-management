using warehouse.Helpers;
using warehouse.PageAssembly;
using TechTalk.SpecFlow;
using static warehouse.Helpers.TestDataSource;

namespace warehouse.StepDefinitions
{
    [Binding, Scope(Tag = "Basket")]
    public class AddBasketStepDefinition
    {
        private BasePages? BasePage => BasePagesHelper.GetBasePage;

        // LOGIN=================================================
        [Given(@"verify that login page is visible successfully")]
        public void ThenVerifyThatLoginPageIsVisibleSuccessfully()
        {
            Assert.IsTrue(BasePage?.LoginPage.IsLoginPageVisible(), "Login page tidak tampil!");
        }

        [When(@"user enters valid username and valid password")]
        public void WhenUserEntersValidUsernameAndValidPassword()
        {
            BasePage?.LoginPage.EnterCredentials(Username, Password);
        }

        [When(@"user clicks the login button")]
        public void WhenUserClicksTheLoginButton()
        {
            BasePage?.LoginPage.ClickLoginButton();
        }

        [Then(@"user should be redirected to the dashboard page successfully")]
        public void ThenUserShouldBeRedirectedToTheDashboardPageSuccessfully()
        {
            bool isNavbarVisible = BasePage?.Navbar.IsNavbarVisible() ?? false;
            Assert.IsTrue(isNavbarVisible, "❌ Navbar tidak tampil — kemungkinan login gagal atau halaman belum termuat.");
        }


        // BASKET MANAGEMENT==================================
        [When(@"user clicks on ""BASKET MANAGEMENT"" section")]
        public void WhenUserClicksOnBasketManagementSection()
        {
            BasePage?.HomePage.ClickBasketManagement();
        }

        [Then(@"user should be redirected to Basket Management page successfully")]
        public void ThenUserShouldBeRedirectedToBasketManagementPageSuccessfully()
        {
            bool isBasketManagementVisible = BasePage?.HomePage.VerifyBasketManagementPage() ?? false;
            Assert.IsTrue(isBasketManagementVisible, "❌ Basket tidak tampil");
        }

        // ADD BASKET ====================================
        [When(@"user clicks on ""Add Basket"" button")]
        public void WhenUserClicksOnAddBasketButton()
        {
            BasePage?.HomePage.ClickAddBasketButton();
        }

        [Then(@"verify that Add Basket page is visible")]
        public void ThenVerifyThatAddBasketPageIsVisible()
        {
            bool isAddBasketPageVisible = BasePage?.AddBasket.VerifyAddBasketPage() ?? false;
            Assert.IsTrue(isAddBasketPageVisible, "❌ Halaman Add Basket tidak tampil setelah klik tombol Add Basket!");
        }

        // CREATE BARCODE =============================================

        [When(@"user selects category ""(.*)"" and staging ""(.*)""")]
        public void WhenUserSelectsCategoryAndStaging(string category, string staging)
        {
            // Pilih dropdown category dan staging
            BasePage?.AddBasket.SelectCategory(category);
            BasePage?.AddBasket.SelectStaging(staging);
        }

        [When(@"user clicks on ""Create Barcode"" button")]
        public void WhenUserClicksOnCreateBarcodeButton()
        {
            BasePage?.AddBasket.ClickCreateBarcode();
        }

        [Then(@"verify that Basket ID and Barcode are displayed")]
        public void ThenVerifyThatBasketIDAndBarcodeAreDisplayed()
        {
            // Pastikan Basket ID & Barcode muncul
            bool isGenerated = BasePage?.AddBasket.VerifyBasketIdAndBarcodeDisplayed() ?? false;
            Assert.IsTrue(isGenerated, "❌ Basket ID atau Barcode tidak muncul setelah klik Create Barcode!");

            // Simpan Basket ID untuk verifikasi di halaman list nanti
            GeneratedBasketId = BasePage?.AddBasket.GetGeneratedBasketId();
            Assert.IsNotNull(GeneratedBasketId, "❌ Basket ID tidak dapat diambil!");
        }

        // SAVE BASKET ==================================
        [When(@"user clicks on ""Save"" button")]
        public void WhenUserClicksOnSaveButton()
        {
            BasePage?.AddBasket.ClickSave();
        }

        [Then(@"verify basket is saved successfully")]
        public void ThenVerifyBasketIsSavedSuccessfully()
        {
            bool isSuccess = BasePage?.AddBasket.VerifySuccessAddBasket() ?? false;
            Assert.IsTrue(isSuccess, "❌ Tidak ada alert sukses setelah menyimpan basket!");
            BasePage?.AddBasket.ClickAgree();
        }

        // VERIFY ON LIST PAGE=============================
        [When(@"user clicks on last page of basket list")]
        public void WhenUserClicksOnLastPageOfBasketList()
        {
            BasePage?.AddBasket.ClickLastPage();
        }
        
        [Then(@"verify that newly created basket appears in the list")]
        public void ThenVerifyThatNewlyCreatedBasketAppearsInTheList()
        {
            Assert.IsNotNull(GeneratedBasketId, "❌ Basket ID belum diset, tidak bisa diverifikasi di list page!");

            bool isBasketFound = BasePage?.AddBasket.VerifyBasketIdInList(GeneratedBasketId!) ?? false;
            Assert.IsTrue(isBasketFound, $"❌ Basket ID '{GeneratedBasketId}' tidak ditemukan di list page terakhir!");
        }
    }
}