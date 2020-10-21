using System;
using System.Windows.Forms;

namespace BioRad.Example_Application
{

    ///  <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// It implements the main execution thread (entry point) of the example application. 
    /// Start the MainForm and handle and log any un-caught exceptions up to maximum number,
    /// above which exit the application.
    /// </summary>
    static partial class Program
    {

        #region member data

        /// The Example Application Main Form<Remarks/>
        public static MainForm m_main_form;

        ///Counter used to ensure the application exits after catching more than a maximum number of unhandled exceptions<Remarks/>
        private static int m_unhandled_exception_count = 0;

        ///The maximum number of unhandled exceptions that can be caught before the program will force itself to exit<Remarks/>
        private static int m_max_unhandled_exceptions = 3;

        ///The  unhandled AppDomain exception handler<Remarks/>
        private static UnhandledExceptionEventHandler m_app_domain_exception_handler = new UnhandledExceptionEventHandler(OnAppDomainException);

        ///The  unhandled Thread exception handler<Remarks/>
        private static System.Threading.ThreadExceptionEventHandler m_thread_exception_handler = new System.Threading.ThreadExceptionEventHandler(OnThreadException);

        #endregion

        #region methods

        /// <summary>
        ///main entry point 
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {

            int returnCode = 0;
            AppDomain.CurrentDomain.UnhandledException += m_app_domain_exception_handler;
            System.Windows.Forms.Application.ThreadException += m_thread_exception_handler;
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            m_main_form = new MainForm();
            try
            {
                System.Windows.Forms.Application.Run(m_main_form);
            }
            catch (Exception ex)
            {
                LogExceptionAndExit(ex);
            }
            return returnCode;
        }

        /// <summary>
        /// Handles otherwise unhandled exceptions by logging the exception up to m_max_unhandled_exceptions times.
        /// Exits the application at the (m_max_unhandled_exceptions)+1-th exception.
        /// </summary>
        /// <param name="ex">the exception</param>
        private static void LogExceptionAndExit(Exception ex)
        {
            if (m_unhandled_exception_count <= m_max_unhandled_exceptions)
            {
                try
                {
                    m_unhandled_exception_count++;
                    m_main_form.LogNewLine(ex.ToString());
                }
                catch (Exception inner_ex)
                {
                    Console.WriteLine(inner_ex.ToString());
                    AppDomain.CurrentDomain.UnhandledException -= m_app_domain_exception_handler;
                    System.Windows.Forms.Application.ThreadException -= m_thread_exception_handler;
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException -= m_app_domain_exception_handler;
                System.Windows.Forms.Application.ThreadException -= m_thread_exception_handler;
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Handles AppDomain.UnhandledException events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnAppDomainException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogExceptionAndExit(ex);
            }
            else
            {
                // didn't get the originating exception object, create a default one
                ex = new Exception("Unidentified AppDomain Exception Occurred");
                LogExceptionAndExit(ex);
            }
        }

        /// <summary>
        /// Handles Application.ThreadExcpetion events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            if (ex != null)
            {
                LogExceptionAndExit(ex);
            }
            else
            {
                // couldn't get an exception object, create a default one
                ex = new Exception("Unidentified Thread Exception Occurred");
                LogExceptionAndExit(ex);
            }
        }

        #endregion

    }
}
