using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MainProject
{
    public partial class GUI : Form
    {
        bool ChromeHeadless = true;
        Browser Browser = null;
        Thread ThreadHostBrowser = null;
        Dictionary<int, Thread> ListasThreads = new Dictionary<int, Thread>();
        List<Button> ListasButoes = new List<Button>();
        List<Label> ListasLabels = new List<Label>();
        List<ProgressBar> ListasProgress = new List<ProgressBar>();
        List<TextBox> ListasTempos = new List<TextBox>();
        List<Lista> ListasFarm = new List<Lista>();       

        public GUI()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char chr = e.KeyChar;
            if (!Char.IsDigit(chr) && chr != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char chr = e.KeyChar;
            if (!Char.IsDigit(chr) && chr != 8)
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Logar") {
                Logs("A Logar");
                ThreadHostBrowser = new Thread(TrataThread);
                ThreadHostBrowser.Start();
            }
            else
            {
                ThreadHostBrowser.Abort();
                Browser.Quit();
                this.Invoke((MethodInvoker)delegate {
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    button2.Enabled = false;
                    button2.Text = "Logar";
                    button2.Enabled = true;
                    listBoxFarming.Controls.Clear();
                });
                Logs("Jogador deslogado");                
            }
            
        }

        private void GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Browser != null)
            {
                ThreadHostBrowser.Abort();
                Browser.Quit();               
            }
            Logs("Bot desligado. Bye !");
            Environment.Exit(Environment.ExitCode);
        }

        //---------------------------------------- METODOS MEUS --------------------------------------------------
        private void TrataThread()
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            if (ChromeHeadless)
            {
                options.AddArguments("headless");
            }

            this.Invoke((MethodInvoker)delegate {
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                button2.Enabled = false;
            });

            this.Browser = new Browser(driverService, options);  

            if (!this.Browser.Login(textBox3.Text, textBox4.Text, textBox5.Text))
            {
                this.Invoke((MethodInvoker)delegate {
                    Logs("Login Inválido");
                    Browser = null;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    button2.Enabled = true;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate {
                    Logs("Login com Sucesso");
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                    textBox5.Enabled = false;
                    button2.Enabled = true;
                    button2.Text = "Deslogar";
                });
                Thread.Sleep(1500);
                Logado();
            }
        }

        public void Logado()
        {    
            List<String> NomeListasFarm = Browser.ListasFarm();            
            
            if (NomeListasFarm.Count == 0)
            {
                Logs("Nao Existem Listas");
                return;
            }

            Logs("Pronto para Farmar");

            
            for (int i = 0; i < NomeListasFarm.Count; i++)
            {
                Lista ListaFarm = new Lista(NomeListasFarm[i], i);
                ListasFarm.Add(ListaFarm);
                this.Invoke((MethodInvoker)delegate
                {
                    listBoxFarming.Controls.Add(ListaFarm.BarraProgresso);
                    listBoxFarming.Controls.Add(ListaFarm.LabelNome);
                    listBoxFarming.Controls.Add(ListaFarm.TextBoxTempoMinimo);
                    listBoxFarming.Controls.Add(ListaFarm.TextBoxTempoMaximo);
                    listBoxFarming.Controls.Add(ListaFarm.ButaoEnviar);
                });

            }
        }

        public void TimeOutLista(int i)
        {
            int min = Int32.Parse(ListasTempos[i * 2].Text);
            int max = Int32.Parse(ListasTempos[i * 2 + 1].Text);
            int Rand = new Random().Next(min, max);

            this.Invoke((MethodInvoker)delegate
            {
                ListasProgress[i].Maximum = Rand * 60;
                ListasProgress[i].Minimum = 0;
                ListasProgress[i].Value = 0;
            });
            Logs("Lista " + ListasFarm[i].Nome + " a dormir por " + Rand.ToString() + " minutos");

            for (int j = 0; j < Rand * 60; j++)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ListasProgress[i].Increment(1);

                });
                Thread.Sleep(1000);
            }
        }

        public void Logs(string log)
        {
            string Data = DateTime.Now.ToString("dd/MM HH:mm:ss");
            this.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Insert(0, Data + " --> " + log);
            });

            string path = textBox3.Text + "_log.txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(Data + " --> " + log);
                tw.Close();
            }
            else if (File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(Data + " --> " + log);
                }
            }
        }
    }
}
