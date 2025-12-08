using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace Example2
{
    [TestFixture]
    public class Class1
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
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        [Test]
        public void Test1_CheckPageStructure()
        {
            driver.Navigate().GoToUrl("https://www.rzd.ru/");

            // Хедер
            var header = wait.Until(ExpectedConditions
                .ElementIsVisible(By.TagName("header")));
            Assert.That(header, Is.Not.Null, "Хедер не найден");

            var headerImages = header.FindElements(By.CssSelector("img, svg"));
            Assert.That(headerImages.Count, Is.GreaterThan(0), "В хедере нет изображений");

            // Меню
            var nav = wait.Until(ExpectedConditions
                .ElementIsVisible(By.TagName("nav")));
            var navLinks = nav.FindElements(By.TagName("a"));
            Assert.That(navLinks.Count, Is.GreaterThanOrEqualTo(4),
                "Недостаточно пунктов меню");

            // Основной контейнер
            var main = wait.Until(ExpectedConditions
                .ElementExists(By.CssSelector("main, #app-root")));
            Assert.That(main, Is.Not.Null, "Основной контейнер приложения не найден");
        }

        [Test]
        public void Test2_NavigationMenuLinksAreValidAndUnique()
        {
            driver.Navigate().GoToUrl("https://www.rzd.ru/");

            // Находим меню
            var nav = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("nav")));

            // Ссылки в меню
            var links = nav.FindElements(By.TagName("a"))
                .Where(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text))
                .ToList();

            Assert.That(links.Count, Is.GreaterThanOrEqualTo(5),
                "Меню должно содержать минимум 5 видимых пунктов");

            // Проверка: у всех есть href
            foreach (var link in links)
            {
                string text = link.Text.Trim();
                string href = link.GetAttribute("href");

                Assert.That(text.Length, Is.GreaterThan(1),
                    "Слишком короткий текст пункта меню");

                Assert.That(href, Is.Not.Null.And.Not.Empty,
                    $"У пункта меню '{text}' отсутствует атрибут href");

                Assert.That(href.StartsWith("http"),
                    $"У пункта меню '{text}' некорректный href: {href}");
            }

            // Проверка: все href уникальны
            var hrefs = links.Select(l => l.GetAttribute("href")).ToList();
            var duplicates = hrefs
                .GroupBy(h => h)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.That(duplicates.Count, Is.EqualTo(0),
                "Найдены дублирующиеся ссылки в меню");
        }

        [Test]
        public void Test3_OpenMenu_LoginOrAnotherPage()
        {
            driver.Navigate().GoToUrl("https://www.rzd.ru/");

            // Ищем ссылку/кнопку для входа или меню «Пассажирам», «Личный кабинет»
            IWebElement loginLink = null;
            var links = driver.FindElements(By.TagName("a"));
            foreach (var a in links)
            {
                if (a.Displayed && !string.IsNullOrEmpty(a.Text) &&
                    (a.Text.ToLower().Contains("вход") || a.Text.ToLower().Contains("логин") || a.Text.ToLower().Contains("пассажирам")))
                {
                    loginLink = a;
                    break;
                }
            }

            Assert.That(loginLink, Is.Not.Null, "Ссылка входа / меню пользователей не найдена");
            loginLink.Click();

            // Проверяем, что открылась форма логина — ищем input type=password
            var pwd = wait.Until(d => d.FindElements(By.CssSelector("input[type='password']")).FirstOrDefault());
            Assert.That(pwd, Is.Not.Null, "Форма логина не найдена");
        }

        [Test]
        public void Test4_CheckFooterContainsLinks()
        {
            driver.Navigate().GoToUrl("https://www.rzd.ru/");

            // Скролим в низ
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            // Находим футер
            var footer = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("footer")));
            var links = footer.FindElements(By.TagName("a"));

            Assert.That(links.Count, Is.GreaterThanOrEqualTo(5), "Ожидается минимум 5 ссылок в футере");
        }

        [Test]
        public void Test5_CheckMainMenuItemsExist()
        {
            driver.Navigate().GoToUrl("https://www.rzd.ru/");

            var menu = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("nav")));
            var items = menu.FindElements(By.TagName("a"));

            Assert.That(items.Count, Is.GreaterThanOrEqualTo(5), "Ожидается минимум 5 пунктов в основном меню");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null;
        }
    }
}
//Test1_CheckPageStructure
//Проверяет, что на главной странице есть хедер, меню навигации и основной контейнер приложения.

//Test2_NavigationMenuLinksAreValidAndUnique
//Проверяет, что пункты меню навигации видимые, имеют корректный текст, валидные ссылки и что этих ссылок нет дубликатов.

//Test3_OpenMenu_LoginOrAnotherPage
//Находит ссылку на вход или пользовательский раздел, кликает по ней и проверяет, что открылась форма логина.

//Test4_CheckFooterContainsLinks
//Прокручивает страницу вниз, находит футер и проверяет, что в нём есть минимум 5 ссылок.

//Test5_CheckMainMenuItemsExist
//Проверяет, что основное меню содержит не менее 5 пунктов.