using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace BioRad.Example_Client_Wrapper
{
    /// <summary>
    /// Copyright ©2014 Bio-Rad Laboratories, Inc. All rights reserved.
    /// 
    /// This class is part of the CFX Manager API Examples kit. 
    /// 
    /// Implements CFX Manager related utilities used by the example application. Provides the locations
    /// of certains standard CFX Manager directories, defines the minimum API compatible version of
    /// CFX Manager, and provides utilities to determine whether CFX Manager is installed and/or running,
    /// and to start and shutdown an instance of CFX Manager in server mode (No UI mode).  
    /// </summary>
    public static class CFXManagerUtilities
    {
        #region Constants

        /// Root Bio-Rad directory name<Remarks/>
        private const string c_BioRadDirectory = @"Bio-Rad";

        ///CFXManager executable file name<Remarks/>
        private const string c_CFXManagerExecutableFile = "BioRadCFXManager.exe";

        ///The minumum CFX Manager product version compatible with the published API<Remarks/>
        private static Version c_Min_CFXManager_Version = new Version("3.1.1621");

        /// The name of the CFX Manager Express Load Directory<Remarks/>
        private const string c_CFXManagerExpressLoadDirectory = "ExpressLoad";

        /// The relative path to the CFX Manager admin user directory<Remarks/>
        private const string c_CFXManagerUserAdmin = @"Users\admin";

        #endregion

        #region Member Data

        /// The absolute path to the directory containing the CFX Manager executable<Remarks/>
        private static string m_CFXManagerExecutablePath = null;

        /// The name of the CFX Manager executable file<Remarks/>
        private static string m_CFXManagerExecutable = null;

        /// The root of the Bio-Rad specific program data directory<Remarks/>
        private static string m_CFXManagerDataPathBioRadRoot = null;

        /// Flag tracking whether an API compatible version of CFX Manager is installed on the host system<Remarks/>
        private static bool m_CompatibleCFXManagerIsInstalled = false;

        /// The version information corresponding to the highest installed version of CFX Manager<Remarks/>
        private static Version m_CFXManagerVersion = null;

        /// The file version of the highest installed version of CFX Manager<Remarks/>
        private static FileVersionInfo m_CFXManagerFileVersionInfo = null;

        ///The executing CFX Manager master processs, when CFX Manager has been started in server mode by this class.<Remarks/>
        private static Process m_executing_cfx_manager_process;

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// Static constructor. 
        /// Initializes information related to the installed version of CFX Manager by calling InitializeCFXManagerUtilities().
        /// </summary>
        static CFXManagerUtilities()
        {
            InitializeCFXManagerUtilities();
        }

        /// <summary>
        /// Initializes the CFXManagerUtilities class by obtaining all pertinent information related the first found installed
        /// CFX Manager that meets the minimum version information hardcoded in c_Min_CFXManager_Version, which is the minumum
        /// version compatible with the CFX Manager API service.
        /// </summary>
        private static void InitializeCFXManagerUtilities()
        {
            m_CFXManagerFileVersionInfo = HighestInstalledCfXManagerVersion();
            if (m_CFXManagerFileVersionInfo != null)
            {
                m_CFXManagerVersion = new Version(m_CFXManagerFileVersionInfo.FileVersion);
                if (m_CFXManagerVersion >= c_Min_CFXManager_Version)
                {
                    m_CFXManagerExecutablePath = Path.GetDirectoryName(m_CFXManagerFileVersionInfo.FileName);
                    int BioRadStart = m_CFXManagerExecutablePath.IndexOf(c_BioRadDirectory);
                    m_CFXManagerDataPathBioRadRoot = m_CFXManagerExecutablePath.Substring(BioRadStart);
                    m_CFXManagerExecutable = Path.Combine(m_CFXManagerExecutablePath, c_CFXManagerExecutableFile);
                    m_CompatibleCFXManagerIsInstalled = true;
                }
            }
        }

        /// <summary>
        /// Gets the FileVersionInfo of the highest CFX Manager version installed in the standard Bio-Rad root directory.
        /// Called once by InitializeCFXManagerUtilities() during construction.
        /// </summary>
        /// <returns></returns>
        private static FileVersionInfo HighestInstalledCfXManagerVersion()
        {
            string directory = string.Empty;
            bool is64BitOS = Environment.Is64BitProcess;
            List<string> defaultFolders = new List<string>();
            defaultFolders.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), c_BioRadDirectory));
            if (is64BitOS)
            {
                defaultFolders.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), c_BioRadDirectory));
            }
            List<FileVersionInfo> fileVersionInfos = new List<FileVersionInfo>();
            foreach (string folder in defaultFolders)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                fileVersionInfos.AddRange(FileSearch(c_CFXManagerExecutableFile, di));
            }
            if (fileVersionInfos.Count == 0)
            {
                // Expand the search here to make this method work with non-standard installations.
                return null;
            }
            fileVersionInfos = (from fi in fileVersionInfos orderby fi.ProductVersion descending select fi).ToList();
            return fileVersionInfos[0];
        }

        /// <summary>
        /// Search the specified directories for the specified file name. Return a list of version information
        /// for each instance found. Called by HighestInstalledCfXManagerVersion() once during construction.
        /// </summary>
        /// <param name="searchFilename"></param>
        /// <param name="di"></param>
        /// <returns></returns>
        private static List<FileVersionInfo> FileSearch(string searchFilename, DirectoryInfo di)
        {
            List<FileVersionInfo> list = new List<FileVersionInfo>();
            if (di != null && di.Exists)
            {
                DirectoryInfo[] directories = di.GetDirectories();
                foreach (DirectoryInfo d in directories)
                {
                    foreach (FileInfo file in d.GetFiles(searchFilename, SearchOption.AllDirectories))
                    {
                        list.Add(FileVersionInfo.GetVersionInfo(file.FullName));
                    }
                }
            }
            return list;
        }

        #endregion

        #region Accessors

        #region CFX Manager executution management

        /// <summary>
        ///The minumum CFX Manager product version compatible with the published API service
        /// </summary>
        public static Version MiniumumCFXManagerVersion
        {
            get { return c_Min_CFXManager_Version; }
        }

        /// <summary>
        /// True IFF a version of CFX Manager compatible with the published API service is
        /// installed on the host computer. If this is not true, all utility methods in this 
        /// class will be rendered non-functional.
        /// </summary>
        public static bool APICompatibleCFXManagerIsInstalled
        {
            get { return m_CompatibleCFXManagerIsInstalled; }
        }

        /// <summary>
        /// True IFF an API compatible version of CFX Manager is currently executing
        /// </summary>
        public static bool CFXManagerIsExecuting
        {
            get
            {
                bool isRunning = false;
                if (APICompatibleCFXManagerIsInstalled)
                {
                    Process[] processList = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(m_CFXManagerExecutable));
                    isRunning = (processList.Length > 0);
                }
                return isRunning;
            }
        }

        /// <summary>
        /// True IFF an API service compatible version of CFX Manager that was started in server (no UI) 
        /// mode using these utilities is currently executing. (if CFXManagerIsExecuting is true and this is
        /// false, than an externally started (UI active) instance of CFXManager is executing).
        /// </summary>
        public static bool CFXManagerWasStartedByApp
        {
            get
            {
                return m_executing_cfx_manager_process != null;
            }
        }

        #endregion

        #region CFX Manager standard directories

        /// <summary>
        /// Returns The CFXManager-specific CommonDocuments path
        /// </summary>
        /// <returns></returns>
        public static string CFXManagerCommonDocuments
        {
            get
            {
                return m_CFXManagerExecutablePath==null?String.Empty:Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), m_CFXManagerDataPathBioRadRoot);
            }
        }

        /// <summary>
        /// Returns The CFXManager [CommonDocuments]/Users/admin folder which is used as the default application 
        /// document location by CFX Manager
        /// </summary>
        /// <returns></returns>
        public static string CFXManagerUserAdminDirectory
        {
            get { return m_CFXManagerExecutablePath == null ? String.Empty : Path.Combine(CFXManagerCommonDocuments, c_CFXManagerUserAdmin); }
        }

        #endregion

        #endregion

        #region Method used to start CFX Manager in server mode

        /// <summary>
        /// Launch CFX Manager in server mode IFF an API compatible version is installed and is 
        /// not currently executing. Note that this method will wait to return until the started   
        /// CFX Manager process has completed initialization, which may take several seconds.
        /// </summary>
        /// <returns>True IFF CFX Manager was successfully started</returns>
        public static bool StartCFXManagerAsServer()
        {
            m_executing_cfx_manager_process = null;
            if (m_CompatibleCFXManagerIsInstalled && !CFXManagerIsExecuting)
            {
                Process new_process = new Process();
                new_process.StartInfo.UseShellExecute = true;
                new_process.StartInfo.RedirectStandardOutput = false;
                new_process.EnableRaisingEvents = true;
                new_process.StartInfo.FileName = m_CFXManagerExecutable;
                new_process.StartInfo.Arguments = string.Format("-startup");   //To run on actual machine: FOR RELEASE
                //new_process.StartInfo.Arguments = string.Format("-simulation"); //To run a PCR simulation {Edit:Aff23Jun2020}
                new_process.Start();
                m_executing_cfx_manager_process = new_process;
                int max_attempts = 20;
                int attempts = 0;
                //Wait until CFX Manager has actually started executing before returning, to ensure that the API
                //service is available to the calling method when this method returns.
                while (attempts < max_attempts && !CFXManagerIsExecuting)
                {
                    Thread.Sleep(1000);
                }
                return attempts < max_attempts;
            }
            return false;
        }

        /// <summary>
        /// Kill the started CFX Manager process, and its associated sub-processes. This functionality is required by the
        /// consumer application in the event that CFX Manager is started by this class, but the consumer application
        /// is unable to connect to the API service. When the service is available, the API shutdown command should be 
        /// used to stop CFX Manager. 
        /// </summary>
        public static void StopCFXManager()
        {
            if (m_executing_cfx_manager_process != null)
            {
                if (!(m_executing_cfx_manager_process.HasExited))
                {
                    m_executing_cfx_manager_process.Kill();
                    foreach (Process p in Process.GetProcesses())
                    {
                        if (p.ProcessName.StartsWith("BioRadC1000Server") ||
                            p.ProcessName.StartsWith("BioRadMiniOpticonDiscovery"))
                            p.Kill();
                    }
                }
            }
        }

        #endregion

    }
}
