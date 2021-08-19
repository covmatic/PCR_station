using System;
using System.IO;
using System.ServiceModel;

namespace BioRad.Example_Client
{
    /// <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// It implements a low level client interface to the CFX Manager API Service.
    /// 
    /// It is referenced by the CFXManagerClientWrapper class, which implements a high level client 
    /// interface by encapsulating the low level operations provided here within a functional
    /// framework suitable for consumption by business-level application logic.
    /// </summary>
    public static class CFXManagerClient
    {
        #region call-back place-holder

        /// <summary>
        ///The OnServiceIsClosing call back event is not currently implemented, but a stub instance context 
        ///for it is required in order to create the client.
        /// </summary>
        private class SubscribedServiceEvents : ISOCFXCommandServiceCallback
        {
            /// <summary>
            ///Stub for the OnServiceIsClosing call-back event
            /// </summary>
            public void OnServiceIsClosing()
            {
                ;
            }
        }

        #endregion

        /// <summary>
        /// Log function
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="filename"></param>
        private static void Log(string logMessage, string filename = "log.txt")
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

        #region Member Data

        /// <summary>
        /// The client instance
        /// </summary>
        private static SOCFXCommandServiceClient m_Client = null;

        #endregion

        #region methods

        /// <summary>
        /// Adjust the service response time-out. A consuming application may want to adjust this to account for 
        /// special cases where it either wants to ignore predicted time-outs, or handle special-case transactions 
        /// with response times that fall beyond the optimal default setting. 
        /// </summary>
        /// <param name="time_out_seconds">
        /// The time in seconds allowed for a transaction to complete before a service time-out exception is generated.
        /// </param>
        public static void SetResponseTimeOut(int time_out_seconds)
        {
            Log("Timeout set to: " + time_out_seconds);
            m_Client.InnerChannel.OperationTimeout = TimeSpan.FromSeconds(time_out_seconds);
        }

        /// <summary>
        /// Create the client using the stub instance context SubscribedServiceEvents defined above
        /// </summary>
        /// <returns>true IFF the client communications channel was successfully created and opened</returns>
        public static bool CreateClient()
        {
            m_Client = new SOCFXCommandServiceClient(new InstanceContext(new SubscribedServiceEvents()));
            if (m_Client.State == System.ServiceModel.CommunicationState.Created)
            {
                m_Client.Open();
            }
            return m_Client.State == System.ServiceModel.CommunicationState.Opened;
        }

        /// <summary>
        /// Abort the client if it is not null. 
        /// </summary>
        public static void AbortClient()
        {
            if (m_Client != null)
            {
                m_Client.Abort();
                m_Client = null;
            }
        }

        /// <summary>
        /// Ping method for use by consumers to verify that the client is connected to the service
        /// (exists and is opened)
        /// </summary>
        /// <returns>true IFF the client is connected to the service</returns>
        public static bool IsConnected()
        {
            if (m_Client != null)
            {
                return (m_Client.State == System.ServiceModel.CommunicationState.Opened);
            }
            return false;
        }

        private static int id = 0;

        /// <summary>
        /// Send a service request
        /// </summary>
        /// <param name="request">An xml service request conforming to the API schema</param>
        /// <returns>An xml service response message conforming to the API schema </returns>
        public static string SendServiceRequest(string request)
        {
            string ret;
            
            Log(String.Format("Sending request {0}: {1}", id, request));
            ret = m_Client.XmlCommand(request);
            Log(String.Format("Got answer {0}: {1}", id, ret));
            id++;
            return ret;
        }

        #endregion

    }
}
