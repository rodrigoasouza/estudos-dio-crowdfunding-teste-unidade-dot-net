using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Text;

namespace Vaquinha.AutomatedUITests
{
    public class DriverFactory
    {
        private readonly IWebDriver _Driver;

        public DriverFactory()
        {
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            
            // Faz criação de porta para abrir o browser.
            service.Port = new Random().Next(64000, 64800);
            
            // Inicializa o IWebDriver do selenium, é ele que disponibiliza as consultas e manipulacoes das paginas. 
            CodePagesEncodingProvider.Instance.GetEncoding(437);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("-headless");
            options.AddArgument("-safe-mode");
            options.AddArgument("-ignore-certificate-errors");
            options.Profile = new FirefoxProfile
            {
                AcceptUntrustedCertificates = true,
                AssumeUntrustedCertificateIssuer = false
            };
            
            _Driver = new FirefoxDriver(service, options);
            
            _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _Driver.Manage().Window.Maximize();
        }

        // Navega para determinada URL
        public void NavigateToUrl(string url)
        {            
            _Driver.Navigate().GoToUrl(url);
        }

        // Finaliza driver e serviço.
        public void Close()
        {
            _Driver.Quit();
        }

        // Disponibiliza driver.
        public IWebDriver GetWebDriver()
        {
            return _Driver;
        }
    }
}