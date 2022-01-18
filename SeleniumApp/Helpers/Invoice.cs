using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SeleniumApp.Helpers
{
    public static class Invoice
    {
        public static void Open()
        {
            Tester.ClickAndWaitForElement(By.Id("Menu-Invoices"), By.Id("import-invoice"));
            Thread.Sleep(500);
        }

        public static Dictionary<string, string> GetInvoiceFields()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            IReadOnlyCollection<IWebElement> elements = Tester._driver.FindElements(By.XPath("//table[@id=\"fields\"]/tr"));
            foreach (var element in elements)
            {
                result.Add(element.FindElement(By.XPath("td[@class=\"fieldName\"]")).Text, element.FindElement(By.XPath("td[not(@class=\"fieldName\")]")).Text);
            }
            return result;
        }

        public static List<Dictionary<string,string>> GetInvoiceLineItems()
        {
            List<Dictionary<string,string>> result = new List<Dictionary<string,string>>();
            return Helpers.ViewList.GetViewList();
        }
    }
}
