using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainProject
{
    public partial class UI : Form
    {
        bool chromeHeadless = true;
        int tempoMinimoEntreCadaComando = 2;
        int tempoMaximoEntreCadaComando = 5;
        Browser Browser;
        Thread ThreadHostBrowser;
        Dictionary<Thread, bool> listasThreads;
        List<Button> listasButoes;
        List<Label> listasLabels;
        List<ProgressBar> listasProgress;
        List<TextBox> listasTempos;
        List<Lista> listasFarm;
        Semaphore semaforo;

        List<Action> IrParaListaFarms = new List<Action>();
       

        public UI()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Logar") {
                Logs("A fazer o login");
                button2.Text = "Deslogar";
                LançaThreadHostBrowser();
            }
            else if(button2.Text == "Deslogar")
            {
                Logs("Jogador deslogado");
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                button2.Enabled = false;
                button2.Text = "Logar";
                button2.Enabled = true;
                panel2.Controls.Clear();
                LibertaMemoriaDados();  
            }            
        }
        
        private void LançaThreadHostBrowser()
        {
            ThreadHostBrowser = new Thread(SetupThreadHostBrowser)
            {
                IsBackground = true
            };
            ThreadHostBrowser.Start();
        }

        private void LibertaMemoriaDados()
        {
            foreach (var thread in listasThreads.Keys) {
                thread.Abort();
            }
            listasButoes.Clear();
            listasLabels.Clear();
            listasProgress.Clear();
            listasTempos.Clear();
            listasFarm.Clear();
            listasThreads.Clear();

            if (semaforo != null)
            {
                semaforo.Dispose();
            }

            if (Browser!= null)
            {
                Browser.Quit();
            }

            if(ThreadHostBrowser != null)
            {
                ThreadHostBrowser.Abort();
            }
        }
        
        private void GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            LibertaMemoriaDados();
            Environment.Exit(Environment.ExitCode);
        }

        private void ButaoFarm_Click(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            int index = -1;

            foreach (var lista in listasFarm)
            {
                if (pressedButton == lista.ButaoFarm) {
                    index = lista.Index;
                    break;
                }
            }

            if(index == -1) {
                return;
            }

            if (pressedButton.Text == "Farmar") {
                LançaThreadFarmar(index);
                Invoke((MethodInvoker)delegate
                {
                    listasButoes[index].Text = "Parar";
                    listasLabels[index].ForeColor = Color.Green;
                });                
                Logs($"Começou a farmar {listasFarm[index].Nome}");
            }
            else if (pressedButton.Text == "Parar")
            {
                PararThreadFarm(index);
                Invoke((MethodInvoker)delegate
                {
                    listasButoes[index].Text = "Farmar";
                    listasLabels[index].ForeColor = Color.Red;
                });
                Logs("Parou de farmar lista" + listasFarm[index].Nome);
            }
        }
        
        private void LançaThreadFarmar(int index)
        {
            Thread ThreadFarm = new Thread(() => EnviaFarms(index)) {
                IsBackground = true
            };
            listasFarm[index].ThreadFarm = ThreadFarm;
            listasThreads.Add(ThreadFarm, true);

            ThreadFarm.Start();
            
        }

        private void PararThreadFarm(int index)
        {
            Thread ThreadAux = listasFarm[index].ThreadFarm;
            listasThreads[ThreadAux] = false;
            Task.Factory.StartNew(() => RetiraThreadFarmLista(ThreadAux));
            listasProgress[index].Value = 0;
        }

        private void RetiraThreadFarmLista(Thread aRetirar)
        {
            aRetirar.Join();
            listasThreads.Remove(aRetirar);
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // ENCRIPTA DADOS USER E ESCREVE FICHEIRO
        {
            string paraEncriptar = $"{textBox3.Text};{textBox4.Text};{textBox5.Text}";
            string encriptada = Encriptação.Encrypt(paraEncriptar);

            string path = "account.txt";
            if (!File.Exists(path)) { File.Create(path).Dispose(); }

            using (var tw = new StreamWriter(path))
            {
                tw.WriteLine(encriptada);
            }
        }

        private void UI_Load(object sender, EventArgs e)
        {
            CarregarDadosConta();
        }

        private void CarregarDadosConta()
        {
            string path = "account.txt";
            string encriptada = string.Empty;

            if (File.Exists(path))
            {
                using (var streamReader = new StreamReader(path))
                {
                    encriptada = streamReader.ReadLine();
                }

                string desencriptada = string.Empty;
                if(encriptada.Length > 0)
                {
                    desencriptada = Encriptação.Decrypt(encriptada);
                    string[] dados = desencriptada.Split(';');
                    textBox3.Text = dados[0]; //preenche user
                    textBox4.Text = dados[1]; //preenche pass
                    textBox5.Text = dados[2]; // preenche server
                }
            }
        }

        private void SetupThreadHostBrowser()
        {
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            if (chromeHeadless)
            {
                options.AddArguments("headless");
            }

            Invoke((MethodInvoker)delegate {
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                button2.Enabled = false;
            });

            Browser = new Browser(driverService, options);
        }

        private void LoginBrowser() { 

            if (!Browser.Login(textBox3.Text, textBox4.Text, textBox5.Text))
            {
                Invoke((MethodInvoker)delegate {
                    Browser = null;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    button2.Enabled = true;
                });
                Logs("Login inválido");
            }
            else
            {
                Invoke((MethodInvoker)delegate {
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                    textBox5.Enabled = false;
                    button2.Enabled = true;
                });
                Logs("Login com sucesso");
                Dormir();                
                CarregaListasEForms();
            }
        }

        private void CarregaListasEForms()
        {
            Dormir();
            Logs(Browser.IrCentroAldeia());
            Dormir();
            Logs(Browser.EntrarEdificio(39));
            Dormir();
            Logs(Browser.MudarTabEdificio(4));
            Dormir();

            var NomeListasFarm = Browser.GetNomeListasFarm();            
            
            if (NomeListasFarm.Count == 0)
            {
                Logs("Nao Existem Listas");
                return;
            }

            Logs("Pronto para Farmar");

            semaforo = new Semaphore(1, 1);
            listasThreads = new Dictionary<Thread, bool>();
            listasButoes = new List<Button>();
            listasLabels = new List<Label>();
            listasProgress = new List<ProgressBar>();
            listasTempos = new List<TextBox>();
            listasFarm = new List<Lista>();
            int intialTop = 100;
            Label LabelNome;
            Button ButaoEnviar;
            ProgressBar BarraProgresso;
            TextBox TextBoxTempoMinimo;
            TextBox TextBoxTempoMaximo;
        
            for (int i = 0; i < NomeListasFarm.Count; i++)
            {
                LabelNome = new Label
                {
                    Top = intialTop,
                    Left = panel2.Left + 50,
                    AutoSize = true,
                    Text = NomeListasFarm[i],
                    ForeColor = Color.Red
                };
                listasLabels.Add(LabelNome);

                BarraProgresso = new ProgressBar
                {
                    Top = intialTop + 3,
                    Left = LabelNome.Left + 150,
                    Width = 200,
                    Height = 15
                };
                listasProgress.Add(BarraProgresso);

                TextBoxTempoMinimo = new TextBox
                {
                    Top = intialTop,
                    Left = BarraProgresso.Left + BarraProgresso.Width + 50,
                    Width = 40,
                    Height = 15,
                    Text = "2"
                };
                listasTempos.Add(TextBoxTempoMinimo);

                TextBoxTempoMaximo = new TextBox
                {
                    Top = intialTop,
                    Left = TextBoxTempoMinimo.Left + 50,
                    Width = 40,
                    Height = 15,
                    Text = "5"
                };
                listasTempos.Add(TextBoxTempoMaximo);

                ButaoEnviar = new Button
                {
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                    Width = 100,
                    Height = 25,
                    Top = intialTop,
                    Left = TextBoxTempoMaximo.Left + 75,
                    Text = "Farmar"
                };
                ButaoEnviar.Click += ButaoFarm_Click;
                listasButoes.Add(ButaoEnviar);

                listasFarm.Add(new Lista(NomeListasFarm[i], i, ButaoEnviar));

                Invoke((MethodInvoker)delegate
                {
                    panel2.Controls.Add(LabelNome);
                    panel2.Controls.Add(BarraProgresso);
                    panel2.Controls.Add(TextBoxTempoMinimo);
                    panel2.Controls.Add(TextBoxTempoMaximo);
                    panel2.Controls.Add(ButaoEnviar);
                });

                intialTop += 40;
            }
        }

        private void EnviaFarms(int index)
        {  
            while (true)
            {
                semaforo.WaitOne();
                Dormir();

                if (listasThreads[Thread.CurrentThread] == false) {
                    semaforo.Release(1);
                    return;
                }

                string FarmsEnviadosComSucesso = Browser.EnviaFarm(true, index);

                if (FarmsEnviadosComSucesso == "Morreu")
                {
                    Logs($"Algo correu mal a enviar a lista {listasFarm[index].Nome}. A reenviar");
                    Thread.Sleep(new Random().Next(tempoMinimoEntreCadaComando, tempoMaximoEntreCadaComando) * 1000);
                    semaforo.Release(1);
                    continue;
                }
                else if (FarmsEnviadosComSucesso.Length > 3)
                {
                    FarmsEnviadosComSucesso = "0";
                }
                Logs($"Foram enviados {FarmsEnviadosComSucesso} assaltos da lista {listasFarm[index].Nome}");
                semaforo.Release(1);

                if(TimeOutLista(index) == -1) {
                    return;
                }
            }
        }

        private int TimeOutLista(int index)
        {
            if (listasThreads[Thread.CurrentThread] == false) {
                return -1;
            }

            int tempoMinDelay = Int32.Parse(listasTempos[index * 2].Text);
            int tempoMaxDelay = Int32.Parse(listasTempos[index * 2 + 1].Text);
            int rand = new Random().Next(tempoMinDelay, tempoMaxDelay);

            Invoke((MethodInvoker)delegate
            {
                listasProgress[index].Maximum = rand * 60;
                listasProgress[index].Minimum = 0;
                listasProgress[index].Value = 0;
            });

            Logs("Lista " + listasFarm[index].Nome + " a dormir por " + rand.ToString() + " minutos");

            for (int j = 0; j < rand * 60; j++)
            {
                if (listasThreads[Thread.CurrentThread] == false) { listasThreads.Remove(Thread.CurrentThread); return -1; }
                Invoke((MethodInvoker)delegate
                {
                    listasProgress[index].Increment(1);
                });
                Thread.Sleep(1000);
            }
            return 1;
        }

        public void Logs(string log)
        {
            string data = DateTime.Now.ToString("dd/MM HH:mm:ss");
            Directory.CreateDirectory("Logs");

            Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Insert(0, $"{data} --> {log}");
            });

            EscreveLogsFicheiro(log, data);
        }

        
        public void EscreveLogsFicheiro(string log, string data)
        {
            string path = "Logs\\" + textBox3.Text + "_log.txt";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            using (var textWritter = File.AppendText(path))
            {
                textWritter.WriteLine($"{data} --> {log}");
            }
        }

        private void Dormir()
        {
            Thread.Sleep(new Random().Next(tempoMinimoEntreCadaComando, tempoMaximoEntreCadaComando) * 1000);
        }
    }
}
