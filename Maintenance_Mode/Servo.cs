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
    public partial class Servo : Form
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
        int global_error;              // global variable for form1

        SpeechSynthesizer synth = new SpeechSynthesizer();

        public Servo()
        {
            InitializeComponent();
            getAvailablePorts();

            // Set the Minimum, Maximum, and initial Value. Box 1
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = 2;
            numericUpDown1.Minimum = 0;
            // Set the Minimum, Maximum, and initial Value. Box 2
            numericUpDown2.Value = 0;
            numericUpDown2.Maximum = 1;
            numericUpDown2.Minimum = 0;

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
                if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Readline timeout fail" + Environment.NewLine); else Debug_window.AppendText("Brak odpowiedzi systemu" + Environment.NewLine);
                return -1;
            }
            status = Convert.ToInt32(reply);
            if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Status = " + reply + Environment.NewLine); else Debug_window.AppendText(" Status " + reply + Environment.NewLine);
            return status;
        }

        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames(); // lists an array of values defined as "ports". "ports" then = to the all the available com ports
            comboBox1.Items.AddRange(ports);             // adds the list of ports availble to the "comboBox1"
            serialPort1.BaudRate = COM_BAUD;
        }

        // Executed when form is first loaded. Main activity is to get a list of the available COM ports
        // on the computer and add them to the dropdown box (comboBox1).
        //
        /*private void Servo_Load(object sender, EventArgs e)
        {
           

            comboBox1.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
            comboBox1.SelectedIndex = 0;
            serialPort1.BaudRate = COM_BAUD;
            global_error = OK;
            Thread.Sleep(2000);   // sleep 2 seconds
        }
        */

        private void Com_Choice_Click(object sender, EventArgs e)
        {
            string com_port = comboBox1.SelectedItem.ToString();
            if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Selected COM Port = " + com_port + Environment.NewLine); else Debug_window.AppendText(" wybierz port " + com_port + Environment.NewLine);
            try
            {
                if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Trying to open " + com_port + Environment.NewLine); else Debug_window.AppendText("Proba wybrania portu" + com_port + Environment.NewLine);
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch
            {
                if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Cannot open " + com_port+ Environment.NewLine); else Debug_window.AppendText("Nie mozna otworzyc portu" + com_port + Environment.NewLine);
                return;
            }
            if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText(com_port + " now open" + Environment.NewLine); else Debug_window.AppendText(com_port + "Port otwarty" + Environment.NewLine);
        }


        //
        // send 's' servo command to MBED
        //

        private void Servo_Detail_Click(object sender, EventArgs e)
        {
            string command = "s "
                   + Convert.ToString(numericUpDown1.Value)
                   + " "
                   + Convert.ToString(numericUpDown2.Value);
            Debug_window.AppendText(command + Environment.NewLine);
            serialPort1.WriteLine(command);
            if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Reading reply" + Environment.NewLine); else Debug_window.AppendText("" + Environment.NewLine);
            int status = get_reply();
        }

        //
        // send 'r' read command to MBED
        //

        private void Servo_Read_Click(object sender, EventArgs e)
        {
            String command = "r";
            Debug_window.AppendText(command + Environment.NewLine);
            serialPort1.WriteLine(command);
            if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Reading reply" + Environment.NewLine); else Debug_window.AppendText("Odczyt systemu" + Environment.NewLine);
            int status = get_reply();
            //
            // only read data if status reply was 0 (i.e. was successful)
            //
            if (status == 0)
            {
                if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Reading data" + Environment.NewLine); else Debug_window.AppendText("Odczyt systemu" + Environment.NewLine);
                string data = serialPort1.ReadLine();
                if (Servo_Detail.Text.Equals("Go")) Debug_window.AppendText("Data = " + data + Environment.NewLine); else Debug_window.AppendText("Dane odczytu" + data + Environment.NewLine);
            }
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug_window.Clear();
        }
    }
}

