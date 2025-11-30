using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Lab
{
    [TestFixture]
    public class Tests_lab1
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1_TextBox()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Text Box']"))).Click();

            driver.FindElement(By.Id("userName")).SendKeys("Иван Иванов");
            driver.FindElement(By.Id("userEmail")).SendKeys("ivan@example.com");
            driver.FindElement(By.Id("currentAddress")).SendKeys("Москва, ул. Пушкина, д.1");
            driver.FindElement(By.Id("permanentAddress")).SendKeys("Санкт-Петербург, Невский пр., д.100");

            driver.FindElement(By.Id("submit")).Click();

            var output = driver.FindElement(By.Id("output")).Text;
            Assert.That(output, Does.Contain("Иван Иванов"));
            Assert.That(output, Does.Contain("ivan@example.com"));
            Assert.That(output, Does.Contain("Москва, ул. Пушкина, д.1"));
            Assert.That(output, Does.Contain("Санкт-Петербург, Невский пр., д.100"));
        }

        [Test]
        public void Test2_CheckBox()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Check Box']"))).Click();

            // Нажимаем +
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("button[title='Expand all']"))).Click();

            // Отмечаем Notes, Veu (View), Private
            driver.FindElement(By.XPath("//span[text()='Notes']/preceding-sibling::span[@class='rct-checkbox']")).Click();
            driver.FindElement(By.XPath("//span[text()='Veu']/preceding-sibling::span[@class='rct-checkbox']")).Click();
            driver.FindElement(By.XPath("//span[text()='Private']/preceding-sibling::span[@class='rct-checkbox']")).Click();

            // Нажимаем -
            driver.FindElement(By.CssSelector("button[title='Collapse all']")).Click();
        }

        [Test]
        public void Test3_RadioButton()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Radio Button']"))).Click();

            // Выбираем Yes
            driver.FindElement(By.XPath("//label[text()='Yes']")).Click();
            Assert.That(driver.FindElement(By.ClassName("text-success")).Text, Is.EqualTo("Yes"));

            // Выбираем Impressive
            driver.FindElement(By.XPath("//label[text()='Impressive']")).Click();
            Assert.That(driver.FindElement(By.ClassName("text-success")).Text, Is.EqualTo("Impressive"));

            // Проверяем, что No недоступна
            var noButton = driver.FindElement(By.Id("noRadio"));
            Assert.That(noButton.Enabled, Is.False, "Кнопка 'No' должна быть недоступна");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }

    public class Tests_lab2
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1()
        {
            try
            {
                // перешли на страницу с кнопками
                driver.Navigate().GoToUrl("https://demoqa.com/");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Buttons']"))).Click();

                // дождались загрузки кнопок
                wait.Until(d => d.FindElement(By.Id("doubleClickBtn")));

                // Нажимаем на кнопку Click Me (ищем по тексту, так как ID динамический)
                var clickMeButton = driver.FindElement(By.XPath("//button[text()='Click Me']"));
                clickMeButton.Click();

                // проверили
                var dynamicClickMessage = driver.FindElement(By.Id("dynamicClickMessage"));
                Assert.That(dynamicClickMessage.Text, Is.EqualTo("You have done a dynamic click"));

                // Двойной клик на Double Click Me (используем статический ID)
                var doubleClickButton = driver.FindElement(By.Id("doubleClickBtn"));
                new Actions(driver).DoubleClick(doubleClickButton).Perform();

                // проверили
                var doubleClickMessage = driver.FindElement(By.Id("doubleClickMessage"));
                Assert.That(doubleClickMessage.Text, Is.EqualTo("You have done a double click"));

                // Правый клик на Right Click Me (используем статический ID)
                var rightClickButton = driver.FindElement(By.Id("rightClickBtn"));
                new Actions(driver).ContextClick(rightClickButton).Perform();

                // проверили
                var rightClickMessage = driver.FindElement(By.Id("rightClickMessage"));
                Assert.That(rightClickMessage.Text, Is.EqualTo("You have done a right click"));

                // вывели
                Console.WriteLine("Все тесты прошли успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }

        [Test]
        public void Test2()
        {
            try
            {
                driver.Navigate().GoToUrl("https://demoqa.com/");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Links']"))).Click();

                // Нажимаем Home
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("simpleLink"))).Click();

                string originalWindow = driver.CurrentWindowHandle;

                // Ждём появления второй вкладки
                wait.Until(d => d.WindowHandles.Count == 2);

                string newWindow = driver.WindowHandles.First(h => h != originalWindow);
                driver.SwitchTo().Window(newWindow);

                // Проверка URL
                Assert.That(driver.Url, Is.EqualTo("https://demoqa.com/"));

                // Закрываем вкладку и возвращаемся обратно
                driver.Close();
                driver.SwitchTo().Window(originalWindow);

                // Кликаем по "Moved"
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("moved"))).Click();

                var response = wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("linkResponse"))
                ).First();

                Assert.That(response.Text, Does.Contain("Link has responded with staus 301"));
                Assert.That(response.Text, Does.Contain("Moved Permanently"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }

        [Test]
        public void Test3()
        {
            try
            {
                driver.Navigate().GoToUrl("https://demoqa.com/");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Upload and Download']"))).Click();

                // создаём тестовый файл
                string filePath = Path.GetFullPath("testfile.txt");
                File.WriteAllText(filePath, "test upload");

                var upload = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("uploadFile")));
                upload.SendKeys(filePath);

                var result = wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("uploadedFilePath"))
                );

                string fileName = Path.GetFileName(filePath);

                // Проверяем, что в поле есть имя файла
                Assert.That(result.Text, Does.Contain(fileName), $"Ожидалось имя файла '{fileName}' в тексте: {result.Text}");

                // получить имя файла напрямую из input.files[0].name через JS
                var js = (IJavaScriptExecutor)driver;
                var uploadedName = (string)js.ExecuteScript(
                    "return document.getElementById('uploadFile').files[0].name;");
                Assert.That(uploadedName, Is.EqualTo(fileName), "Имя файла в input.files[0].name не совпало с ожидаемым.");

              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }


    public class Tests_lab3
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Elements']"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Dynamic Properties']"))).Click();

            // Кнопка Color Change
            var colorBtn = driver.FindElement(By.Id("colorChange"));
            string oldColor = colorBtn.GetCssValue("color");

            // Ждём изменения цвета
            wait.Until(d =>
            {
                string newColor = colorBtn.GetCssValue("color");
                return newColor != oldColor;
            });

            // Обновляем страницу
            driver.Navigate().Refresh();

            // Ждём появления кнопки Visible After 5 Seconds
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("visibleAfter")));
        }

        [Test]
        public void Test2()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Forms']"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Practice Form']"))).Click();

            // Заполнение формы
            driver.FindElement(By.Id("firstName")).SendKeys("Иван");
            driver.FindElement(By.Id("lastName")).SendKeys("Петров");
            driver.FindElement(By.Id("userEmail")).SendKeys("ivan.petrov@example.com");

            driver.FindElement(By.XPath("//label[text()='Male']")).Click();

            driver.FindElement(By.Id("userNumber")).SendKeys("9991234567");

            // Дата рождения
            driver.FindElement(By.Id("dateOfBirthInput")).Click();
            driver.FindElement(By.ClassName("react-datepicker__year-select")).SendKeys("2000");
            driver.FindElement(By.ClassName("react-datepicker__month-select")).SendKeys("January");
            driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__day') and text()='10']")).Click();

            // Subject: вводим + Enter
            var subject = driver.FindElement(By.Id("subjectsInput"));
            subject.SendKeys("Maths");
            subject.SendKeys(Keys.Enter);
            subject.SendKeys("Physics");
            subject.SendKeys(Keys.Enter);
            subject.SendKeys("English");
            subject.SendKeys(Keys.Enter);

            // Хобби
            driver.FindElement(By.XPath("//label[text()='Sports']")).Click();
            driver.FindElement(By.XPath("//label[text()='Music']")).Click();

            // Адрес
            driver.FindElement(By.Id("currentAddress")).SendKeys("Москва, улица Пушкина");

            // Выбор штата
            driver.FindElement(By.Id("react-select-3-input")).SendKeys("NCR");
            driver.FindElement(By.Id("react-select-3-input")).SendKeys(Keys.Enter);

            // Город
            driver.FindElement(By.Id("react-select-4-input")).SendKeys("Delhi");
            driver.FindElement(By.Id("react-select-4-input")).SendKeys(Keys.Enter);

            // Submit
            driver.FindElement(By.Id("submit")).Click();

            // Проверка данных в модальном окне
            var modal = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("example-modal-sizes-title-lg")));
            Assert.That(modal.Text, Is.EqualTo("Thanks for submitting the form"));

            string table = driver.FindElement(By.ClassName("table-responsive")).Text;
            Assert.That(table, Does.Contain("Иван"));
            Assert.That(table, Does.Contain("Петров"));
            Assert.That(table, Does.Contain("ivan.petrov@example.com"));
            Assert.That(table, Does.Contain("9991234567"));
            Assert.That(table, Does.Contain("Maths"));
            Assert.That(table, Does.Contain("Physics"));
            Assert.That(table, Does.Contain("English"));

            // Закрыть окно
            driver.FindElement(By.Id("closeLargeModal")).Click();
        }

        [Test]
        public void Test3()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Alerts, Frame & Windows']"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Browser Windows']"))).Click();

            string originalWindow = driver.CurrentWindowHandle;

            // New Tab
            driver.FindElement(By.Id("tabButton")).Click();
            wait.Until(d => d.WindowHandles.Count == 2);

            string newTab = driver.WindowHandles.First(h => h != originalWindow);
            driver.SwitchTo().Window(newTab);

            Assert.That(driver.Url, Is.EqualTo("https://demoqa.com/sample"));
            driver.Close();
            driver.SwitchTo().Window(originalWindow);

            // New Window
            driver.FindElement(By.Id("windowButton")).Click();
            wait.Until(d => d.WindowHandles.Count == 2);

            string newWindow = driver.WindowHandles.First(h => h != originalWindow);
            driver.SwitchTo().Window(newWindow);

            Assert.That(driver.Url, Is.EqualTo("https://demoqa.com/sample"));
            driver.Close();
            driver.SwitchTo().Window(originalWindow);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }

    public class Tests_lab4
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Alerts, Frame & Windows']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Alerts']"))).Click();

            // 1) First alert
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.Id("alertButton"))).Click();

            IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            Assert.That(alert.Text, Is.EqualTo("You clicked a button"));
            alert.Accept();

            // 2) Second alert (5 sec timer)
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.Id("timerAlertButton"))).Click();

            alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            Assert.That(alert.Text, Is.EqualTo("This alert appeared after 5 seconds"));
            alert.Accept();

            // 3) Third alert (Confirm)
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.Id("confirmButton"))).Click();

            alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            Assert.That(alert.Text, Is.EqualTo("Do you confirm action?"));
            alert.Dismiss(); // Отмена

            // 4) Fourth alert (Prompt)
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.Id("promtButton"))).Click();

            alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            alert.SendKeys("Hello world");
            alert.Accept();
        }

        [Test]
        public void Test2()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Alerts, Frame & Windows']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Frames']"))).Click();

            // Frame 1
            driver.SwitchTo().Frame("frame1");
            string text1 = driver.FindElement(By.Id("sampleHeading")).Text;
            Assert.That(text1, Is.EqualTo("This is a sample page"));
            driver.SwitchTo().DefaultContent();

            // Frame 2
            driver.SwitchTo().Frame("frame2");
            string text2 = driver.FindElement(By.Id("sampleHeading")).Text;
            Assert.That(text2, Is.EqualTo("This is a sample page"));
            driver.SwitchTo().DefaultContent();
        }

        [Test]
        public void Test3()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Alerts, Frame & Windows']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Nested Frames']"))).Click();

            // Переходим в родительский фрейм
            driver.SwitchTo().Frame(driver.FindElement(By.Id("frame1")));

            // Проверяем текст в parent frame
            string parentText = driver.FindElement(By.TagName("body")).Text;
            Assert.That(parentText, Does.Contain("Parent frame"));

            // Переходим в child iframe
            driver.SwitchTo().Frame(0);
            string childText = driver.FindElement(By.TagName("p")).Text;
            Assert.That(childText, Is.EqualTo("Child Iframe"));

            driver.SwitchTo().DefaultContent();
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }

    public class Tests_lab5
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Alerts, Frame & Windows']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Modal Dialogs']"))).Click();

            // Открываем Small Modal
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.Id("showSmallModal"))).Click();

            // Заголовок
            var header = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementIsVisible(By.Id("example-modal-sizes-title-sm")));

            Assert.That(header.Text, Is.EqualTo("Small Modal"));

            // Основной текст
            var body = driver.FindElement(By.CssSelector(".modal-body"));
            Assert.That(body.Text, Does.Contain("This is a small modal. It has very less content"));

            // Закрываем
            driver.FindElement(By.Id("closeSmallModal")).Click();
        }

        [Test]
        public void Test2()
        {
 driver.Navigate().GoToUrl("https://demoqa.com/");
    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementToBeClickable(By.XPath("//h5[text()='Widgets']"))).Click();

    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementToBeClickable(By.XPath("//span[text()='Accordian']"))).Click();

    // 1 — What is Lorem Ipsum?
    var item1 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementToBeClickable(By.Id("section1Heading")));
    item1.Click();

    var content1 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementIsVisible(By.Id("section1Content")));
    Assert.That(content1.Text.Length, Is.GreaterThan(10));

    // 2 — Where does it come from?
    var item2 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementToBeClickable(By.Id("section2Heading")));
    item2.Click();

    var content2 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementIsVisible(By.Id("section2Content")));
    Assert.That(content2.Text.Length, Is.GreaterThan(10));

    // 3 — Why do we use it?
    var item3 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementToBeClickable(By.Id("section3Heading")));
    item3.Click();

    var content3 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
        .ElementIsVisible(By.Id("section3Content")));
    Assert.That(content3.Text.Length, Is.GreaterThan(10));
        }

        [Test]
        public void Test3()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Widgets']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Auto Complete']"))).Click();

            // MULTIPLE — вводим Black, Red, Magenta
            var multi = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementIsVisible(By.Id("autoCompleteMultipleInput")));

            string[] colors = { "Black", "Red", "Magenta" };

            foreach (var color in colors)
            {
                multi.SendKeys(color);
                multi.SendKeys(Keys.Enter);
                Thread.Sleep(200);
            }

            // SINGLE — поставить Black
            var single = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementIsVisible(By.Id("autoCompleteSingleInput")));

            single.SendKeys("Black");
            single.SendKeys(Keys.Enter);

            Thread.Sleep(300);

            // SINGLE — заменить на Red
            var clearButton = driver.FindElement(By.CssSelector(".auto-complete__indicator.auto-complete__clear-indicator"));
            clearButton.Click();

            single.SendKeys("Red");
            single.SendKeys(Keys.Enter);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }


    public class Tests_lab6
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Widgets']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Date Picker']"))).Click();

            // ---- SELECT DATE ---- (1 декабря 2023)
            var dateInput = wait.Until(
                SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("datePickerMonthYearInput"))
            );
            dateInput.Click();

            // Выбор года
            driver.FindElement(By.CssSelector(".react-datepicker__year-select"))
                .FindElement(By.CssSelector("option[value='2023']")).Click();

            // Выбор месяца (December — value=11)
            driver.FindElement(By.CssSelector(".react-datepicker__month-select"))
                .FindElement(By.CssSelector("option[value='11']")).Click();

            // Выбор дня — 1
            driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__day') and text()='1']")).Click();

            Assert.That(dateInput.GetAttribute("value"), Is.EqualTo("12/01/2023"));


            var dtInput = driver.FindElement(By.Id("dateAndTimePickerInput"));
            dtInput.Click();

            // Месяц/год заголовок и переход
            driver.FindElement(By.CssSelector(".react-datepicker__month-read-view")).Click();
            driver.FindElement(By.XPath("//div[text()='November']")).Click();

            driver.FindElement(By.CssSelector(".react-datepicker__year-read-view")).Click();
            driver.FindElement(By.XPath("//div[text()='2022']")).Click();

            // День 2
            driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__day') and text()='2']")).Click();

            // Выбор времени 20:00
            driver.FindElement(By.XPath("//li[text()='20:00']")).Click();

            Assert.That(dtInput.GetAttribute("value"), Does.Contain("November"));
            Assert.That(dtInput.GetAttribute("value"), Does.Contain("2022"));
        }

        [Test]
        public void Test2()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Widgets']"))).Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Slider']"))).Click();

            // находим input[type=range]
            IWebElement slider = wait.Until(
                SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//input[@type='range']"))
            );

            // элемент, который показывает текущее значение
            var valueBox = driver.FindElement(By.Id("sliderValue"));

            int target = 50;

            // клик по слайдеру для фокуса
            slider.Click();

            // текущие значение
            int current = int.Parse(slider.GetAttribute("value"));

            // если уже нужное — выход
            if (current != target)
            {
                int diff = target - current;
                string key = diff > 0 ? Keys.ArrowRight : Keys.ArrowLeft;
                int steps = Math.Abs(diff);

                // ограничение — защита от бесконечного цикла
                int maxAttempts = 3;
                int attempt = 0;

                while (attempt < maxAttempts)
                {
                    for (int i = 0; i < steps; i++)
                    {
                        slider.SendKeys(key);
                        Thread.Sleep(30); // небольшая пауза для стабильности
                    }

                    // дадим JS обработчикам примениться
                    Thread.Sleep(100);

                    // прочитаем текущее значение (у input[type=range])
                    current = int.Parse(slider.GetAttribute("value"));

                    if (current == target)
                        break; // достигли цели

                    // если не достигли — скорректируем: пересчитаем diff дальше
                    diff = target - current;
                    steps = Math.Abs(diff);
                    key = diff > 0 ? Keys.ArrowRight : Keys.ArrowLeft;
                    attempt++;
                }
            }

            // финальная проверка
            Assert.That(valueBox.GetAttribute("value"), Is.EqualTo(target.ToString()));
        }

        [Test]
        public void Test3()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Перейти в раздел 'Widgets'
            IWebElement widgetsSection = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//h5[text()='Widgets']")));

            js.ExecuteScript("arguments[0].scrollIntoView(true);", widgetsSection);
            Thread.Sleep(500);
            js.ExecuteScript("arguments[0].click();", widgetsSection);

            // Выбрать пункт 'Tabs'
            IWebElement tabsMenuItem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementToBeClickable(By.XPath("//span[text()='Tabs']")));

            js.ExecuteScript("arguments[0].scrollIntoView(true);", tabsMenuItem);
            Thread.Sleep(500);
            js.ExecuteScript("arguments[0].click();", tabsMenuItem);

            // Дождаться загрузки страницы с табами
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementIsVisible(By.Id("demo-tab-what")));

            // Проверить вкладку What (активна по умолчанию)
            IWebElement whatTab = driver.FindElement(By.Id("demo-tab-what"));
            IWebElement whatContent = driver.FindElement(By.Id("demo-tabpane-what"));

            Assert.That(whatTab.GetAttribute("class"), Does.Contain("active"), "Вкладка What должна быть активной");
            Assert.That(whatContent.Text, Is.Not.Empty, "Вкладка What должна содержать текст");
            Assert.That(whatContent.Displayed, Is.True, "Контент вкладки What должен отображаться");

            // Открыть вкладку Origin
            IWebElement originTab = driver.FindElement(By.Id("demo-tab-origin"));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", originTab);
            Thread.Sleep(300);
            js.ExecuteScript("arguments[0].click();", originTab);

            // Проверить вкладку Origin
            IWebElement originContent = driver.FindElement(By.Id("demo-tabpane-origin"));

            // Ждем пока вкладка станет активной
            wait.Until(d => originTab.GetAttribute("class").Contains("active"));
            wait.Until(d => originContent.Displayed && !string.IsNullOrEmpty(originContent.Text));

            Assert.That(originTab.GetAttribute("class"), Does.Contain("active"), "Вкладка Origin должна быть активной");
            Assert.That(originContent.Text, Is.Not.Empty, "Вкладка Origin должна содержать текст");
            Assert.That(originContent.Displayed, Is.True, "Контент вкладки Origin должен отображаться");

            // Открыть вкладку Use
            IWebElement useTab = driver.FindElement(By.Id("demo-tab-use"));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", useTab);
            Thread.Sleep(300);
            js.ExecuteScript("arguments[0].click();", useTab);

            // Проверить вкладку Use
            IWebElement useContent = driver.FindElement(By.Id("demo-tabpane-use"));

            // Ждем пока вкладка станет активной
            wait.Until(d => useTab.GetAttribute("class").Contains("active"));
            wait.Until(d => useContent.Displayed && !string.IsNullOrEmpty(useContent.Text));

            Assert.That(useTab.GetAttribute("class"), Does.Contain("active"), "Вкладка Use должна быть активной");
            Assert.That(useContent.Text, Is.Not.Empty, "Вкладка Use должна содержать текст");
            Assert.That(useContent.Displayed, Is.True, "Контент вкладки Use должен отображаться");

            // Проверить что вкладка More недоступна - только проверка класса disabled
            IWebElement moreTab = driver.FindElement(By.Id("demo-tab-more"));
            Assert.That(moreTab.GetAttribute("class"), Does.Contain("disabled"), "Вкладка More должна иметь класс disabled");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }

    public class Tests_lab7
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/select-menu");

            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.getElementById('fixedban')?.remove();");
            js.ExecuteScript("document.querySelector('footer')?.remove();");

            // Select Value → A root option
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("withOptGroup"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='A root option']"))).Click();

            // Select One → Ms.
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("selectOne"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Ms.']"))).Click();

            // Old Style Select Menu → Black
            new SelectElement(driver.FindElement(By.Id("oldSelectMenu"))).SelectByText("Black");

            // Multiselect → Black, Red (корректная версия)
            var multiInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("react-select-4-input")
            ));

            multiInput.SendKeys("Black");
            multiInput.SendKeys(Keys.Enter);

            multiInput.SendKeys("Red");
            multiInput.SendKeys(Keys.Enter);

            // Standard multi select → Opel
            new SelectElement(driver.FindElement(By.Id("cars"))).SelectByText("Opel");
        }

        [Test]
        public void Test2()
        {
            
            driver.Navigate().GoToUrl("https://demoqa.com/");

            
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Interactions']")));
            driver.FindElement(By.XPath("//h5[text()='Interactions']")).Click();

           
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Sortable']")));
            driver.FindElement(By.XPath("//span[text()='Sortable']")).Click();

           
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-list")));
            driver.FindElement(By.Id("demo-tab-list")).Click();

            
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".vertical-list-container")));
            Actions actions = new Actions(driver);

           
            var listItems = driver.FindElements(By.CssSelector(".vertical-list-container .list-group-item")).ToList();

           
            for (int i = listItems.Count - 1; i > 0; i--)
            {
                IWebElement sourceElement = driver.FindElements(By.CssSelector(".vertical-list-container .list-group-item"))[i];
                IWebElement targetElement = driver.FindElements(By.CssSelector(".vertical-list-container .list-group-item"))[0];

                actions.DragAndDrop(sourceElement, targetElement).Perform();
                System.Threading.Thread.Sleep(200); // Небольшая задержка для стабильности
            }
        }

        public void ScrollTo(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "arguments[0].scrollIntoView(true);", element);
        }
        [Test]
        public void Test3()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

           
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//h5[text()='Interactions']"))).Click();

            
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Selectable']"))).Click();

           
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-grid"))).Click();

           
            var allCells = wait.Until(d => d.FindElements(By.CssSelector("#gridContainer .list-group-item")));
            foreach (var cell in allCells)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", cell);
                wait.Until(ExpectedConditions.ElementToBeClickable(cell)).Click();
            }

            Thread.Sleep(500);

           
            foreach (var cell in allCells)
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(cell)).Click();
            }

            Thread.Sleep(500);

         
            var five = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//li[text()='Five']")));
            five.Click();

            Thread.Sleep(1000);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }

    public class Tests_lab8
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArguments("--lang=ru-ru");
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        
        }


        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");

            // Скрываем фиксированный баннер
            ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('fixedban').style.display='none';");

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

           
            var bookStore = wait.Until(d =>
            {
                var el = d.FindElement(By.XPath("//h5[text()='Book Store Application']"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            ScrollToElement(bookStore);
            bookStore.Click();

          
            var login = wait.Until(d =>
            {
                var el = d.FindElement(By.XPath("//span[text()='Login']"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            ScrollToElement(login);
            login.Click();

            driver.FindElement(By.Id("userName")).SendKeys("TestUser");
            driver.FindElement(By.Id("password")).SendKeys("123");
            driver.FindElement(By.Id("login")).Click();

            var error1 = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("name"));
                return (el.Displayed) ? el : null;
            });
            Assert.That(error1.Text, Is.EqualTo("Invalid username or password!"));

           
            var newUser = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("newUser"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            ScrollToElement(newUser);
            newUser.Click();

            driver.FindElement(By.Id("firstname")).SendKeys("Alex");
            driver.FindElement(By.Id("lastname")).SendKeys("Popov");
            driver.FindElement(By.Id("userName")).SendKeys("TestUser2");
            driver.FindElement(By.Id("password")).SendKeys("1");

           
            driver.FindElement(By.Id("register")).Click();

           
            var error2 = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("name"));
                return (el.Displayed) ? el : null;
            });
            Assert.That(error2.Text, Is.EqualTo("Please verify reCaptcha to register!"));

            
            wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath("//iframe[contains(@title, 'reCAPTCHA')]")));
            var captchaCheckbox = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("recaptcha-anchor"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            captchaCheckbox.Click();
            driver.SwitchTo().DefaultContent();

            // ------------------------- Register с валидным паролем -------------------------
            var passwordField = driver.FindElement(By.Id("password"));
            passwordField.Clear();
            passwordField.SendKeys("Abcde123!"); // валидный пароль

            driver.FindElement(By.Id("register")).Click();

            // ------------------------- Модальное окно после успешной регистрации -------------------------
            var modalOk = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("closeSmallModal-ok"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            modalOk.Click();
        }

      
        public void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }
}
