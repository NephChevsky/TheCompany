using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumApp.Helpers
{
    public static class User
    {
        public static string email;

        public static void SignUp()
        {
            Tester.ClickAndWaitForElement(By.Id("signup"), By.Id("email"));
            email = Guid.NewGuid().ToString() + "@testauto.com";
            Tester._driver.FindElement(By.Id("email")).SendKeys(email);
            Tester._driver.FindElement(By.Id("password")).SendKeys("TheCompany!01");
            Tester._driver.FindElement(By.Id("confirmPassword")).SendKeys("TheCompany!01");
            Tester.ClickAndWaitForElement(By.Id("register"), By.XPath("//app-home"));
        }

        public static void LogOut()
        {
            Tester.ClickAndWaitForElement(By.Id("logout"), By.Id("email"));
        }

        public static void LogIn(string login = null)
        {
            Tester.ClickAndWaitForElement(By.Id("signin"), By.Id("email"));
            if (login != null)
            {
                email = login;
            }
            Tester._driver.FindElement(By.Id("email")).SendKeys(email);
            Tester._driver.FindElement(By.Id("password")).SendKeys("TheCompany!01");
            Tester.ClickAndWaitForElement(By.Id("login"), By.XPath("//app-home"));
        }
    }
}
