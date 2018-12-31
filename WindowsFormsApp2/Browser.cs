using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MainProject
{
    class Browser : ChromeDriver
    {
        private string server = String.Empty;
        Semaphore semaforo = new Semaphore(1, 1);

        public Browser(ChromeDriverService service, ChromeOptions chromeOptions) : base(service, chromeOptions)
        {
        }

        public bool Login(string nick, string pass, string server)
        {      
            this.Navigate().GoToUrl(server);
            this.FindElementByName("name").SendKeys(nick);
            this.FindElementByName("password").SendKeys(pass);
            this.FindElementByName("password").Submit();

            if (this.FindElementsByClassName("error").Count > 0)
            {
                this.Quit();
                return false;
            }
            else
            {
                this.server = server;
                return true;
            }
        }

        public List<String> ListasFarm()
        {
            this.Navigate().GoToUrl(server + "build.php?tt=99&id=39");
            List<String> Listas = new List<String>();

            var a = this.FindElementsByClassName("listTitleText");
            for (int i = 0; i < a.Count; i++)
            {
                Listas.Add(a[i].Text);
            }
            return Listas;
        }

        public string EnviaFarm(bool FarmSemPerdas, int index)
        {
            semaforo.WaitOne();

            if (FarmSemPerdas)
            {
                ExecuteScript("var i = arguments[0]; var lista = document.querySelectorAll('.list')[i].children[1];lista.children[Math.floor(Math.random() * lista.childElementCount)].children[0].firstElementChild.click()", index);
                //ExecuteScript("var index = arguments[0];document.querySelectorAll('.markAll.check')[index].click();var lista = document.querySelectorAll('.list')[index];var childs = lista.children[1].children;for (var i = 0; i < childs.length; i++){child = childs[i];aux = childs[i].children[5].children[0].className; if(aux == 'iReport iReport3' || aux == 'iReport iReport2'){child.children[0].children[0].click();}}", index);
                Thread.Sleep(5432);
                ExecuteScript("var i = arguments[0]; document.querySelectorAll('.listContent')[i].children[3].click()", index);
            }
            else
            {
                FindElementsByClassName("markAll")[index * 2 + 1].Click();
                FindElementsByClassName("button-content")[index * 2 + 1].Click();
            }
            semaforo.Release(1);
            return (string)ExecuteScript("var i = arguments[0]; console.log(document.querySelectorAll('.listContent')[i].children[3].innerText.split(' assaltos')[0]); return document.querySelectorAll('.listContent')[i].children[3].innerText.split(' assaltos')[0]", index);
        }
    }
}
