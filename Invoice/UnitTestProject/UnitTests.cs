// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        private readonly string executable =
            @"C:\Users\CHRISIS-PC\Desktop\8th Semester\EPL449\epl343.winter18.team10\Invoice\Invoice\bin\Debug\InvoiceX.exe";

        [TestMethod]
        public void CreateInvoiceTest()
        {
            var app = Application.Launch(executable);

            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);

                // Login
                window.FindFirstDescendant(cf => cf.ByAutomationId("txtUsername")).AsTextBox().Enter("admin");
                window.FindFirstDescendant(cf => cf.ByAutomationId("txtPassword")).AsTextBox().Enter("pass");
                window.FindFirstDescendant(cf => cf.ByAutomationId("btn_login")).AsButton().Click();

                Thread.Sleep(5000);

                // Navigate to Create Invoice
                window = app.GetMainWindow(automation);
                window.FindFirstDescendant(cf => cf.ByAutomationId("btnInvoice")).AsButton().Click();
                window.FindFirstDescendant(cf => cf.ByAutomationId("btnCreate")).AsButton().Click();

                // Create Invoice
                window.FindFirstDescendant(cf => cf.ByAutomationId("comboBox_customer")).AsComboBox().Select(0);
                window.FindFirstDescendant(cf => cf.ByAutomationId("issuedBy")).AsTextBox().Enter("AutomatedTest");
                window.FindFirstDescendant(cf => cf.ByAutomationId("comboBox_Product")).AsComboBox().Select(0);
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_ProductQuantity")).AsTextBox().Enter("10");
                window.FindFirstDescendant(cf => cf.ByName("Add")).AsButton().Click();
                window.FindFirstDescendant(cf => cf.ByAutomationId("Btn_Complete")).AsButton().Click();
                window.FindFirstDescendant(cf => cf.ByName("OK")).AsButton().Click();

                // Assert
                Assert.AreEqual(window.FindFirstDescendant(cf => cf.ByAutomationId("txtBox_issuedBy")).AsTextBox().Text,
                    "AutomatedTest");

                window.FindFirstDescendant(cf => cf.ByName("Close")).AsButton().Click();
                var exitWindow = window.FindFirstDescendant(cf => cf.ByName("Exit")).AsWindow();
                exitWindow.FindFirstDescendant(cf => cf.ByName("Yes")).AsButton().Click();
            }
        }

        [TestMethod]
        public void CreateCustomerTest()
        {
            var app = Application.Launch(executable);

            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);

                // Login
                window.FindFirstDescendant(cf => cf.ByAutomationId("txtUsername")).AsTextBox().Enter("admin");
                window.FindFirstDescendant(cf => cf.ByAutomationId("txtPassword")).AsTextBox().Enter("pass");
                window.FindFirstDescendant(cf => cf.ByAutomationId("btn_login")).AsButton().Click();

                Thread.Sleep(5000);

                // Navigate to Create Custoemr
                window = app.GetMainWindow(automation);
                window.FindFirstDescendant(cf => cf.ByAutomationId("btnCustomers")).AsButton().Click();
                window.FindFirstDescendant(cf => cf.ByAutomationId("btnCreate")).AsButton().Click();

                // Create Customer
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerName")).AsTextBox()
                    .Enter("Automated Customer");
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_PhoneNumber")).AsTextBox()
                    .Enter("99000000");
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerEmail")).AsTextBox()
                    .Enter("automation@hotmail.com");
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerBalance")).AsTextBox()
                    .Enter("1000");
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerCountry")).AsTextBox()
                    .Enter("Automania");
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerCity")).AsTextBox()
                    .Enter("Automatron");
                window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerAddress")).AsTextBox()
                    .Enter("Auto Street");

                window.FindFirstDescendant(cf => cf.ByName("Create")).AsButton().Click();
                window.FindFirstDescendant(cf => cf.ByName("OK")).AsButton().Click();

                // Go to view all customers and get the last added customer
                window.FindFirstDescendant(cf => cf.ByAutomationId("btnView")).AsButton().Click();

                var arr = window.FindFirstDescendant(cf => cf.ByAutomationId("customerDataGrid")).AsDataGridView()
                    .FindAllChildren();
                arr[arr.Length - 1].FindFirstDescendant(cf => cf.ByAutomationId("btnOptions")).AsButton().Click();

                var windows = app.GetAllTopLevelWindows(automation);
                foreach (var win in windows)
                    if (win.AsWindow().ClassName == "Popup")
                        win.FindFirstDescendant(cf => cf.ByName("Edit Customer")).AsMenuItem().Click();

                // Assert
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerName")).AsTextBox().Text,
                    "Automated Customer");
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_PhoneNumber")).AsTextBox().Text,
                    "99000000");
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerEmail")).AsTextBox().Text,
                    "automation@hotmail.com");
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerBalance")).AsTextBox().Text,
                    "1000");
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerCountry")).AsTextBox().Text,
                    "Automania");
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerCity")).AsTextBox().Text,
                    "Automatron");
                Assert.AreEqual(
                    window.FindFirstDescendant(cf => cf.ByAutomationId("textBox_CustomerAddress")).AsTextBox().Text,
                    "Auto Street");

                window.FindFirstDescendant(cf => cf.ByName("Close")).AsButton().Click();
                var exitWindow = window.FindFirstDescendant(cf => cf.ByName("Exit")).AsWindow();
                exitWindow.FindFirstDescendant(cf => cf.ByName("Yes")).AsButton().Click();
            }
        }
    }
}