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
        public Form1()
        {
            InitializeComponent();
            FolderBrowserDialog src = new FolderBrowserDialog();
            src.Description = "Seleziona la cartella download predefinita:";
            src.RootFolder = Environment.SpecialFolder.MyComputer;
            System.Windows.Forms.MessageBox.Show("My message here");
        }
    }
}
