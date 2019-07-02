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

namespace Maintenance_Mode
{
    public partial class Selection : Form
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();  // added

        public Selection()
        {
            InitializeComponent();
        }

        private void Sensor_Button_Click(object sender, EventArgs e)
        {
            Sensor frm = new Sensor();
            frm.Show();
            this.Close();
        }

        private void Servo_Button_Click(object sender, EventArgs e)
        {

            Servo frm = new Servo();
            frm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            synth.Speak(textBox1.Text);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Stock_Button_Click(object sender, EventArgs e)
        {
            Stock frm = new Stock();
            frm.Show();
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation frm = new Operation();
            frm.Show();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
