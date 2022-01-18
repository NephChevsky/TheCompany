using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumApp.Helpers
{
    public static class ViewList
    {
        public static List<Dictionary<string, string>> GetViewList()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<string> headers = new List<string>();
            IReadOnlyCollection<IWebElement> elements = Tester._driver.FindElements(By.XPath("//th"));
            foreach (IWebElement element in elements)
            {
                headers.Add(element.Text);
            }
            elements = Tester._driver.FindElements(By.XPath("//tbody/tr"));
            foreach (IWebElement element in elements)
            {
                Dictionary<string, string> entry = new Dictionary<string, string>();
                IReadOnlyCollection<IWebElement> fields = element.FindElements(By.XPath(".//td/app-field/div/div"));
                int count = 0;
                foreach (IWebElement field in fields)
                {
                    entry.Add(headers[count], field.Text);
                    count++;
                }
                result.Add(entry);
            }
            return result;
        }
    }
}
