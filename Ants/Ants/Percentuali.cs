using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ants
{
    public partial class Percentuali : Form
    {
        String mPerpre = "", mPerram = ""; 

        public Percentuali()
        {
            InitializeComponent();
        }

        private void Inserisci_Click(object sender, EventArgs e)
        {
            if (label3.Text == "Campo non valido" || label4.Text == "Campo non valido" || textBox1.Text == "" || textBox2.Text == "")
            {
                label5.Text = "I dati inseriti non sono corretti";
            }
            else
            {
                mPerpre = textBox1.Text;
                mPerram = textBox2.Text;
                this.Close();
            }
        }
        
        public String Perpre
        {
            get { return mPerpre; }
        }

        public String Perram
        {
            get { return mPerram; }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Percentuali_Load(object sender, EventArgs e)
        {

        }
    }
}
