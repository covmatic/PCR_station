using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using BioRad.Example_Client;
using System.Threading;

namespace BioRad.Example_Client_Wrapper
{
    /// <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// This class implements a high-level client interface to the CFX Manager API suitable for consumption by
    /// application-level logic. 
    /// 
    /// It references the CFXManagerClient class to communicate with CXF Manager service API, and implements private
    /// logic to encapsulate various low-level details, such as client registration management, XML encoding and decoding, 
    /// and connectivity management. Presents API operations to consumers as parameterized methods which return 
    /// simplified instrument status records. Wraps server-side errors and client exceptions in an error event.
    /// </summary>
    public class CFXManagerClientWrapper
    {

        #region member data

        /// Used to lock critical sections <remarks/>
        private object m_lock_object = new Object();

        ///<remarks>
        /// The registration ID associated with the client session. The service uses the ID to guarantee 
        /// that only one client is able to interface with the API service at a time. The ID is a required 
        /// parameter to all non-registration service requests. This class manages the registration process 
        /// and the registration ID internally, hiding the details from the consumer application.
        /// </remarks>
        private string m_registrationID;

        /// <remarks>
        /// The MessageType runtime object used to send requests to the client. It is re-used/re-populated
        /// for each service request. MessageType is the XML structure used to specify requests to the 
        /// CFX Manager API service. MessageType includes an item field which contains the request type being 
        /// sent, where the request type corresponds to one of the CFX Manage API requests supported by the schema. 
        /// The different request type runtime object member variables defined in the remainder of this region 
        /// correspond to each of the supported API requests. At request time, a given request runtime object is 
        /// populated with appropriate member data and attached to m_MasterMessage in order to create the runtime 
        /// object representation of the request. m_MasterMessage is then serialized and forwarded to the client 
        /// for transmission of the request to the service. The schema runtime objects used to implement this 
        /// framework have been generated from the API schema using the Microsoft XML Schema Definition Tool 
        /// (Xsd.exe), and are defined in the source file CfxComSchema.cs. 
        /// </remarks>
        private static MessageType m_MasterMessage = new MessageType();

        private static RegisterServiceType m_Register_Item = new RegisterServiceType();
        private static UnRegisterServiceType m_Unregister_Item = new UnRegisterServiceType();
        private static OpenLidType m_OpenLid_Item = new OpenLidType();
        private static CloseLidType m_CloseLid_Item = new CloseLidType();
        private static FlashLedType m_Blink_Item = new FlashLedType();
        private static StopRunType m_Stop_Item = new StopRunType();
        private static PauseRunType m_Pause_Item = new PauseRunType();
        private static ResumeRunType m_Resume_Item = new ResumeRunType();
        private static RunProtocolType m_RunProtocol_Item = new RunProtocolType();
        private static GenerateReportType m_GenerateReport_Item = new GenerateReportType();
        private static ShutDownType m_ShutDown_Item = new ShutDownType();
        private static QueryInstrumentBlocksType m_InstrumentStatus_Item = new QueryInstrumentBlocksType();

        private static int m_RunProtocol_Timeout = 30;      // [s] Timeout for the communication channel

        #endregion

        #region Schema object wrappers

        /// <summary>
        /// String representations of CFX system status values as defined in the schema by 
        /// the StatusType enumeration. 
        /// </summary>
        public class InstrumentStatusValue
        {
            /// <remarks/>
            public static string Idle = "Idle";
            /// <remarks/>
            public static string Paused = "Paused";
            /// <remarks/>
            public static string Running = "Running";
            /// <remarks/>
            public static string InfiniteHold = "Infinite Hold";
            /// <remarks/>
            public static string Synchronizing = "Synchronizing";
            /// <remarks/>
            public static string Calibrating = "Calibrating";
            /// <remarks/>
            public static string LidOpen = "Lid Open";
            /// <remarks/>
            public static string Initializing = "Initializing";
            /// <remarks/>
            public static string WaitingManualStart = "Waiting Manual Start";
            /// <remarks/>
            public static string Processing = "Processing";
            /// <remarks/>
            public static string Preserving = "Preserving";
            /// <remarks/>
            public static string Error = "Error";
            /// <remarks/>
            public static string LocalRun = "Local Run";
        }

        /// <summary>
        /// String representations of CFX motorized lid position values as enumerated in the schema. These
        /// are used in constructing the simplified status objects that this class provides to the consumer
        /// application.
        /// </summary>
        public struct MotorizedLidPosition
        {
            /// <remarks/>
            public const string OPEN = "OPEN";
            /// <remarks/>
            public const string CLOSED = "CLOSED";
            /// <remarks/>
            public const string OPENING = "OPENING";
            /// <remarks/>
            public const string CLOSING = "CLOSING";
            /// <remarks/>
            public const string UNKNOWN = "UNKNOWN";
            /// <remarks/>
            public const string MANUAL = "MANUAL";
            /// <remarks/>
            public const string NONE = "NONE";
            /// <remarks/>
            public const string STOP = "STOP";
            /// <remarks/>
            public const string ERROR = "ERROR";
        }

        /// <summary>
        /// A simplified error record, replacing the current and invariant culture description fields found in the
        /// schema defined ErrorType class, which are designed to be used to assist localization, with a simple 
        /// description field. Objects of this class are used to convey error information to the consumer application.
        /// </summary>
        public class ErrorRecord
        {
            /// The value of Code when the error was generated by an exception<remarks/>
            public static int c_ExceptionCode = 0x9999;
            /// The value of Code when the error has no associated code<remarks/>
            public static int c_NoCode = 0x0000;
            /// The value of Source when the error was reported by the service API<remarks/>
            public static string c_ServiceAPISource = "ServiceAPI";
            private DateTime m_time_reported;
            private string m_source;
            private int m_error_code;
            private string m_description;

            /// <summary>
            /// Create a new ErrorRecord from an ErrorType runtime object
            /// </summary>
            /// <param name="schema_error_type">An ErrorType runtime object</param>
            /// <param name="source">A description of thesource of the serror (optional)</param>
            internal ErrorRecord(ErrorType schema_error_type, string source = null)
            {
                m_time_reported = schema_error_type.DateTime;
                if (!int.TryParse(schema_error_type.ErrorCode, out m_error_code))
                {
                    m_error_code = 0;
                }
                m_description = schema_error_type.ErrorDescriptionInvariantCulture;
                m_source = source;
            }

            /// <summary>
            /// Create a new ErrorRecord from an exception
            /// </summary>
            /// <param name="ex">An exception</param>
            ///   /// <param name="source">The name of the method that caught the exception</param>
            internal ErrorRecord(Exception ex, string source)
            {
                m_time_reported = DateTime.Now;
                m_error_code = c_ExceptionCode;
                m_description = ex.Message;
                m_source = source;
            }


            /// <summary>
            /// Create a new ErrorRecord from scratch
            /// </summary>
            /// <param name="errorCode">An error code convertible to HEX. Usually ErrorRecord.c_NoCode</param>
            /// <param name="description">A description of the error</param>
            /// <param name="source">The source method of the error (Optional)</param>
            internal ErrorRecord(int errorCode, string description, string source = null)
            {
                m_time_reported = DateTime.Now;
                m_error_code = errorCode;
                m_description = description;
                m_source = source;
            }

            /// The datetime of the reported error<remarks/>
            public DateTime Time { get { return m_time_reported; } }

            ///<remarks>
            ///The error code, dependent on the source of the error. For unanticipated Exceptions will this be
            ///"Unanticiapted Exception". For CFX system errors, it will be the associated instrument error code.
            ///</remarks>
            public int Code { get { return m_error_code; } }

            ///A description of the error as reported by the source<remarks/>
            public string Description { get { return m_description; } }

            ///<remarks>
            ///When an exception generated the error, the name of the method that caught the exception
            /// </remarks>
            public string Source { get { return m_source; } }
        }

        /// <summary>
        /// Wraps the InstrumentBlockType schema class into a status record containing the informational
        /// components required by a typical appication layer
        /// </summary>
        public struct InstrumentStatus
        {

            #region member data

            //Hide the constructed values of the status information here. Descriptions of the individual status components
            //are included below with the corresponding public accessor definitions.
            private ErrorRecord m_current_error;
            private string m_base_serial_number;
            private string m_base_name;
            private string m_block_type;
            private string m_status;
            private string m_motorized_lid_position;
            private string m_protocol_step;
            private string m_repeat_step;
            private string m_time_remaining;
            private float m_lid_temperature;
            private float m_sample_temperature;

            #endregion

            #region constructor

            /// <summary>
            /// Create a new InstrumentStatus structure from an InstrumentBlockType runtime object. If the status contains
            /// an error report, it will be assigned to the CurrentError accessor, which will otherwise be null.
            /// </summary>
            /// <param name="instrument_block_type">An InstrumentBlockType runtime object</param>
            internal InstrumentStatus(InstrumentBlockType instrument_block_type)
            {
                // local delegate used to convert the reported remaining seconds into a string of the form hh:mm:ss
                Func<float, string> Seconds2hhmmss = (float seconds_remaining) =>
                {
                    long hours = (long)seconds_remaining / 3600;
                    long minutes = (((long)seconds_remaining - (hours * 3600))) / 60;
                    long seconds = (long)seconds_remaining - (hours * 3600) - (minutes * 60);
                    return String.Format("{0:d2}:{1:d2}:{2:d2}", hours, minutes, seconds);
                };

                //InstrumentStatus constructor entry point
                m_current_error = null;
                if (instrument_block_type.ErrorArray != null)
                {
                    if (instrument_block_type.ErrorArray.Length > 0)
                    {
                        //This will usually be the only reported error, but will in any case be the latest
                        m_current_error = new ErrorRecord(instrument_block_type.ErrorArray[0]);
                    }
                }
                m_base_serial_number = instrument_block_type.SerialNumber;
                m_base_name = instrument_block_type.NickName;
                m_block_type = instrument_block_type.Description;
                m_status = instrument_block_type.Status.ToString();
                m_motorized_lid_position = instrument_block_type.MotorizedLidPosition.ToString();
                m_protocol_step = string.Format("{0} of {1}", instrument_block_type.Step, instrument_block_type.Steps);
                m_repeat_step = string.Format("{0} of {1}", instrument_block_type.Cycle, instrument_block_type.Cycles);
                m_time_remaining = Seconds2hhmmss(instrument_block_type.EstimatedRemainingRunTime);
                m_lid_temperature = instrument_block_type.LidTemperature.Temperature;
                m_sample_temperature = instrument_block_type.BlockTemperature.Temperature;
            }

            #endregion

            #region public status information

            /// The error currently being reported by the instrument, or null if no error is being reported// <remarks/>
            public ErrorRecord CurrentError { get { return m_current_error; } }

            /// The serial number of the CFX system base<remarks/>
            public string BaseSerialNumber { get { return m_base_serial_number; } }

            /// The name (if any) assigned to CFX system base<remarks/>
            public string BaseName { get { return m_base_name; } }

            /// The type of optical reaction block (e.g., CFX96, CFX384..)<remarks/>
            public string BlockType { get { return m_block_type; } }

            /// The currently reported instrument status<remarks/>
            public string Status { get { return m_status; } }

            /// The currently reported position of the motorized lid (or "Manual" if the lid is not motorized)<remarks/>
            public string MototrizedLidPosition { get { return m_motorized_lid_position; } }

            /// The currently running protocol step, in the form "x of y" where x is the current step and y is the total number of steps<remarks/>
            public string ProtocolStep { get { return m_protocol_step; } }

            /// The current step within the current repeat cycle, in the form "x of y" where x is the current repeat step and y is the total number of repeat steps<remarks/>
            public string RepeatStep { get { return m_repeat_step; } }

            /// The estimated time remaining in the currently running protocol, as hh:mm:ss<remarks/>
            public string EstimatedTimeRemaining { get { return m_time_remaining; } }

            /// The instrument lid temperature, in degrees Celsius<remarks/>
            public float LidTemp { get { return m_lid_temperature; } }

            /// The sample temperature, in degrees Celsius<remarks/>
            public float SampleTemp { get { return m_sample_temperature; } }

            #endregion
        }

        #endregion

        #region XML encoding and decoding

        /// <summary>
        /// Generate a serialized XML request from the specified MessageType runtime object. 
        /// This method is used to encode the runtime MessageType object into serialized 
        /// XML for transmission to the service by the client. Note that empty string values
        /// in the runtime object must be the empty string, and not null, or the correspondimg
        /// element will be omitted from the generated xml, violating the schema.
        /// </summary>
        /// <param name="message_type_object">MessageType runtime object</param>
        /// <returns>Serialized XML representation of the specified MessageType object</returns>
        private string Message2XML(MessageType message_type_object)
        {
            string XMLMessage = string.Empty;
            try
            {
                using (StringWriter writer = new StringWriter())
                {
                    XmlSerializer xs = new XmlSerializer(typeof(MessageType));
                    xs.Serialize(writer, message_type_object);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return string.Empty;
        }

        /// <summary>
        /// Create an InstrumentBlocksType object from the given XML response received from the API service.
        /// InstrumentBlocksType is the top-level XML structure used to specify responses from the CFX Manager 
        /// API service. This method encodes the given serialized XML response into an InstrumentBlocksType 
        /// runtime object for conversion into a schema wrapper status object for further processing by the
        /// InstrumentBlocksType2InstrumentStatusList() method defined below.
        /// </summary>
        /// <param name="xml_response">Serialized XML service response</param>
        /// <returns>InstrumentBlocksType runtime object representation of the specified response</returns>
        private InstrumentBlocksType XML2BlocksType(string xml_response)
        {
            try
            {
                if (!string.IsNullOrEmpty(xml_response))
                {
                    using (StringReader reader = new StringReader(xml_response))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(InstrumentBlocksType));
                        return (InstrumentBlocksType)xs.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        /// Convert the specified InstrumentBlocksType runtime object into a list of InstrumentStatus objects for
        /// use by the consumer application. Also checks the InstrumentBlocksType for errors reported by the 
        /// service (Not to be confused with errors reported by instruments via the service!)The service will report 
        /// errors if it is unable to complete a request normally. Such errors will be extremely rare in a production 
        /// environment. and generally indicate an unrecoverable system context requiring rebooting of the hosting computer 
        /// and/or all connected instruments.
        /// </summary>
        /// <param name="instrument_blocks_type"></param>
        /// <returns></returns>
        private List<InstrumentStatus> BlocksType2StatusList(InstrumentBlocksType instrument_blocks_type)
        {
            List<InstrumentStatus> current_statuses = new List<InstrumentStatus>();
            try
            {
                //First parse and report any errors reported by the service
                if (instrument_blocks_type.ErrorArray != null)
                {
                    foreach (ErrorType err in instrument_blocks_type.ErrorArray)
                    {
                        OnClientError(new ClientErrorEventArgs(new ErrorRecord(err, ErrorRecord.c_ServiceAPISource)));
                    }
                }
                //Then build the list of InstrumentStatus records
                if (instrument_blocks_type.BlockArray != null)
                {
                    foreach (InstrumentBlockType instrument_block_type in instrument_blocks_type.BlockArray)
                    {
                        current_statuses.Add(new InstrumentStatus(instrument_block_type));
                    }
                }
                return current_statuses;
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
                return current_statuses;
            }
        }

        #endregion

        #region registration management

        /// <summary>
        /// Register the underlying client with the CFX Manager API service. Called by the consumer facing OpenClient()
        /// method to register the underlying client with the service once the connection is established.
        /// 
        /// The registration ID aquired by this request is required by all other service requests. This class hides the 
        /// registration ID and its associated role in service transactions from consuming applications. 
        /// 
        /// Wraps CFXManagerClient:SendServiceRequest() in the local exception handling framework 
        /// </summary>
        /// <returns>true IFF no exceptions occurred during the operation</returns>
        private bool Register()
        {
            m_registrationID = string.Empty;
            try
            {
                m_MasterMessage.Item = m_Register_Item;
                m_MasterMessage.RegistrationID = string.Empty;
                InstrumentBlocksType response = XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage)));
                m_registrationID = response.RegistrationID;
                m_MasterMessage.RegistrationID = m_registrationID;
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            if (string.IsNullOrEmpty(m_registrationID))
            {
                OnClientError(new ClientErrorEventArgs(
                    new ErrorRecord(ErrorRecord.c_NoCode,
                        "Registration with the CFX Manager API service failed. A previously connected client may have exited without unregistering. Try re-starting CFX Manager",
                        System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return !string.IsNullOrEmpty(m_registrationID);
        }

        /// <summary>
        /// Unregister the underlying client with CFX Manager API service. Called by the consumer facing CloseClient()
        /// method. It is important that the client unregister with the service before closing since the service uses
        /// the registration to enforce its single client policy.
        /// 
        /// Wraps CFXManagerClient:SendServiceRequest() in the local exception handling framework 
        /// </summary>
        /// <returns>true IFF no exceptions occurred during the operation</returns>
        private InstrumentBlocksType Unregister()
        {
            try
            {
                m_MasterMessage.Item = m_Unregister_Item;
                return XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage)));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        #endregion

        #region connection management

        /// <summary>
        /// Create a client and register with the CFX Manager API service. Wraps CFXManagerClient:CreateClient() 
        /// in the local exception handling framework 
        /// </summary>
        /// <returns>true IFF the client has been created connected to the API service</returns>
        public bool OpenClient()
        {
            lock (m_lock_object)
            {
                try
                {
                    if (CFXManagerClient.CreateClient())
                    {
                        return ClientIsConnected() ? Register() : false;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                return false;
            }
        }

        /// <summary>
        /// Ping method to verify an open and active service channel is currently established between 
        /// the client and the service. 
        ///  
        /// Wraps CFXManagerClient:IsConnected() in the local exception handling framework 
        /// </summary>
        /// <returns>true IFF the client is open and connected to the API service</returns>
        public bool ClientIsConnected()
        {
            lock (m_lock_object)
            {
                try
                {
                    return CFXManagerClient.IsConnected();
                }
                catch (Exception ex)
                {
                    OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                return false;
            }
        }

        /// <summary>
        ///Close the client. Must be called by the consumer application prior to exiting to ensure that the client
        ///unregisters from the service, and that all client related resources are freed. Failure to call this
        ///method will at the very least prevent any future client connections from being established with the API
        ///service during the current CFX Manager session.
        ///
        ///Wraps CFXManagerClient:CloseClient() in the local exception handling framework 
        /// </summary>
        public void CloseClient()
        {
            lock (m_lock_object)
            {
                try
                {
                    if (CFXManagerClient.IsConnected())
                    {
                        Unregister();
                    }
                    CFXManagerClient.AbortClient();
                }
                catch (Exception ex)
                {
                    OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        #endregion

        #region CFX Manager API operations

        /// <summary>
        /// Returns an InstrumentBlocksType runtime object containing the CFX system status of all connected instruments,
        /// along with any associated service or instrument error reports. 
        /// </summary>
        /// <returns>List of InstrumentStatus objects containing the statuses of all connected CFX systems.</returns>
        public List<InstrumentStatus> GetInstrumentStatus()
        {
            try
            {
                m_MasterMessage.Item = m_InstrumentStatus_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        /// Open the mechanized lid of the specified CFX system.
        /// </summary>
        /// <param name="serialNumber">Serial number of CFX system base</param>
        /// <returns>List of InstrumentStatus objects containing just the status of the target instrument</returns>
        public List<InstrumentStatus> OpenLid(string serialNumber)
        {
            try
            {
                m_OpenLid_Item.SerialNumber = serialNumber;
                m_MasterMessage.Item = m_OpenLid_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        /// Close the mechanized lid of the specified CFX system.
        /// </summary>
        /// <param name="serialNumber">Serial number of CFX system base</param>
        /// <returns>List of InstrumentStatus objects containing just the status of the target instrument</returns>
        public List<InstrumentStatus> CloseLid(string serialNumber)
        {
            try
            {
                m_CloseLid_Item.SerialNumber = serialNumber;
                m_MasterMessage.Item = m_CloseLid_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        ///Cause the LED on the lid of the specified CFX system to blink for approximately five seconds. 
        /// </summary>
        /// <param name="serialNumber">Serial number of CFX system base</param>
        /// <returns>List of InstrumentStatus objects containing just the status of the target instrument</returns>
        public List<InstrumentStatus> Blink(string serialNumber)
        {
            try
            {
                m_Blink_Item.SerialNumber = serialNumber;
                m_MasterMessage.Item = m_Blink_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        /// Start a protocol run using the given parameters on the specified CFX system. 
        /// </summary>
        /// <param name="serialNumber">
        /// Serial number of CFX system base
        /// </param>
        /// <param name="protocolFile">
        /// Fully qualified protocol, LIMS, or PrimePCR file name
        /// </param>
        /// <param name="plateFile">
        /// Full path to plate file if protocolFile is a real-time protocol(.pcrd) file.
        /// Null if protocolFile is a conventional protocol, a LIMS(.csv) file, or PrimePCR(.plrn) file.
        /// </param>
        /// <param name="runNote">
        /// Text to appear as Notes in the report file
        /// </param>
        /// <param name="runID">
        /// Text to appear as Run ID, (typically a plate barcode), in the report file
        /// </param>
        /// <param name="DataFileName">
        /// Fully qualified file name of the data file to be created, or empty to use CFX manager default settings
        /// </param>
        /// <param name="LockInstrumentPanel">
        /// Locking the instrument front panel during the run? True to lock
        /// </param>
        /// <param name="GenerateReport">
        /// Generate report after the run? True to generate report
        /// </param>
        /// <param name="ReportTemplate">
        /// Full path to report template file to be used to create reports or empty to use default template
        /// </param>
        /// <param name="ReportFileName">
        /// Fully qualified file name of the data file to be created, or empty to use CFX manager default settings
        /// </param>
        /// <param name="EmailAddresses">
        /// Comma seperated Email addresses to send data files and reports to at run completion.
        /// </param>
        /// <returns>
        /// InstrumentBlocksType runtime object containing the status of the target instrument and any errors that occurred during the operation
        /// </returns>
        public List<InstrumentStatus> RunProtocol(
             string serialNumber,
             string protocolFile,
             string plateFile,
             string runNote,
             string runID,
             string DataFileName,
             bool LockInstrumentPanel,
             bool GenerateReport,
             string ReportTemplate,
             string ReportFileName,
             string EmailAddresses)
        {
            try
            {
                m_RunProtocol_Item.SerialNumber = serialNumber;
                m_RunProtocol_Item.ProtocolFile = protocolFile;
                m_RunProtocol_Item.PlateFile = plateFile;
                m_RunProtocol_Item.Note = runNote == null ? string.Empty : runNote;
                m_RunProtocol_Item.RunId = runID == null ? string.Empty : runID;
                m_RunProtocol_Item.DataFile = DataFileName;
                m_RunProtocol_Item.LockInstrumentPanel = LockInstrumentPanel;
                m_RunProtocol_Item.GenerateReportEndOfRun = GenerateReport;
                m_RunProtocol_Item.GenerateReportTemplateFile = ReportTemplate;
                m_RunProtocol_Item.GenerateReportOutputFile = ReportFileName;
                m_RunProtocol_Item.EmailAddresses = EmailAddresses;
                m_RunProtocol_Item.GenerateReportType = ReportTypes.pdf;
                m_MasterMessage.Item = m_RunProtocol_Item;
                CFXManagerClient.SetResponseTimeOut(m_RunProtocol_Timeout);
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        ///Cancel a currently running protocol on the specified CFX system
        /// </summary>
        /// <param name="serialNumber">Serial number of CFX system base</param>
        /// <returns>List of InstrumentStatus objects containing just the status of the target instrument</returns>
        public List<InstrumentStatus> StopRun(string serialNumber)
        {
            try
            {
                m_Stop_Item.SerialNumber = serialNumber;
                m_MasterMessage.Item = m_Stop_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        ///Pause a currently running protocol on the specified CFX system
        /// </summary>
        /// <param name="serialNumber">Serial number of CFX system base</param>
        /// <returns>List of InstrumentStatus objects containing just the status of the target instrument</returns>
        public List<InstrumentStatus> PauseRun(string serialNumber)
        {
            try
            {
                m_Pause_Item.SerialNumber = serialNumber;
                m_MasterMessage.Item = m_Pause_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        ///Resume a paused protocol on the specified CFX system
        /// </summary>
        /// <param name="serialNumber">Serial number of CFX system base</param>
        /// <returns>List of InstrumentStatus objects containing just the status of the target instrument</returns>
        public List<InstrumentStatus> ResumeRun(string serialNumber)
        {
            try
            {
                m_Resume_Item.SerialNumber = serialNumber;
                m_MasterMessage.Item = m_Resume_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        /// Generate PDF report from the specified data file
        /// </summary>
        /// <param name="dataFile">The data file from which to generate the report</param>
        /// <param name="templateFile">The report template file to be use to generate the report</param>
        /// <param name="outputFile">
        /// Fully qualified file name of the report file will be written.
        /// If empty or null then the name of the data file will be used with the filename extention replaced according to the report file type.
        /// </param>
        /// <list type="">
        /// <item>Portable data format ("PDF").</item>
        /// <item>Mime-type HTML ("mHTML").</item>
        /// <item>Plain Text ("text").</item>
        /// </list>
        /// <returns>List of InstrumentStatus objects containing the status of all connected instruments</returns>
        public List<InstrumentStatus> GenerateReport(
             string dataFile,
             string templateFile,
             string outputFile
             )
        {
            try
            {
                m_GenerateReport_Item.DataFile = dataFile;
                m_GenerateReport_Item.OutputFile = outputFile;
                m_GenerateReport_Item.TemplateFile = templateFile;
                m_GenerateReport_Item.Type = ReportTypes.pdf;
                m_MasterMessage.Item = m_GenerateReport_Item;
                return BlocksType2StatusList(XML2BlocksType(CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage))));
            }
            catch (Exception ex)
            {
                OnClientError(new ClientErrorEventArgs(new ErrorRecord(ex, System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            return null;
        }

        /// <summary>
        /// Shutdown the CFX Manager application hosting the service. This should be used by consumer applications to
        /// shut-down server mode (no UI) instances of the CFX Manager application which they have explcitly started.
        /// If this command is not used to shut down such instances (the application is shut-down by simply killing 
        /// the process, for example) then there is a high likelyhood that certain associated independent processes 
        /// will remain running and prevent any further normal operation of CFX Manager until host system reboot.
        /// </summary>
        public void ShutDown()
        {
            try
            {
                //We know we will probably get a time-out here because the service will be closed during the shut-down, 
                //so set the time-out to 1 second be sure we don't have to wait any longer than that at the send command.
                CFXManagerClient.SetResponseTimeOut(1);
                m_MasterMessage.Item = m_ShutDown_Item;
                CFXManagerClient.SendServiceRequest(Message2XML(m_MasterMessage));
            }
            catch
            {
                ;
                //an expected time-out will often occur here because the shut-down sequence closes the service
                //pretty quicky, usually before a response can be generated.  
            }
        }

        #endregion

        #region  Error Handling

        /// <summary>
        /// Any errors or exceptions generated in this class, or generated in the CFXManagerClient class, will be
        /// reported to the consuming application via this event. 
        /// </summary>
        public event EventHandler<ClientErrorEventArgs> ClientError;

        /// <summary>
        ///  The ClientError Event Handler
        /// </summary>
        public virtual void OnClientError(ClientErrorEventArgs e)
        {
            try
            {
                CFXManagerClient.AbortClient();
            }
            catch
            {
                ;
            }
            if (ClientError != null)
            {
                ClientError(m_lock_object, e);
            }
        }

        /// <summary>
        /// ClientError event arguments. 
        /// 
        /// Low level error handling (handling of client errors, reported service errors, or unanticipated exceptions occurring
        /// in the wrapper layer) consists of reporting the detected error or exception to the consumer application via an event. 
        /// These arguments are designed to provide basic "what and where" information to the consuming application for reporting 
        /// purposes. In any given production environment, it might or might not make sense to perform specialized processing of 
        /// certain anticipated client errors, such as service time-outs, within this class, rather than forwarding them to the consumer.
        /// </summary>
        public class ClientErrorEventArgs : System.EventArgs
        {
            /// <summary>
            ///  Contructor 
            /// </summary>
            public ClientErrorEventArgs(ErrorRecord error_report)
            {
                ErrorReport = error_report;
            }

            /// <summary>   
            /// ErrorRecord instance describing the error. 
            /// </summary>
            public readonly ErrorRecord ErrorReport;

        }

        #endregion
    }

}
