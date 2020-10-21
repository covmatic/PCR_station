namespace BioRad.Example_Application
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.m_panel_buttons = new System.Windows.Forms.Panel();
            this.m_button_Open = new System.Windows.Forms.Button();
            this.m_button_Blink = new System.Windows.Forms.Button();
            this.m_button_RunProtocol = new System.Windows.Forms.Button();
            this.m_button_Close = new System.Windows.Forms.Button();
            this.m_button_GenerateReport = new System.Windows.Forms.Button();
            this.m_button_StopRun = new System.Windows.Forms.Button();
            this.m_button_PauseRun = new System.Windows.Forms.Button();
            this.m_button_ResumeRun = new System.Windows.Forms.Button();
            this.m_DataViewGrid_CFXSystemsGrid = new System.Windows.Forms.DataGridView();
            this.SelectInstrument = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NickName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LidPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Step = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Repeat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remaining = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sample = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_richTextBox_display = new System.Windows.Forms.RichTextBox();
            this.m_panel_buttons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_DataViewGrid_CFXSystemsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // m_panel_buttons
            // 
            this.m_panel_buttons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_panel_buttons.Controls.Add(this.m_button_Open);
            this.m_panel_buttons.Controls.Add(this.m_button_Blink);
            this.m_panel_buttons.Controls.Add(this.m_button_RunProtocol);
            this.m_panel_buttons.Controls.Add(this.m_button_Close);
            this.m_panel_buttons.Controls.Add(this.m_button_GenerateReport);
            this.m_panel_buttons.Controls.Add(this.m_button_StopRun);
            this.m_panel_buttons.Controls.Add(this.m_button_PauseRun);
            this.m_panel_buttons.Controls.Add(this.m_button_ResumeRun);
            resources.ApplyResources(this.m_panel_buttons, "m_panel_buttons");
            this.m_panel_buttons.Name = "m_panel_buttons";
            // 
            // m_button_Open
            // 
            resources.ApplyResources(this.m_button_Open, "m_button_Open");
            this.m_button_Open.Name = "m_button_Open";
            this.m_button_Open.UseVisualStyleBackColor = true;
            this.m_button_Open.Click += new System.EventHandler(this.m_button_Open_Click);
            // 
            // m_button_Blink
            // 
            resources.ApplyResources(this.m_button_Blink, "m_button_Blink");
            this.m_button_Blink.Name = "m_button_Blink";
            this.m_button_Blink.UseVisualStyleBackColor = true;
            this.m_button_Blink.Click += new System.EventHandler(this.m_button_Blink_Click);
            // 
            // m_button_RunProtocol
            // 
            resources.ApplyResources(this.m_button_RunProtocol, "m_button_RunProtocol");
            this.m_button_RunProtocol.Name = "m_button_RunProtocol";
            this.m_button_RunProtocol.UseVisualStyleBackColor = true;
            this.m_button_RunProtocol.Click += new System.EventHandler(this.m_button_RunProtocol_Click);
            // 
            // m_button_Close
            // 
            resources.ApplyResources(this.m_button_Close, "m_button_Close");
            this.m_button_Close.Name = "m_button_Close";
            this.m_button_Close.UseVisualStyleBackColor = true;
            this.m_button_Close.Click += new System.EventHandler(this.m_button_Close_Click);
            // 
            // m_button_GenerateReport
            // 
            resources.ApplyResources(this.m_button_GenerateReport, "m_button_GenerateReport");
            this.m_button_GenerateReport.Name = "m_button_GenerateReport";
            this.m_button_GenerateReport.UseVisualStyleBackColor = true;
            this.m_button_GenerateReport.Click += new System.EventHandler(this.m_button_GenerateReport_Click);
            // 
            // m_button_StopRun
            // 
            resources.ApplyResources(this.m_button_StopRun, "m_button_StopRun");
            this.m_button_StopRun.Name = "m_button_StopRun";
            this.m_button_StopRun.UseVisualStyleBackColor = true;
            this.m_button_StopRun.Click += new System.EventHandler(this.m_button_StopRun_Click);
            // 
            // m_button_PauseRun
            // 
            resources.ApplyResources(this.m_button_PauseRun, "m_button_PauseRun");
            this.m_button_PauseRun.Name = "m_button_PauseRun";
            this.m_button_PauseRun.UseVisualStyleBackColor = true;
            this.m_button_PauseRun.Click += new System.EventHandler(this.m_button_PauseRun_Click);
            // 
            // m_button_ResumeRun
            // 
            resources.ApplyResources(this.m_button_ResumeRun, "m_button_ResumeRun");
            this.m_button_ResumeRun.Name = "m_button_ResumeRun";
            this.m_button_ResumeRun.UseVisualStyleBackColor = true;
            this.m_button_ResumeRun.Click += new System.EventHandler(this.m_button_ResumeRun_Click);
            // 
            // m_DataViewGrid_CFXSystemsGrid
            // 
            this.m_DataViewGrid_CFXSystemsGrid.AllowUserToAddRows = false;
            this.m_DataViewGrid_CFXSystemsGrid.AllowUserToDeleteRows = false;
            this.m_DataViewGrid_CFXSystemsGrid.AllowUserToResizeColumns = false;
            this.m_DataViewGrid_CFXSystemsGrid.AllowUserToResizeRows = false;
            this.m_DataViewGrid_CFXSystemsGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_DataViewGrid_CFXSystemsGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.m_DataViewGrid_CFXSystemsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_DataViewGrid_CFXSystemsGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.m_DataViewGrid_CFXSystemsGrid, "m_DataViewGrid_CFXSystemsGrid");
            this.m_DataViewGrid_CFXSystemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.m_DataViewGrid_CFXSystemsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectInstrument,
            this.SerialNumber,
            this.NickName,
            this.Type,
            this.Status,
            this.LidPos,
            this.Step,
            this.Repeat,
            this.Remaining,
            this.Sample,
            this.Lid});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.m_DataViewGrid_CFXSystemsGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.m_DataViewGrid_CFXSystemsGrid.MultiSelect = false;
            this.m_DataViewGrid_CFXSystemsGrid.Name = "m_DataViewGrid_CFXSystemsGrid";
            this.m_DataViewGrid_CFXSystemsGrid.RowHeadersVisible = false;
            this.m_DataViewGrid_CFXSystemsGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_DataViewGrid_CFXSystemsGrid_CellContentClick);
            // 
            // SelectInstrument
            // 
            this.SelectInstrument.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SelectInstrument.FillWeight = 55.8493F;
            resources.ApplyResources(this.SelectInstrument, "SelectInstrument");
            this.SelectInstrument.Name = "SelectInstrument";
            // 
            // SerialNumber
            // 
            this.SerialNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SerialNumber.FillWeight = 71.62913F;
            resources.ApplyResources(this.SerialNumber, "SerialNumber");
            this.SerialNumber.Name = "SerialNumber";
            this.SerialNumber.ReadOnly = true;
            // 
            // NickName
            // 
            this.NickName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.NickName.FillWeight = 68.52369F;
            resources.ApplyResources(this.NickName, "NickName");
            this.NickName.Name = "NickName";
            this.NickName.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Type.FillWeight = 63.88653F;
            resources.ApplyResources(this.Type, "Type");
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Status.FillWeight = 59.43943F;
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // LidPos
            // 
            this.LidPos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.LidPos, "LidPos");
            this.LidPos.Name = "LidPos";
            this.LidPos.ReadOnly = true;
            // 
            // Step
            // 
            this.Step.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Step.FillWeight = 54.31652F;
            resources.ApplyResources(this.Step, "Step");
            this.Step.Name = "Step";
            this.Step.ReadOnly = true;
            // 
            // Repeat
            // 
            this.Repeat.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Repeat.FillWeight = 48.4369F;
            resources.ApplyResources(this.Repeat, "Repeat");
            this.Repeat.Name = "Repeat";
            this.Repeat.ReadOnly = true;
            // 
            // Remaining
            // 
            this.Remaining.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Remaining.FillWeight = 41.71046F;
            resources.ApplyResources(this.Remaining, "Remaining");
            this.Remaining.Name = "Remaining";
            this.Remaining.ReadOnly = true;
            // 
            // Sample
            // 
            this.Sample.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Sample.FillWeight = 34.03673F;
            resources.ApplyResources(this.Sample, "Sample");
            this.Sample.Name = "Sample";
            this.Sample.ReadOnly = true;
            // 
            // Lid
            // 
            this.Lid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Lid.FillWeight = 25.30383F;
            resources.ApplyResources(this.Lid, "Lid");
            this.Lid.Name = "Lid";
            this.Lid.ReadOnly = true;
            // 
            // m_richTextBox_display
            // 
            this.m_richTextBox_display.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.m_richTextBox_display, "m_richTextBox_display");
            this.m_richTextBox_display.Name = "m_richTextBox_display";
            this.m_richTextBox_display.ReadOnly = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_DataViewGrid_CFXSystemsGrid);
            this.Controls.Add(this.m_panel_buttons);
            this.Controls.Add(this.m_richTextBox_display);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeBegin += new System.EventHandler(this.ExampleMainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.ExampleMainForm_ResizeEnd);
            this.m_panel_buttons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_DataViewGrid_CFXSystemsGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_button_Close;
        private System.Windows.Forms.Button m_button_Open;
        private System.Windows.Forms.DataGridView m_DataViewGrid_CFXSystemsGrid;
        private System.Windows.Forms.Button m_button_PauseRun;
        private System.Windows.Forms.Button m_button_Blink;
        private System.Windows.Forms.Button m_button_ResumeRun;
        private System.Windows.Forms.Button m_button_StopRun;
        private System.Windows.Forms.Button m_button_RunProtocol;
        private System.Windows.Forms.Button m_button_GenerateReport;
        private System.Windows.Forms.RichTextBox m_richTextBox_display;
        private System.Windows.Forms.Panel m_panel_buttons;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectInstrument;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn NickName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn LidPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn Step;
        private System.Windows.Forms.DataGridViewTextBoxColumn Repeat;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remaining;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sample;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lid;
    }
}