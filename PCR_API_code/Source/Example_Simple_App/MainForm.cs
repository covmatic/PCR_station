using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using BioRad.Example_Client_Wrapper;

namespace BioRad.Example_SimpleApp
{
    /// <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// Implements an simple example application using the CFXManagerClientWrapper class as 
    /// middleware to communicate with the CFX Manager API Service. Demonstrates connecting to
    /// the service, polling for instrument status, and performing basic instrument operations.
    /// Works against the first detected instrument. 
    /// 
    /// CFX Manager must be running in User Mode for the application to function. Clicking the
    /// initialize button will create a client connection to the currently running CFX Manager.
    /// Clicking the disconnect button will close the client connection.
    /// </summary>
    public partial class MainForm : Form
    {

        #region member data

        /// flag tracking whether an instrument is curently connected <remarks/>
        bool m_ConnectedToInstrument = false;

        /// flag tracking whether currently connected to the API service <remarks/>
        bool m_ConnectedToService = false;

        /// Client wrapper instance used to communicate with the API <remarks/>
        public CFXManagerClientWrapper m_CFX = new CFXManagerClientWrapper();

        /// Serial number of the target CFX system (first detected instrument)<remarks/>
        string m_InstrumentSerialNumber;

        ///Used to prevent multiple reporting of client errors<remarks/>
        int m_lastReportedClientError = -1;

        ///Used to prevent multiple reporting of instrument errors<remarks/>
        int m_lastReportedInstrumentError = -1;

        #endregion

        #region construction, initialization, common, and closing

        /// <summary>
        /// Construct the main form. Subscribe to the wrapper's ClientError event
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            m_CFX.ClientError += HandleClientError;
        }

        /// <summary>
        /// Initialize the polling iterval to the value of the polling interval spin box, which is initialized 
        /// to 1000 (milliseconds)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            m_timerStatusPoll.Interval = (int)m_numericUpDownStatusPollInterval.Value;
        }

        /// <summary>
        /// Close the client and save the form properties on close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Used to display a consistently formatted MessageBox for the various infomational messages
        /// presented by the app. 
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMsg(string msg, MessageBoxIcon icon){
            MessageBox.Show(string.Format("{0}", msg), "Bio-Rad Example Simple App", MessageBoxButtons.OK, icon,MessageBoxDefaultButton.Button1);
        }

        #endregion

        #region status polling

        /// <summary>
        /// At each timer tick (polling interval) get the current status of the first detected
        /// instrument and update the status text box on the form. If the instruments reports a
        /// previously unreported error, report with a message box, interpreting device errors as
        /// hex values to reflect the documented CFX system error coding system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_timerStatusPoll_Tick(object sender, EventArgs e)
        {
            m_listBoxCurrentStatus.Items.Clear();
            List<CFXManagerClientWrapper.InstrumentStatus> result = null;
            if (m_ConnectedToService == true)
            {
                result = m_CFX.GetInstrumentStatus();
                if (result != null)
                {
                    if (result.Count == 0)
                    {
                        m_ConnectedToInstrument = false;
                        m_listBoxCurrentStatus.Items.Clear();
                        m_listBoxCurrentStatus.Items.Add("No instruments detected");
                        return;
                    }
                    m_ConnectedToInstrument = true;
                    m_listBoxCurrentStatus.Items.Add(result[0].BaseSerialNumber);
                    m_listBoxCurrentStatus.Items.Add(result[0].MototrizedLidPosition);
                    m_listBoxCurrentStatus.Items.Add(result[0].Status);
                    m_listBoxCurrentStatus.Items.Add(result[0].EstimatedTimeRemaining);
                    m_listBoxCurrentStatus.Items.Add(result[0].SampleTemp);
                    m_listBoxCurrentStatus.Items.Add(result[0].ProtocolStep);
                    m_listBoxCurrentStatus.Items.Add(result[0].RepeatStep);
                    if (result[0].CurrentError != null)
                    {
                        if (m_lastReportedInstrumentError != result[0].CurrentError.Code)
                        {
                            m_lastReportedInstrumentError = result[0].CurrentError.Code;
                           ShowMsg(string.Format("CFX system {0} reported error code 0x{1:X}: {2}",
                               result[0].BaseSerialNumber, result[0].CurrentError.Code, result[0].CurrentError.Description),
                                MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        m_lastReportedInstrumentError = -1;
                    }
                }
                else
                {
                    m_ConnectedToService = false;
                    m_ConnectedToInstrument = false;
                }
            }
            else
            {
                m_listBoxCurrentStatus.Items.Add("Not connected");
            }
        }

        /// <summary>
        /// Update the timer inteval when the value of the interval spin box is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_numericUpDownStatusPollInterval_ValueChanged(object sender, EventArgs e)
        {
            if ((int)m_numericUpDownStatusPollInterval.Value > 0)
            {
                m_timerStatusPoll.Interval = (int)m_numericUpDownStatusPollInterval.Value;
            }
            else
            {
                ShowMsg("Polling interval must be greater than zero", MessageBoxIcon.Hand);
            }
        }

        #endregion

        #region client management

        /// <summary>
        /// The Initialze button is clicked. Open a client and obtain the status of connected intruments
        /// Set application's target instrument to the serial number of the first detected instrument
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonInitialize_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToService)
            {
                ShowMsg("Already connected to the API Service", MessageBoxIcon.Hand);
                return;
            }
            m_ConnectedToService = m_CFX.OpenClient();
            if (m_ConnectedToService)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.GetInstrumentStatus();
                if (result.Count > 0)
                {
                    // take the first instrument found
                    m_InstrumentSerialNumber = result[0].BaseSerialNumber;
                    m_ConnectedToInstrument = true;
                }
            }
            else
            {
               ShowMsg(
                    "Failed to connect to the API service. Be sure CFX Manager is running. A previous client may have been closed without unregistering. Try re-starting CFX Manager.",
                     MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Disconnect from the client. Called when the diconnect button is clicked, and when the form is closed.
        /// </summary>
        private void Disconnect()
        {
            m_ConnectedToInstrument = false;
            //wait for any status poll to finish
            System.Threading.Thread.Sleep(m_timerStatusPoll.Interval);
            m_ConnectedToService = false;
            m_CFX.CloseClient();
        }
        /// <summary>
        /// The disconnect button is clicked. Close the client and stop the polling timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToService)
            {
                Disconnect();
            }
            else
            {
               ShowMsg("Not connected to API Service", MessageBoxIcon.Information );
            }
        }

        /// <summary>
        /// Handle Client Error Events generated by the wrapper when a client operation fails or an unexpected
        /// exception occurs. Presents a message box reporting the error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleClientError(object sender, CFXManagerClientWrapper.ClientErrorEventArgs e)
        {
            if (e.ErrorReport.Code != m_lastReportedClientError)
            {
                m_lastReportedClientError = e.ErrorReport.Code;
               ShowMsg(string.Format("Client Error: {0} (Error Code: 0x{1:X})", e.ErrorReport.Description, e.ErrorReport.Code), MessageBoxIcon.Error);
            }
        }

        #endregion

        #region parameter garnering

        /// <summary>
        /// The browse protocol button is clicked. Present the open dialog form that is integrated 
        /// with this form to obtain the protocol file name. This application does not accept LIMS
        /// or PrimePCR files. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonBrowseProtocolFile_Click(object sender, EventArgs e)
        {
            m_openFileDialog.Filter = "Protocol Files|*.prcl";
            DialogResult result = m_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_textBoxProtocolFile.Text = m_openFileDialog.FileName;
            }
        }

        /// <summary>
        /// The browse plate button is clicked. Present the open dialog form that is integrated 
        /// with this form to obtain the plate file name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonBrowsePlateFile_Click(object sender, EventArgs e)
        {
            m_openFileDialog.Filter = "Plate Files|*.pltd";
            DialogResult result = m_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_textBoxPlateFile.Text = m_openFileDialog.FileName;
            }
        }

        /// <summary>
        /// The browse data folder button is clicked. Present the select folder dialog form 
        /// that is integrated with this form to obtain the data file folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonBrowseDataFolder_Click(object sender, EventArgs e)
        {

            DialogResult result = m_folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_textBoxOutputDataFolder.Text = m_folderBrowserDialog.SelectedPath;
            }
        }

        #endregion

        #region Instrument Control Operations

        /// <summary>
        /// The Open Lid button is clicked. Send an open lid request to the instrument, or present a 
        /// message box if no instruments are connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonOpenLid_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToInstrument == true)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.OpenLid(m_InstrumentSerialNumber);
            }
            else
            {
                ShowMsg("No instrument connected",MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// The Close Lid button is clicked. Send an close lid request to the instrument, or present a 
        /// message box if not instruments are connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonCloseLid_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToInstrument == true)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.CloseLid(m_InstrumentSerialNumber);
            }
            else
            {
                ShowMsg("No instrument connected",MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// The Pause button is clicked. Send an pause request to the instrument, or present a 
        /// message box if not instruments are connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonPauseRun_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToInstrument == true)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.PauseRun(m_InstrumentSerialNumber);
            }
            else
            {
                ShowMsg("No instrument connected",MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// The Resume button is clicked. Send an resume request to the instrument, or present a 
        /// message box if not instruments are connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonResumeRun_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToInstrument == true)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.ResumeRun(m_InstrumentSerialNumber);
            }
            else
            {
                ShowMsg("No instrument connected",MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// The Start Run button is clicked. Send an start run request to the instrument, using the currently
        /// garnered protocol file, plate file, and output director. Name the output file "myDataFile.pcrd" 
        /// (This will be overwritten when ever a new run with the same outpurt directory is performed). Set
        /// the run id and note field to the empty string. Do not generate a report. Do not lock the
        /// instrument pane. Do not send post run emails. If no instrument is detected, present a message box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonStartRun_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToInstrument == true)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.RunProtocol(m_InstrumentSerialNumber,
                    m_textBoxProtocolFile.Text,
                    m_textBoxPlateFile.Text,
                    "",  // run note
                    "",  // run ID
                    Path.Combine(m_textBoxOutputDataFolder.Text, "myDataFile.pcrd"),
                    false,  // lock instrument panel
                    false,  // generate report
                    "",  // report template
                    "",  // report file name
                    "");  // email addresses

            }
            else
            {
                ShowMsg("No instrument connected",MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// The Stop Run button is clicked. Send an stop request to the instrument, or present a 
        /// message box if not instruments are connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonStopRun_Click(object sender, EventArgs e)
        {
            if (m_ConnectedToInstrument == true)
            {
                List<CFXManagerClientWrapper.InstrumentStatus> result = m_CFX.StopRun(m_InstrumentSerialNumber);
            }
            else
            {
                ShowMsg("No instrument connected",MessageBoxIcon.Information);
            }
        }

        #endregion

    }
}
