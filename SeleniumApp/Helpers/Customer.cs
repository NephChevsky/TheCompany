using OpenQA.Selenium;
using System.Collections.Generic;

namespace SeleniumApp.Helpers
{
    public static class Customer
    {
        public static void Open()
        {
            Tester.ClickAndWaitForElement(By.Id("Customers"), By.Id("create"));
        }

        public static void Create()
        {
            Tester.ClickAndWaitForElement(By.Id("create"), By.Id("FirstName"));
            Tester._driver.FindElement(By.Id("FirstName")).SendKeys("Philippe");
            Tester._driver.FindElement(By.Id("LastName")).SendKeys("Balleydier");
            Tester._driver.FindElement(By.Id("Email")).SendKeys("philippe.balleydier@gmail.com");
            Tester._driver.FindElement(By.Id("PhoneNumber")).SendKeys("+3395172127");
            Tester._driver.FindElement(By.Id("MobilePhoneNumber")).SendKeys("+3395172127");
            Tester._driver.FindElement(By.Id("Address")).SendKeys("77 cours Charlemagne\n69003 Lyon");
            Tester._driver.FindElement(By.Id("CustomerNumber")).SendKeys("CN0001");
            Tester._driver.FindElement(By.Id("NumberPlate")).SendKeys("DP-887-LW");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));
        }

        public static Dictionary<string, string> GetCustomer()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            IReadOnlyCollection<IWebElement> elements = Tester._driver.FindElements(By.XPath("//tr"));
            foreach (var element in elements)
            {
                result.Add(element.FindElement(By.XPath("td[@class=\"fieldName\"]")).Text, element.FindElement(By.XPath("td[not(@class=\"fieldName\")]")).Text);
            }
            return result;
        }
    }
}
