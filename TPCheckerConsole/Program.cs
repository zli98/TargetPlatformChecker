using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetPlatformChecker;

namespace TPCheckerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FileFinder fileFinder = new FileFinder();
            string[] files = fileFinder.GetFiles(args[0]);

            if (!files.Any())
            {
                Console.WriteLine("Please specify the file or directory at the command line.");
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    TargetPlatformInfo tpInfo = new TargetPlatform().GetTargetPlatformInfo(file);
                    Console.WriteLine("File       : " + tpInfo.FilePath);
                    Console.WriteLine("Binary Type: " + tpInfo.BinaryPlatform);
                    Console.WriteLine("CPU Type   : " + tpInfo.ProcessorArchitecture);
                    Console.WriteLine(Environment.NewLine);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format("Exception: file = {0}. Error = {1}", file, ex.Message));
                }
            }

            Console.WriteLine("Done. Please press the ENTER key to exit the application.");
            Console.ReadLine();
        }


    }
}
