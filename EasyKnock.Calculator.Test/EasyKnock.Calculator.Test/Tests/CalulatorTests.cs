using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Firefox;
using EasyKnock.Calculator.Test.Pages;
using System.IO;
using OpenQA.Selenium.Interactions;
using EasyKnock.Calculator.Test.Utilities;

namespace EasyKnock.Calculator.Test.Tests
{
    public class CalulatorTests
    {
        public IWebDriver _driver;
        private IConfiguration _configuration;
        private Actions _actions { get; set; }
        private HomePage _homepage { get; set; }

        /// <summary>
        /// Loads settings file
        /// </summary>
        public void InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _configuration = config;
        }

        /// <summary>
        /// Set the correct driver depending of the "Browser" app setting.
        /// </summary>
        private void SetDriver()
        {
            switch (_configuration["Browser"])
            {
                case "Firefox":
                    _driver = new FirefoxDriver(_configuration["DriversPath"]); ;
                    break;
                case "Chrome":
                    _driver = new ChromeDriver(_configuration["DriversPath"]);
                    break;
                default:
                    throw new Exception("Invalid browser");
            }

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            _driver.Navigate().GoToUrl(_configuration["URL"]);
            _driver.Manage().Window.Maximize();
        }

        // <summary>
        /// Initializes browser driver and required settings before each test execution.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            InitConfiguration();
            SetDriver();

            _homepage = new HomePage(_driver);
            _actions = new Actions(_driver);
            _actions.MoveToElement(_homepage.CalculateButton);
            _actions.Perform();
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_001
        /// Test Scenario: Verify that calculator is displayed with all the required elements.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// Expected Result: Calculator should be displayed with all the required elements
        /// (Home Value tex field, Mortage Balance text field, Other Liens text field, Calculate button).
        /// </summary>
        [Test]
        public void TC_CALCULATOR_001()
        {
            Assert.IsTrue(_homepage.HomeValue.Displayed);
            Assert.IsTrue(_homepage.MortageBalance.Displayed);
            Assert.IsTrue(_homepage.OtherLiens.Displayed);
            Assert.IsTrue(_homepage.CalculateButton.Displayed);
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_002
        /// Test Scenario: Verify that calculator is displaying correct results with using Mortage Balance and Other Liens and that depending the formula should be 0 or more.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// 3) Enter Home Value
        /// 4) Enter Mortage Balance
        /// 4) Enter Other Liens
        /// 5) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(200000, 10000, 5000)]
        public void TC_CALCULATOR_002(double homeValue, double homeMortage, double otherLiens)
        {
            double localCalculation = _homepage.CalculateWIthLocalSystem(homeValue, homeMortage, otherLiens);

            _homepage.CalaculateWithEasyNock(homeValue, homeMortage, otherLiens);
            double easyKnockCalculation = double.Parse(_homepage.EstimatedCash.Text.Replace("$", "").Replace(",", ""));

            Assert.AreEqual(localCalculation, easyKnockCalculation);
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_003
        /// Test Scenario: Verify that calculator is displaying following message "Sorry! Based on the info you provided, you may not qualify for an EasyKnock program."
        /// with using Mortage Balance and Other Liens and that depending the formula should be less than 0.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// 3) Enter Home Value
        /// 4) Enter Mortage Balance
        /// 4) Enter Other Liens
        /// 5) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(2000000, 150000, 5000)]
        public void TC_CALCULATOR_003(double homeValue, double homeMortage, double otherLiens)
        {
            _homepage.CalaculateWithEasyNock(homeValue, homeMortage, otherLiens);

            Assert.AreEqual("Sorry! Based on the info you provided, you may not qualify for an EasyKnock program.", _homepage.NotQualifiedMessage.Text);
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_004
        /// Test Scenario: Verify that calculator is displaying correct results with using only Mortage Balance and that depending the formula should be 0 or more.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// 3) Enter Home Value
        /// 4) Enter Mortage Balance
        /// 5) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(550000, 15000)]
        public void TC_CALCULATOR_004(double homeValue, double homeMortage)
        {
            double localCalculation = _homepage.CalculateWIthLocalSystem(homeValue, homeMortage, 0);

            _homepage.CalaculateWithEasyNock(homeValue, homeMortage, 0);
            double easyKnockCalculation = double.Parse(_homepage.EstimatedCash.Text.Replace("$", "").Replace(",", ""));

            Assert.AreEqual(localCalculation, easyKnockCalculation);
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_005
        /// Test Scenario: Verify that calculator is displaying correct results with using only Mortage Balance and that depending the formula should be 0 or more.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// 3) Enter Home Value
        /// 4) Enter Other Liens
        /// 5) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(5000000, 15000)]
        public void TC_CALCULATOR_005(double homeValue, double otherLiens)
        {
            double localCalculation = _homepage.CalculateWIthLocalSystem(homeValue, 0, otherLiens);

            _homepage.CalaculateWithEasyNock(homeValue, 0, otherLiens);
            double easyKnockCalculation = double.Parse(_homepage.EstimatedCash.Text.Replace("$", "").Replace(",", ""));

            Assert.AreEqual(localCalculation, easyKnockCalculation);
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_006
        /// Test Scenario: Verify that calculator is displaying correct projected range using Mortage Balance and Other Liens and that depending the formula should be 0 or more.
        /// Note: Don't know what's the calculus or formula to get the range but this could be a test for that.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// 3) Enter Home Value
        /// 4) Enter Mortage Balance
        /// 5) Enter Other Liens
        /// 6) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(3500000, 12000, 3000)]
        public void TC_CALCULATOR_006(double homeValue, double homeMortage, double otherLiens)
        {
            _homepage.CalaculateWithEasyNock(homeValue, homeMortage, otherLiens);
            double easyKnockCalculation = double.Parse(_homepage.EstimatedCash.Text.Replace("$", "").Replace(",", ""));

            Assert.IsTrue(_homepage.ProjectedRange.Text.Contains(_homepage.EstimatedCash.Text));
        }

        /// <summary>
        /// Test Case ID: TC_CALCULATOR_007
        /// Test Scenario: Verify that Get Offer button is redirecting to the correct page.
        /// Note: Don't know what's the calculus or formula to get the range but this could be a test for that.
        /// Steps:
        /// 1) Go to https://www.easyknock.com/programs/sellstay/
        /// 2) Scroll down to Calculator to "How Much Cash Can You Access?" section
        /// 3) Enter Home Value
        /// 4) Enter Mortage Balance
        /// 5) Enter Other Liens
        /// 6) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(4600000, 12000, 3000)]
        public void TC_CALCULATOR_007(double homeValue, double homeMortage, double otherLiens)
        {
            _homepage.CalaculateWithEasyNock(homeValue, homeMortage, otherLiens);


            LocatorHelper.ClickWithJavaScript(_homepage.GetMyOfferButton, _driver);

            Assert.IsTrue(_driver.Url.Contains("getoffer")); //In real life project I would add another page object for get offer page.
        }

        /// <summary>
        /// Close driver instance after test execution.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _driver.Close();
        }
    }
}
