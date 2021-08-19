using System;
using System.Text;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using BioRad.Example_Client_Wrapper;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using PCR_Data_Processor;
using System.Collections;

namespace BioRad.Example_Application
{
    /// <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// Implements demonstration application-layer logic using the CFXManagerClientWrapper class as 
    /// middleware to communicate with the CFX Manager API Service. Demonstrates the use of the
    /// most commonly utilized CFX Manager API operations and provides an example of how to use 
    /// instrument status polling to monitor and manage the operation of multiple instruments
    /// simultaneously.
    /// 
    /// Performs CFX Manager version checking. If a compatible version of CFX Manager is running in 
    /// User Mode when the applications is started, it will establish a client connection with that
    /// instance of CFX Manager. If CFX Manager is not running when the application is started, it
    /// will start a Server Mode instance of CFX manager and establish a client connection with 
    /// that instance.
    /// 
    /// Maintains a table of currently connected instruments and pertinent status metrics. Applies
    /// API operations to all currently selected instruments. Logs instrument operational status
    /// changes and operation status change interpretations to a TextBox console.
    /// </summary>
    public partial class MainForm : Form
    {

        #region Constants

        ///The application name that will appear on the title bar of the Main Form<remarks/>
        private static string c_app_name = "Bio-Rad CFX Manager API Example Application";

        ///The interval in milliseconds at which to fire the system timer used to trigger status updates<remarks/>
        protected const double c_status_update_interval = 1000.00;

        ///The interval in milliseconds at which to fire the system timer when attempting to reconnect to the service<remarks/>
        protected const double c_connect_retry_interval = 5000.00;

        #endregion

        #region Member Data

        /// <remarks>
        /// The CFXManagerClientWrapper object that serves as middleware between this application and
        /// the CFX Manager API Service.
        /// </remarks>
        public CFXManagerClientWrapper m_ClientWrapper = new CFXManagerClientWrapper();

        /// <remarks>
        /// The system timer used to trigger regular instrument status updates while the application is  
        /// connected to the CFX Manager API service. 
        /// </remarks>
        private System.Timers.Timer m_status_update_timer = new System.Timers.Timer();

        /// <remarks>
        /// A list of all cyclers that have been detected since the application was started. Used to
        /// recognize if an instrument becomes disconnected/reconnected after being detected.
        /// </remarks>
        private List<string> m_detected_cyclers = new List<string>();

        /// <remarks>
        ///The windows form used to collect the parameters required to start a PCR run via the
        ///CFX Manager API service. 
        /// </remarks>
        public FormStartRun m_StartRunForm = new FormStartRun();

        ///Used as a mutual-exclusion lock during status updates<remarks/>
        object m_update_status_lock = new object();
        private bool m_update_status_running = false;
        object m_task_syncronizing_object = new object();


        /// Flag used to skip status updates when the user is repositioning the application window<remarks/>
        bool m_UI_IsBusy = false;

        ///Flag used to skip status updates when a non-status API operation request is in progress<remarks/>
        bool m_OP_InProgress = false;

        ///Flag used to track connectivity between the client and the CFX Manager API service<remarks/>
        bool m_am_connected = false;

        static int count = 0; // This is a user defined variable. NOT FROM BIO-RAD API. It solves the problem for startrunform
                              // pop-up form opening twice during a run

        static string ui_log_filename = "";

        Queue action_queue = new Queue();

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// MainForm constructor. Call InitializeComponent() as required for Designer support, then call 
        /// InitializeMainForm() to perform application-specific initialization.
        /// </summary>
        public MainForm()
        {
            ui_log_filename = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_log_ui.txt";
            InitializeComponent();
            InitializeMainForm();
        }

        /// <summary>
        /// Initialize the example application. If a compatible version of CFX Manager is executing, establish a connection
        /// with it, otherwise start CFX Manager in server (no UI) mode and establish a connection with that instance. 
        /// Subscribe to the ClientError event published by the client wrapper. Initialize local member data. Present an
        /// explanatory dialog and exit the application if no compatible version of CFX Manager is installed on the host machine,
        /// or if the client fails for any reason to establish a connection with the CFX Manager API service.
        /// </summary>
        private void InitializeMainForm()
        {

            #region local delegates implementing utilities specific to this method

            // Prepare to establish communications with the CFX Manager API using methods provided by the
            // CFXManagerUtilities class. If a compatible version of CFX Manager is installed and is already
            //executing, do nothing. If it is installed, but not executing, attempt to start it in server (no UI) mode. 
            //Returns true IFF a compatible version  of CFX Manager is executing at the completion of the method. 
            Func<bool> ReadyCFXManager = () =>
            {
                //If the service endpoint address is not localhost, skip this logic and return true, making the assumption 
                //that CFX Manager is running on the remote host, since we cannot detect or start it remotely.
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ClientSection clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
                ChannelEndpointElementCollection endpointCollection = clientSection.ElementInformation.Properties[string.Empty].Value as ChannelEndpointElementCollection;
                if (endpointCollection[0].Address.Host != "localhost")
                {
                    LogNewLine("The service endpoint address is not localhost. Skipping CFX Manager detection logic.");
                    return true;
                }

                if (CFXManagerUtilities.APICompatibleCFXManagerIsInstalled)
                {
                    if (!CFXManagerUtilities.CFXManagerIsExecuting)
                    {
                        LogNewLine("Starting CFX Manager in server mode..");
                        //Note that this call will block this thread for up to several seconds while CFX Manager initializes.
                        return CFXManagerUtilities.StartCFXManagerAsServer();
                    }
                    else
                    {
                        LogNewLine("Detected executing instance of CFX Manager..");
                    }
                    return true;
                }
                return false;
            };

            //Initialize the system timer that is used to systematically poll the CFX Manager API for status updates
            Action<Form> StartTimer = (Form child) =>
            {
                this.m_status_update_timer.Interval = c_status_update_interval;
                m_status_update_timer.Elapsed += new ElapsedEventHandler(UpdateTimer_Elapsed);
                m_status_update_timer.SynchronizingObject = child;
                m_status_update_timer.AutoReset = true;
                m_status_update_timer.Start();
            };
            #endregion

            // InitializeMainForm() Entry Point
            this.Text = string.Format(c_app_name);
            LogNewLine(string.Format("{0} is initializing..", c_app_name));
            m_ClientWrapper.ClientError += HandleClientError;
            if (ReadyCFXManager())
            {
                if (m_ClientWrapper.OpenClient())
                {
                    LogNewLine("Client connection established with CFX Manager API service..");
                    m_am_connected = true;
                    StartTimer(this);
                }
                else
                {
                    LogNewLine(string.Format("The {0} was unable to connect to the CFX Manager API service. ", c_app_name));
                    LogNewLine(string.Format("Review the log above for possible causes, then click the close box to exit the application. "));
                    m_panel_buttons.Enabled = false;
                }
            }
            else
            {
                string fail_msg = string.Format("CFX Manager Version {0} or higher must be installed in order to run the {1}. The application will now exit.",
                    CFXManagerUtilities.MiniumumCFXManagerVersion, c_app_name);
                MessageBox.Show(string.Format("{0}", fail_msg), c_app_name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                  MessageBoxDefaultButton.Button1);
                Close();
            }
        }

        #endregion

        #region Methods

        #region status update management

        /// <summary>
        /// If the mouse clicks on the form border (to re-size, move, or close)
        /// disable timer-fired status updates by setting UI_IsBusy to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExampleMainForm_ResizeBegin(object sender, EventArgs e)
        {
            m_UI_IsBusy = true;
        }

        /// <summary>
        /// When a form border mouse click is released, resume timer-fired status updates
        /// by setting UI_IsBusy to false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExampleMainForm_ResizeEnd(object sender, EventArgs e)
        {
            m_UI_IsBusy = false;
        }

        private void UpdateTimer_Elapsed_Thread(object o)
        {
            Console.WriteLine("Thread timer called on thread: " + Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// System Timer Elapsed - Set during initialization to occur every 1 second. Perform an update of the status grid if
        /// the user is not attempting to move the application window, and if no non-status API operation requests are in progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Elapsed(object sender, EventArgs e)
        {
            Log("Timer elapsed on thread: " + Thread.CurrentThread.ManagedThreadId);
            lock (m_update_status_lock)
            {
                if (!m_update_status_running)
                {
                    m_update_status_running = true;
                    Log("Entered :-)");
                    // Do not process events that arrive after the timer has been disabled
                    if (m_status_update_timer.Enabled)
                    {
                        // Do not process events that arrive while the window is being moved, or a non-status API operation request is in progress
                        if (!m_UI_IsBusy && !m_OP_InProgress)
                        {
                            if (!m_am_connected)
                            {
                                //try to reestablish the client connection if it has been lost
                                if (m_ClientWrapper.OpenClient())
                                {
                                    LogNewLine(string.Format("The client has reestablished connectivity with the CFX Manager API service."));
                                    m_am_connected = true;
                                    this.m_status_update_timer.Interval = c_status_update_interval;
                                    m_panel_buttons.Enabled = true;
                                }
                            }
                            if (m_ClientWrapper.ClientIsConnected())
                            {
                                List<CFXManagerClientWrapper.InstrumentStatus> status_list = m_ClientWrapper.GetInstrumentStatus();
                                UpdateStatus(status_list, false);
                            }
                            else if (m_am_connected)
                            {
                                m_am_connected = false;
                                this.m_status_update_timer.Interval = c_connect_retry_interval;
                                m_panel_buttons.Enabled = false;
                                LogNewLine(string.Format("The client has lost connectivity with the CFX Manager API service."));
                                UpdateStatus(null);
                            }
                        }
                    }
                    while (action_queue.Count > 0)
                    {
                        Log("Dequeing!! Count = " + action_queue.Count);
                        var action = action_queue.Dequeue();
                        switch (action)
                        {
                            case "RunProtocol":
                                RunProtocol();
                                break;
                            default:
                                throw new Exception("Action not found: " + action);
                        }
                    }
                    m_update_status_running = false;
                    Log("Timer ended!");
                }
                else
                {
                    Log("Skipped!");
                }
            }
        }

        /// <summary>
        /// Process the specified list of instrument status objects. Update the status grid to reflect the current status 
        /// of all included intruments. Report any errors associated with the included instruments. If the is_op_response
        /// parameter is false, perform new and disconnected instrument detection.
        /// </summary>
        /// <param name="status_list">A List of InstrumentStatus objects to be processed </param>
        /// <param name="is_op_response">
        /// If true, assume the update is a single instrument status recieved in response to a basic API operation request
        /// (Do not perform instrument disconnection detection logic). If false, process as an all-instruments status update, 
        /// and include new and disconnected instrument detection logic. True by default.
        /// </param>
        private void UpdateStatus(List<CFXManagerClientWrapper.InstrumentStatus> status_list, bool is_op_response = true)
        {

            #region local delegates implementing utilities specific to this method

            //Populate the specified DataGridRow with the specified InstrumentStatus record, and process and report any status changes
            Action<CFXManagerClientWrapper.InstrumentStatus, DataGridViewRow, bool> PopulateRow = (CFXManagerClientWrapper.InstrumentStatus instrument_status, DataGridViewRow row, bool adding) =>
            {
                if (!adding)
                {
                    ProcessStatusChange(instrument_status.BaseSerialNumber, row.Cells["Status"].Value.ToString(), instrument_status.Status);
                    if (string.Compare(row.Cells["Status"].Value.ToString(), "Disconnected") == 0)
                    {
                        //Instrument has reconnected, so re-enable its selection checkbox
                        row.Cells["SelectInstrument"].ReadOnly = false;
                    }
                }
                else
                {
                    //Since the adding parameter is true, this is the first detection of this instrument
                    LogNewLine(string.Format("Instrument {0} has been detected.", instrument_status.BaseSerialNumber));
                    row.Cells["SerialNumber"].Value = instrument_status.BaseSerialNumber;
                }
                row.Cells["Status"].Value = instrument_status.Status;
                row.Cells["Step"].Value = instrument_status.ProtocolStep;
                row.Cells["Repeat"].Value = instrument_status.RepeatStep;
                row.Cells["Remaining"].Value = instrument_status.EstimatedTimeRemaining;
                row.Cells["Sample"].Value = instrument_status.SampleTemp.ToString("#0.0");
                row.Cells["Lid"].Value = instrument_status.LidTemp.ToString("#0.0");
                row.Cells["LidPos"].Value = instrument_status.MototrizedLidPosition;
                row.Cells["Type"].Value = instrument_status.BlockType;
                row.Cells["NickName"].Value = instrument_status.BaseName;
            };

            //Review the status list for new instruments and instrument errors. Add any newly detected instruments 
            //to the status grid. Report any instrument errors.
            Action ReviewStatusList = () =>
            {
                foreach (CFXManagerClientWrapper.InstrumentStatus instrument_status in status_list)
                {
                    if (!m_detected_cyclers.Contains(instrument_status.BaseSerialNumber))
                    {
                        m_detected_cyclers.Add(instrument_status.BaseSerialNumber);
                        m_DataViewGrid_CFXSystemsGrid.Rows.Add();
                        PopulateRow(instrument_status, m_DataViewGrid_CFXSystemsGrid.Rows[m_DataViewGrid_CFXSystemsGrid.Rows.Count - 1], true);
                    }

                    if (instrument_status.CurrentError != null)
                    {
                        LogNewLine(string.Format("CFX system {0} reported error code 0x{1:X}: {2}",
                            instrument_status.BaseSerialNumber, instrument_status.CurrentError.Code, instrument_status.CurrentError.Description));
                    }

                    if (m_DataViewGrid_CFXSystemsGrid.Rows.Count > 0)
                    {
                        m_DataViewGrid_CFXSystemsGrid.Rows[0].Cells["SelectInstrument"].Value = true;
                    }
                }
            };

            //Populate the specified row to reflect that the instrument has been disconnected. Disable its selection checkbox
            Action<DataGridViewRow> SetRowDisconnected = (DataGridViewRow row) =>
            {
                LogNewLine(string.Format("Instrument {0} has been disconnected.", row.Cells["SerialNumber"].Value));
                row.Tag = null;
                row.Cells["Status"].Value = "Disconnected";
                row.Cells["Step"].Value = string.Empty;
                row.Cells["Repeat"].Value = string.Empty;
                row.Cells["Remaining"].Value = string.Empty;
                row.Cells["Sample"].Value = string.Empty;
                row.Cells["Lid"].Value = string.Empty;
                row.Cells["LidPos"].Value = string.Empty;
                ((DataGridViewCheckBoxCell)row.Cells["SelectInstrument"]).Value = false;
                row.Cells["SelectInstrument"].ReadOnly = true;
            };

            //Update the status grid to reflect all of the new status records. If the is_op_response parameter is false, look
            //for previously detetcted cyclers that have been disconnected
            Action UpdateAll = () =>
            {
                string current_serial_number = String.Empty;
                CFXManagerClientWrapper.InstrumentStatus current_status;
                foreach (DataGridViewRow current_row in m_DataViewGrid_CFXSystemsGrid.Rows)
                {
                    if (!status_list.Exists(status => string.Compare(status.BaseSerialNumber, current_row.Cells["SerialNumber"].Value.ToString()) == 0))
                    {
                        if (!is_op_response)
                        {
                            if (string.Compare(current_row.Cells["Status"].Value.ToString(), "Disconnected") != 0)
                            {
                                SetRowDisconnected(current_row);
                            }
                        }
                        continue;
                    }
                    current_status = status_list.Find(status => string.Compare(status.BaseSerialNumber, current_row.Cells["SerialNumber"].Value.ToString()) == 0);
                    PopulateRow(current_status, current_row, false);
                }
            };

            #endregion

            //UpdateStatus() entry point
            if (status_list == null)//Connectivity with the API service has been lost
            {
                foreach (DataGridViewRow current_row in m_DataViewGrid_CFXSystemsGrid.Rows)
                {
                    SetRowDisconnected(current_row);
                }
                return;
            }
            ReviewStatusList();
            UpdateAll();
        }

        /// <summary>
        /// Analyze and report on the given status change for the specified instrument. Called when an instrument's status is updated. 
        /// In addition to reporting the specified status change, this method infers and reports on certain high-level workflow 
        /// events such as the starting or ending of protocols based on their unique status-transition signatures.
        /// </summary>
        /// <param name="serial_number">The instrument serial number</param>
        /// <param name="previous_status">The previously detected instrument status</param>
        /// <param name="current_status">The newly detetcted instrumenst status</param>
        private void ProcessStatusChange(string serial_number, string previous_status, string current_status)
        {
            string PCRserial = Convert.ToString(m_StartRunForm.SerialNumber);
            string outputFileName = Convert.ToString(m_StartRunForm.OutputFileName);
            //Only take action if there has been a change
            if (string.Compare(previous_status, current_status) != 0)
            {
                switch (previous_status)
                {
                    case "Disconnected":
                        LogNewLine(string.Format("Instrument {0} has been reconnected.", serial_number));
                        break;
                    case "Idle":
                        switch (current_status)
                        {
                            case "Initializing":
                                LogNewLine(string.Format("Instrument {0} is initializing.", serial_number));
                                break;
                            //This in case the status polling missed the Initializing state for any reason
                            case "Running":
                                LogNewLine(string.Format("A protocol has begun running on instrument {0}", serial_number));
                                break;
                            default:
                                //This is unexpected, except when the instrument goes into error status, which will be further reported on by the error handling logic
                                LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                                break;
                        }
                        break;
                    case "Initializing":
                        switch (current_status)
                        {
                            case "Running":
                                LogNewLine(string.Format("A protocol has begun running on instrument {0}", serial_number));
                                break;
                            default:
                                //This generally should only occur intermittently when transient Idle states between Initializing and Running are caught by the polling
                                LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                                break;
                        }
                        break;
                    case "Running":
                        switch (current_status)
                        {
                            case "Processing":
                                LogNewLine(string.Format("A protocol has finished running on instrument {0}, which is currently processing the data.", serial_number));
                                break;
                            //This in case the status polling missed the Procssing state for any reason
                            case "Idle":
                                LogNewLine(string.Format("A protocol has finished running on instrument {0}", serial_number));
                                break;
                            default:
                                //This generally should generally not occur
                                LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                                break;
                        }
                        break;
                    case "Processing":
                        switch (current_status)
                        {
                            case "Idle":
                                LogNewLine(string.Format("Instrument {0} has completed processing the data from a completed protocol run.", serial_number));
         
                                var t = Task.Run(async delegate
                                {
                                    await Task.Delay(60000);    // API Spec says it can take up to a minute to process data
                                    return 0;
                                });
                                LogNewLine("Waiting for the data to became available... (1 minute)");
                                t.Wait();// wait to allow the PCR to process the data and produce CSV files
                                DataProcess.process_data(PCRserial, outputFileName); //Call the Data Processor to read the CSV files generated
                                m_ClientWrapper.ShutDown(); //Disconnect the PCR from the API Client
                                Application.Exit(); //Close the API application
                                break;
                            default:
                                //This generally should generally not occur
                                LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                                break;
                        }
                        break;
                    case "Synchronizing":
                        switch(current_status)
                        {
                            case "Idle":
                                if (count == 0)
                                {
                                    ++count;
                                    LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                                }
                                break;
                            default:
                                LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                                break;
                        }
                        break;
                    default:
                        //Capture and report other transitions that we are not particularly interested in
                        LogNewLine(string.Format("The status of instrument {0} has changed from {1} to {2}", serial_number, previous_status, current_status));
                        break;
                }
            }
        }

        #endregion

        #region CFX Manager API operations

        /// <summary>
        /// Checks that at least one instrument is selected in the status grid. Called prior to 
        /// attempting to execute button triggered operation requests to enable the calling methods 
        /// to log the case where there are no selected instruments and no operation can be requested. 
        /// </summary>
        /// <returns>true IFF at least one instrument is selected in the status grid</returns>
        private bool AnyInstrumentsSelected()
        {
            foreach (DataGridViewRow row in m_DataViewGrid_CFXSystemsGrid.Rows)
            {
                if (row.Cells["SelectInstrument"].Value != null)
                {
                    if ((bool)(row.Cells["SelectInstrument"].Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Used to execute basic instrument operation requests that require just the instrument serial
        /// number as a parameter. Executes the specified operation request delegate against all cyclers 
        /// that are currently selected in the status grid.
        /// </summary>
        /// <param name="request">
        /// Any of the API request methods defined in CFXManagerClientWrapper class that require only the 
        /// instrument serial number as a parameter.
        /// </param>
        private void RequestOfAllSelectedCyclers(Func<String, List<CFXManagerClientWrapper.InstrumentStatus>> request)
        {
            lock (m_update_status_lock)
            {
                //Turn off timer-triggered status updates during the operation(s)
                m_OP_InProgress = true;
                if (!AnyInstrumentsSelected())
                {
                    LogNewLine(string.Format("Request {0} cannot be processed because no instruments are selected.", request.Method.Name));
                }
                else
                {
                    foreach (DataGridViewRow row in m_DataViewGrid_CFXSystemsGrid.Rows)
                    {
                        if (row.Cells["SelectInstrument"].Value != null)
                        {
                            if ((bool)(row.Cells["SelectInstrument"].Value))
                            {
                                LogNewLine(string.Format("Sending request {0} to CFX system {1}...", request.Method.Name, (string)row.Cells["SerialNumber"].Value));
                                UpdateStatus(request((string)row.Cells["SerialNumber"].Value));
                            }
                        }
                    }
                }
                m_OP_InProgress = false;
            }
        }

        /// <summary>
        /// The Blink button was clicked.
        /// Blink the LED of all cyclers that are checked in the status grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_Blink_Click(object sender, EventArgs e)
        {
            RequestOfAllSelectedCyclers(m_ClientWrapper.Blink);
        }

        /// <summary>
        /// The Open Lid button was clicked.
        /// Open the lids of all cyclers that are checked in the status grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_Open_Click(object sender, EventArgs e)
        {
            RequestOfAllSelectedCyclers(m_ClientWrapper.OpenLid);
        }

        /// <summary>
        /// The Close Lid button was clicked.
        /// Close the lids of all cyclers that are checked in the status grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_Close_Click(object sender, EventArgs e)
        {
            RequestOfAllSelectedCyclers(m_ClientWrapper.CloseLid);
        }

        /// <summary>
        /// The Generate Report button was clicked.
        /// Use the API GenerateReport operation to generate a PDF report of a user selected
        /// data file, using the default CFX Manager report template. The report will be written 
        /// to the same directory as the selected data file. If a report file of the same name
        /// already exists at that location, it will be overridden with the newly generated report. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_GenerateReport_Click(object sender, EventArgs e)
        {

            #region Utility delegates local to this method

            //Display an open file dialog to obtain the target data file filename
            Func<string> GetDataFile = () =>
            {
                string datafile_name = String.Empty;
                using (OpenFileDialog GetDataFileDialog = new OpenFileDialog())
                {
                    GetDataFileDialog.Filter = "(*.pcrd)|*.pcrd";
                    GetDataFileDialog.Title = "Select a Data File";
                    GetDataFileDialog.Multiselect = false;
                    GetDataFileDialog.InitialDirectory = "";
                    if (GetDataFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        datafile_name = GetDataFileDialog.FileName;
                    }
                }
                return datafile_name;
            };

            #endregion

            //m_button_GenerateReport_Click() entry point
            lock (m_update_status_lock)
            {
                //Turn off timer-triggered status updates during the operation(s)
                m_OP_InProgress = true;
                string target_data_file = GetDataFile();
                if (!string.IsNullOrEmpty(target_data_file))
                {
                    string report_file_name = Path.ChangeExtension(target_data_file, "pdf");
                    LogNewLine(string.Format("Generating report: {0}", report_file_name));
                    LogNewLine(string.Format("Note: It could take up to a minute or more for the report to be generated and appear at the specified location"));
                    //GenerateReport returns a complete instrument status list because it is not instrument specific, so we set the UpdateStatus 
                    //is_op_response parameter to false to get a full status update.
                    UpdateStatus(m_ClientWrapper.GenerateReport(target_data_file, string.Empty, report_file_name), false);
                }
                m_OP_InProgress = false;
            }
        }

        /// <summary>
        /// The Start Run button was clicked.
        /// For each selected instrument on the status grid, display the StartRun form to collect the parameters required.
        /// for the RunProtocol operation, and call the client wrappers RunProtocol method using the collected parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_RunProtocol_Click(object sender, EventArgs e)
        {
            action_queue.Enqueue("RunProtocol");
            Log("RunProtocol Enqueued!");
        }

        private void RunProtocol()
        {
            lock (m_update_status_lock)
            {
                Log("Run protocol entered :-)");
                //Turn off timer-triggered status updates during the operation(s)
                m_OP_InProgress = true;
                if (!AnyInstrumentsSelected())
                {
                    LogNewLine(string.Format("Request RunProtocol cannot be processed because no instruments are selected."));
                }
                else
                {
                    foreach (DataGridViewRow row in m_DataViewGrid_CFXSystemsGrid.Rows)
                    {
                        if (row.Cells["SelectInstrument"].Value != null)
                        {
                            if ((bool)(row.Cells["SelectInstrument"].Value))
                            {
                                m_StartRunForm.SerialNumber = row.Cells["SerialNumber"].Value.ToString();
                                DialogResult results = m_StartRunForm.ShowDialog(this);
                                if (results != System.Windows.Forms.DialogResult.OK)
                                {
                                    continue;
                                }
                                LogNewLine(string.Format("Sending RunProtocol to CFX system {0}...", m_StartRunForm.SerialNumber));
                                UpdateStatus(m_ClientWrapper.RunProtocol(
                                             m_StartRunForm.SerialNumber,
                                             m_StartRunForm.ProtocolFile,
                                             m_StartRunForm.PlateFile,
                                             m_StartRunForm.Notes,
                                             m_StartRunForm.RunID,
                                             m_StartRunForm.DataFile,
                                             false,
                                             m_StartRunForm.GenerateReport,
                                             m_StartRunForm.ReportTemplate,
                                             m_StartRunForm.ReportFile,
                                             m_StartRunForm.EmailRecipients));
                            }
                        }
                    }
                }
                m_OP_InProgress = false;
            }
            Log("RunProtocol ended!");
        }

        /// <summary>
        /// The Pause Run button was clicked.
        /// Send a PauseRun request to all cyclers that are checked in the status grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_PauseRun_Click(object sender, EventArgs e)
        {
            RequestOfAllSelectedCyclers(m_ClientWrapper.PauseRun);
        }

        /// <summary>
        /// The Resume Run button was clicked.
        /// Send a ResumeRun request to all cyclers that are checked in the status grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_ResumeRun_Click(object sender, EventArgs e)
        {
            RequestOfAllSelectedCyclers(m_ClientWrapper.ResumeRun);
        }

        /// <summary>
        /// The Stop Run button was clicked.
        /// Send a StopRun request to all cyclers that are checked in the status grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_StopRun_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to STOP the Run?", "???", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                RequestOfAllSelectedCyclers(m_ClientWrapper.StopRun);
            }
        }

        #endregion

        #region error handling and logging

        /// <summary>
        /// Report errors generated by the API client. Most client generated errors are not expected during
        /// normal operation and reflect serious and/or unrecoverable issues. However, certain client errors, such as
        /// exceptions on service response time-outs or on attempts to open the client when the service is not listening
        /// can be expected to occur under anticipated circumstances in most applications, and should be ignored 
        /// or otherwise processed as recoverable errors. Here, failed attempts to open the client when the
        /// service is not running are ignored in order to allow the application to reconnect if CFX Manager
        /// is closed and then re-opened while the application is running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleClientError(object sender, CFXManagerClientWrapper.ClientErrorEventArgs e)
        {
            //Text that appears in the exception message associated with a client open attempt 
            //failing due to no service endpoint
            const string open_fail_text = "no endpoint listening ";

            string error_message;

            //Ignore the exception generated when the client can't open because the service is not listening
            //because the application will by design continually retry to establish a new connection and
            //the exception will continually be thrown until the Service is restarted.
            if (!e.ErrorReport.Description.Contains(open_fail_text))
            {
                if (e.ErrorReport.Code == CFXManagerClientWrapper.ErrorRecord.c_ExceptionCode)
                {
                    error_message = string.Format("An exception ocurred while executing method \"{0}\":{1}", e.ErrorReport.Source, e.ErrorReport.Description);
                }
                else
                {
                    error_message = string.Format("Client Reported an error. Code: 0x{0:X} ; Description: {1} ; Source: {2}",
                        e.ErrorReport.Code, e.ErrorReport.Description, e.ErrorReport.Source);
                }
                LogNewLine(error_message);
            }
        }

        /// <summary>
        /// Write the specified message to the message display textbox on the UI form, keeping
        /// the the text scrolled to the bottom so that the most recent entries are always visible.
        /// </summary>
        /// <param name="message"></param>
        public void LogNewLine(string message)
        {
            m_richTextBox_display.Text += message + "\n";
            m_richTextBox_display.SelectionStart = m_richTextBox_display.Text.Length;
            m_richTextBox_display.ScrollToCaret();
            Log(message, ui_log_filename);
        }

        #endregion


        /// <summary>
        /// Log function
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="filename"></param>
        private static void Log(string logMessage, string filename = "log_ui.txt")
        {
            if (filename != "")
            {
                using (StreamWriter w = File.AppendText(filename))
                {
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine("  :");
                    w.WriteLine($"  :{logMessage}");
                    w.WriteLine("-------------------------------");
                }
            }
        }

        #region Cleanup

        /// <summary>
        /// The close box on the main form has been clicked. Stop the update status timer to avoid
        /// any confusion during the clean-up operations. If the application was communicating with
        /// an externally executing (UI visible) instance of CFX Manager, close the client connection,
        /// which will unregister with the API service and allow other clients to connect to it. If
        /// the application started CFX Manager in server (No UI) mode, then issue the ShutDown command
        /// to properly close the CFX Manager instance. Not using ShutDown to close the CFX Manager
        /// could lead to ghost processes on the host computer which would would prevent the further use 
        /// of CFX Manager in any mode until host re-boot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_status_update_timer != null)
            {
                m_status_update_timer.Stop();
            }
            if (CFXManagerUtilities.CFXManagerWasStartedByApp && CFXManagerUtilities.CFXManagerIsExecuting)
            {
                if (m_ClientWrapper.ClientIsConnected())
                {
                    m_ClientWrapper.ShutDown();
                }
                else
                {
                    CFXManagerUtilities.StopCFXManager();
                }
            }
            else
            {
                m_ClientWrapper.CloseClient();
            }
        }

        #endregion

        #endregion

        private void m_DataViewGrid_CFXSystemsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}