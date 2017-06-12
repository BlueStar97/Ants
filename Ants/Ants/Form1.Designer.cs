namespace Ants
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.IDprocperc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.problem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.killdelete = new System.Windows.Forms.Button();
            this.quit = new System.Windows.Forms.Button();
            this.control = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IDprocperc,
            this.problem});
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(351, 343);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // IDprocperc
            // 
            this.IDprocperc.Text = "ID Processo/Percorso";
            this.IDprocperc.Width = 121;
            // 
            // problem
            // 
            this.problem.Text = "Problema";
            this.problem.Width = 400;
            // 
            // killdelete
            // 
            this.killdelete.Location = new System.Drawing.Point(370, 12);
            this.killdelete.Name = "killdelete";
            this.killdelete.Size = new System.Drawing.Size(103, 27);
            this.killdelete.TabIndex = 1;
            this.killdelete.Text = "Uccidi/Elimina";
            this.killdelete.UseVisualStyleBackColor = true;
            this.killdelete.Click += new System.EventHandler(this.killdelete_Click);
            // 
            // quit
            // 
            this.quit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.quit.Location = new System.Drawing.Point(370, 328);
            this.quit.Name = "quit";
            this.quit.Size = new System.Drawing.Size(103, 27);
            this.quit.TabIndex = 2;
            this.quit.Text = "Esci";
            this.quit.UseVisualStyleBackColor = true;
            // 
            // control
            // 
            this.control.Location = new System.Drawing.Point(369, 45);
            this.control.Name = "control";
            this.control.Size = new System.Drawing.Size(104, 36);
            this.control.TabIndex = 3;
            this.control.Text = "Controllo del sistema";
            this.control.UseVisualStyleBackColor = true;
            this.control.Click += new System.EventHandler(this.control_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 367);
            this.Controls.Add(this.control);
            this.Controls.Add(this.quit);
            this.Controls.Add(this.killdelete);
            this.Controls.Add(this.listView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button killdelete;
        private System.Windows.Forms.ColumnHeader IDprocperc;
        private System.Windows.Forms.ColumnHeader problem;
        private System.Windows.Forms.Button quit;
        private System.Windows.Forms.Button control;
    }
}

