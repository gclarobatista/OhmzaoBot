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
        public bool EvitarPerdas { get; set; }
        public Thread ThreadFarm { get; set; }
        public Button ButaoFarm;

        public Lista(string nome, int index, Button butaoFarm)
        {
            Nome = nome;
            Index = index;
            ButaoFarm = butaoFarm;
        }
    }
}
