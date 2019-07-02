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
using System.IO.Ports; // let's us know the computer is allowing us access to the com ports
using System.Threading;    // added to use sleep feature
//testing

namespace Maintenance_Mode
{
    public partial class Sensor : Form
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

        private class Item
        {
            public string Name;
            public int Value;
            public Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }
            public Sensor()
        {
            InitializeComponent();
            getAvailablePorts();
            //Sensor_selection.Items.Add(new Item("RGB Sensor", 0));
            //Sensor_selection.Items.Add(new Item("Distance Sensor", 1));
            //Motor_Direction.Items.Add(new Item("Stop", 0));
            //Motor_Direction.Items.Add(new Item("Forward", 1));
            //Motor_Direction.Items.Add(new Item("Backwards", 2));
            label2.Visible = true;
            label4.Visible = true;
            //Motor_Direction.Visible = false;
            Belt_Move.Visible = true;
            SensorUpDown.Value = 0;
            SensorUpDown.Maximum = 2;
            SensorUpDown.Minimum = 0;
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
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Readline timeout fail" + Environment.NewLine); else Debug_window.AppendText("Brak odpowiedzi systemu" + Environment.NewLine);
                return -1;
            }
            status = Convert.ToInt32(reply);
            if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Status = " + reply + Environment.NewLine); else Debug_window.AppendText("Status = " + reply + Environment.NewLine);
            return status;
        }

        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames(); // lists an array of values defined as "ports". "ports" then = to the all the available com ports
            comboBox1.Items.AddRange(ports);             // adds the list of ports availble to the "comboBox1"
            serialPort1.BaudRate = COM_BAUD;
        }

        private void Com_Choice_Click(object sender, EventArgs e)
        {
             string com_port = comboBox1.SelectedItem.ToString();
            if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Selected COM Port = " + com_port + Environment.NewLine); else Debug_window.AppendText("Wybor poru = " + com_port + Environment.NewLine);
            try
            {
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Trying to open " + com_port + Environment.NewLine); else Debug_window.AppendText("Sporobuj otworzyc port " + com_port + Environment.NewLine);
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch
            {
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Cannot open " + com_port + Environment.NewLine); else Debug_window.AppendText("Port niedostepny " + com_port + Environment.NewLine);
                return;
            }
            if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText(com_port + " now open" + Environment.NewLine); else Debug_window.AppendText(com_port + " otworz teraz" + Environment.NewLine);
            }

        private void Read_Sensor_Click(object sender, EventArgs e)
        {
            try
            {
                string command = "c";
                Debug_window.AppendText(command + Environment.NewLine);
                serialPort1.WriteLine(command);
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Reading reply " + Environment.NewLine); else Debug_window.AppendText("Odpowiedz systemu" + Environment.NewLine);
                int status = get_reply();
                //Debug_window.Text = serialPort1.ReadLine();
                if (status == 0)
                {
                    if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Reading data" + Environment.NewLine); else Debug_window.AppendText("Odpowiedz systemu" + Environment.NewLine);
                    string data = serialPort1.ReadLine();
                    if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Data = " + data + Environment.NewLine); else Debug_window.AppendText(" Otrzymane wartosci " + data + Environment.NewLine);
                }


            }
            catch (TimeoutException)
            {
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Timeout Exception" + Environment.NewLine); else Debug_window.AppendText("System wykryl problem" + Environment.NewLine);
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

        private void Motor_control_Click(object sender, EventArgs e)
        {
            try
            {
                string command = "b"
                + Convert.ToString(SensorUpDown.Value);
                Debug_window.AppendText(command + Environment.NewLine);
                serialPort1.WriteLine(command);
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Reading reply " + Environment.NewLine); else Debug_window.AppendText("" + Environment.NewLine);
                int status = get_reply();
                //Debug_window.Text = serialPort1.ReadLine();
                
                


            }
            catch (TimeoutException)
            {
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Timeout Exception" + Environment.NewLine); else Debug_window.AppendText("" + Environment.NewLine);
            }
        }

        private void Read_Distance_Click(object sender, EventArgs e)
        {
            try
            {
                string command = "d";
                Debug_window.AppendText(command + Environment.NewLine);
                serialPort1.WriteLine(command);
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Reading reply" + Environment.NewLine); else Debug_window.AppendText("" + Environment.NewLine);
                int status = get_reply();
                //Debug_window.Text = serialPort1.ReadLine();
                if (status == 0)
                {
                    if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Reading data" + Environment.NewLine); else Debug_window.AppendText("" + Environment.NewLine);
                    string data = serialPort1.ReadLine();
                    if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Data = " + data + Environment.NewLine); else Debug_window.AppendText(" = " + data + Environment.NewLine);
                }

            }
            catch (TimeoutException)
            {
                if (label2.Text.Equals("Sensor Control")) Debug_window.AppendText("Timeout exception" + Environment.NewLine); else Debug_window.AppendText("" + Environment.NewLine);
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            Debug_window.Clear();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug_window.Clear();
        }
    }
}
