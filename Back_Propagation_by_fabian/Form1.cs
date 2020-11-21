using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Threading;

namespace Back_Propagation_by_fabian
{
    public partial class Form1 : Form
    {
        public Form1()
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


        public Form1(string mensaje)
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
            MessageBox.Show(mensaje,"Atencion");
        }


        static List<double[]> input;
        static List<double[]> output;

        public string nombre_archivo;

        static int inputCount;
        static int outputCount;

        static double inputMax;
        static double inputMin;

        static double outputMax ;
        static double outputMin ;

        static string cadena;

        static bool loadNetwork = false;
        static bool saveNetwork = true;

        public double error_maximo;
        public int iteraciones;
        public double rata;

        public int nec1 = 0;
        public int nec2 = 0;
        public int nec3 = 0;

        static string dataPath ;
        static string neuronPath = @"C:\Back_Propagation_by_fabian\resultados\datos_entrenamiento.bin";

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
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + "ingrese todos los valores de configuracion de la red");
                return false;
            }
           
        }

        //funcion para mapear datos de entrada y salida
        static bool ReadData()
        {
            input = new List<double[]>();
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
                MessageBox.Show("Atencion!","No ha cargado los datos");
                return false;
            }            

        }

        //funciones para la normalizacion e invertirla
        static double Normalize(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }
        static double InverseNormalize(double value, double min, double max)
        {
            return value * (max - min) + min;
        }

        //boton cargar archivo
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

        //funcion para evaluar la red
        static void outputRequest()
        {
            Perceptron p;
            //cargo los datos de la red
            FileStream fs = new FileStream(neuronPath, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                p = (Perceptron)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                MessageBox.Show("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            //comienzo a buscar las salidas
           
            double[] val = new double[inputCount];
            char delimitador = ';';
            string[] valores = cadena.Split(delimitador);

            for (int i = 0; i < inputCount; i++)
            {
                double valor = Convert.ToDouble(valores[i]);
                val[i] = Normalize(valor, inputMin, inputMax);
            }

            double[] sal = p.Activate(val);
            String respuesta = "";
            for (int i = 0; i < outputCount; i++)
            {
               respuesta = respuesta+"Respuesta " + (i+1) + ": " + InverseNormalize(sal[i], outputMin, outputMax) + " ";
            }
            MessageBox.Show(respuesta);
        }

        //funcion que utilizo para evaluar la red
        public void entrenar_red()
        {
            //limpio la grafica
            this.chart1.Series["Error"].Points.Clear();
            //instancio objeto
            Perceptron p;

            if (!loadNetwork)
            {
                if (ReadData() == true)
                {
                    //verifico el numero de capas que escogio el usuario
                    if (nec1 != 0 && nec2 != 0 && nec3 != 0)
                    {
                        p = new Perceptron(new int[] { input[0].Length, nec1, nec2, nec3, output[0].Length });
                    }
                    else
                    {
                        if (nec1 != 0 && nec2 !=0)
                        {
                            p = new Perceptron(new int[] { input[0].Length, nec1, nec2, output[0].Length });
                        }
                        else
                        {
                            p = new Perceptron(new int[] { input[0].Length, nec1, output[0].Length });
                        }
                    }
                   
                    //llamo al metodo entrenar
                    p.Learn(input, output, rata, error_maximo, iteraciones);


                    //graficar 
                    string line;
                    int contador = 1;
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\LogTail.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        /////////////////////////////////////////////////////////////
                        labelerror.Text = line;
                        labeliteracion.Text = contador + "";
                        this.chart1.Series["Error"].Color = Color.Red;
                        this.chart1.Series["Error"].Points.AddXY(contador, Convert.ToDouble(line));
                        //////////////////////////////////////////////////////////////


                        if (contador % 200 == 0)
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
                    if (saveNetwork)
                    {
                        FileStream fs = new FileStream(neuronPath, FileMode.Create);
                        BinaryFormatter formatter = new BinaryFormatter();
                        try
                        {
                            formatter.Serialize(fs, p);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                            throw;
                        }
                        finally
                        {
                            fs.Close();
                        }
                    }
                }
            }
        }

        //clases para la red neuronal
        [Serializable]
        public class Perceptron
        {
            List<Layer> layers;

            public Perceptron(int[] neuronsPerlayer)
            {
                layers = new List<Layer>();
                Random r = new Random();

                for (int i = 0; i < neuronsPerlayer.Length; i++)
                {
                    layers.Add(new Layer(neuronsPerlayer[i], i == 0 ? neuronsPerlayer[i] : neuronsPerlayer[i - 1], r));
                }
            }
            public double[] Activate(double[] inputs)
            {
                double[] outputs = new double[0];
                for (int i = 1; i < layers.Count; i++)
                {
                    outputs = layers[i].Activate(inputs);
                    inputs = outputs;
                }
                return outputs;
            }
            double IndividualError(double[] realOutput, double[] desiredOutput)
            {
                double err = 0;
                for (int i = 0; i < realOutput.Length; i++)
                {
                    err += Math.Pow(realOutput[i] - desiredOutput[i], 2);
                }
                return err;
            }
            double GeneralError(List<double[]> input, List<double[]> desiredOutput)
            {
                double err = 0;
                for (int i = 0; i < input.Count; i++)
                {
                    err += IndividualError(Activate(input[i]), desiredOutput[i]);
                }
                return err;
            }
           
            List<string> log;
            //funcion entrenar
            public bool Learn(List<double[]> input, List<double[]> desiredOutput, double alpha, double maxError, int maxIterations)
            {
                double err = 99999;
                log = new List<string>();
                int iterator = 1;
                while (err > maxError)
                {
                    ApplyBackPropagation(input, desiredOutput, alpha);
                    err = GeneralError(input, desiredOutput);
                    err = Math.Round(err,3);
                    log.Add(err.ToString());
                    Console.WriteLine(err);

                    iterator = iterator + 1; 
                    
                    maxIterations--;
                    if (maxIterations <= 0)
                    {
                        System.IO.File.Delete(@"C:\LogTail.txt");
                        System.IO.File.WriteAllLines(@"C:\LogTail.txt", log.ToArray());
                        return false;
                    }
                }
                System.IO.File.Delete(@"C:\LogTail.txt");
                System.IO.File.WriteAllLines(@"C:\LogTail.txt", log.ToArray());
                return true;
            }

            List<double[]> sigmas;
            List<double[,]> deltas;

            //propagacion en cascada aplicando la derivada de la funsion
            void SetSigmas(double[] desiredOutput)
            {
                sigmas = new List<double[]>();
                for (int i = 0; i < layers.Count; i++)
                {
                    sigmas.Add(new double[layers[i].numberOfNeurons]);
                }
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < layers[i].numberOfNeurons; j++)
                    {
                        if (i == layers.Count - 1)
                        {
                            double y = layers[i].neurons[j].lastActivation;
                            sigmas[i][j] = (Neuron.Sigmoid(y) - desiredOutput[j]) * Neuron.SigmoidDerivated(y);
                        }
                        else
                        {
                            double sum = 0;
                            for (int k = 0; k < layers[i + 1].numberOfNeurons; k++)
                            {
                                sum += layers[i + 1].neurons[k].weights[j] * sigmas[i + 1][k];
                            }
                            sigmas[i][j] = Neuron.SigmoidDerivated(layers[i].neurons[j].lastActivation) * sum;
                        }
                    }
                }
            }
            void SetDeltas()
            {
                deltas = new List<double[,]>();
                for (int i = 0; i < layers.Count; i++)
                {
                    deltas.Add(new double[layers[i].numberOfNeurons, layers[i].neurons[0].weights.Length]);
                }
            }
            //actualizar umbrales
            void AddDelta()
            {
                for (int i = 1; i < layers.Count; i++)
                {
                    for (int j = 0; j < layers[i].numberOfNeurons; j++)
                    {
                        for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                        {
                            deltas[i][j, k] += sigmas[i][j] * Neuron.Sigmoid(layers[i - 1].neurons[k].lastActivation);
                        }
                    }
                }
            }
            //buscar errores no lineales
            void UpdateBias(double alpha)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    for (int j = 0; j < layers[i].numberOfNeurons; j++)
                    {
                        layers[i].neurons[j].bias -= alpha * sigmas[i][j];
                    }
                }
            }
            //actualizar pesos
            void UpdateWeights(double alpha)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    for (int j = 0; j < layers[i].numberOfNeurons; j++)
                    {
                        for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                        {
                            layers[i].neurons[j].weights[k] -= alpha * deltas[i][j, k];
                        }
                    }
                }
            }

            //funcion para aplicar backpropagation
            void ApplyBackPropagation(List<double[]> input, List<double[]> desiredOutput, double alpha)
            {
                SetDeltas();
                for (int i = 0; i < input.Count; i++)
                {
                    Activate(input[i]);
                    SetSigmas(desiredOutput[i]);
                    UpdateBias(alpha);
                    AddDelta();
                }
                UpdateWeights(alpha);

            }
        }
        //clase capa oculta
        [Serializable]
        class Layer
        {
            public List<Neuron> neurons;
            public int numberOfNeurons;
            public double[] output;

            public Layer(int _numberOfNeurons, int numberOfInputs, Random r)
            {
                numberOfNeurons = _numberOfNeurons;
                neurons = new List<Neuron>();
                for (int i = 0; i < numberOfNeurons; i++)
                {
                    neurons.Add(new Neuron(numberOfInputs, r));
                }
            }

            public double[] Activate(double[] inputs)
            {
                List<double> outputs = new List<double>();
                for (int i = 0; i < numberOfNeurons; i++)
                {
                    outputs.Add(neurons[i].Activate(inputs));
                }
                output = outputs.ToArray();
                return outputs.ToArray();
            }

        }

        //clase neurona
        [Serializable]
        class Neuron
        {
            public double[] weights;
            public double lastActivation;
            public double bias;

            public Neuron(int numberOfInputs, Random r)
            {
                bias = 10 * r.NextDouble() - 5;
                weights = new double[numberOfInputs];
                for (int i = 0; i < numberOfInputs; i++)
                {
                    weights[i] = 10 * r.NextDouble() - 5;
                }
            }
            public double Activate(double[] inputs)
            {
                double activation = bias;

                for (int i = 0; i < weights.Length; i++)
                {
                    activation += weights[i] * inputs[i];
                }

                lastActivation = activation;
                return Sigmoid(activation);
            }
            public static double Sigmoid(double input)
            {
                return 1 / (1 + Math.Exp(-input));
            }
            public static double SigmoidDerivated(double input)
            {
                double y = Sigmoid(input);
                return y * (1 - y);
            }

            public static double Sinuidal(double input)
            {
                return Math.Sin(input);
            }

            public static double SinuidalDerivated(double input)
            {
                return Math.Cos(input);
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

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
                    this.fac2.Visible = false;
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

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Setearvalores()==true){
                entrenar_red();
            } 
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void labeliteracion_Click(object sender, EventArgs e)
        {

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
                outputRequest();
            }
            catch (Exception ex)
            {
                MessageBox.Show("por favor ingrese las variables que se le piden");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void nc2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
