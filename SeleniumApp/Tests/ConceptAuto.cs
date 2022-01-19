using DbApp.Models;
using ModelsApp.DbModels;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Xunit;

namespace SeleniumApp
{
    public class ConceptAuto : IDisposable
    {
        [Fact]
        public void Scenario1()
        {
            ServiceController sc = new ServiceController("TheCompany Worker Service");
            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Stop();
            }
            
            Tester._driver.Navigate().GoToUrl(Tester._rootUrl);

            Helpers.User.SignUp();

            Helpers.User.LogOut();

            Helpers.User.LogIn();

            Helpers.Settings.Open();
            Tester.ClickAndWaitForElement(By.Id("Menu-Settings-Invoice"), By.Id("add"));
            Tester.ClickAndWaitForElement(By.Id("add"), By.Id("name"));
            Tester._driver.FindElement(By.Id("name")).SendKeys("NumberPlate");
            Tester.ClickAndWaitForElement(By.Id("create"), By.XPath("//app-home"));

            Tester.ClickAndWaitForElement(By.Id("Menu-Settings-Customer"), By.Id("add"));
            Tester.ClickAndWaitForElement(By.Id("add"), By.Id("name"));
            Tester._driver.FindElement(By.Id("name")).SendKeys("NumberPlate");
            Tester.ClickAndWaitForElement(By.Id("create"), By.XPath("//app-home"));

            Tester.ClickAndWaitForElement(By.Id("Menu-Settings-CompanyInformation"), By.Id("Name"));
            Tester._driver.FindElement(By.Id("Name")).SendKeys("Concept Auto Rhone Services");
            Tester._driver.FindElement(By.Id("Address")).SendKeys("66 Avenue Georges Clemenceau\n69230 St Genis Laval");
            Tester._driver.FindElement(By.Id("PhoneNumber")).SendKeys("0472670909");
            Tester._driver.FindElement(By.Id("Siret")).SendKeys("37987775600020");
            Tester._driver.FindElement(By.Id("Logo")).SendKeys("D:\\Dev\\TheCompany\\Samples\\company-logo.png");
            Tester.ClickAndWaitForElement(By.Id("save"), By.XPath("//app-home"));

            Tester.ClickAndWaitForElement(By.Id("Menu-Settings-CompanyInformation"), By.Id("Name"));
            Tester._driver.FindElement(By.Id("Siret")).Clear();
            Tester._driver.FindElement(By.Id("Siret")).SendKeys("37987775600021");
            Tester.ClickAndWaitForElement(By.Id("save"), By.XPath("//app-home"));

            Tester.ClickAndWaitForElement(By.Id("Menu-Settings-InvoiceExtraction"), By.XPath("//input[@id='Number-x']"));
            Tester._driver.FindElement(By.Id("Number-x")).SendKeys("373");
            Tester._driver.FindElement(By.Id("Number-y")).SendKeys("944");
            Tester._driver.FindElement(By.Id("Number-height")).SendKeys("42");
            Tester._driver.FindElement(By.Id("Number-width")).SendKeys("166");
            Tester._driver.FindElement(By.Id("RecipientNumber-x")).SendKeys("1081");
            Tester._driver.FindElement(By.Id("RecipientNumber-y")).SendKeys("911");
            Tester._driver.FindElement(By.Id("RecipientNumber-height")).SendKeys("37");
            Tester._driver.FindElement(By.Id("RecipientNumber-width")).SendKeys("1");
            Tester._driver.FindElement(By.Id("RecipientLastName-x")).SendKeys("1371");
            Tester._driver.FindElement(By.Id("RecipientLastName-y")).SendKeys("463");
            Tester._driver.FindElement(By.Id("RecipientLastName-height")).SendKeys("37");
            Tester._driver.FindElement(By.Id("RecipientLastName-width")).SendKeys("629");
            Tester._driver.FindElement(By.Id("RecipientAddress-x")).SendKeys("1371");
            Tester._driver.FindElement(By.Id("RecipientAddress-y")).SendKeys("463");
            Tester._driver.FindElement(By.Id("RecipientAddress-height")).SendKeys("260");
            Tester._driver.FindElement(By.Id("RecipientAddress-width")).SendKeys("629");
            Tester._driver.FindElement(By.Id("NumberPlate-x")).SendKeys("1015");
            Tester._driver.FindElement(By.Id("NumberPlate-y")).SendKeys("1139");
            Tester._driver.FindElement(By.Id("NumberPlate-height")).SendKeys("42");
            Tester._driver.FindElement(By.Id("NumberPlate-width")).SendKeys("1");
            Tester._driver.FindElement(By.Id("Box-y")).SendKeys("1348");
            Tester._driver.FindElement(By.Id("Box-height")).SendKeys("1352");
            Tester._driver.FindElement(By.Id("Reference-x")).SendKeys("145");
            Tester._driver.FindElement(By.Id("Reference-width")).SendKeys("335");
            Tester._driver.FindElement(By.Id("Description-x")).SendKeys("487");
            Tester._driver.FindElement(By.Id("Description-width")).SendKeys("1013");
            Tester._driver.FindElement(By.Id("Quantity-x")).SendKeys("1507");
            Tester._driver.FindElement(By.Id("Quantity-width")).SendKeys("203");
            Tester._driver.FindElement(By.Id("PriceVAT-x")).SendKeys("1724");
            Tester._driver.FindElement(By.Id("PriceVAT-width")).SendKeys("426");
            Tester._driver.FindElement(By.Id("TotalPrice-x")).SendKeys("2182");
            Tester._driver.FindElement(By.Id("TotalPrice-width")).SendKeys("1");
            Tester.ClickAndWaitForElement(By.Id("save"), By.XPath("//app-home"));

            Tester.ClickAndWaitForElement(By.Id("Menu-Settings-InvoiceExtraction"), By.XPath("//input[@id='Number-x']"));
            Tester._driver.FindElement(By.Id("RecipientNumber-width")).Clear();
            Tester._driver.FindElement(By.Id("RecipientNumber-width")).SendKeys("134");
            Tester._driver.FindElement(By.Id("NumberPlate-width")).Clear();
            Tester._driver.FindElement(By.Id("NumberPlate-width")).SendKeys("212");
            Tester._driver.FindElement(By.Id("TotalPrice-width")).Clear();
            Tester._driver.FindElement(By.Id("TotalPrice-width")).SendKeys("118");
            Tester.ClickAndWaitForElement(By.Id("save"), By.XPath("//app-home"));

            Helpers.Invoice.Open();
            Tester.ClickAndWaitForElement(By.Id("import-invoice"), By.Id("file"));
            Tester._driver.FindElement(By.Id("file")).SendKeys("D:\\Dev\\TheCompany\\Samples\\invoices.zip");
            Tester.ClickAndWaitForElement(By.Id("import"), By.Id("import-invoice"));

            Helpers.Invoice.Open();
            Tester.ClickAndWaitForElement(By.Id("import-invoice"), By.Id("file"));
            Tester._driver.FindElement(By.Id("file")).SendKeys("D:\\Dev\\TheCompany\\Samples\\fact mecanique 2.PDF");
            Tester.ClickAndWaitForElement(By.Id("import"), By.Id("import-invoice"));

            Tester.ClickAndWaitForElement(By.Id("Menu-LineItems"), By.Id("create"));
            Tester.ClickAndWaitForElement(By.Id("create"), By.Id("Reference"));
            Tester._driver.FindElement(By.Id("Reference")).SendKeys("R0001");
            Tester._driver.FindElement(By.Id("Description")).SendKeys("Stylo");
            Tester.ClickAndWaitForElement(By.Id("Unit"), By.XPath("//span[@class=\"mat-option-text\"][text()=\"U\"]"));
            Tester._driver.FindElement(By.XPath("//span[@class=\"mat-option-text\"][text()=\"U\"]")).Click();
            Tester._driver.FindElement(By.Id("VAT")).SendKeys("20");
            Tester._driver.FindElement(By.Id("PriceNoVAT")).SendKeys("5");
            Tester._driver.FindElement(By.Id("PriceVAT")).SendKeys("1");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));

            Tester.ClickAndWaitForElement(By.Id("edit"), By.Id("Reference"));
            Tester._driver.FindElement(By.Id("PriceVAT")).Clear();
            Tester._driver.FindElement(By.Id("PriceVAT")).SendKeys("6");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));

            Tester.ClickAndWaitForElement(By.Id("Menu-LineItems"), By.Id("create"));
            Tester.ClickAndWaitForElement(By.Id("create"), By.Id("Reference"));
            Tester._driver.FindElement(By.Id("Reference")).SendKeys("R0002");
            Tester._driver.FindElement(By.Id("Description")).SendKeys("Cahier");
            Tester.ClickAndWaitForElement(By.Id("Unit"), By.XPath("//span[@class=\"mat-option-text\"][text()=\"U\"]"));
            Tester._driver.FindElement(By.XPath("//span[@class=\"mat-option-text\"][text()=\"U\"]")).Click();
            Tester._driver.FindElement(By.Id("VAT")).SendKeys("10");
            Tester._driver.FindElement(By.Id("PriceNoVAT")).SendKeys("10");
            Tester._driver.FindElement(By.Id("PriceVAT")).SendKeys("11");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));

            Helpers.Customer.Open();
            Helpers.Customer.Create();

            Tester.ClickAndWaitForElement(By.Id("edit"), By.Id("FirstName"));
            Tester._driver.FindElement(By.Id("Address")).Clear();
            Tester._driver.FindElement(By.Id("Address")).SendKeys("77 cours Charlemagne\n69002 Lyon");
            Tester._driver.FindElement(By.Id("NumberPlate")).Clear();
            Tester._driver.FindElement(By.Id("NumberPlate")).SendKeys("DP-886-LW");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));

            Helpers.Invoice.Open();
            Tester.ClickAndWaitForElement(By.Id("create-invoice"), By.Id("Number"));
            Tester._driver.FindElement(By.Id("Number")).SendKeys("IN0002");
            Tester._driver.FindElement(By.Id("RecipientNumber")).SendKeys("CN0001");
            Tester._driver.FindElement(By.XPath("//span[@class=\"mat-option-text\"][text()=\" CN0001 \"]")).Click();
            Tester._driver.FindElement(By.Id("NumberPlate")).SendKeys("DP-887-LW");
            Tester.ClickAndWaitForElement(By.Id("add-line"), By.Id("0-Reference"));
            Tester.ClickAndWaitForElement(By.Id("add-line"), By.Id("1-Reference"));
            Tester._driver.FindElement(By.Id("0-Reference")).SendKeys("R0001");
            Tester._driver.FindElement(By.XPath("//span[@class=\"mat-option-text\"][text()=\" R0001 \"]")).Click();
            Tester._driver.FindElement(By.Id("0-Quantity")).SendKeys("5");
            Tester._driver.FindElement(By.Id("0-TotalPrice")).SendKeys("30");
            Tester._driver.FindElement(By.Id("1-Reference")).SendKeys("R0002");
            Tester._driver.FindElement(By.XPath("//span[@class=\"mat-option-text\"][text()=\" R0002 \"]")).Click();
            Tester._driver.FindElement(By.Id("1-Quantity")).SendKeys("1");
            Tester._driver.FindElement(By.Id("1-TotalPrice")).SendKeys("5");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));

            Tester.ClickAndWaitForElement(By.Id("edit"), By.Id("Number"));
            Tester._driver.FindElement(By.Id("Number")).Clear();
            Tester._driver.FindElement(By.Id("Number")).SendKeys("IN0001");
            Tester._driver.FindElement(By.Id("NumberPlate")).Clear();
            Tester._driver.FindElement(By.Id("NumberPlate")).SendKeys("DP-886-LW");
            Tester._driver.FindElement(By.Id("1-TotalPrice")).Clear();
            Tester._driver.FindElement(By.Id("1-TotalPrice")).SendKeys("11");
            Tester.ClickAndWaitForElement(By.Id("save"), By.Id("edit"));

            sc.Start();

            Guid owner;
            using (var db = new TheCompanyDbContext(Guid.Empty))
            {
                ModelsApp.DbModels.User user = db.Users.Where(x => x.Login == Helpers.User.email).SingleOrDefault();
                owner = user.Id;
            }

            using (var db = new TheCompanyDbContext(owner))
            {
                var wait = new WebDriverWait(Tester._driver, TimeSpan.FromMinutes(5));
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.Until(drv =>
                {
                    int count = db.Invoices.Where(x => x.IsExtracted == true).Count();
                    if (count == 6)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                wait.Until(drv =>
                {
                    int count = db.Invoices.Where(x => x.IsGenerated == true).Count();
                    if (count == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }

            Helpers.Customer.Open();
            Comparator.Equal("Scenario1", "CustomerViewList", Helpers.ViewList.GetViewList());
            int nbCustomer = Tester._driver.FindElements(By.XPath("//tbody/tr")).Count;
            for (int i = 0; i < nbCustomer; i++)
            {
                Helpers.Customer.Open();
                IWebElement customer = Tester._driver.FindElement(By.XPath("//tbody/tr["+(i+1)+"]"));
                Tester.ClickAndWaitForElement(customer, By.XPath("//td[@class=\"fieldName\"]"));
                Comparator.Equal("Scenario1", "Customer" + i, Helpers.Customer.GetCustomer());
            }

            Helpers.Invoice.Open();
            Comparator.Equal("Scenario1", "InvoiceViewList", Helpers.ViewList.GetViewList());
            int nbInvoice = Tester._driver.FindElements(By.XPath("//tbody/tr")).Count;
            for (int i = 0; i < nbInvoice; i++)
            {
                Helpers.Invoice.Open();
                IWebElement invoice = Tester._driver.FindElement(By.XPath("//tbody/tr["+(i+1)+"]"));
                Tester.ClickAndWaitForElement(invoice, By.XPath("//td[@class=\"fieldName\"]"));
                Comparator.Equal("Scenario1", "InvoiceFields" + i, Helpers.Invoice.GetInvoiceFields());
                Comparator.Equal("Scenario1", "InvoiceLineItems" + i, Helpers.Invoice.GetInvoiceLineItems());
            }

            Comparator.Run();
        }

        public void Dispose()
        {
            Tester.Dispose();
        }
    }
}
