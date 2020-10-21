namespace BioRad.Example_SimpleApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_buttonPauseRun = new System.Windows.Forms.Button();
            this.m_listBoxCurrentStatus = new System.Windows.Forms.ListBox();
            this.m_buttonDisconnect = new System.Windows.Forms.Button();
            this.m_buttonStopRun = new System.Windows.Forms.Button();
            this.m_buttonStartRun = new System.Windows.Forms.Button();
            this.m_timerStatusPoll = new System.Windows.Forms.Timer(this.components);
            this.m_buttonResumeRun = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.m_numericUpDownStatusPollInterval = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.m_buttonCloseLid = new System.Windows.Forms.Button();
            this.m_buttonOpenLid = new System.Windows.Forms.Button();
            this.m_folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.m_buttonBrowseDataFolder = new System.Windows.Forms.Button();
            this.m_textBoxOutputDataFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_textBoxPlateFile = new System.Windows.Forms.TextBox();
            this.m_textBoxProtocolFile = new System.Windows.Forms.TextBox();
            this.m_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.m_buttonInitialize = new System.Windows.Forms.Button();
            this.m_buttonBrowsePlateFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.m_buttonBrowseProtocolFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownStatusPollInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // m_buttonPauseRun
            // 
            this.m_buttonPauseRun.Location = new System.Drawing.Point(506, 258);
            this.m_buttonPauseRun.Name = "m_buttonPauseRun";
            this.m_buttonPauseRun.Size = new System.Drawing.Size(126, 23);
            this.m_buttonPauseRun.TabIndex = 45;
            this.m_buttonPauseRun.Text = "Pause Run";
            this.m_buttonPauseRun.UseVisualStyleBackColor = true;
            this.m_buttonPauseRun.Click += new System.EventHandler(this.m_buttonPauseRun_Click);
            // 
            // m_listBoxCurrentStatus
            // 
            this.m_listBoxCurrentStatus.FormattingEnabled = true;
            this.m_listBoxCurrentStatus.Location = new System.Drawing.Point(208, 170);
            this.m_listBoxCurrentStatus.Name = "m_listBoxCurrentStatus";
            this.m_listBoxCurrentStatus.Size = new System.Drawing.Size(210, 134);
            this.m_listBoxCurrentStatus.TabIndex = 44;
            // 
            // m_buttonDisconnect
            // 
            this.m_buttonDisconnect.Location = new System.Drawing.Point(34, 215);
            this.m_buttonDisconnect.Name = "m_buttonDisconnect";
            this.m_buttonDisconnect.Size = new System.Drawing.Size(126, 23);
            this.m_buttonDisconnect.TabIndex = 43;
            this.m_buttonDisconnect.Text = "Disconnect";
            this.m_buttonDisconnect.UseVisualStyleBackColor = true;
            this.m_buttonDisconnect.Click += new System.EventHandler(this.m_buttonDisconnect_Click);
            // 
            // m_buttonStopRun
            // 
            this.m_buttonStopRun.Location = new System.Drawing.Point(506, 215);
            this.m_buttonStopRun.Name = "m_buttonStopRun";
            this.m_buttonStopRun.Size = new System.Drawing.Size(126, 23);
            this.m_buttonStopRun.TabIndex = 42;
            this.m_buttonStopRun.Text = "Stop Run";
            this.m_buttonStopRun.UseVisualStyleBackColor = true;
            this.m_buttonStopRun.Click += new System.EventHandler(this.m_buttonStopRun_Click);
            // 
            // m_buttonStartRun
            // 
            this.m_buttonStartRun.Location = new System.Drawing.Point(506, 188);
            this.m_buttonStartRun.Name = "m_buttonStartRun";
            this.m_buttonStartRun.Size = new System.Drawing.Size(126, 23);
            this.m_buttonStartRun.TabIndex = 41;
            this.m_buttonStartRun.Text = "Start Run";
            this.m_buttonStartRun.UseVisualStyleBackColor = true;
            this.m_buttonStartRun.Click += new System.EventHandler(this.m_buttonStartRun_Click);
            // 
            // m_timerStatusPoll
            // 
            this.m_timerStatusPoll.Enabled = true;
            this.m_timerStatusPoll.Tick += new System.EventHandler(this.m_timerStatusPoll_Tick);
            // 
            // m_buttonResumeRun
            // 
            this.m_buttonResumeRun.Location = new System.Drawing.Point(506, 287);
            this.m_buttonResumeRun.Name = "m_buttonResumeRun";
            this.m_buttonResumeRun.Size = new System.Drawing.Size(126, 23);
            this.m_buttonResumeRun.TabIndex = 46;
            this.m_buttonResumeRun.Text = "Resume Run";
            this.m_buttonResumeRun.UseVisualStyleBackColor = true;
            this.m_buttonResumeRun.Click += new System.EventHandler(this.m_buttonResumeRun_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(208, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Current Instrument Status";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(354, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "milliseconds";
            // 
            // m_numericUpDownStatusPollInterval
            // 
            this.m_numericUpDownStatusPollInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.m_numericUpDownStatusPollInterval.Location = new System.Drawing.Point(286, 117);
            this.m_numericUpDownStatusPollInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.m_numericUpDownStatusPollInterval.Name = "m_numericUpDownStatusPollInterval";
            this.m_numericUpDownStatusPollInterval.Size = new System.Drawing.Size(61, 20);
            this.m_numericUpDownStatusPollInterval.TabIndex = 38;
            this.m_numericUpDownStatusPollInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.m_numericUpDownStatusPollInterval.ValueChanged += new System.EventHandler(this.m_numericUpDownStatusPollInterval_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(205, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Status Interval";
            // 
            // m_buttonCloseLid
            // 
            this.m_buttonCloseLid.Location = new System.Drawing.Point(506, 143);
            this.m_buttonCloseLid.Name = "m_buttonCloseLid";
            this.m_buttonCloseLid.Size = new System.Drawing.Size(126, 23);
            this.m_buttonCloseLid.TabIndex = 36;
            this.m_buttonCloseLid.Text = "Close Lid";
            this.m_buttonCloseLid.UseVisualStyleBackColor = true;
            this.m_buttonCloseLid.Click += new System.EventHandler(this.m_buttonCloseLid_Click);
            // 
            // m_buttonOpenLid
            // 
            this.m_buttonOpenLid.Location = new System.Drawing.Point(506, 114);
            this.m_buttonOpenLid.Name = "m_buttonOpenLid";
            this.m_buttonOpenLid.Size = new System.Drawing.Size(126, 23);
            this.m_buttonOpenLid.TabIndex = 35;
            this.m_buttonOpenLid.Text = "Open Lid";
            this.m_buttonOpenLid.UseVisualStyleBackColor = true;
            this.m_buttonOpenLid.Click += new System.EventHandler(this.m_buttonOpenLid_Click);
            // 
            // m_buttonBrowseDataFolder
            // 
            this.m_buttonBrowseDataFolder.Location = new System.Drawing.Point(638, 63);
            this.m_buttonBrowseDataFolder.Name = "m_buttonBrowseDataFolder";
            this.m_buttonBrowseDataFolder.Size = new System.Drawing.Size(75, 23);
            this.m_buttonBrowseDataFolder.TabIndex = 33;
            this.m_buttonBrowseDataFolder.Text = "Browse...";
            this.m_buttonBrowseDataFolder.UseVisualStyleBackColor = true;
            this.m_buttonBrowseDataFolder.Click += new System.EventHandler(this.m_buttonBrowseDataFolder_Click);
            // 
            // m_textBoxOutputDataFolder
            // 
            this.m_textBoxOutputDataFolder.Location = new System.Drawing.Point(115, 66);
            this.m_textBoxOutputDataFolder.Name = "m_textBoxOutputDataFolder";
            this.m_textBoxOutputDataFolder.Size = new System.Drawing.Size(517, 20);
            this.m_textBoxOutputDataFolder.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Output Data Folder";
            // 
            // m_textBoxPlateFile
            // 
            this.m_textBoxPlateFile.Location = new System.Drawing.Point(83, 37);
            this.m_textBoxPlateFile.Name = "m_textBoxPlateFile";
            this.m_textBoxPlateFile.Size = new System.Drawing.Size(549, 20);
            this.m_textBoxPlateFile.TabIndex = 29;
            // 
            // m_textBoxProtocolFile
            // 
            this.m_textBoxProtocolFile.Location = new System.Drawing.Point(83, 9);
            this.m_textBoxProtocolFile.Name = "m_textBoxProtocolFile";
            this.m_textBoxProtocolFile.Size = new System.Drawing.Size(549, 20);
            this.m_textBoxProtocolFile.TabIndex = 26;
            // 
            // m_buttonInitialize
            // 
            this.m_buttonInitialize.Location = new System.Drawing.Point(34, 186);
            this.m_buttonInitialize.Name = "m_buttonInitialize";
            this.m_buttonInitialize.Size = new System.Drawing.Size(126, 23);
            this.m_buttonInitialize.TabIndex = 34;
            this.m_buttonInitialize.Text = "Initialize";
            this.m_buttonInitialize.UseVisualStyleBackColor = true;
            this.m_buttonInitialize.Click += new System.EventHandler(this.m_buttonInitialize_Click);
            // 
            // m_buttonBrowsePlateFile
            // 
            this.m_buttonBrowsePlateFile.Location = new System.Drawing.Point(638, 34);
            this.m_buttonBrowsePlateFile.Name = "m_buttonBrowsePlateFile";
            this.m_buttonBrowsePlateFile.Size = new System.Drawing.Size(75, 23);
            this.m_buttonBrowsePlateFile.TabIndex = 30;
            this.m_buttonBrowsePlateFile.Text = "Browse...";
            this.m_buttonBrowsePlateFile.UseVisualStyleBackColor = true;
            this.m_buttonBrowsePlateFile.Click += new System.EventHandler(this.m_buttonBrowsePlateFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Plate File";
            // 
            // m_buttonBrowseProtocolFile
            // 
            this.m_buttonBrowseProtocolFile.Location = new System.Drawing.Point(638, 7);
            this.m_buttonBrowseProtocolFile.Name = "m_buttonBrowseProtocolFile";
            this.m_buttonBrowseProtocolFile.Size = new System.Drawing.Size(75, 23);
            this.m_buttonBrowseProtocolFile.TabIndex = 27;
            this.m_buttonBrowseProtocolFile.Text = "Browse...";
            this.m_buttonBrowseProtocolFile.UseVisualStyleBackColor = true;
            this.m_buttonBrowseProtocolFile.Click += new System.EventHandler(this.m_buttonBrowseProtocolFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Protocol File";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 354);
            this.Controls.Add(this.m_buttonPauseRun);
            this.Controls.Add(this.m_listBoxCurrentStatus);
            this.Controls.Add(this.m_buttonDisconnect);
            this.Controls.Add(this.m_buttonStopRun);
            this.Controls.Add(this.m_buttonStartRun);
            this.Controls.Add(this.m_buttonResumeRun);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.m_numericUpDownStatusPollInterval);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_buttonCloseLid);
            this.Controls.Add(this.m_buttonOpenLid);
            this.Controls.Add(this.m_buttonBrowseDataFolder);
            this.Controls.Add(this.m_textBoxOutputDataFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_textBoxPlateFile);
            this.Controls.Add(this.m_textBoxProtocolFile);
            this.Controls.Add(this.m_buttonInitialize);
            this.Controls.Add(this.m_buttonBrowsePlateFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_buttonBrowseProtocolFile);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(743, 393);
            this.MinimumSize = new System.Drawing.Size(743, 393);
            this.Name = "MainForm";
            this.Text = "Bio-Rad CFX Manager API - Simple Example";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownStatusPollInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_buttonPauseRun;
        private System.Windows.Forms.ListBox m_listBoxCurrentStatus;
        private System.Windows.Forms.Button m_buttonDisconnect;
        private System.Windows.Forms.Button m_buttonStopRun;
        private System.Windows.Forms.Button m_buttonStartRun;
        private System.Windows.Forms.Timer m_timerStatusPoll;
        private System.Windows.Forms.Button m_buttonResumeRun;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown m_numericUpDownStatusPollInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button m_buttonCloseLid;
        private System.Windows.Forms.Button m_buttonOpenLid;
        private System.Windows.Forms.FolderBrowserDialog m_folderBrowserDialog;
        private System.Windows.Forms.Button m_buttonBrowseDataFolder;
        private System.Windows.Forms.TextBox m_textBoxOutputDataFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox m_textBoxPlateFile;
        private System.Windows.Forms.TextBox m_textBoxProtocolFile;
        private System.Windows.Forms.OpenFileDialog m_openFileDialog;
        private System.Windows.Forms.Button m_buttonInitialize;
        private System.Windows.Forms.Button m_buttonBrowsePlateFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button m_buttonBrowseProtocolFile;
        private System.Windows.Forms.Label label1;
    }
}