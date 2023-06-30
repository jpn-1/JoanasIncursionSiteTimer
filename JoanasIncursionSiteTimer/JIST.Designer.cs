
namespace JoanasIncursionSiteTimer
{
    partial class JIST
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnSetup = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.cbIsSetup = new System.Windows.Forms.CheckBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.lblMouseWarning = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.lvTimers = new System.Windows.Forms.ListView();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSetup
            // 
            this.btnSetup.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSetup.Location = new System.Drawing.Point(12, 12);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(75, 23);
            this.btnSetup.TabIndex = 0;
            this.btnSetup.Text = "Setup";
            this.btnSetup.UseVisualStyleBackColor = true;
            this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Enabled = false;
            this.btnStartStop.Location = new System.Drawing.Point(162, 12);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 1;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // cbIsSetup
            // 
            this.cbIsSetup.AutoSize = true;
            this.cbIsSetup.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.cbIsSetup.Enabled = false;
            this.cbIsSetup.Location = new System.Drawing.Point(102, 16);
            this.cbIsSetup.Name = "cbIsSetup";
            this.cbIsSetup.Size = new System.Drawing.Size(54, 17);
            this.cbIsSetup.TabIndex = 3;
            this.cbIsSetup.Text = "Setup";
            this.cbIsSetup.UseVisualStyleBackColor = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // rtbOutput
            // 
            this.rtbOutput.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.rtbOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbOutput.Enabled = false;
            this.rtbOutput.Location = new System.Drawing.Point(12, 58);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(225, 31);
            this.rtbOutput.TabIndex = 4;
            this.rtbOutput.Text = "";
            // 
            // lblMouseWarning
            // 
            this.lblMouseWarning.AutoSize = true;
            this.lblMouseWarning.Location = new System.Drawing.Point(12, 38);
            this.lblMouseWarning.Name = "lblMouseWarning";
            this.lblMouseWarning.Size = new System.Drawing.Size(0, 13);
            this.lblMouseWarning.TabIndex = 6;
            // 
            // lvTimers
            // 
            this.lvTimers.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.lvTimers.HideSelection = false;
            this.lvTimers.Location = new System.Drawing.Point(12, 95);
            this.lvTimers.Name = "lvTimers";
            this.lvTimers.Size = new System.Drawing.Size(224, 123);
            this.lvTimers.TabIndex = 12;
            this.lvTimers.UseCompatibleStateImageBehavior = false;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Location = new System.Drawing.Point(12, 228);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(0, 13);
            this.lblVersion.TabIndex = 14;
            // 
            // JIST
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(248, 250);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lvTimers);
            this.Controls.Add(this.lblMouseWarning);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.cbIsSetup);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.btnSetup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.MaximizeBox = false;
            this.Name = "JIST";
            this.Opacity = 0.85D;
            this.Text = "J.I.S.T.";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JIST_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.CheckBox cbIsSetup;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Label lblMouseWarning;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ListView lvTimers;
        private System.Windows.Forms.Label lblVersion;
    }
}

