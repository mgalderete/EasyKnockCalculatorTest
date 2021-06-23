using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKnock.Calculator.Test.Utilities
{
    public static class LocatorHelper
    {
        //Clicking with javascript since some elements are obscured by others in the page.
        public static void ClickWithJavaScript(IWebElement element, IWebDriver driver)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", element);
        }
    }
}
