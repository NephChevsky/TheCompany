using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumApp.Helpers;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SeleniumApp
{
    public static class Tester
    {
        public static IWebDriver _driver;
        //public static string _rootUrl = "https://localhost/";
        public static string _rootUrl = "http://localhost:4200";
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
        public static TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(250);

        static Tester()
        {
            _driver = new ChromeDriver();
        }

        public static void ClickAndWaitForElement(By element, By expectedElement)
        {
            IWebElement webElement;
            try
            {
                webElement = _driver.FindElement(element);
            }
            catch
            {
                Thread.Sleep(200);
                webElement = _driver.FindElement(element);
            }
            ClickAndWaitForElement(webElement, expectedElement);
        }

        public static void ClickAndWaitForElement(IWebElement element, By expectedElement)
        {
            element.Click();
            Thread.Sleep(250);
            var wait = new WebDriverWait(_driver, DefaultTimeout);
            wait.PollingInterval = DefaultPollingInterval;
            wait.Until(drv => IsVisible(expectedElement));
        }

        public static bool IsVisible(By element)
        {
            try
            {
                IWebElement webElement = _driver.FindElement(element);
                if (webElement != null && webElement.Displayed && webElement.Enabled)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
