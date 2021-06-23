using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using System;
using System.Data;
using EasyKnock.Calculator.Test.Utilities;

namespace EasyKnock.Calculator.Test.Pages
{
    /// <summary>
    /// Description: Since the calculator is in the home page all the locators and methods should be here.
    /// </summary>
    public class HomePage
    {
        private IWebDriver _driver;
        private const double formulaValue = .665;

        public HomePage(IWebDriver driver)
        {
            _driver = driver;
        }

        #region CalculatorLocators
        public IWebElement HomeValue
        {
            get { return _driver.FindElement(By.Id("homeValue")); }
        }

        public IWebElement MortageBalance
        {
            get { return _driver.FindElement(By.Id("mortgageBalance")); }
        }

        public IWebElement OtherLiens
        {
            get { return _driver.FindElement(By.Id("liens")); }
        }

        public IWebElement CalculateButton
        {
            get { return _driver.FindElement(By.XPath("//button[text() = 'Calculate']")); }
        }

        public IWebElement EstimatedCash
        {
            get { return _driver.FindElement(By.XPath("//*[@id='__next']/div/main/div[6]/section/div/div/div[1]/div/h3/span")); }
        }

        public IWebElement NotQualifiedMessage
        {
            get { return _driver.FindElement(By.XPath("//h3[@class='styles__ResultsTitle-sc-9ejkdd-11 dGgtdN']")); }
        }

        public IWebElement ProjectedRange
        {
            get { return _driver.FindElement(By.XPath("//*[@id='__next']/div/main/div[6]/section/div/div/div[1]/div/p/span")); }
        }

        public IWebElement GetMyOfferButton
        {
            get { return _driver.FindElement(By.CssSelector("//a[text() = 'Get My Offer']")); }
        }
        #endregion

        /// <summary>
        /// Description: This method performs the operation based on the formula to have a comparative value.
        /// </summary>

        public double CalculateWIthLocalSystem(double homeValue, double MortageBalance, double OtherLiens)
        {
            return (homeValue * formulaValue - MortageBalance - OtherLiens);
        }

        /// <summary>
        /// Description: This method performs any arithmetic operation contained in a string, useful to have a comparative value.
        /// </summary>

        public void CalaculateWithEasyNock(double homeValue, double mortageBalance, double otherLiens)
        {
            HomeValue.SendKeys(homeValue.ToString());
            MortageBalance.SendKeys(mortageBalance.ToString());
            OtherLiens.SendKeys(otherLiens.ToString());

            LocatorHelper.ClickWithJavaScript(CalculateButton, _driver);         
        }

    }
}
