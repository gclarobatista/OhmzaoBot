using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainProject
{
    class Lista
    {
        public string Nome { get; set; }
        public int Index { get; set; }
        Browser Browser = null;
        public Label LabelNome;
        public Button ButaoEnviar;
        public ProgressBar BarraProgresso;
        public TextBox TextBoxTempoMinimo;
        public TextBox TextBoxTempoMaximo;
        public Thread ThreadLista;
        static int intialTop = 100;

        public Lista(string nome, int index, Browser b)
        {
            Nome = nome;
            Index = index;
            Browser = b;

            LabelNome = new Label();
            LabelNome.Top = intialTop;
            LabelNome.Left = 100;
            LabelNome.AutoSize = true;
            LabelNome.Text = Nome;
            LabelNome.ForeColor = Color.Red;

            BarraProgresso= new ProgressBar();
            BarraProgresso.Top = intialTop + 3;
            BarraProgresso.Left = LabelNome.Left + 150;
            BarraProgresso.Width = 200;
            BarraProgresso.Height = 15;

            TextBoxTempoMinimo = new TextBox();
            TextBoxTempoMinimo.Top = intialTop;
            TextBoxTempoMinimo.Left = BarraProgresso.Left + BarraProgresso.Width + 50;
            TextBoxTempoMinimo.Width = 40;
            TextBoxTempoMinimo.Height = 15;
            TextBoxTempoMinimo.Text = "2";

            TextBoxTempoMaximo = new TextBox();
            TextBoxTempoMaximo.Top = intialTop;
            TextBoxTempoMaximo.Left = TextBoxTempoMinimo.Left + 50;
            TextBoxTempoMaximo.Width = 40;
            TextBoxTempoMaximo.Height = 15;
            TextBoxTempoMaximo.Text = "5";

            ButaoEnviar = new Button();
            ButaoEnviar.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            ButaoEnviar.Width = 100;
            ButaoEnviar.Height = 25;
            ButaoEnviar.Top = intialTop;
            ButaoEnviar.Left = TextBoxTempoMaximo.Left + 75;
            ButaoEnviar.Text = "Farmar";
            ButaoEnviar.Click += CriaThreadFarmar;

            intialTop += 40;
        }
        public void CriaThreadFarmar(object sender, EventArgs e)
        {
            if (ButaoEnviar.Text == "Farmar")
            {
                //COMEÇAR A FARMAR
                ThreadLista = new Thread(EnviaFarms);
                ThreadLista.IsBackground = true;
                ThreadLista.Start();
                //Logs("Começou a farmar " + ListasFarm[index].Nome);
                ButaoEnviar.Text = "Parar";
                ButaoEnviar.ForeColor = Color.Green;
            }

            else
            {
                //PARAR DE FARMAR
                ThreadLista.Abort();
                ThreadLista.Value = 0;
                Logs("Parou de farmar lista" + ListasFarm[index].Nome);
                this.Invoke((MethodInvoker)delegate
                {
                    ListasButoes[index].Text = "Farmar";
                    ListasLabels[index].ForeColor = Color.Red;
                });
            }*/
        }
        public void EnviaFarms()
        {
            while (true)
            {
                //string FarmsEnviadosComSucesso = Browser.EnviaFarm(true, i);
                //if (FarmsEnviadosComSucesso.Length > 3) FarmsEnviadosComSucesso = "0";
                //Logs("Foram enviados " + FarmsEnviadosComSucesso + " assaltos da lista " + ListasFarm[i].Nome);
                Thread.Sleep(new Random().Next(2000, 5000));
                //semaforo.Release(1);
                //TimeOutLista(i);
            }
        }
    }
}
