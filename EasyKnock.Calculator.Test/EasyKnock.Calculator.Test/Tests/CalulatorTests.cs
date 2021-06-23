using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Firefox;
using EasyKnock.Calculator.Test.Pages;
using System.IO;
using OpenQA.Selenium.Interactions;

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
                    _driver = new FirefoxDriver("C:\\drivers"); ;
                    break;
                case "Chrome":
                    _driver = new ChromeDriver("C:\\drivers");
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
        /// 4) Enter Home Balance
        /// 4) Enter Other Lines
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
        /// 4) Enter Home Balance
        /// 4) Enter Other Lines
        /// 5) Click on Calculate button
        /// </summary>
        [Test]
        [TestCase(200000, 150000, 5000)]
        public void TC_CALCULATOR_003(double homeValue, double homeMortage, double otherLiens)
        {
            _homepage.CalaculateWithEasyNock(homeValue, homeMortage, otherLiens);

            Assert.AreEqual("Sorry! Based on the info you provided, you may not qualify for an EasyKnock program.", _homepage.NotQualifiedMessage.Text);
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
