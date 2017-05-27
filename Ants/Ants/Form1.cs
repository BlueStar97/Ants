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
    public partial class Form1 : Form
    {
        String path = "", perpre="", perram="";
        FolderBrowserDialog src = new FolderBrowserDialog();
        public Form1()
        {
            InitializeComponent();
            src.Description = "Seleziona la cartella download predefinita:";
            src.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (src.ShowDialog() == DialogResult.OK)
                path = src.SelectedPath;
            else
            {
                System.Windows.Forms.MessageBox.Show("Errore nella selezione del Folder dei Download.");
                Application.Exit();
            }

            Percentuali p = new Percentuali();
            p.ShowDialog(this);

            perpre = p.Perpre;
            perram = p.Perram;
        }


    }
}
