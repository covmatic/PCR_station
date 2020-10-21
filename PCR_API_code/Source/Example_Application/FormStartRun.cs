using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using BioRad.Example_Client_Wrapper;

namespace BioRad.Example_Application
{
    /// <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// It implements a form that collects PCR run parameter information. It is used by the main form 
    /// to collect parameter information prior to starting runs. 
    /// </summary>
    public partial class FormStartRun : Form
    {

        #region Constants

        ///File dialog filter for protocol, LIMS, and PrimePCR files <remarks/>
        public const string c_ProtocolFilter = "(*.prcl,*.plrn,*.csv)|*.prcl;*.plrn;*.csv";

        ///File dialog filter for plate files <remarks/>
        public const string c_PlateFilter = "(*.pltd)|*.pltd";

        ///File dialog filter for data files <remarks/>
        public const string c_DataFilter = "(*.pcrd)|*.pcrd";

        ///File dialog filter for report template files <remarks/>
        public const string c_ReportTemplateFilter = "(*.strt)|*.strt";

        ///Protocol file extension <remarks/>
        public const string c_ProtocolExtension = ".prcl";

        ///LIMS file extension <remarks/>
        public const string c_LIMSExtension = ".plrn";

        ///PrimePCR file extension <remarks/>
        public const string c_PrimePCRExtension = ".csv";

        ///Plate file extension <remarks/>
        public const string c_PlateExtension = ".pltd";

        ///Data file extension <remarks/>
        public const string c_DataExtension = ".pcrd";

        ///Report file extension <remarks/>
        public const string c_ReportExtension = ".pdf";

        ///Report template file extension <remarks/>
        public const string c_ReportTemplateExtension = ".strt";

        #endregion

        #region member data

        ///<remarks>
        ///The base serial number of the CFX system that the protocol is to be run on. This appears as part of the form
        ///title, indicating which instrument the defined run will be started on.
        ///</remarks>
        private string m_serial_number = "Not Assigned";

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// Initializes a new instance of the FormStartRun class. Calls the Designer required InitializeComponent().
        /// Sets the content accessors to their default values, which will get loaded into the content of the various
        /// controls by the form_load event handler.
        /// </summary>
        public FormStartRun()
        {
            InitializeComponent();

            //ProtocolFile = "Browse to select a protocol, LIMS or PrimePCR file to run"; {Original code}
            m_label_protocol.Text = @"C:\PCR_BioRad\protocol_files\KHB_4_genes_protocol.prcl"; //{Edit:Aff23Jun2020}
            ProtocolFile = @"C:\PCR_BioRad\protocol_files\KHB_4_genes_protocol.prcl"; //{Edit:Aff23Jun2020}

            //PlateFile = "Browse to select a plate file"; {Original code}
            m_label_plate.Text = @"C:\PCR_BioRad\plate_files\KHB_4_genes_plate.pltd"; //{Edit:Aff23Jun2020}
            PlateFile = @"C:\PCR_BioRad\plate_files\KHB_4_genes_plate.pltd"; //{Edit:Aff23Jun2020}

            //DataFileFolder = CFXManagerUtilities.CFXManagerUserAdminDirectory; {Original Code}
            DataFileFolder = @"C:\PCR_BioRad\pcrd_results"; //{Edit:Aff23Jun2020}
            m_label_data_folder.Text = @"C:\PCR_BioRad\pcrd_results"; //{Edit:Aff23Jun2020}

            OutputFileName = string.Format("{0}_{1}", "Data", DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture));
            Notes = "Run from the CFX Manager API service Example Application";
            RunID = "Ciao!";
            GenerateReport = false;

            //ReportFolder = CFXManagerUtilities.CFXManagerUserAdminDirectory; {Original Code}
            m_label_report_folder.Text = @"C:\PCR_BioRad\pcrd_results"; //{Edit:Aff23Jun2020}
            ReportFolder = @"C:\PCR_BioRad\pcrd_results"; //{Edit:Aff23Jun2020}

            UseDefaultReportTemplate = true;
            ReportTemplate = "Browse to select a custom report template to use to generate reports";
            EmailResults = false;
            EmailRecipients = string.Empty;
        }

        /// <summary>
        /// The Form_Load event handler. Populate the text of the various controls with the default content
        /// that was set at construction. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormRunParams_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                try
                {
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                    SuspendLayout();
                    m_label_protocol.Text = ProtocolFile;
                    m_label_plate.Text = PlateFile;
                    m_label_data_folder.Text = DataFileFolder;
                    m_textBox_outputfile_name.Text = OutputFileName;
                    m_textBox_notes.Text = Notes;
                    m_textBox_runID.Text = RunID;
                    m_checkBox_generate_report.Checked = GenerateReport;
                    m_checkBox_UseDefaultTemplate.Checked = UseDefaultReportTemplate;
                    m_label_report_folder.Text = ReportFolder;
                    m_label_report_template.Text = ReportTemplate;
                    m_checkBox_email.Checked = EmailResults;
                    m_textBox_email_addresses.Text = EmailRecipients;
                    m_panel_required_params.Focus();
                    Refresh();
                }
                finally
                {
                    ResumeLayout();
                }
            }
        }

        #endregion

        #region Content Accessors

        ///The base serial number of the CFXsystem that the protocol is to be run on<remarks/>
        public string SerialNumber
        {
            get { return m_serial_number; }
            set
            {
                m_serial_number = value;
                this.Text = string.Format("Start Protocol on CFX system {0}", m_serial_number);
            }
        }

        ///The fully qualified file name of the generated data file<remarks/>
        public string DataFile
        {
            get { return Path.Combine(DataFileFolder, string.Format("{0}{1}", OutputFileName, c_DataExtension)); }
        }

        ///The fully qualified file name of the generated report file, when applicable<remarks/>
        public string ReportFile
        {
            get { return Path.Combine(ReportFolder, string.Format("{0}{1}", OutputFileName, c_ReportExtension)); }
        }

        ///The protocol, LIMS, or PrimePCR file to be run<remarks/>
        public string ProtocolFile { get; set; }

        ///The plate file to be used when the protocol file is a real-time .pcrd file<remarks/>
        public string PlateFile { get; set; }

        ///The directory to which the generated data file is written<remarks/>
        private string DataFileFolder { get; set; }

        ///The file name (no path or extension) of the generated data and report files <remarks/>
        public string OutputFileName { get; set; }

        ///Text to appear as "Notes" in the generated data file <remarks/>
        public string Notes { get; set; }

        ///Text to appear as "ID/Bar Code" in the generated data file <remarks/>
        public string RunID { get; set; }

        ///If true, a PDF report will automatically be generated at the end of the run<remarks/>
        public bool GenerateReport { get; set; }

        ///If true, reports will be generated using the CFX Manager default report template.<remarks/>
        public bool UseDefaultReportTemplate { get; set; }

        ///The directory to which the generated report file is written.<remarks/>
        private string ReportFolder { get; set; }

        ///The fully qualified file name of the report template to be used to generate reports, when not using the default template<remarks/>
        public string ReportTemplate { get; set; }

        ///If true, generated data and report files will be Emailed to  specified recipients after the run<remarks/>
        public bool EmailResults { get; set; }

        ///Comma separated list of Email addresses that generated data and report files will be Emailed to. <remarks/>
        public string EmailRecipients { get; set; }

        #endregion

        #region Workflow Methods

        #region utilities

        /// <summary>
        /// Utility used by the file selection button event handlers. Presents a file
        /// selection dialog using the specified title and file type filter, and returns the
        /// selected file.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="title"></param>
        /// <returns>The selected file, or the empty string if selection was cancelled.</returns>
        private string SelectFile(string filter, string title)
        {
            m_dialog_OpenFileDialog.Filter = filter;
            m_dialog_OpenFileDialog.Title = title;
            m_dialog_OpenFileDialog.Multiselect = false;
            m_dialog_OpenFileDialog.InitialDirectory = "";
            if (m_dialog_OpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                return m_dialog_OpenFileDialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Utility used by the directory selection button event handlers. Presents a directory
        /// selection dialog and returns the selected directory.
        /// </summary>
        /// <returns>The selected directory, or null if the selection was cancelled</returns>
        private string SelectFolder()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return folderBrowserDialog.SelectedPath;
                }
            }
            return null;
        }

        #endregion

        #region content management

        /// <summary>
        /// Load the current values of the various form controls into the content accessors for use by the Main Form.
        /// Triggered when the Start Run button is clicked. Performs some basic content validation.
        /// </summary>
        /// <returns>true if content validation succeeds. false if content validation fails.</returns>
        public bool LoadParamsFromForm()
        {

            #region local utility delegates

            // Display a dialog using the bad_param_msg paramter as content
            Action<string> AlertBadContent = (string bad_param_msg) =>
            {
                MessageBox.Show(string.Format("{0}", bad_param_msg), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            };

            // Does the specifed string contain only valid file name characters?
            Func<string, bool> IsLegalFileName = (string fileName) =>
            {
                char[] invalidFileChars = Path.GetInvalidFileNameChars();
                foreach (char invalidChar in invalidFileChars)
                {
                    if (fileName.Contains(invalidChar.ToString()))
                    {
                        return false;
                    }
                }
                return true;
            };

            #endregion

            //LoadParamsFromForm() Entry Point
            ProtocolFile = m_label_protocol.Text;
            if (string.Compare(c_LIMSExtension, Path.GetExtension(ProtocolFile)) == 0 ||
                string.Compare(c_PrimePCRExtension, Path.GetExtension(ProtocolFile)) == 0)
            {
                m_label_plate.Text = string.Empty;
                PlateFile = string.Empty;
            }
            else
            {
                PlateFile = m_label_plate.Text;
            }
            OutputFileName = m_textBox_outputfile_name.Text;
            DataFileFolder = m_label_data_folder.Text;
            Notes = m_textBox_notes.Text;
            RunID = m_textBox_runID.Text;
            GenerateReport = m_checkBox_generate_report.Checked;
            ReportFolder = m_label_report_folder.Text;
            UseDefaultReportTemplate = m_checkBox_UseDefaultTemplate.Checked;
            ReportTemplate = m_checkBox_UseDefaultTemplate.Checked ? String.Empty : m_label_report_template.Text;
            EmailResults = m_checkBox_email.Checked;
            EmailRecipients = m_textBox_email_addresses.Text;
            if (!(string.Compare(c_ProtocolExtension, Path.GetExtension(ProtocolFile)) == 0 ||
                string.Compare(c_LIMSExtension, Path.GetExtension(ProtocolFile)) == 0 ||
                string.Compare(c_PrimePCRExtension, Path.GetExtension(ProtocolFile)) == 0))
            {
                AlertBadContent("A valid protocol, LIMS, or PrimePCR file must be selected as the protocol file.");
                return false;
            }
            if (!(string.Compare(c_PlateExtension, Path.GetExtension(PlateFile)) == 0) && string.Compare(c_ProtocolExtension, Path.GetExtension(ProtocolFile)) == 0)
            {
                AlertBadContent("A valid plate file must be specified.");
                return false;
            }
            if (!(string.Compare(c_ReportTemplateExtension, Path.GetExtension(ReportTemplate)) == 0) && !m_checkBox_UseDefaultTemplate.Checked)
            {
                AlertBadContent("A valid report template file must be specified.");
                return false;
            }
            if (!IsLegalFileName(m_textBox_outputfile_name.Text))
            {
                AlertBadContent("The specified output file name is not a legal filename.");
                return false;
            }
            return true;
        }

        #endregion

        #region control event handlers

        ///Protocol File button was clicked.<summary/>
        private void m_button_protocol_Click(object sender, EventArgs e)
        {
            m_label_protocol.Text = SelectFile(c_ProtocolFilter, "Select a Protocol, LIMS, or PrimePCR file");
        }

        ///Plate File button was clicked.<summary/>
        private void m_button_plate_Click(object sender, EventArgs e)
        {
            m_label_plate.Text = SelectFile(c_PlateFilter, "Select a Plate file");
        }

        ///Data File Folder button was clicked.<summary/>
        private void m_button_datafile_folder_Click(object sender, EventArgs e)
        {
            m_label_data_folder.Text = SelectFolder();
        }

        ///Report Folder button was clicked.<summary/>
        private void m_button_report_folder_Click(object sender, EventArgs e)
        {
            m_label_report_folder.Text = SelectFolder();
        }

        ///Report template button was clicked.<summary/>
        private void m_button_report_tmplate_Click(object sender, EventArgs e)
        {
            m_label_plate.Text = SelectFile(c_ReportTemplateFilter, "Select a Report Template file");
        }

        /// <summary>
        /// Start Run button was clicked. If the content is valid, click the hidden m_button_dialog_OK  which
        /// will return a dialog result of OK to the main form.
        /// <summary/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_StartRun_Click(object sender, EventArgs e)
        {
            if (LoadParamsFromForm())
            {
                m_button_dialog_OK.PerformClick();
            }
        }

        /// <summary>
        /// The hidden m_button_dialog_OK was clicked (by the m_button_StartRun_Click handler). This will cause a 
        /// dialog result of OK to be returned to the main form. Hide this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_dialog_OK_Click(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion

        #endregion

        private void m_label_outputfile_name_heading_Click(object sender, EventArgs e)
        {

        }

        private void m_textBox_outputfile_name_TextChanged(object sender, EventArgs e)
        {

        }

        private void m_label_data_folder_Click(object sender, EventArgs e)
        {

        }

        private void m_label_plate_Click(object sender, EventArgs e)
        {

        }

        private void m_label_protocol_Click(object sender, EventArgs e)
        {

        }
    }
}