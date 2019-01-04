using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenQA.Selenium.Chrome;

namespace MainProject
{
    class Browser : ChromeDriver
    {
        private string server = String.Empty;

        public Browser(ChromeDriverService service, ChromeOptions chromeOptions) : base(service, chromeOptions) { }

        public bool Login(string nick, string pass, string server)
        {      
            Navigate().GoToUrl(server);
            FindElementByName("name").SendKeys(nick);
            FindElementByName("password").SendKeys(pass);
            FindElementByName("password").Submit();

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

        public List<String> GetNomeListasFarm()
        {
            List<String> nomeListas = new List<String>();
            var WebElementListas = FindElementsByClassName("listTitleText");

            foreach(var WebElementLista in WebElementListas)
            {
                nomeListas.Add(WebElementLista.Text);
            }

            return nomeListas;
        }

        public string EnviaFarm(bool FarmSemPerdas, int index)
        {
            if (FarmSemPerdas)
            {
                try
                {
                    //ExecuteScript("var i = arguments[0]; var lista = document.querySelectorAll('.list')[i].children[1];lista.children[Math.floor(Math.random() * lista.childElementCount)].children[0].firstElementChild.click()", index);
                    ExecuteScript("var index = arguments[0];document.querySelectorAll('.markAll.check')[index].click();var lista = document.querySelectorAll('.list')[index];var childs = lista.children[1].children;for (var i = 0; i < childs.length; i++){child = childs[i];aux = childs[i].children[5].children[0].className; if(aux == 'iReport iReport3' || aux == 'iReport iReport2'){child.children[0].children[0].click();}}", index);
                    ExecuteScript("var i = arguments[0]; document.querySelectorAll('.listContent')[i].children[3].click()", index);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return "Morreu";
                }
            }
            else
            {
                FindElementsByClassName("markAll")[index * 2 + 1].Click();
                FindElementsByClassName("button-content")[index * 2 + 1].Click();
            }

            try
            {
                return (string)ExecuteScript("var i = arguments[0]; return document.querySelectorAll('.listContent')[i].children[3].innerText.split(' assaltos')[0]", index); 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "Morreu";
            }
        }

        public string IrCentroAldeia()
        {
            try
            {
                FindElementByCssSelector("#n2 > a").Click();
                return "Ir para o centro da aldeia";
            }
            catch (Exception e)
            {
                return $"Exception {e.Message}";
            }
        }

        public string EntrarEdificio(int id)
        {
            try
            {
                FindElementByXPath($"//*[@id='village_map']/div[{id}]/div").Click();
                return "Entrei no edificio";
            }
            catch (Exception e)
            {
                return $"Exception {e.Message}";
            }
        }

        public string MudarTabEdificio(int i)
        {
            var elemento = FindElementsByClassName("tabItem")[i];
            string Texto = elemento.Text;
            elemento.Click();
            return "Entrei no separador " + Texto;
        }
    }
}
