using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Back_Propagation_by_fabian
{
    public partial class Perceptron_Multicapa : Form
    {
        public Perceptron_Multicapa()
        {
            InitializeComponent();
            this.nc1.Visible = false;
            this.nc2.Visible = false;
            this.nc3.Visible = false;
            this.lnc1.Visible = false;
            this.lnc2.Visible = false;
            this.lnc3.Visible = false;
            this.fac1.Visible = false;
            this.fac2.Visible = false;
            this.fac3.Visible = false;
        }

        public Perceptron_Multicapa(string mensaje)
        {
            InitializeComponent();
            this.nc1.Visible = false;
            this.nc2.Visible = false;
            this.nc3.Visible = false;
            this.lnc1.Visible = false;
            this.lnc2.Visible = false;
            this.lnc3.Visible = false;
            this.fac1.Visible = false;
            this.fac2.Visible = false;
            this.fac3.Visible = false;
            MessageBox.Show(mensaje);
        }
        static List<double[]> input;
        static List<double[]> output; 

        public string nombre_archivo;

        static int inputCount;
        static int outputCount;

        static double inputMax;
        static double inputMin;

        static double outputMax;
        static double outputMin;

        static string cadena;

        static bool loadNetwork = false;
        static bool saveNetwork = true;

        public double error_maximo;
        public int iteraciones;
        public double rata;

        public int nec1 = 0;
        public int nec2 = 0;
        public int nec3 = 0;

        static string dataPath;
        static string neuronPath = @"C:\Back_Propagation_by_fabian\resultados\datos_entrenamiento_perceptron.bin";

        //funcion para setear valores ingresados por el usuario
        private bool Setearvalores()
        {
            try
            {
                inputCount = Convert.ToInt32(this.textBox1.Text);
                outputCount = Convert.ToInt32(this.textBox2.Text);
                inputMax = Convert.ToDouble(this.textBox3.Text);
                inputMin = Convert.ToDouble(this.textBox4.Text);
                outputMax = Convert.ToDouble(this.textBox5.Text);
                outputMin = Convert.ToDouble(this.textBox6.Text);
                error_maximo = Convert.ToDouble(this.errror_maximo.Text);
                iteraciones = Convert.ToInt32(this.numero_iteraciones.Text);
                nec1 = Convert.ToInt32(this.nc1.Text);
                nec2 = Convert.ToInt32(this.nc2.Text);
                nec3 = Convert.ToInt32(this.nc3.Text);
                rata = Convert.ToDouble(this.text_rata.Text);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + "ingrese todos los valores de configuracion de la red");
                return false;
            }

        }

        //funcion para mapear datos de entrada y salida
        static bool ReadData()
        {
            //inicilizo la lista de entradas y salidas
            input =  new List<double[]>();
            output = new List<double[]>();

            try
            {
                string data = System.IO.File.ReadAllText(dataPath).Replace("\r", "");

                string[] row = data.Split(Environment.NewLine.ToCharArray());
                for (int i = 0; i < row.Length; i++)
                {
                    string[] rowData = row[i].Split(';');

                    double[] inputs = new double[inputCount];
                    double[] outputs = new double[outputCount];

                    for (int j = 0; j < rowData.Length; j++)
                    {
                        if (j < inputCount)
                        {
                            inputs[j] = Normalize(double.Parse(rowData[j]), inputMin, inputMax);
                        }
                        else
                        {
                            outputs[j - inputCount] = Normalize(double.Parse(rowData[j]), outputMin, outputMax);
                        }
                    }

                    input.Add(inputs);
                    output.Add(outputs);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("No ha cargado los datos","Atencion!");
                return false;
            }

        }

        //funciones para la normalizacion e invertirla
        static double Normalize(double value, double min, double max)
        {
            double valuen = (value - min) / (max - min);
            return valuen;
        }
        static double InverseNormalize(double value, double min, double max)
        {
            return value * (max - min) + min;
        }

        //metodo para obtener pesos aleatorios
        public double[,] llenar_pesos_aleatorios(double[,] matriz)
        {
            Random aleatorio = new Random();
            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    matriz[i, j] = aleatorio.NextDouble();
                }
            }

            return matriz;
        }

        //metodo para obtener umbrales aleatorios
        public double[] llenar_umbrales_aleatorios(double[] vector)
        {
            Random aleatorio = new Random();
            for (int i = 0; i < vector.GetLength(0); i++)
            {
                vector[i] = aleatorio.NextDouble();
            }
            return vector;
        }

        //funcion que utilizo para evaluar la red
        public void entrenar_red()
        {
            //limpio la grafica
            this.chart1.Series["Error"].Points.Clear();
            if (!loadNetwork)
            {
                if (ReadData() == true)
                {
                    //verifico el numero de capas que escogio el usuario
                    //3 capas
                    if (nec1 != 0 && nec2 != 0 && nec3 != 0)
                    {
                        // inicializo pesos y umbrales capa oculta 1
                        double[,] w = new double[inputCount, nec1];
                        double[] u = new double[nec1];
                        w = llenar_pesos_aleatorios(w);
                        u = llenar_umbrales_aleatorios(u);

                        // inicializo pesos y umbrales capa oculta 2
                        double[,] w2 = new double[nec1, nec2];
                        double[] u2 = new double[nec2];

                        w2 = llenar_pesos_aleatorios(w2);
                        u2 = llenar_umbrales_aleatorios(u2);

                        // inicializo pesos y umbrales capa oculta 3
                        double[,] w3 = new double[nec2, nec3];
                        double[] u3 = new double[nec3];

                        w3 = llenar_pesos_aleatorios(w3);
                        u3 = llenar_umbrales_aleatorios(u3);

                        // inicializo pesos y umbrales capa salida
                        double[,] w4 = new double[nec3, outputCount];
                        double[] u4 = new double[outputCount];

                        w4 = llenar_pesos_aleatorios(w4);
                        u4 = llenar_umbrales_aleatorios(u4);

                        //vectores de salidas
                        double[] salidas = new double[nec1];
                        double[] salidas2 = new double[nec2];
                        double[] salidas3 = new double[nec3];
                        double[] salidas4 = new double[outputCount];

                        int iteracion = 0;
                        double RMS = 0;

                        //hago las iteraciones que sean necesarias
                        List<string> log = new List<string>();
                        while (iteracion < iteraciones)
                        {
                            double ETP = 0;
                            int patrones = 0;
                            foreach (var patron in input)
                            {
                                //calculo la salida de la capa de entrada a capa oculta 1
                                for (int i = 0; i < nec1; i++)
                                {
                                    double sumatoria = 0;
                                    for (int j = 0; j < inputCount; j++)
                                    {
                                        sumatoria = sumatoria + (patron[j] * w[j, i]);
                                    }
                                    sumatoria = sumatoria - u[i];
                                    //verifico la funcion de activacion que escogio el usuario
                                    switch (fac1.SelectedItem.ToString().Trim())
                                    {
                                        case "Sigmiode":
                                            salidas[i] = 1 / (1 + Math.Exp(-sumatoria));
                                            break;
                                        case "Tangente H":
                                            salidas[i] = Math.Tanh(sumatoria);
                                            break;
                                        case "Sinusoidal":
                                            salidas[i] = Math.Sin(sumatoria);
                                            break;
                                    }
                                }

                                //calculo la salida de la capa 1  a capa oculta 2
                                for (int i = 0; i < nec2; i++)
                                {
                                    double sumatoria = 0;
                                    for (int j = 0; j < nec1; j++)
                                    {
                                        sumatoria = sumatoria + (salidas[j] * w2[j, i]);
                                    }
                                    sumatoria = sumatoria - u2[i];
                                    //verifico la funcion de activacion que escogio el usuario
                                    switch (fac2.SelectedItem.ToString().Trim())
                                    {
                                        case "Sigmiode":
                                            salidas2[i] = 1 / (1 + Math.Exp(-sumatoria));
                                            break;
                                        case "Tangente H":
                                            salidas2[i] = Math.Tanh(sumatoria);
                                            break;
                                        case "Sinusoidal":
                                            salidas2[i] = Math.Sin(sumatoria);
                                            break;
                                    }
                                }

                                //calculo la salida de la capa 2  a capa oculta 3
                                for (int i = 0; i < nec3; i++)
                                {
                                    double sumatoria = 0;
                                    for (int j = 0; j < nec2; j++)
                                    {
                                        sumatoria = sumatoria + (salidas2[j] * w3[j, i]);
                                    }
                                    sumatoria = sumatoria - u3[i];
                                    //verifico la funcion de activacion que escogio el usuario
                                    switch (fac3.SelectedItem.ToString().Trim())
                                    {
                                        case "Sigmiode":
                                            salidas3[i] = 1 / (1 + Math.Exp(-sumatoria));
                                            break;
                                        case "Tangente H":
                                            salidas3[i] = Math.Tanh(sumatoria);
                                            break;
                                        case "Sinusoidal":
                                            salidas3[i] = Math.Sin(sumatoria);
                                            break;
                                    }
                                }

                                //calcular salidas de la capa 2 a la capa de salida
                                for (int i = 0; i < outputCount; i++)
                                {
                                    double sumatoria = 0;
                                    for (int j = 0; j < nec3; j++)
                                    {
                                        sumatoria = sumatoria + (salidas3[j] * w4[j, i]);
                                    }
                                    sumatoria = sumatoria - u4[i];
                                    //verifico la funcion de activacion que escogio el usuario
                                    switch (fas.SelectedItem.ToString().Trim())
                                    {
                                        case "Sigmiode":
                                            salidas4[i] = 1 / (1 + Math.Exp(-sumatoria));
                                            break;
                                        case "Tangente H":
                                            salidas4[i] = Math.Tanh(sumatoria);
                                            break;
                                        case "Sinusoidal":
                                            salidas4[i] = Math.Sin(sumatoria);
                                            break;
                                    }
                                }
                                //calcular valor error lineal y error del patron                                
                                double [] Elineal = new double[outputCount];                               
                                double[] salida_patron = output[patrones];

                                for (int i = 0; i < outputCount; i++)
                                {
                                    double yr = salidas4[i];
                                    double yd = salida_patron[i];
                                    Elineal[i] = (yd - yr);                                 
                                }

                                double Ep = 0;
                                for (int i = 0; i<outputCount;i++)
                                {
                                    Ep = Ep + Math.Abs(Elineal[i]);
                                }
                                Ep = Ep / outputCount;

                                //actualizacion de pesos

                                for (int i = 0; i < inputCount; i++)
                                {
                                    for (int j = 0; j < nec1; j++)
                                    {
                                        w[i, j] = w[i, j] + rata * Ep * patron[i];
                                    }
                                }


                                for (int i = 0; i < nec1; i++)
                                {
                                    for (int j = 0; j < nec2; j++)
                                    {
                                        w2[i, j] = w2[i, j] + rata * Ep * salidas[i];
                                    }
                                }

                                for (int i = 0; i < nec2; i++)
                                {
                                    for (int j = 0; j < nec3; j++)
                                    {
                                        w3[i, j] = w3[i, j] + rata * Ep * salidas2[i];
                                    }
                                }

                                for (int i = 0; i < nec3; i++)
                                {
                                    for (int j = 0; j < outputCount; j++)
                                    {
                                        w4[i, j] = w4[i, j] + rata * Elineal[j] * salidas3[i];
                                    }
                                }
                                //actualizar umbrales
                                //actualizacion de los humbrales

                                for (int i = 0; i < nec1; i++)
                                {
                                    u[i] = u[i] + rata * Ep * 1;
                                }

                                for (int i = 0; i < nec2; i++)
                                {
                                    u2[i] = u2[i] + rata * Ep * 1;
                                }

                                for (int i = 0; i < nec3; i++)
                                {
                                    u3[i] = u3[i] + rata * Ep * 1;
                                }

                                for (int i = 0; i < outputCount; i++)
                                {
                                    u4[i] = u4[i] + rata * Elineal[i] * 1;
                                }
                                //error total del patron
                                ETP = ETP + Ep;
                                patrones++;
                            }

                            RMS = ETP / patrones;
                            log.Add(RMS.ToString());

                            //verifico si el error es menor al error maximo peromitido
                            if (RMS <= error_maximo)
                            {
                                MessageBox.Show("Error RMS menor a Error maximo permitido");
                                //guardar pesos optimos en archivo plano
                                string ruta1 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa1.txt";
                                string ruta2 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa2.txt";
                                string ruta3 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa3.txt";
                                string ruta4 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capasalida.txt";
                                //se crea el archivo
                                using (StreamWriter mylogs = File.AppendText(ruta1))
                                {
                                    for (int i = 0; i < inputCount; i++)
                                    {
                                        string texto = "";
                                        for (int j = 0; j < nec1; j++)
                                        {
                                            if (j == (nec1 - 1))
                                            {
                                                texto = texto + w[i, j];
                                            }
                                            else
                                            {
                                                texto = texto + w[i, j] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                    }
                                    mylogs.Close();
                                }
                                using (StreamWriter mylogs = File.AppendText(ruta2))
                                {
                                    for (int i = 0; i < nec1; i++)
                                    {
                                        string texto = "";
                                        for (int j = 0; j < nec2; j++)
                                        {
                                            if (j == (nec2 - 1))
                                            {
                                                texto = texto + w2[i, j];
                                            }
                                            else
                                            {
                                                texto = texto + w2[i, j] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                    }
                                    mylogs.Close();
                                }
                                using (StreamWriter mylogs = File.AppendText(ruta3))
                                {
                                    for (int i = 0; i < nec2; i++)
                                    {
                                        string texto = "";
                                        for (int j = 0; j < nec3; j++)
                                        {
                                            if (j == (nec3 - 1))
                                            {
                                                texto = texto + w3[i, j];
                                            }
                                            else
                                            {
                                                texto = texto + w3[i, j] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                    }
                                    mylogs.Close();
                                }
                                using (StreamWriter mylogs = File.AppendText(ruta4))
                                {
                                    for (int i = 0; i < nec3; i++)
                                    {
                                        string texto = "";
                                        for (int j = 0; j < outputCount; j++)
                                        {
                                            if (j == (outputCount - 1))
                                            {
                                                texto = texto + w4[i, j];
                                            }
                                            else
                                            {
                                                texto = texto + w4[i, j] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                    }
                                    mylogs.Close();
                                }
                                //guardar umbral optimos en archivo plano
                                string ruta11 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa1.txt";
                                string ruta12 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa2.txt";
                                string ruta13 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa3.txt";
                                string ruta14 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capasalida.txt";
                                //se crea el archivo
                                using (StreamWriter mylogs = File.AppendText(ruta11))
                                {
                                    string texto = "";
                                    for (int i = 0; i < nec1; i++)
                                    {
                                        if (i == (nec1 - 1))
                                        {
                                            texto = texto + u[i];
                                        }
                                        else
                                        {
                                            texto = texto + u[i] + ";";
                                        }
                                    }
                                    mylogs.WriteLine(texto);
                                    mylogs.Close();
                                }
                                using (StreamWriter mylogs = File.AppendText(ruta12))
                                {
                                    string texto = "";
                                    for (int i = 0; i < nec2; i++)
                                    {
                                        if (i == (nec2 - 1))
                                        {
                                            texto = texto + u2[i];
                                        }
                                        else
                                        {
                                            texto = texto + u2[i] + ";";
                                        }
                                    }
                                    mylogs.WriteLine(texto);
                                    mylogs.Close();
                                }
                                using (StreamWriter mylogs = File.AppendText(ruta13))
                                {
                                    string texto = "";
                                    for (int i = 0; i < nec3; i++)
                                    {
                                        if (i == (nec3 - 1))
                                        {
                                            texto = texto + u3[i];
                                        }
                                        else
                                        {
                                            texto = texto + u3[i] + ";";
                                        }
                                    }
                                    mylogs.WriteLine(texto);
                                    mylogs.Close();
                                }
                                using (StreamWriter mylogs = File.AppendText(ruta14))
                                {
                                    string texto = "";
                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        if (i == (outputCount - 1))
                                        {
                                            texto = texto + u4[i];
                                        }
                                        else
                                        {
                                            texto = texto + u4[i] + ";";
                                        }
                                    }
                                    mylogs.WriteLine(texto);
                                    mylogs.Close();
                                }
                                break;
                            }
                            else
                            {
                                iteracion = iteracion + 1;
                            }
                        }
                        System.IO.File.Delete(@"C:\LogTailPerceptron.txt");
                        System.IO.File.WriteAllLines(@"C:\LogTailPerceptron.txt", log.ToArray());
                    }
                    else
                    {
                        /////////////////////////////2 capas////////////////////////////////////////////////
                        if (nec1 != 0 && nec2 != 0)
                        {
                            // inicializo pesos y umbrales capa oculta 1
                            double[,] w = new double[inputCount, nec1];
                            double[] u = new double[nec1];
                            w = llenar_pesos_aleatorios(w);
                            u = llenar_umbrales_aleatorios(u);



                            // inicializo pesos y umbrales capa oculta 2
                            double[,] w2 = new double[nec1, nec2];
                            double[] u2 = new double[nec2];

                            w2 = llenar_pesos_aleatorios(w2);
                            u2 = llenar_umbrales_aleatorios(u2);


                            // inicializo pesos y umbrales capa salida
                            double[,] w3 = new double[nec2, outputCount];
                            double[] u3 = new double[outputCount];

                            w3 = llenar_pesos_aleatorios(w3);
                            u3 = llenar_umbrales_aleatorios(u3);

                            //vectores de salidas
                            double[] salidas = new double[nec1];
                            double[] salidas2 = new double[nec2];
                            double[] salidas3 = new double[outputCount];

                            int iteracion = 0;
                            double RMS = 0;

                            //hago las iteraciones que sean necesarias
                            List<string> log = new List<string>(); ;
                            while (iteracion < iteraciones)
                            {
                                double ETP = 0;
                                int patrones = 0;
                                foreach (var patron in input)
                                {
                                    //calculo la salida de la capa de entrada a capa oculta 1
                                    for (int i = 0; i < nec1; i++)
                                    {
                                        double sumatoria = 0;
                                        for (int j = 0; j < inputCount; j++)
                                        {
                                            sumatoria = sumatoria + (patron[j] * w[j, i]);
                                        }
                                        sumatoria = sumatoria - u[i];
                                        //verifico la funcion de activacion que escogio el usuario
                                        switch (fac1.SelectedItem.ToString().Trim())
                                        {
                                            case "Sigmiode":
                                                salidas[i] = 1 / (1 + Math.Exp(-sumatoria));
                                                break;
                                            case "Tangente H":
                                                salidas[i] = Math.Tanh(sumatoria);
                                                break;
                                            case "Sinusoidal":
                                                salidas[i] = Math.Sin(sumatoria);
                                                break;
                                        }
                                    }

                                    //calculo la salida de la capa de entrada a capa oculta 1
                                    for (int i = 0; i < nec2; i++)
                                    {
                                        double sumatoria = 0;
                                        for (int j = 0; j < nec1; j++)
                                        {
                                            sumatoria = sumatoria + (salidas[j] * w2[j, i]);
                                        }
                                        sumatoria = sumatoria - u2[i];
                                        //verifico la funcion de activacion que escogio el usuario
                                        switch (fac2.SelectedItem.ToString().Trim())
                                        {
                                            case "Sigmiode":
                                                salidas2[i] = 1 / (1 + Math.Exp(-sumatoria));
                                                break;
                                            case "Tangente H":
                                                salidas2[i] = Math.Tanh(sumatoria);
                                                break;
                                            case "Sinusoidal":
                                                salidas2[i] = Math.Sin(sumatoria);
                                                break;
                                        }
                                    }

                                    //calcular salidas de la capa 2 a la capa de salida
                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        double sumatoria = 0;
                                        for (int j = 0; j < nec2; j++)
                                        {
                                            sumatoria = sumatoria + (salidas2[j] * w3[j, i]);
                                        }
                                        sumatoria = sumatoria - u3[i];
                                        //verifico la funcion de activacion que escogio el usuario
                                        switch (fas.SelectedItem.ToString().Trim())
                                        {
                                            case "Sigmiode":
                                                salidas3[i] = 1 / (1 + Math.Exp(-sumatoria));
                                                break;
                                            case "Tangente H":
                                                salidas3[i] = Math.Tanh(sumatoria);
                                                break;
                                            case "Sinusoidal":
                                                salidas3[i] = Math.Sin(sumatoria);
                                                break;
                                        }
                                    }
                                    //calcular valor error lineal y error del patron                                
                                    double [] Elineal = new double[outputCount];                                  
                                    double[] salida_patron = output[patrones];

                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        double yr = salidas3[i];
                                        double yd = salida_patron[i];
                                        Elineal[i] =  (yd - yr);     
                                    }

                                    double Ep = 0;
                                    for (int i = 0; i<outputCount;i++)
                                    {
                                        Ep = Ep + Math.Abs(Elineal[i]);
                                    }
                                    Ep = Ep / outputCount;
                               
                                   //actualizacion de pesos

                                    for (int i = 0; i < inputCount; i++)
                                    {
                                        for (int j = 0; j < nec1; j++)
                                        {
                                            w[i, j] = w[i, j] + rata * Ep * patron[i];
                                        }
                                    }


                                    for (int i = 0; i < nec1; i++)
                                    {
                                        for (int j = 0; j < nec2; j++)
                                        {
                                            w2[i, j] = w2[i, j] + rata * Ep * salidas[i];
                                        }
                                    }

                                    for (int i = 0; i < nec2; i++)
                                    {
                                        for (int j = 0; j < outputCount; j++)
                                        {
                                            w3[i, j] = w3[i, j] + rata * Elineal[j] * salidas2[i];
                                        }
                                    }
                                    //actualizar umbrales
                                    //actualizacion de los humbrales

                                    for (int i = 0; i < nec1; i++)
                                    {
                                        u[i] = u[i] + rata * Ep * 1;
                                    }

                                    for (int i = 0; i < nec2; i++)
                                    {
                                        u2[i] = u2[i] + rata * Ep * 1;
                                    }

                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        u3[i] = u3[i] + rata * Elineal[i] * 1;
                                    }

                                    //error total del patron
                                    ETP = ETP + Ep;
                                    patrones++;
                                }
                                RMS = ETP / patrones;
                                log.Add(RMS.ToString());


                                if (RMS <= error_maximo)
                                {
                                    MessageBox.Show("Error RMS menor a Error maximo permitido");
                                    //guardar pesos optimos en archivo plano
                                    string ruta1 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa1.txt";
                                    string ruta2 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa2.txt";
                                    string ruta3 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capasalida.txt";
                                    //se crea el archivo
                                    using (StreamWriter mylogs = File.AppendText(ruta1))
                                    {
                                        for (int i = 0; i < inputCount; i++)
                                        {
                                            string texto = "";
                                            for (int j = 0; j < nec1; j++)
                                            {
                                                if (j == (nec1 - 1))
                                                {
                                                    texto = texto + w[i, j];
                                                }
                                                else
                                                {
                                                    texto = texto + w[i, j] + ";";
                                                }
                                            }
                                            mylogs.WriteLine(texto);
                                        }
                                        mylogs.Close();
                                    }
                                    using (StreamWriter mylogs = File.AppendText(ruta2))
                                    {
                                        for (int i = 0; i < nec1; i++)
                                        {
                                            string texto = "";
                                            for (int j = 0; j < nec2; j++)
                                            {
                                                if (j == (nec2 - 1))
                                                {
                                                    texto = texto + w2[i, j];
                                                }
                                                else
                                                {
                                                    texto = texto + w2[i, j] + ";";
                                                }
                                            }
                                            mylogs.WriteLine(texto);
                                        }
                                        mylogs.Close();
                                    }
                                    using (StreamWriter mylogs = File.AppendText(ruta3))
                                    {
                                        for (int i = 0; i < nec2; i++)
                                        {
                                            string texto = "";
                                            for (int j = 0; j < outputCount; j++)
                                            {
                                                if (j == (outputCount - 1))
                                                {
                                                    texto = texto + w3[i, j];
                                                }
                                                else
                                                {
                                                    texto = texto + w3[i, j] + ";";
                                                }
                                            }
                                            mylogs.WriteLine(texto);
                                        }
                                        mylogs.Close();
                                    }
                                    //guardar umbral optimos en archivo plano
                                    string ruta11 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa1.txt";
                                    string ruta12 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa2.txt";
                                    string ruta13 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capasalida.txt";
                                    //se crea el archivo
                                    using (StreamWriter mylogs = File.AppendText(ruta11))
                                    {
                                        string texto = "";
                                        for (int i = 0; i < nec1; i++)
                                        {
                                            if (i == (nec1 - 1))
                                            {
                                                texto = texto + u[i];
                                            }
                                            else
                                            {
                                                texto = texto + u[i] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                        mylogs.Close();
                                    }
                                    using (StreamWriter mylogs = File.AppendText(ruta12))
                                    {
                                        string texto = "";
                                        for (int i = 0; i < nec2; i++)
                                        {
                                            if (i == (nec2 - 1))
                                            {
                                                texto = texto + u2[i];
                                            }
                                            else
                                            {
                                                texto = texto + u2[i] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                        mylogs.Close();
                                    }
                                    using (StreamWriter mylogs = File.AppendText(ruta13))
                                    {
                                        string texto = "";
                                        for (int i = 0; i < outputCount; i++)
                                        {
                                            if (i == (outputCount - 1))
                                            {
                                                texto = texto + u3[i];
                                            }
                                            else
                                            {
                                                texto = texto + u3[i] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                        mylogs.Close();
                                    }
                                    break;
                                }
                                else
                                {
                                    iteracion = iteracion + 1;
                                }
                            }
                            System.IO.File.Delete(@"C:\LogTailPerceptron.txt");
                            System.IO.File.WriteAllLines(@"C:\LogTailPerceptron.txt", log.ToArray());
                        }
                        ///////////////////////////1 capa///////////////////////////
                        else
                        {
                            
                            // inicializo pesos y umbrales capa de entrada a capa1
                            double[,] w = new double[inputCount, nec1];
                            double[] u = new double[nec1];
                            w = llenar_pesos_aleatorios(w);
                            u = llenar_umbrales_aleatorios(u);

                           

                            // inicializo pesos y umbrales capa de salida
                            double[,] w2 = new double[nec1, outputCount];
                            double[] u2 = new double[outputCount];

                            w2 = llenar_pesos_aleatorios(w2);
                            u2 = llenar_umbrales_aleatorios(u2);

                            //vectores de salidas
                            double[] salidas = new double[nec1];
                            double[] salidas2 = new double[outputCount];

                            int iteracion = 0;
                            double RMS = 0;

                            //hago las iteraciones que sean necesarias
                            List<string> log  = new List<string>(); ;
                            while (iteracion <iteraciones)
                            {
                                double ETP = 0;
                                int patrones = 0;
                                foreach (var patron in input)
                                {
                                    //calculo la salida de la capa de entrada a capa oculta 1
                                    for (int i = 0; i < nec1; i++)
                                    {
                                        double sumatoria = 0;
                                        for (int j = 0; j < inputCount; j++)
                                        {
                                            sumatoria = sumatoria + (patron[j] * w[j, i]);
                                        }
                                        sumatoria = sumatoria - u[i];
                                        //verifico la funcion de activacion que escogio el usuario
                                        switch (fac1.SelectedItem.ToString().Trim())
                                        {
                                            case "Sigmiode":
                                                salidas[i] = 1 / (1 + Math.Exp(-sumatoria));
                                                break;
                                            case "Tangente H":
                                                salidas[i] = Math.Tanh(sumatoria);
                                                break;
                                            case "Sinusoidal":
                                                salidas[i] = Math.Sin(sumatoria);
                                                break;
                                        }
                                    }

                                    //calcular salidas de la capa 1 a la capa de salida
                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        double sumatoria = 0;
                                        for (int j = 0; j < nec1; j++)
                                        {
                                            sumatoria = sumatoria + (salidas[j] * w2[j, i]);
                                        }
                                        sumatoria = sumatoria - u2[i];
                                        //verifico la funcion de activacion que escogio el usuario
                                        switch (fac1.SelectedItem.ToString().Trim())
                                        {
                                            case "Sigmiode":
                                                salidas2[i] = 1 / (1 + Math.Exp(-sumatoria));
                                                break;
                                            case "Tangente H":
                                                salidas2[i] = Math.Tanh(sumatoria);
                                                break;
                                            case "Sinusoidal":
                                                salidas2[i] = Math.Sin(sumatoria);
                                                break;
                                        }
                                    }
                                    //calcular valor error lineal y error del patron                                
                                    double [] Elineal = new double[outputCount];                               
                                    double[] salida_patron = output[patrones];

                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        double yr = salidas2[i];
                                        double yd = salida_patron[i];
                                        Elineal[i] = (yd - yr);
                                    }

                                    double Ep = 0;
                                    for (int i = 0; i< outputCount; i++)
                                    {
                                        Ep = Ep + Math.Abs(Elineal[i]);
                                    }

                                    Ep = Ep / outputCount;
                                    ETP = ETP + Ep;

                                    //actualizacion de pesos

                                    for (int i = 0; i < inputCount; i++)
                                    {
                                        for (int j = 0; j < nec1; j++)
                                        {
                                            w[i, j] = w[i, j] + rata * Ep * patron[i];
                                        }
                                    }


                                    for (int i = 0; i < nec1; i++)
                                    {
                                        for (int j = 0; j < outputCount; j++)
                                        {
                                            w2[i, j] = w2[i, j] + rata * Elineal[j] * salidas[i];
                                        }
                                    }

                                    //actualizacion de los humbrales

                                    for (int i  = 0; i< nec1;i++)
                                    { 
                                        u[i] = u[i] + rata * Ep * 1;
                                    }

                                    for (int i = 0; i < outputCount; i++)
                                    {
                                        u2[i] = u2[i] + rata * Elineal[i] * 1;
                                    }

                                    patrones++;
                                }

                                RMS = ETP/patrones;
                                log.Add(RMS.ToString());
                               

                                if(RMS <= error_maximo)
                                {
                                    MessageBox.Show("Error RMS menor a Error maximo permitido");
                                    //guardar pesos optimos en archivo plano
                                    string ruta1 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa1.txt";
                                    string ruta2 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capasalida.txt";
                                    //se crea el archivo
                                    using (StreamWriter mylogs = File.AppendText(ruta1))               
                                    {
                                        for (int i = 0; i < inputCount; i++)
                                        {
                                            string texto = "";
                                            for (int j = 0; j < nec1; j++)
                                            {
                                                if (j == (nec1-1))
                                                {
                                                    texto = texto + w[i, j];
                                                }
                                                else
                                                {
                                                   texto = texto + w[i, j] + ";";
                                                }
                                            }
                                            mylogs.WriteLine(texto);
                                        }
                                        mylogs.Close();
                                    }
                                    using (StreamWriter mylogs = File.AppendText(ruta2))
                                    {
                                        for (int i = 0; i < nec1; i++)
                                        {
                                            string texto = "";
                                            for (int j = 0; j < outputCount; j++)
                                            {
                                                if (j == (outputCount-1))
                                                {
                                                    texto = texto + w2[i, j];
                                                }
                                                else
                                                {
                                                    texto = texto + w2[i, j] + ";";
                                                }
                                            }
                                            mylogs.WriteLine(texto);
                                        }
                                        mylogs.Close();
                                    }
                                    //guardar umbral optimos en archivo plano
                                    string ruta11 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa1.txt";
                                    string ruta12 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capasalida.txt";
                                    //se crea el archivo
                                    using (StreamWriter mylogs = File.AppendText(ruta11))
                                    {
                                        string texto = "";
                                        for (int i = 0; i < nec1; i++)
                                        {
                                            if (i == (nec1-1))
                                            {
                                                texto = texto + u[i];
                                            }
                                            else
                                            {
                                                texto = texto + u[i] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                        mylogs.Close();
                                    }
                                    using (StreamWriter mylogs = File.AppendText(ruta12))
                                    {
                                        string texto = "";
                                        for (int i = 0; i < outputCount; i++)
                                        {
                                            if(i == (outputCount - 1))
                                            {
                                                texto = texto + u2[i];
                                            }
                                            else
                                            {
                                                texto = texto + u2[i] + ";";
                                            }
                                        }
                                        mylogs.WriteLine(texto);
                                        mylogs.Close();
                                    }
                                    break;                     
                                }
                                else
                                {
                                    iteracion = iteracion + 1;
                                }
                            }

                            System.IO.File.Delete(@"C:\LogTailPerceptron.txt");
                            System.IO.File.WriteAllLines(@"C:\LogTailPerceptron.txt", log.ToArray());
                           
                        }
                    }

                   

                    //graficar 
                    string line;
                    int contador = 1;
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\LogTailPerceptron.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        /////////////////////////////////////////////////////////////
                        double err = Math.Round(Convert.ToDouble(line),2);
                        labelerror.Text = err+"";
                        labeliteracion.Text = contador + "";
                        this.chart1.Series["Error"].Color = Color.Red;
                        this.chart1.Series["Error"].Points.AddXY(contador, Convert.ToDouble(line));
                        //////////////////////////////////////////////////////////////


                        if (contador % 150 == 0)
                        {
                            int times = 1;
                            while (times > 0)
                            {
                                Application.DoEvents();
                                Thread.Sleep(2);
                                times--;
                            }
                        }

                        contador = contador + 1;
                    }
                    //verifico si la red fue entrenada con exito
                    if (contador == iteraciones + 1)
                    {
                        string message = "-----------llego al numero maximo de iteraciones------------";
                        string caption = "Atencion";
                        MessageBox.Show(message, caption);
                    }
                    else
                    {
                        string message = "-----------Red entrenada con exito------------";
                        string caption = "Atencion";
                        MessageBox.Show(message, caption);
                    }

                    file.Close();
                    //finalizar graficar            
                }
            }
        }
        //variables que se llenan automaticamente
        public double[,] w;
        public double[,] w2;
        public double[,] w3;
        public double[,] w4;

        public double[] u;
        public double[] u2;
        public double[] u3;
        public double[] u4;
        //metodo simular red
        public void simular_red()
        {
          
            switch (comboBox1.SelectedItem.ToString().Trim())
            {
                case "1":
                    string ruta1 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa1.txt";
                    string ruta2 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capasalida.txt";
                    string ruta11 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa1.txt";
                    string ruta12 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capasalida.txt";
                    try
                    {
                        //leo el primer archivo
                        int filas;
                        using (StreamReader r = new StreamReader(ruta1))
                        {
                            var file = r.ReadToEnd();
                            var lines = file.Split(new char[] { '\n' });
                            filas = lines.Count();
                        }
                       
                        StreamReader objReader = new StreamReader(ruta1);
                        string linea = "";
                        int fila = 0;
                        do
                        {
                            linea = objReader.ReadLine();
                            if (fila == 0)
                            {
                                string[] arreglo = linea.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w = new double[filas, columnas];
                            }              
                            if ((linea != null))
                            {
                                string[] arreglo = linea.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w[fila, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila += 1;
                            }
                        } while (!(linea == null));
                        objReader.Close();
                        //leo el segundo archivo
                        int filas2;
                        using (StreamReader r = new StreamReader(ruta2))
                        {
                            var file2 = r.ReadToEnd();
                            var lines2 = file2.Split(new char[] { '\n' });
                            filas2 = lines2.Count();
                        }
                       
                        StreamReader objReader2 = new StreamReader(ruta2);
                        string linea2 = "";
                        int fila2 = 0;
                        do
                        {
                            linea2 = objReader2.ReadLine();
                            if (fila2 == 0)
                            {
                                string[] arreglo = linea2.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w2 = new double[filas2, columnas];
                            }
                            if ((linea2 != null))
                            {
                                string[] arreglo = linea2.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w2[fila2, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila2 += 1;
                            }
                        } while (!(linea2 == null));
                        objReader2.Close();
                        //leo el tercer archivo
                        StreamReader objReader3= new StreamReader(ruta11);
                        string linea3 = "";             
                        do
                        {
                            linea3 = objReader3.ReadLine();
                            if ((linea3 != null))
                            {
                                string[] arreglo = linea3.Split(';');
                                u = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u[j] = Convert.ToDouble(arreglo[j]);
                                }         
                            }
                        } while (!(linea3 == null));
                        objReader3.Close();
                        //leo el cuarto archivo
                        StreamReader objReader4 = new StreamReader(ruta12);
                        string linea4 = "";
                        do
                        {
                            linea4 = objReader4.ReadLine();
                            if ((linea4 != null))
                            {
                                string[] arreglo = linea4.Split(';');
                                u2 = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u2[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea4 == null));
                        objReader4.Close();

                        //comienzo a buscar las salidas primero normalizo

                        double[] val = new double[inputCount];
                        char delimitador = ';';
                        string[] valores = cadena.Split(delimitador);

                        for (int i = 0; i < inputCount; i++)
                        {
                            double valor = Convert.ToDouble(valores[i]);
                            val[i] = Normalize(valor, inputMin, inputMax);
                        }
                        //vectores de salidas
                        double[] salidas = new double[w.GetLength(1)];
                        double[] salidas2 = new double[w2.GetLength(1)];
                        //calculo las salidas 
                        for (int i = 0; i < w.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (val[j] * w[j, i]);
                            }
                            sumatoria = sumatoria - u[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac1.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }

                        for (int i = 0; i < w2.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w2.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (salidas[j] * w2[j, i]);
                            }
                            sumatoria = sumatoria - u2[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac1.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas2[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas2[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas2[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }
                        string mensaje = " ";
                        for(int i = 0; i < outputCount; i++)
                        {
                            double resultado = InverseNormalize(salidas2[i], outputMin, outputMax);
                            mensaje = mensaje + "Salida" + (i + 1) + " = " + resultado + " ";
                        }
                        MessageBox.Show(mensaje);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("no se ha entrenado aun la red");
                    }
                    break;

                case "2":
                    string peso1 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa1.txt";
                    string peso2 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa2.txt";
                    string peso3 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capasalida.txt";
                    string umbral1 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa1.txt";
                    string umbral2 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa2.txt";
                    string umbral3 = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capasalida.txt";
                    try
                    {
                        //leo el primer archivo
                        int filas;
                        using (StreamReader r = new StreamReader(peso1)) 
                        {  
                            var file = r.ReadToEnd();
                            var lines = file.Split(new char[] { '\n' });
                            filas = lines.Count();
                        }
                        
                        StreamReader objReader = new StreamReader(peso1);
                        string linea = "";
                        int fila = 0;
                        do
                        {
                            linea = objReader.ReadLine();
                            if (fila == 0)
                            {
                                string[] arreglo = linea.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w = new double[filas, columnas];
                            }
                            if ((linea != null))
                            {
                                string[] arreglo = linea.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w[fila, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila += 1;
                            }
                        } while (!(linea == null));
                        objReader.Close();
                        //leo el segundo archivo

                        int filas2;
                        using (StreamReader r = new StreamReader(peso2))
                        {
                            var file2 = r.ReadToEnd();
                            var lines2 = file2.Split(new char[] { '\n' });
                            filas2 = lines2.Count();
                        }
                        
                        StreamReader objReader2 = new StreamReader(peso2);
                        string linea2 = "";
                        int fila2 = 0;
                        do
                        {
                            linea2 = objReader2.ReadLine();
                            if (fila2 == 0)
                            {
                                string[] arreglo = linea2.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w2 = new double[filas2, columnas];
                            }
                            if ((linea2 != null))
                            {
                                string[] arreglo = linea2.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w2[fila2, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila2 += 1;
                            }
                        } while (!(linea2 == null));
                        objReader2.Close();
                        //leo el tercer archivo

                        int filas3;
                        using (StreamReader r = new StreamReader(peso3))
                        {
                            var file3 = r.ReadToEnd();
                            var lines3 = file3.Split(new char[] { '\n' });
                            filas3 = lines3.Count();
                        }
                        
                        StreamReader objReader3 = new StreamReader(peso3);
                        string linea3 = "";
                        int fila3 = 0;
                        do
                        {
                            linea3 = objReader3.ReadLine();
                            if (fila3 == 0)
                            {
                                string[] arreglo = linea3.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w3 = new double[filas3, columnas];
                            }
                            if ((linea3 != null))
                            {
                                string[] arreglo = linea3.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w3[fila3, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila3 += 1;
                            }
                        } while (!(linea3 == null));
                        objReader3.Close();
                        //leo el cuarto archivo
                        StreamReader objReader4 = new StreamReader(umbral1);
                        string linea4 = "";
                        do
                        {
                            linea4 = objReader4.ReadLine();
                            if ((linea4 != null))
                            {
                                string[] arreglo = linea4.Split(';');
                                u = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea4 == null));
                        objReader4.Close();
                        //leo el quinto archivo
                        StreamReader objReader5 = new StreamReader(umbral2);
                        string linea5 = "";
                        do
                        {
                            linea5 = objReader5.ReadLine();
                            if ((linea5 != null))
                            {
                                string[] arreglo = linea5.Split(';');
                                u2 = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u2[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea5 == null));
                        objReader5.Close();
                        //leo el sexto archivo
                        StreamReader objReader6 = new StreamReader(umbral3);
                        string linea6 = "";
                        do
                        {
                            linea6 = objReader6.ReadLine();
                            if ((linea6 != null))
                            {
                                string[] arreglo = linea6.Split(';');
                                u3 = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u3[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea6 == null));
                        objReader6.Close();

                        //comienzo a buscar las salidas primero normalizo

                        double[] val = new double[inputCount];
                        char delimitador = ';';
                        string[] valores = cadena.Split(delimitador);

                        for (int i = 0; i < inputCount; i++)
                        {
                            double valor = Convert.ToDouble(valores[i]);
                            val[i] = Normalize(valor, inputMin, inputMax);
                        }
                        //vectores de salidas
                        double[] salidas = new double[w.GetLength(1)];
                        double[] salidas2 = new double[w2.GetLength(1)];
                        double[] salidas3 = new double[w3.GetLength(1)];

                        //calculo las salidas capa 1
                        for (int i = 0; i < w.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (val[j] * w[j, i]);
                            }
                            sumatoria = sumatoria - u[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac1.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }

                        //segunda capa
                        for (int i = 0; i < w2.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w2.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (salidas[j] * w2[j, i]);
                            }
                            sumatoria = sumatoria - u2[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac2.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas2[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas2[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas2[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }

                        //capa salida

                        for (int i = 0; i < w3.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w3.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (salidas2[j] * w3[j,i]);
                            }
                            sumatoria = sumatoria - u3[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fas.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas3[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas3[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas3[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }
                        string mensaje = " ";
                        for (int i = 0; i < outputCount; i++)
                        {
                            double resultado = InverseNormalize(salidas3[i], outputMin, outputMax);
                            mensaje = mensaje + "Salida" + (i + 1) + " = " + resultado + " ";
                        }
                        MessageBox.Show(mensaje);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Ah ocurrido un error al cargar los pesos y umbrales optimos");
                    }
                        break;

                case "3":

                    string peso13c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa1.txt";
                    string peso23c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa2.txt";
                    string peso33c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capa3.txt";
                    string peso43c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\pesos_optimos_capasalida.txt";
                    string umbral13c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa1.txt";
                    string umbral23c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa2.txt";
                    string umbral33c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capa3.txt";
                    string umbral43c = @"C:\Back_Propagation_by_fabian\resultados_perceptron\umbrales_optimos_capasalida.txt";
                    try
                    {
                        //leo el primer archivo
                        int filas;
                        using (StreamReader r = new StreamReader(peso13c))
                        {
                            var file = r.ReadToEnd();
                            var lines = file.Split(new char[] { '\n' });
                            filas = lines.Count();
                        }
                       
                        StreamReader objReader = new StreamReader(peso13c);
                        string linea = "";
                        int fila = 0;
                        do
                        {
                            linea = objReader.ReadLine();
                            if (fila == 0)
                            {
                                string[] arreglo = linea.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w = new double[filas, columnas];
                            }
                            if ((linea != null))
                            {
                                string[] arreglo = linea.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w[fila, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila += 1;
                            }
                        } while (!(linea == null));
                        objReader.Close();
                        //leo el segundo archivo
                        int filas2;
                        using (StreamReader r = new StreamReader(peso23c))
                        {
                            var file2 = r.ReadToEnd();
                            var lines2 = file2.Split(new char[] { '\n' });
                            filas2 = lines2.Count();
                        }
                        
                        StreamReader objReader2 = new StreamReader(peso23c);
                        string linea2 = "";
                        int fila2 = 0;
                        do
                        {
                            linea2 = objReader2.ReadLine();
                            if (fila2 == 0)
                            {
                                string[] arreglo = linea2.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w2 = new double[filas2, columnas];
                            }
                            if ((linea2 != null))
                            {
                                string[] arreglo = linea2.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w2[fila2, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila2 += 1;
                            }
                        } while (!(linea2 == null));
                        objReader2.Close();
                        //leo el tercer archivo
                        int filas3;
                        using (StreamReader r = new StreamReader(peso33c))
                        {
                            var file3 = r.ReadToEnd();
                            var lines3 = file3.Split(new char[] { '\n' });
                            filas3 = lines3.Count();
                        }
                       
                        StreamReader objReader3 = new StreamReader(peso33c);
                        string linea3 = "";
                        int fila3 = 0;
                        do
                        {
                            linea3 = objReader3.ReadLine();
                            if (fila3 == 0)
                            {
                                string[] arreglo = linea3.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w3 = new double[filas3, columnas];
                            }
                            if ((linea3 != null))
                            {
                                string[] arreglo = linea3.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w3[fila3, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila3 += 1;
                            }
                        } while (!(linea3 == null));
                        objReader3.Close();
                        //leo el cuarto archivo
                        int filas4;
                        using (StreamReader r = new StreamReader(peso43c))
                        {
                            var file4 = r.ReadToEnd();
                            var lines4 = file4.Split(new char[] { '\n' });
                            filas4 = lines4.Count();
                        }
                        
                        StreamReader objReader4 = new StreamReader(peso43c);
                        string linea4 = "";
                        int fila4 = 0;
                        do
                        {
                            linea4 = objReader4.ReadLine();
                            if (fila4 == 0)
                            {
                                string[] arreglo = linea4.Split(';');
                                int columnas = arreglo.GetLength(0);
                                w4 = new double[filas4, columnas];
                            }
                            if ((linea4 != null))
                            {
                                string[] arreglo = linea4.Split(';');
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    w4[fila4, j] = Convert.ToDouble(arreglo[j]);
                                }
                                fila4 += 1;
                            }
                        } while (!(linea4 == null));
                        objReader4.Close();
                        //leo el quinto archivo
                        StreamReader objReader5 = new StreamReader(umbral13c);
                        string linea5 = "";
                        do
                        {
                            linea5 = objReader5.ReadLine();
                            if ((linea5 != null))
                            {
                                string[] arreglo = linea5.Split(';');
                                u = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea5 == null));
                        objReader5.Close();
                        //leo el sexto archivo
                        StreamReader objReader6 = new StreamReader(umbral23c);
                        string linea6 = "";
                        do
                        {
                            linea6 = objReader6.ReadLine();
                            if ((linea6 != null))
                            {
                                string[] arreglo = linea6.Split(';');
                                u2 = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u2[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea6 == null));
                        objReader6.Close();
                        //leo el septimo archivo
                        StreamReader objReader7 = new StreamReader(umbral33c);
                        string linea7 = "";
                        do
                        {
                            linea7 = objReader7.ReadLine();
                            if ((linea7 != null))
                            {
                                string[] arreglo = linea7.Split(';');
                                u3 = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u3[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea7 == null));
                        objReader7.Close();
                        //leo el octavo archivo
                        StreamReader objReader8 = new StreamReader(umbral43c);
                        string linea8 = "";
                        do
                        {
                            linea8 = objReader8.ReadLine();
                            if ((linea8 != null))
                            {
                                string[] arreglo = linea8.Split(';');
                                u4 = new double[arreglo.GetLength(0)];
                                for (int j = 0; j < arreglo.GetLength(0); j++)
                                {
                                    u4[j] = Convert.ToDouble(arreglo[j]);
                                }
                            }
                        } while (!(linea8 == null));
                        objReader8.Close();

                        //comienzo a buscar las salidas primero normalizo

                        double[] val = new double[inputCount];
                        char delimitador = ';';
                        string[] valores = cadena.Split(delimitador);

                        for (int i = 0; i < inputCount; i++)
                        {
                            double valor = Convert.ToDouble(valores[i]);
                            val[i] = Normalize(valor, inputMin, inputMax);
                        }
                        //vectores de salidas
                        double[] salidas = new double[w.GetLength(1)];
                        double[] salidas2 = new double[w2.GetLength(1)];
                        double[] salidas3 = new double[w3.GetLength(1)];
                        double[] salidas4 = new double[w4.GetLength(1)];

                        //calculo las salidas capa 1
                        for (int i = 0; i < w.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (val[j] * w[j, i]);
                            }
                            sumatoria = sumatoria - u[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac1.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }

                        //segunda capa
                        for (int i = 0; i < w2.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w2.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (salidas[j] * w2[j, i]);
                            }
                            sumatoria = sumatoria - u2[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac2.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas2[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas2[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas2[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }

                        //tercera

                        for (int i = 0; i < w3.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w3.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (salidas2[j] * w3[j, i]);
                            }
                            sumatoria = sumatoria - u3[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fac3.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas3[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas3[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas3[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }

                        //capa salida

                        for (int i = 0; i < w4.GetLength(1); i++)
                        {
                            double sumatoria = 0;
                            for (int j = 0; j < w4.GetLength(0); j++)
                            {
                                sumatoria = sumatoria + (salidas3[j] * w4[j, i]);
                            }
                            sumatoria = sumatoria - u4[i];
                            //verifico la funcion de activacion que escogio el usuario
                            switch (fas.SelectedItem.ToString().Trim())
                            {
                                case "Sigmiode":
                                    salidas4[i] = 1 / (1 + Math.Exp(-sumatoria));
                                    break;
                                case "Tangente H":
                                    salidas4[i] = Math.Tanh(sumatoria);
                                    break;
                                case "Sinusoidal":
                                    salidas4[i] = Math.Sin(sumatoria);
                                    break;
                            }
                        }
                        string mensaje = " ";
                        for (int i = 0; i < outputCount; i++)
                        {
                            double resultado = Math.Round(InverseNormalize(salidas4[i], outputMin, outputMax),2);
                            mensaje = mensaje + "Salida" + (i + 1) + " = " + resultado + " ";
                        }
                        MessageBox.Show(mensaje);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("ocurrio un error al cargar los archivos");
                    }
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Setearvalores() == true)
            {
                entrenar_red();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString().Trim())
            {
                case "1":
                    this.nc1.Visible = false;
                    this.nc2.Visible = false;
                    this.nc3.Visible = false;
                    this.lnc1.Visible = false;
                    this.lnc2.Visible = false;
                    this.lnc3.Visible = false;
                    this.fac1.Visible = false;
                    this.fac2.Visible = false;
                    this.fac3.Visible = false;

                    this.nc1.Visible = true;
                    this.lnc1.Visible = true;
                    this.fac1.Visible = true;

                    break;

                case "2":
                    this.nc1.Visible = false;
                    this.nc2.Visible = false;
                    this.nc3.Visible = false;
                    this.lnc1.Visible = false;
                    this.lnc2.Visible = false;
                    this.lnc3.Visible = false;
                    this.fac1.Visible = false;
                    this.fac2.Visible = false; 
                    this.fac3.Visible = false;

                    this.nc1.Visible = true;
                    this.lnc1.Visible = true;
                    this.nc2.Visible = true;
                    this.lnc2.Visible = true;
                    this.fac1.Visible = true;
                    this.fac2.Visible = true;
                    break;

                case "3":
                    this.nc1.Visible = false;
                    this.nc2.Visible = false;
                    this.nc3.Visible = false;
                    this.lnc1.Visible = false;
                    this.lnc2.Visible = false;
                    this.lnc3.Visible = false;
                    this.fac1.Visible = false;
                    this.fac1.Visible = false;
                    this.fac3.Visible = false;

                    this.nc1.Visible = true;
                    this.lnc1.Visible = true;
                    this.nc2.Visible = true;
                    this.lnc2.Visible = true;
                    this.nc3.Visible = true;
                    this.lnc3.Visible = true;
                    this.fac1.Visible = true;
                    this.fac2.Visible = true;
                    this.fac3.Visible = true;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.openFileDialog1.ShowDialog();

                if (!string.IsNullOrEmpty(this.openFileDialog1.FileName))
                {
                    dataPath = this.openFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //seteo los valores de configuracion de red
                inputCount = Convert.ToInt32(this.textBox1.Text);
                outputCount = Convert.ToInt32(this.textBox2.Text);
                inputMax = Convert.ToDouble(this.textBox3.Text);
                inputMin = Convert.ToDouble(this.textBox4.Text);
                outputMax = Convert.ToDouble(this.textBox5.Text);
                outputMin = Convert.ToDouble(this.textBox6.Text);
                cadena = patron_entradas.Text;
                simular_red();
            }
            catch (Exception ex)
            {
                MessageBox.Show("por favor ingrese las variables que se le piden");
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
