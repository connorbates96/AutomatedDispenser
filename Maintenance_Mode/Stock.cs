using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.IO.Ports;     // added to use serial port features
using System.Threading;    // added to use sleep feature

namespace Maintenance_Mode
{
    public partial class Stock : Form
    {
        //***********************************************************
        // Constant definitions
        //***********************************************************
        const int OK = 0;   // some status return constants
        const int FORMAT_ERROR = -1;

        const int COM_BAUD = 9600;
        const int READ_TIMEOUT = 10000;   // timeout for a read reply (10 seconds)

        //***********************************************************
        // global variables
        //***********************************************************

        SpeechSynthesizer synth = new SpeechSynthesizer();

        int global_error;              // global variable for form1
        public Stock()
        {
            InitializeComponent();
            getAvailablePorts();

            // Set the Minimum, Maximum, and initial Value. Box 1
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = 7;
            numericUpDown1.Minimum = 0;
            // Set the Minimum, Maximum, and initial Value. Box 2
            numericUpDown2.Value = 0;
            numericUpDown2.Maximum = 7;
            numericUpDown2.Minimum = 0;
            // Set the Minimum, Maximum, and initial Value. Box 3
            numericUpDown3.Value = 0;
            numericUpDown3.Maximum = 7;
            numericUpDown3.Minimum = 0;
        }


        //***********************************************************
        // User written functions
        //***********************************************************
        // get_reply : Read a status reply from the MBED
        //
        // All commands will return a status reply on completing the specified command.
        // A returned valur of 0 will indicate that all is well

        public int get_reply()
        {

            string reply;
            int status;

            serialPort1.DiscardInBuffer();
            serialPort1.ReadTimeout = READ_TIMEOUT;
            try
            {
                reply = serialPort1.ReadLine();
            }
            catch (TimeoutException)
            {
                if (label1.Text.Equals("Red")) Debug_window.AppendText("Readline timeout fail" + Environment.NewLine); else Debug_window.AppendText("Odczyt systemu bledny" + Environment.NewLine);
                return -1;
            }
            status = Convert.ToInt32(reply);
            if (label1.Text.Equals("Red")) Debug_window.AppendText("Status = " + reply + Environment.NewLine); else Debug_window.AppendText(" Status " + reply + Environment.NewLine);
            return status;
        }

        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames(); // lists an array of values defined as "ports". "ports" then = to the all the available com ports
            comboBox1.Items.AddRange(ports);             // adds the list of ports availble to the "comboBox1"
            serialPort1.BaudRate = COM_BAUD;
        }
        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            Selection frm = new Selection();
            frm.Show();
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            this.Close();
            Application.Exit();
        }

        private void Com_Choice_Click(object sender, EventArgs e)
        {
            string com_port = comboBox1.SelectedItem.ToString();
            if (label1.Text.Equals("Red")) Debug_window.AppendText("Selected COM Port = " + com_port + Environment.NewLine); else Debug_window.AppendText(" Wybierz port" + com_port + Environment.NewLine);
            try
            {
                if (label1.Text.Equals("Red")) Debug_window.AppendText("Trying to open " + com_port + Environment.NewLine); else Debug_window.AppendText("Odczyt portu" + com_port + Environment.NewLine);
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch
            {
                if (label1.Text.Equals("Red")) Debug_window.AppendText("Cannot open " + com_port + Environment.NewLine); else Debug_window.AppendText("Nie mozna otworzyc" + com_port + Environment.NewLine);
                return;
            }
            if (label1.Text.Equals("Red")) Debug_window.AppendText(com_port + " now open" + Environment.NewLine); else Debug_window.AppendText(com_port + "System otwarty" + Environment.NewLine);
           

            //Check Stock Levels------------------

            string command = "q";
            if (label1.Text.Equals("Red")) Debug_window.AppendText("Checking stock levels..." + Environment.NewLine); else Debug_window.AppendText("Spradzanie poziomu kolumn" + Environment.NewLine);
            serialPort1.WriteLine(command);

            int status = get_reply();
            //
            // only read data if status reply was 0 (i.e. was successful)
            //
            if (status == 0)
            {
                string data = serialPort1.ReadLine();
                string[] stock;
                stock = data.Split();

                if (label1.Text.Equals("Red"))
                {
                    Debug_window.AppendText(Environment.NewLine + "STOCK" + Environment.NewLine + Environment.NewLine
                                        + "Red = " + stock[0] + Environment.NewLine
                                        + "Green = " + stock[1] + Environment.NewLine
                                        + "Blue = " + stock[2] + Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    Debug_window.AppendText(Environment.NewLine + "Zasob" + Environment.NewLine + Environment.NewLine
                                        + " Czerwony " + stock[0] + Environment.NewLine
                                        + " Zielony " + stock[1] + Environment.NewLine
                                        + " Niebieski " + stock[2] + Environment.NewLine + Environment.NewLine);
                }

                numericUpDown1.Value = Int32.Parse(stock[0]);
                numericUpDown1.Minimum = Int32.Parse(stock[0]);
                numericUpDown2.Value = Int32.Parse(stock[1]);
                numericUpDown2.Minimum = Int32.Parse(stock[1]);
                numericUpDown3.Value = Int32.Parse(stock[2]);
                numericUpDown3.Minimum = Int32.Parse(stock[2]);


            }
            //------------------------------------

        }

        private void Stock_set_Click(object sender, EventArgs e)
        {
            string command = "w "
                  + Convert.ToString(numericUpDown1.Value)
                  + " "
                  + Convert.ToString(numericUpDown2.Value)
                  +" "
                  + Convert.ToString(numericUpDown3.Value);
            Debug_window.AppendText(command + Environment.NewLine);
            serialPort1.WriteLine(command);
            if (label1.Text.Equals("Red")) Debug_window.AppendText("Stock level set" + Environment.NewLine); else Debug_window.AppendText("Poziom kolummn ustalony" + Environment.NewLine);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug_window.Clear();
        }
    }
}
