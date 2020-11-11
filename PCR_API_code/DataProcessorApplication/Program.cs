using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PCR_Data_Processor;

namespace DataProcessorApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseFileName = "";
            string outPCRSerial = "";
            DataProcess.process_data(outPCRSerial, baseFileName);

            void help()
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                string name = Path.GetFileName(codeBase);

                Console.WriteLine("\n" +
                                  "Data Processor Application for PCR results\n" +
                                  "==========================================\n" +
                                  "\n" +
                                  "This program will take the csv results from\n " +
                                  "the csv folder and produce a json result as\n " +
                                  "when a pcr run is complete.\n" +
                                  "\n" +
                                  "Usage:\n" +
                                  "\n" +
                                  name + " PCRSerial OutputBaseName\n" +
                                  "\n" +
                                  "where:\n" +
                                  "- PCRSerial is the serial number of the PCR;\n" +
                                  "- OutputBaseName is the base name of the csv files (eg. without ' -  -  Quantification Amplification...'\n" +
                                  "\n" +
                                  "Note\n" +
                                  "====\n" +
                                  "You have to export csv files from CFX Maestro software by your own!\n");
            }

            if(args.Length != 2)
            {
                help();
            }
            else
            {
                outPCRSerial = args[0];
                baseFileName = args[1];

                Console.WriteLine("PCR Serial: " + outPCRSerial);
                Console.WriteLine("Base file name: " + baseFileName);

                Console.WriteLine("Processing results...");
                DataProcess.process_data(outPCRSerial, baseFileName);
                Console.WriteLine("Finished.");
            }
            
        }
    }
}
