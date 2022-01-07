using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SeleniumApp.Helpers
{
    public static class Settings
    {
        public static void Open()
        {
            Tester.ClickAndWaitForElement(By.Id("Settings"), By.XPath("//div[contains(concat(\" \", normalize-space(@class), \" \"), \" mat-expansion-panel-content \")]"));
            Thread.Sleep(200);
        }
    }
}
