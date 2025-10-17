using System.Security.Cryptography.X509Certificates;
using OpenQA.Selenium;
using warehouse.Helpers;
using static warehouse.Helpers.TestDataSource;

namespace warehouse.Pages.BasketManagement
{
    public class AddBasket
    {
        private readonly IWebDriver _driver;
        private readonly ExtentReportsHelper _extentReportsHelper;
        private uint TimeoutInSeconds;
        private int Sleep;

        public AddBasket()
        {
            _driver = null;
            _extentReportsHelper = null;

            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        public AddBasket(IWebDriver driver, ExtentReportsHelper reportsHelper)
        {
            _driver = driver;
            _extentReportsHelper = reportsHelper;


            // ambil nilai default dari TestDataSource
            TimeoutInSeconds = TestDataSource.TimeoutInSeconds;
            Sleep = TestDataSource.Sleep;
        }

        // LOCATORS
        private By AddBasketTitle => By.XPath("//h2[contains(text(), 'ADD BASKET')]");
        // Dropdown tombol
        private By CategoryDropdown => By.XPath("//button[@role='combobox' and .//span[text()='Select Category']]");
        private By CategoryOption(string category) => By.XPath($"//div[@role='listbox']//div[normalize-space()='{category}']");

        private By StagingDropdown => By.XPath("//button[@role='combobox' and .//span[contains(text(),'QI to BIN') or text()='Select Staging']]");
        private By StagingOption(string staging) => By.XPath($"//div[@role='listbox']//div[normalize-space()='{staging}']");

        private By CreateBarcodeButton => By.XPath("//button[contains(.,'Create Barcode')]");
        private By BasketIdInput => By.XPath("//input[@name='basketCode']");
        private By SaveButton => By.XPath("//button[contains(@class,'bg-[#4E95D9]')]");
        private By SuccessAlert => By.XPath("//h3[contains(text(),'Success Add Basket')]");
        private By AgreeButton => By.XPath("//button[contains(.,'Agree')]");


        //METHODS
        /// <summary>
        /// Verifikasi halaman Add Basket terbuka
        /// </summary>
        public bool VerifyAddBasketPage()
        {
            return _driver.ControlDisplayed(AddBasketTitle, _extentReportsHelper, "Add Basket Page is visible", true, TimeoutInSeconds);
        }

        /// <summary>
        /// Pilih kategori pada dropdown category
        /// </summary>
        public void SelectCategory(string category)
        {
            _driver.ClickWrapper(CategoryDropdown, _extentReportsHelper, "Click Category Dropdown", TimeoutInSeconds);
            _driver.ClickWrapper(CategoryOption(category), _extentReportsHelper, $"Select Category: {category}", TimeoutInSeconds);
            Thread.Sleep(Sleep);
        }

        /// <summary>
        /// Pilih staging pada dropdown staging
        /// </summary>
        public void SelectStaging(string staging)
        {
            _driver.ClickWrapper(StagingDropdown, _extentReportsHelper, "Click Staging Dropdown", TimeoutInSeconds);
            _driver.ClickWrapper(StagingOption(staging), _extentReportsHelper, $"Select Staging: {staging}", TimeoutInSeconds);
            Thread.Sleep(Sleep);
        }

        /// <summary>
        /// Klik tombol Create Barcode
        /// </summary>
        public void ClickCreateBarcode()
        {
            _driver.ClickWrapper(CreateBarcodeButton, _extentReportsHelper, "Click Create Barcode button", TimeoutInSeconds);
            Thread.Sleep(Sleep);
        }

        /// <summary>
        /// Verifikasi bahwa BasketCode sudah terisi dan barcode muncul.
        /// (tanpa WebDriverWait — pakai polling loop manual)
        /// </summary>
        public bool VerifyBasketIdDisplayed()
        {
            try
            {
                _extentReportsHelper.LogInfo("🔍 Memeriksa apakah Basket ID sudah terisi setelah klik Create Barcode...");

                bool isValueFilled = false;
                string basketValue = "";

                // Polling manual sampai 10x (tiap 500ms) untuk menunggu value muncul
                for (int attempt = 1; attempt <= 10; attempt++)
                {
                    var input = _driver.FindElements(BasketIdInput).FirstOrDefault();
                    if (input != null)
                    {
                        basketValue = input.GetAttribute("value") ?? "";
                        _extentReportsHelper.LogInfo($"🧾 Percobaan {attempt}: BasketID value = '{basketValue}'");

                        if (!string.IsNullOrEmpty(basketValue))
                        {
                            isValueFilled = true;
                            break;
                        }
                    }

                    Thread.Sleep(500);
                }

                if (isValueFilled)
                {
                    _extentReportsHelper.SetStepStatusPass($"✅ Basket ID berhasil muncul: {basketValue}");
                    return true;
                }
                else
                {
                    _extentReportsHelper.SetTestStatusFail("❌ Basket ID belum muncul setelah klik Create Barcode!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _extentReportsHelper.SetTestStatusFail($"⚠️ Terjadi error saat verifikasi Basket ID: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ambil nilai Basket ID yang dihasilkan setelah klik Create Barcode.
        /// </summary>
        public string GetGeneratedBasketId()
        {
            // string basketId = _driver.FindElement(BasketIdInput).GetAttribute("value");
            // _extentReportsHelper.LogInfo($"📦 Generated Basket ID: {basketId}");
            // return basketId;

            var input = _driver.FindElements(BasketIdInput).FirstOrDefault();
            if (input == null)
            {
                _extentReportsHelper.SetTestStatusFail("❌ Tidak dapat menemukan elemen Basket ID input.");
                return string.Empty;
            }

            string basketId = input.GetAttribute("value") ?? "";
            _extentReportsHelper.LogInfo($"📦 Generated Basket ID: {basketId}");
            return basketId;
        }

        /// <summary>
        /// Klik tombol Save
        /// </summary>
        public void ClickSave()
        {
            _driver.ClickWrapper(SaveButton, _extentReportsHelper, "Click Save button", TimeoutInSeconds);
            Thread.Sleep(Sleep);
        }

        /// <summary>
        /// Verifikasi success alert muncul
        /// </summary>
        public bool VerifySuccessAddBasket()
        {
            return _driver.ControlDisplayed(SuccessAlert, _extentReportsHelper, "Success Add Basket alert visible", true, TimeoutInSeconds);
        }

        /// <summary>
        /// Klik tombol Agree pada alert
        /// </summary>
        public void ClickAgree()
        {
            _driver.ClickWrapper(AgreeButton, _extentReportsHelper, "Click Agree button", TimeoutInSeconds);
        }

        // /// <summary>
        // /// Klik tombol halaman terakhir pada tabel list basket.
        // /// </summary>
        // public void ClickLastPage()
        // {
        //     // Pastikan tombol halaman terakhir muncul
        //     bool lastPageVisible = _driver.ControlDisplayed(
        //         LastPageButton,
        //         _extentReportsHelper,
        //         "Last page button is visible before clicking",
        //         true,
        //         TimeoutInSeconds
        //     );

        //     if (lastPageVisible)
        //     {
        //         _driver.ClickWrapper(LastPageButton, _extentReportsHelper, "Click last page button", TimeoutInSeconds);
        //         Thread.Sleep(Sleep);
        //     }
        //     else
        //     {
        //         _extentReportsHelper.SetTestStatusFail("❌ Last page button not found, cannot navigate to last page.");
        //     }
        // }

        // /// <summary>
        // /// Verifikasi apakah Basket ID yang baru dibuat muncul di tabel list.
        // /// </summary>
        // public bool VerifyBasketIdInList(string basketId)
        // {
        //     _extentReportsHelper.LogInfo($"🔎 Verifying basket ID '{basketId}' appears in the list table...");

        //     bool isDisplayed = _driver.ControlDisplayed(
        //         BasketIdCell(basketId),
        //         _extentReportsHelper,
        //         $"Basket ID '{basketId}' found in table",
        //         true,
        //         TimeoutInSeconds
        //     );

        //     if (isDisplayed)
        //         _extentReportsHelper.SetStepStatusPass($"✅ Basket ID '{basketId}' successfully verified in list table");
        //     else
        //         _extentReportsHelper.SetTestStatusFail($"❌ Basket ID '{basketId}' not found in list table");

        //     return isDisplayed;
        // }

    }
}