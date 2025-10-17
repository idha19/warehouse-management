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
        [Given(@"user is logged in and on Basket Management page")]
        public void GivenUserIsLoggedInAndOnBasketManagementPage()
        {
            // pastikan login page terlihat
            Assert.IsTrue(BasePage?.LoginPage.IsLoginPageVisible(), "Login page tidak tampil!");

            // Masukkan kredensial valid untuk login
            BasePage?.LoginPage.EnterCredentials(Username, Password);

            // klik tombol login
            BasePage?.LoginPage.ClickLoginButton();

            // verifikasi dashboard tampil (dari navbar)
            bool isNavbarVisible = BasePage?.Navbar.IsNavbarVisible() ?? false;
            Assert.IsTrue(isNavbarVisible, "❌ Navbar tidak tampil — kemungkinan login gagal atau halaman belum termuat.");

            // Navigasi ke memu Basket Management
            BasePage?.HomePage.ClickBasketManagement();

            // Verifikasi halaman Basket Management tampil
            bool isBasketManagementVisible = BasePage?.HomePage.VerifyBasketManagementPage() ?? false;
            Assert.IsTrue(isBasketManagementVisible, "❌ Basket tidak tampil");
        }

        // ADD BASKET ====================================
        [When(@"user clicks on Add Basket button")]
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

        [When(@"user selects category (.*) and staging (.*)")]
        public void WhenUserSelectsCategoryAndStaging(string category, string staging)
        {
            if (BasePage?.AddBasket == null)
                throw new NullReferenceException("AddBasket page belum diinisialisasi di BasePage!");
        
            // Pilih dropdown category dan staging
            BasePage?.AddBasket.SelectCategory(category);
            BasePage?.AddBasket.SelectStaging(staging);
        }

        [When(@"user clicks on Create Barcode button")]
        public void WhenUserClicksOnCreateBarcodeButton()
        {
            BasePage?.AddBasket.ClickCreateBarcode();
        }

        [Then(@"verify that Basket ID displayed")]
        public void ThenVerifyThatBasketIDAndBarcodeAreDisplayed()
        {
            // Pastikan Basket ID & Barcode muncul
            bool isGenerated = BasePage?.AddBasket.VerifyBasketIdDisplayed() ?? false;
            Assert.IsTrue(isGenerated, "❌ Basket ID atau Barcode tidak muncul setelah klik Create Barcode!");

            // Simpan Basket ID untuk verifikasi di halaman list nanti
            GeneratedBasketId = BasePage?.AddBasket.GetGeneratedBasketId();
            Assert.IsFalse(string.IsNullOrEmpty(GeneratedBasketId), "❌ Basket ID tidak dapat diambil!");

            Console.WriteLine($"✅ Basket ID berhasil diambil: {GeneratedBasketId}");
        }

        // SAVE BASKET ==================================
        [When(@"user clicks on Save button")]
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
    }
}