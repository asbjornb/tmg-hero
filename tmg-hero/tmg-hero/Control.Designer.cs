namespace tmg_hero.winforms
{
    partial class Control
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PlayStop = new System.Windows.Forms.Button();
            this.LoadSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PlayStop
            // 
            this.PlayStop.Location = new System.Drawing.Point(46, 36);
            this.PlayStop.Name = "PlayStop";
            this.PlayStop.Size = new System.Drawing.Size(189, 76);
            this.PlayStop.TabIndex = 0;
            this.PlayStop.Text = "Play";
            this.PlayStop.UseVisualStyleBackColor = true;
            this.PlayStop.Click += new System.EventHandler(this.PlayStop_Click);
            // 
            // LoadSave
            // 
            this.LoadSave.Location = new System.Drawing.Point(544, 313);
            this.LoadSave.Name = "LoadSave";
            this.LoadSave.Size = new System.Drawing.Size(75, 23);
            this.LoadSave.TabIndex = 1;
            this.LoadSave.Text = "Load save";
            this.LoadSave.UseVisualStyleBackColor = true;
            this.LoadSave.Click += new System.EventHandler(this.LoadSave_Click);
            // 
            // Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.LoadSave);
            this.Controls.Add(this.PlayStop);
            this.Name = "Control";
            this.Text = "TMG Hero";
            this.ResumeLayout(false);

        }

        #endregion

        private Button PlayStop;
        private Button LoadSave;
    }
}