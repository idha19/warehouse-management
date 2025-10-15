// -----------------------------------------------------------------------
// <copyright file="WebDiverExtensions.cs" company="PT United Tractos Tbk.">
// Copyright (c) 2021 Ahmad Ilman Fadilah. All rights reserved.
// </copyright>
// <author>Ahmad Ilman Fadilah, ahmadilmanfadilah@gmail.com</author>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using WaitHelpers = SeleniumExtras.WaitHelpers;

namespace warehouse.Helpers;

/// <summary>
/// WebDiverExtensions
/// </summary>
public static class WebDiverExtensions
{
    /// <summary>
    /// GetScreenshot
    /// </summary>
    /// <param name="driver"></param>
    /// <returns></returns>
    public static Screenshot? GetScreenshot(this IWebDriver? driver)
    {
        var screenshot = driver as ITakesScreenshot;
        return screenshot?.GetScreenshot();
    }
}

/// <summary>
/// WebDriverExtensions
/// </summary>
public static class WebDriverExtensions
{
    /// <summary>
    /// FindElementWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static IWebElement FindElementWrapper(this IWebDriver? driver, By? by, uint timeoutInSeconds = 60)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        var element = wait.Until(drv => drv.FindElement(by));
        return element;
    }

    /// <summary>
    /// FindElementsWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static ReadOnlyCollection<IWebElement?>? FindElementsWrapper(this IWebDriver? driver, By? by, uint timeoutInSeconds = 60)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var elements = wait.Until(drv => drv.FindElements(by));
        return elements;
    }

    /// <summary>
    /// MoveToElement
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool MoveToElement(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                return wait.Until(_ =>
                {
                    var action = new Actions(driver);
                    action.MoveToElement(element).Perform();
                    stale = false;
                    extentReportsHelper?.SetStepStatusPass($"move to [{elementName}] on the page.");
                    return true;
                });
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"can't move to [{elementName}] the page.");
        return false;
    }

    /// <summary>
    /// ClickWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void ClickWrapper(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        bool intercepted = true;
        bool stale = true;
        bool notInteractable = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && (stale || intercepted || notInteractable))
        {
            var element = driver?.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                if (element.ElementIsClickable(driver, extentReportsHelper, elementName, timeoutInSeconds))
                {
                    element?.Click();
                    extentReportsHelper?.SetStepStatusPass($"Clicked on the element [{elementName}].");
                    stale = false;
                    intercepted = false;
                    notInteractable = false;
                }
                else
                {
                    stale = false;
                    intercepted = false;
                    notInteractable = false;
                    throw new Exception($"Element [{elementName}] is not displayed");
                }
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            catch (ElementClickInterceptedException ex)
            {
                intercepted = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            catch (ElementNotInteractableException ex)
            {
                notInteractable = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
    }

    /// <summary>
    /// ControlDisplayed
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="isDisplayed"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ControlDisplayed(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, bool isDisplayed = true,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                return wait.Until(_ =>
                {
                    if ((element != null && ((isDisplayed && element.Displayed) || (!isDisplayed && !element.Displayed))))
                    {
                        extentReportsHelper?.SetStepStatusPass($"[{elementName}] is displayed on the page.");
                        stale = false;
                        return true;
                    }
                    stale = false;
                    extentReportsHelper?.SetStepStatusWarning($"[{elementName}] isn't displayed on the page.");
                    return false;
                });
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] isn't displayed on the page.");
        return false;
    }

    /// <summary>
    /// WaitUntilElementIsNotDisplayed
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="isDisplayed"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static void WaitUntilElementIsNotDisplayed(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, bool isDisplayed = true,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(by));
    }

    /// <summary>
    /// WaitUntilElementHasText
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="txt"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    public static void WaitUntilElementHasText(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string txt, string elementName,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(WaitHelpers.ExpectedConditions.TextToBePresentInElementLocated(by, txt));
    }

    /// <summary>
    /// WaitUntilElementHasNotText
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="txt"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    public static void WaitUntilElementHasNotText(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string txt, string elementName,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(WaitHelpers.ExpectedConditions.InvisibilityOfElementWithText(by, txt));
    }

    /// <summary>
    /// ValidateElementText
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="isMatched"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="txt"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementText(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, string txt, bool isMatched = true,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = wait.Until(drv => drv.FindElement(by));
            try
            {
                return wait.Until(_ =>
                {
                    var test1 = element?.Text.ToLower();
                    var test2 = txt?.ToLower();
                    if (element?.Text.ToLower().Equals(txt?.ToLower()) == isMatched)
                    {
                        extentReportsHelper?.SetStepStatusPass($"[{elementName}] has text [{txt}].");
                        stale = false;
                        return true;
                    }
                    extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't have [{txt}].");
                    return false;
                });
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] isn't displayed on the page.");
        return false;
    }

    /// <summary>
    /// ValidateElementTextContains
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="isMatched"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="txt"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementTextContains(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, string txt, bool isMatched = true,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = wait.Until(drv => drv.FindElement(by));
            try
            {
                return wait.Until(_ =>
                {
                    if (element?.Text.ToLower().Contains(txt.ToLower()) == isMatched)
                    {
                        extentReportsHelper?.SetStepStatusPass($"[{elementName}] has text [{txt}].");
                        stale = false;
                        return true;
                    }
                    extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't have [{txt}].");
                    return false;
                });
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] isn't displayed on the page.");
        return false;
    }

    /// <summary>
    /// GetElementsSize
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    private static int? GetElementsSize(this ReadOnlyCollection<IWebElement?>? elements,
    ExtentReportsHelper? extentReportsHelper, string? elementName,
    uint timeoutInSeconds = 60)
    {
        var elementSize = elements?.Count;
        extentReportsHelper?.SetStepStatusPass($"[{elementName}] has [{elementSize}] element.");
        return elementSize;
    }

    /// <summary>
    /// ValidateElementsSize
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="expectedSize"></param>
    /// <param name="isMatched"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementsSize(this IWebDriver? driver, By? by,
    ExtentReportsHelper? extentReportsHelper, string? elementName, int expectedSize, bool isMatched = true,
    uint timeoutInSeconds = 60)
    {
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var elements = driver.FindElementsWrapper(by, timeoutInSeconds);
            try
            {
                var elementsSize = GetElementsSize(elements, extentReportsHelper, elementName, timeoutInSeconds);
                if (elementsSize.Equals(expectedSize) == isMatched)
                {
                    extentReportsHelper?.SetStepStatusPass($"[{elementName}] has [{expectedSize}] element.");
                    stale = false;
                    return true;
                }
                extentReportsHelper?.SetStepStatusPass($"[{elementName}] hasn't [{expectedSize}] element.");
                stale = false;
                return false;
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't have [{expectedSize}] element.");
        return false;
    }


    /// <summary>
    /// SearchElement
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="searchedText"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static int? SearchElement(this IWebDriver? driver, By? by,
    ExtentReportsHelper? extentReportsHelper, string elementName, string searchedText,
    uint timeoutInSeconds = 60)
    {
        var stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        var index = 0;
        while ((attempt < maxAttempt) && stale)
        {
            var elements = driver.FindElementsWrapper(by, timeoutInSeconds);
            try
            {
                for (int i = 0; i < elements?.Count; i++)
                {
                    if (elements?[i]?.Text == searchedText)
                    {
                        stale = false;
                        index = i;
                        return index;
                    }
                }
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        return index;
    }

    /// <summary>
    /// ValidateElementsSizeGreaterThan
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="expectedSize"></param>
    /// <param name="isMatched"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementsSizeGreaterThan(this IWebDriver? driver, By? by,
    ExtentReportsHelper? extentReportsHelper, string? elementName, int expectedSize, bool isMatched = true,
    uint timeoutInSeconds = 60)
    {
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var elements = driver.FindElementsWrapper(by, timeoutInSeconds);
            try
            {
                var elementsSize = GetElementsSize(elements, extentReportsHelper, elementName, timeoutInSeconds);
                var test = expectedSize;
                if (elementsSize > expectedSize == isMatched)
                {
                    extentReportsHelper?.SetStepStatusPass($"[{elementName}] has [{expectedSize}] element.");
                    stale = false;
                    return true;
                }
                extentReportsHelper?.SetStepStatusPass($"[{elementName}] hasn't [{expectedSize}] element.");
                stale = false;
                return false;
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't have [{expectedSize}] element.");
        return false;
    }

    /// <summary>
    /// ValidateElementAttribute
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="attributeName"></param>
    /// <param name="attributeValue"></param>
    /// <param name="isMatched"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementAttribute(this IWebDriver? driver, By? by, ExtentReportsHelper? extentReportsHelper,
    string elementName, string attributeName, string? attributeValue, bool isMatched = true, uint timeoutInSeconds = 60)
    {
        var stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver?.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                var test = element?.GetAttribute(attributeName);
                var test2 = attributeValue;
                var isValidated = element?.GetAttribute(attributeName).Equals(attributeValue) == isMatched;
                if (isValidated)
                {
                    stale = false;
                    return true;
                }
                else return false;
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        return false;
    }

    /// <summary>
    /// SendKeysWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="value"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void SendKeysWrapper(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper,
        string value, string elementName, uint timeoutInSeconds = 60)
    {
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                if (element.ControlEnabled(driver, extentReportsHelper, elementName, timeoutInSeconds))
                {
                    element?.SendKeys(value);
                    stale = false;
                    extentReportsHelper?.SetStepStatusPass($"SendKeys value [{value}] to  element [{elementName}].");
                }
                else
                {
                    stale = false;
                    throw new Exception($"Element [{elementName}] is not enabled");
                }
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
    }


    /// <summary>
    /// ClearWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void ClearWrapper(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper,
        string elementName, uint timeoutInSeconds = 60)
    {
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                if (element.ControlEnabled(driver, extentReportsHelper, elementName, timeoutInSeconds))
                {
                    element.Clear();
                    if (element.Text.Equals(string.Empty))
                    {
                        extentReportsHelper?.SetStepStatusPass($"Cleared element [{elementName}] content.");
                    }
                    else
                    {
                        extentReportsHelper?.SetStepStatusWarning(
                            $"Element [{elementName}] content is not cleared. Element value is [{element.Text}]");
                    }
                }
                else
                {
                    throw new Exception($"Element [{elementName}] is not enabled");
                }
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
    }

    /// <summary>
    /// HoverElement
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool HoverElement(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        bool stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                return wait.Until(_ =>
                {
                    driver.ScrollTo(by, extentReportsHelper, elementName, timeoutInSeconds);
                    var action = new Actions(driver);
                    action.MoveToElement(element).Build().Perform();
                    stale = false;
                    extentReportsHelper?.SetStepStatusPass($"Hover element [{elementName}] on the page.");
                    return true;
                });
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"can't hover to element [{elementName}] the page.");
        return false;
    }


    /// <summary>
    /// ScrollTo
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    public static void ScrollTo(this IWebDriver? driver, By? by,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        var attempt = 0;
        var stale = true;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        while (attempt < maxAttempt && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                var js = driver as IJavaScriptExecutor;
                js?.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                stale = false;
                extentReportsHelper?.SetStepStatusPass($"Scroll to [{elementName}].");
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
    }

    /// <summary>
    /// SelectByTextWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="value"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void SelectByTextWrapper(this IWebDriver? driver, By? by, ExtentReportsHelper? extentReportsHelper,
    string value, string elementName, uint timeoutInSeconds = 60)
    {
        var stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                SelectElement options = new SelectElement(element);
                if (element.ElementIsClickable(driver, extentReportsHelper, elementName, timeoutInSeconds))
                {
                    options?.SelectByText(value);
                    stale = false;
                    extentReportsHelper?.SetStepStatusPass($"SelectByText [{value}] from  element [{elementName}].");
                }
                else
                {
                    throw new Exception($"Element [{elementName}] is not enabled");
                }
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
    }

    /// <summary>
    /// SelectByIndexWrapper
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="index"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void SelectByIndexWrapper(this IWebDriver? driver, By? by, ExtentReportsHelper? extentReportsHelper,
    int index, string elementName, uint timeoutInSeconds = 60)
    {
        var stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            try
            {
                var element = driver.FindElementWrapper(by, timeoutInSeconds);
                SelectElement options = new SelectElement(element);
                if (element.ElementIsClickable(driver, extentReportsHelper, elementName, timeoutInSeconds))
                {
                    options?.SelectByIndex(index);
                    stale = false;
                    extentReportsHelper?.SetStepStatusPass($"SelectByIndex [{index}] or [{options?.Options[index]}] from  element [{elementName}].");
                }
                else
                {
                    throw new Exception($"Element [{elementName}] is not enabled");
                }
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
    }

    /// <summary>
    /// ControlEnabled
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ControlEnabled(this IWebDriver? driver, By? by,
    ExtentReportsHelper? extentReportsHelper, string elementName,
    uint timeoutInSeconds = 60)
    {

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                return wait.Until(_ =>
                {
                    if (element?.Enabled == true)
                    {
                        extentReportsHelper?.SetStepStatusPass($"[{elementName}] is enabled on the screen.");
                        stale = false;
                        return true;
                    }

                    stale = false;
                    extentReportsHelper?.SetStepStatusWarning($"[{elementName}] is not enabled on the screen.");
                    return false;
                });
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] is not enabled on the screen.");
        return false;
    }
}

/// <summary>
/// WebElementExtension
/// </summary>
public static class WebElementExtension
{
    /// <summary>
    /// HoverElement
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool HoverElement(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(_ =>
        {
            try
            {
                var action = new Actions(driver);
                action.MoveToElement(element).ClickAndHold().Perform();
                extentReportsHelper?.SetStepStatusPass($"Hover element [{elementName}] on the page.");
                return true;
            }
            catch (Exception)
            {
                extentReportsHelper?.SetStepStatusWarning($"can't hover to element [{elementName}] the page.");
                return false;
            }
        });
    }

    /// <summary>
    /// MoveToElement
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool MoveToElement(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(_ =>
        {
            try
            {
                var action = new Actions(driver);
                action.MoveToElement(element).Perform();
                extentReportsHelper?.SetStepStatusPass($"move to [{elementName}] on the page.");
                return true;
            }
            catch (Exception)
            {
                extentReportsHelper?.SetStepStatusWarning($"can't move to [{elementName}] the page.");
                return false;
            }
        });
    }

    /// <summary>
    /// ScrollBy
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void ScrollBy(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint x, uint y)
    {
        //driver.Manage().Window.Size. next feature check size dulu
        if (element.ControlDisplayed(driver, extentReportsHelper, elementName))
        {
            var js = driver as IJavaScriptExecutor;
            js?.ExecuteScript($"window.scrollBy({x},{y})");
            extentReportsHelper?.SetStepStatusPass($"Scroll to x ={x} y={y}.");
        }
        else
        {
            extentReportsHelper?.SetTestStatusFail($"[{elementName}] isn't displayed on the page.");
        }
    }

    /// <summary>
    /// ScrollTo
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    public static void ScrollTo(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName)
    {
        var js = driver as IJavaScriptExecutor;
        js?.ExecuteScript("arguments[0].scrollIntoView(true);", element);

        extentReportsHelper?.SetStepStatusPass($"Scroll to [{elementName}].");
    }

    /// <summary>
    /// ControlDisplayed
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="displayed"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ControlDisplayed(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, bool displayed = true,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(_ =>
        {
            if (element != null && (!displayed && !element.Displayed || displayed && element.Displayed))
            {
                extentReportsHelper?.SetStepStatusPass($"[{elementName}] is displayed on the page.");
                return true;
            }

            extentReportsHelper?.SetStepStatusWarning($"[{elementName}] isn't displayed on the page.");
            return false;
        });
    }

    /// <summary>
    /// CheckText
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="by"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static string? CheckText(this IWebDriver? driver, By? by,
    ExtentReportsHelper? extentReportsHelper, string elementName,
    uint timeoutInSeconds = 60)
    {
        var stale = true;
        int attempt = 0;
        int maxAttempt = TestContext.Parameters.Get<int>("MaxFindElement", 1);
        while ((attempt < maxAttempt) && stale)
        {
            var element = driver?.FindElementWrapper(by, timeoutInSeconds);
            try
            {
                return element?.Text;
            }
            catch (StaleElementReferenceException ex)
            {
                stale = true;
                TestContext.Out.WriteLine(ex.Message);
            }
            attempt++;
        }
        return null;
    }

    /// <summary>
    /// ControlEnabled
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ControlEnabled(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(_ =>
        {
            if (element?.Enabled == true)
            {
                extentReportsHelper?.SetStepStatusPass($"[{elementName}] is enabled on the screen.");
                return true;
            }

            extentReportsHelper?.SetStepStatusWarning($"[{elementName}] is not enabled on the screen.");
            return false;
        });
    }

    /// <summary>
    /// ValidateElementTextEquals
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="txt"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementTextEquals(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, string txt,
        uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(_ =>
        {
            if (element?.Text.ToLower().Equals(txt?.ToLower()) == true)
            {
                extentReportsHelper?.SetStepStatusPass($"[{elementName}] has text [{txt}].");
                return true;
            }

            extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't have [{txt}].");
            return false;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="txt"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementTextContains(this IWebElement? element, IWebDriver? driver,
    ExtentReportsHelper? extentReportsHelper, string elementName, string txt,
    uint timeoutInSeconds = 60)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(_ =>
        {
            if (element?.Text.ToLower().Contains(txt.ToLower()) == true)
            {
                extentReportsHelper?.SetStepStatusPass($"[{elementName}] contains text [{txt}].");
                return true;
            }

            extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't contain [{txt}].");
            return false;
        });
    }

    /// <summary>
    /// ClearWrapper
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void ClearWrapper(this IWebElement element, IWebDriver driver,
        ExtentReportsHelper extentReportsHelper,
        string elementName, uint timeoutInSeconds = 60)
    {
        if (element.ControlEnabled(driver, extentReportsHelper, elementName, timeoutInSeconds))
        {
            element.Clear();
            if (element.Text.Equals(string.Empty))
            {
                extentReportsHelper.SetStepStatusPass($"Cleared element [{elementName}] content.");
            }
            else
            {
                extentReportsHelper.SetStepStatusWarning(
                    $"Element [{elementName}] content is not cleared. Element value is [{element.Text}]");
            }
        }
        else
        {
            throw new Exception($"Element [{elementName}] is not enabled");
        }
    }

    /// <summary>
    /// ElementIsClickable
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ElementIsClickable(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        try
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(_ =>
            {
                if ((WaitHelpers.ExpectedConditions.ElementToBeClickable(element) != null))
                {
                    extentReportsHelper?.SetStepStatusPass($"Element [{elementName}] is clickable.");
                    return true;
                }

                extentReportsHelper?.SetStepStatusWarning($"Element [{elementName}] is not clickable.");
                return false;
            });
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ClickWrapper
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void ClickWrapper(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, uint timeoutInSeconds = 60)
    {
        if (element.ElementIsClickable(driver, extentReportsHelper, elementName, timeoutInSeconds))
        {
            element?.Click();
            extentReportsHelper?.SetStepStatusPass($"Clicked on the element [{elementName}].");
        }
        else
        {
            throw new Exception($"Element [{elementName}] is not displayed");
        }
    }

    /// <summary>
    /// SendKeysWrapper
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="value"></param>
    /// <param name="elementName"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <exception cref="Exception"></exception>
    public static void SendKeysWrapper(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper,
        string value, string elementName, uint timeoutInSeconds = 60)
    {
        if (element.ControlEnabled(driver, extentReportsHelper, elementName, timeoutInSeconds))
        {
            element?.SendKeys(value);
            extentReportsHelper?.SetStepStatusPass($"SendKeys value [{value}] to  element [{elementName}].");
        }
        else
        {
            throw new Exception($"Element [{elementName}] is not enabled");
        }
    }

    /// <summary>
    /// ValidatePageTitle
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="title"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidatePageTitle(this IWebDriver? driver, ExtentReportsHelper? extentReportsHelper,
        string title, uint timeoutInSeconds = 300)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(drv =>
        {
            if (drv.Title.Contains(title))
            {
                extentReportsHelper?.SetStepStatusPass($"Page title [{drv.Title}] contains [{title}].");
                return true;
            }

            extentReportsHelper?.SetStepStatusWarning($"Page title [{drv.Title}] does not contain [{title}].");
            return false;
        });
    }

    /// <summary>
    /// ValidateUrlContains
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="urlPart"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateUrlContains(this IWebDriver driver, ExtentReportsHelper extentReportsHelper,
        string urlPart, uint timeoutInSeconds = 120)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(drv =>
        {
            if (drv.Url.Contains(urlPart))
            {
                extentReportsHelper.SetStepStatusPass($"Page URL [{drv.Url}] contains [{urlPart}].");
                return true;
            }

            extentReportsHelper.SetStepStatusWarning($"Page URL [{drv.Url}] does not contain [{urlPart}].");
            return false;
        });
    }

    /// <summary>
    /// ValidateElementText
    /// </summary>
    /// <param name="element"></param>
    /// <param name="driver"></param>
    /// <param name="extentReportsHelper"></param>
    /// <param name="elementName"></param>
    /// <param name="txt"></param>
    /// <param name="isMatched"></param>
    /// <param name="timeoutInSeconds"></param>
    /// <returns></returns>
    public static bool ValidateElementText(this IWebElement? element, IWebDriver? driver,
        ExtentReportsHelper? extentReportsHelper, string elementName, string txt, bool isMatched = true,
        uint timeoutInSeconds = 60)
    {
        if (element?.Text.ToLower().Equals(txt?.ToLower()) == isMatched)
        {
            extentReportsHelper?.SetStepStatusPass($"[{elementName}] has text [{txt}].");
            return true;
        }
        extentReportsHelper?.SetStepStatusWarning($"[{elementName}] doesn't have [{txt}].");
        return false;
    }

    /// <summary>
        /// GetCssValueWrapper
        /// Mengambil nilai CSS dari elemen tertentu dengan report log.
        /// </summary>
        /// <param name="driver">WebDriver aktif</param>
        /// <param name="locator">Lokator elemen</param>
        /// <param name="extentReportsHelper">Helper untuk logging</param>
        /// <param name="elementDescription">Deskripsi elemen</param>
        /// <param name="cssProperty">Properti CSS yang ingin diambil</param>
        /// <param name="timeoutInSeconds">Timeout pencarian elemen</param>
        /// <returns>Nilai CSS dari elemen</returns>
        public static string GetCssValueWrapper(this IWebDriver? driver, By locator, ExtentReportsHelper? extentReportsHelper, string elementDescription, string cssProperty, uint timeoutInSeconds = 30)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            string value = string.Empty;

            try
            {
                var element = driver.FindElement(locator);
                value = element.GetCssValue(cssProperty);

                extentReportsHelper?.SetStepStatusPass($"Berhasil ambil CSS '{cssProperty}' dari elemen: {elementDescription} => <b>{value}</b>");
            }
            catch (Exception ex)
            {
                extentReportsHelper?.SetTestStatusFail($"Gagal ambil CSS '{cssProperty}' dari elemen: {elementDescription}. Error: {ex.Message}");
            }

            return value;
        }
}