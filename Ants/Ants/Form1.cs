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
            notifyIcon1.BalloonTipText = "La finestra è stata minimizzata.";
            notifyIcon1.BalloonTipTitle = "Ants";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.Icon = new Icon(Directory.GetCurrentDirectory() + @"\antEater.ico");
            notifyIcon1.Visible = true;
            this.TopMost = true;
            
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Download blacklist from server

            if (src.ShowDialog() == DialogResult.OK)
                path = src.SelectedPath;
            else
            {
                System.Windows.Forms.MessageBox.Show("Errore nella selezione del Folder dei Download.");
                Application.Exit();
            }
            Percentuali p = new Percentuali();
            p.TopMost = true;
            p.ShowDialog(this);

            perpre = p.Perpre;
            perram = p.Perram;

            Processo proc = new Processo(path, int.Parse(perram), int.Parse(perpre));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(500);
            this.Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.TopMost = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.TopMost = true;
        }


    }
}
