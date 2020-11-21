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
    public partial class Inicio : Form
    {
        public Inicio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 10 entradas y 1 salida, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 3 entradas y 1 salida, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 2 entradas y 1 salida, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 2 entradas y 2 salida, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 3 entradas y 2 salidas, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 4 entradas y 1 salida, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string mensaje = "Este ejercicio costa de 3 entradas y 2 salidas, por favor tener en cuenta esto al momento de entrenar y simular la red...";
            Form1 Formulario = new Form1(mensaje);
            Formulario.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gracias por utilizar esta app desarrollada por estudiantes de la UPC...");
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
