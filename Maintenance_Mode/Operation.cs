using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Speech.Synthesis;
using System.IO.Ports;     // added to use serial port features
using System.Threading;    // added to use sleep feature
using System.Media;


namespace Maintenance_Mode
{
    public partial class Operation : Form
    {
        const int COM_BAUD = 9600;
        const int READ_TIMEOUT = 10000;
        SpeechSynthesizer synth = new SpeechSynthesizer();
        //System.Media.SoundPlayer tunez = new System.Media.SoundPlayer(@"C: \Users\Paul\Source\Repos\Maintenance_Mode\Maintenance_Mode\Resources\dbc.wav");
       
        //Phrase Arrays-----------------------

        string[] englishphrase = {"User Detected", "Warning too close",
                                  "User not in range", "Red detected",
                                  "Orange detected", "Yellow detected",
                                  "Green detected", "Blue detected",
                                  "White detected", "Black detected",
                                  "Error, Wrong blocks", "No blocks detected",
                                  "Order can't be processed, stock levels too low",
                                  "Order can't be processed, red and green stock levels too low",
                                  "Order can't be processed, red and blue stock levels too low",
                                  "Order can't be processed, red stock level too low",
                                  "Order can't be processed, blue and green stock level too low",
                                  "Order can't be processed, green stock level too low",
                                  "Order can't be processed, blue stock level too low",
                                  "Order being processed...",
                                  "Order Complete. Please take your chips.",
                                  "Sensor error, please try again."};

        string[] polishphrase = {"Wykryo uzytkownika", "Uwaga, znajdujesz sie za blisko",
                                 "Nie wykryto uzytkownika", "Czerwony wykryty",
                                 "Pomaranczowy wykryty ", "Zolty wykryty",
                                 "Zielony wykryty", "Niebieski wykryty",
                                 "Bialy wykryty", "Czarny wykryty",
                                 "Kombinacja niewazna", "Nie wykryto bloku",
                                 "Operacja nie moze byc przeprowadzona, zawartosc kolumn niska",
                                 "Operacja nie moze byc przeprowadzona, czerwony i zielony kolor-niski poziom",
                                 "Operacja nie moze byc przeprowadzona, czerwony i niebieski kolor-niski poziom",
                                 "Operacja nie moze byc przeprowadzona, czerwony kolor-poziom niski",
                                 "Operacja nie moze byc przeprowadzona, niebieski i zielony kolor-niski poziom",
                                 "Operacja nie moze byc przeprowadzona, zielony kolor-poziom niski",
                                 "Operacja nie moze byc przeprowadzona, niebieski kolor-poziom niski",
                                 "Operacja moze byc przeprowadzona", "Operacja przeprowadzona poprawnie, prosze wyjac elementy.",
                                 "Sensor error, please try again."};

        //------------------------------------

        public Operation()
        {
            InitializeComponent();

            //Initialising COM Port---------------

            String[] ports = SerialPort.GetPortNames();
            serialPort1.BaudRate = COM_BAUD;
            string com_port = "COM Port";
            try
            {
                com_port = ports[0];
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Initialising..."); else textBox1.AppendText("Inicjowanie systemu");
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch
            {
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Cannot open " + com_port + Environment.NewLine); else textBox1.AppendText("Port niedostepny");
                return;
            }
            if (button1.Text.Equals("Dispense")) textBox1.AppendText(com_port + " now open" + Environment.NewLine + "Please insert blocks" + Environment.NewLine);
            else textBox1.AppendText(com_port + "Port dostepny " + Environment.NewLine + "Prosze umiescic elementy" + Environment.NewLine);

            //------------------------------------
           

        }
       
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
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Readline timeout fail" + Environment.NewLine); else textBox1.AppendText("Brak odpowiedzi systemu" + Environment.NewLine);
                return -1;
            }
            //status = Convert.ToInt32(reply);
            status = 0;
            if (button1.Text.Equals("Dispense")) textBox1.AppendText("Status = " + status + Environment.NewLine); else textBox1.AppendText("Status = " + status + Environment.NewLine);
            return status;
        }


        private void maintenanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            Selection frm = new Selection();
            frm.Show();
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (checkBox1.Checked == false)
            {
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Please ensure the checkbox is ticked" + Environment.NewLine); else textBox1.AppendText("Umewnij sie pole wyboru zostalo odznaczone" + Environment.NewLine);
            }
            else
            {
                //tunez.Play();
                string command = "o";
                textBox1.AppendText(command + Environment.NewLine);
                serialPort1.WriteLine(command);
                for (int i = 1; i <= 10; i++)
                {
                    int status = get_reply();
                    //
                    // only read data if status reply was 0 (i.e. was successful)
                    //

                    if (status == 0)
                    {
                        try
                        {
                            string phrase_index;
                            if (button1.Text.Equals("Dispense")) textBox1.AppendText("Reading data" + Environment.NewLine); else textBox1.AppendText("Odczyt danych" + Environment.NewLine);
                            phrase_index = serialPort1.ReadLine();
                            //textBox1.AppendText("Data = " + data + Environment.NewLine);
                            addText(phrase_index);
                            // textBox1.AppendText("Reading reply " + Environment.NewLine);
                            // int status = get_reply();
                        }
                        catch (TimeoutException)
                        {
                            if (button1.Text.Equals("Dispense")) textBox1.AppendText("Readline timeout fail" + Environment.NewLine); else textBox1.AppendText("Brak odpowiedzi systemu" + Environment.NewLine);
                        }

                    }
                }
            }
            addText(serialPort1.ReadLine());
           // tunez.Stop();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void addText(string text)
        {
            int index = Int32.Parse(text);
            string phrase;
            //Retrieves correct phrase depending on the current language.
            if (button1.Text.Equals("Dispense")) phrase = englishphrase[index]; else phrase = polishphrase[index];
            //-----------------------------------------------------------
            textBox1.Text += (phrase + System.Environment.NewLine);
            synth.Speak(phrase);

        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            this.Controls.Clear();
            this.InitializeComponent();

            String[] ports = SerialPort.GetPortNames();
            serialPort1.BaudRate = COM_BAUD;
            string com_port = "COM Port";
            try
            {
                com_port = ports[0];
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Initialising..."); else textBox1.AppendText("Inicjowanie systemu");
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch
            {
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Cannot open " + com_port + Environment.NewLine); else textBox1.AppendText("Port niedostepny");
                return;
            }
            if (button1.Text.Equals("Dispense")) textBox1.AppendText(com_port + " now open" + Environment.NewLine + "Please insert blocks" + Environment.NewLine);
            else textBox1.AppendText(com_port + "Port dostepny " + Environment.NewLine + "Prosze umiescic elementy" + Environment.NewLine);
        }

        private void polishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
            this.Controls.Clear();
            this.InitializeComponent();

            String[] ports = SerialPort.GetPortNames();
            serialPort1.BaudRate = COM_BAUD;
            string com_port = "COM Port";
            try
            {
                com_port = ports[0];
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Initialising..."); else textBox1.AppendText("Inicjowanie systemu");
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch
            {
                if (button1.Text.Equals("Dispense")) textBox1.AppendText("Cannot open " + com_port + Environment.NewLine); else textBox1.AppendText("Port niedostepny");
                return;
            }
            if (button1.Text.Equals("Dispense")) textBox1.AppendText(com_port + " now open" + Environment.NewLine + "Please insert blocks" + Environment.NewLine);
            else textBox1.AppendText(com_port + "Port dostepny " + Environment.NewLine + "Prosze umiescic elementy" + Environment.NewLine);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Insert the blocks into the input area and remain within 10-25 cm from the machine. Press the 'Dispense' button and wait for the dispensing to be completed. Enjoy the music!");
        }
    }
} 
