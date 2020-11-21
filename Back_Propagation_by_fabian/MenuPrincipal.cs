using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Back_Propagation_by_fabian
{
    public partial class MenuPrincipal : Form
    {
        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Inicio2 inicio2 = new Inicio2();
            inicio2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Inicio inicio = new Inicio();
            inicio.Show();
        }
    }
}
