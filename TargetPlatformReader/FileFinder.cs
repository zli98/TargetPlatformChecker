using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetPlatformChecker
{
    public class FileFinder
    {
        public string[] GetFiles(string path, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string[] files = null;
            if (IsDirectory(path))
            {
                string[] dllArray = Directory.GetFiles(path, "*.dll", searchOption);
                string[] exeArray = Directory.GetFiles(path, "*.exe", searchOption);
                files = new string[dllArray.Length + exeArray.Length];
                dllArray.CopyTo(files, 0);
                exeArray.CopyTo(files, dllArray.Length);
            }
            else
            {
                files = new string[1];
                files[0] = path;
            }

            return files;
        }

        public bool IsDirectory(string path)
        {
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);

            //detect whether its a directory or file
            return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
        }
    }
}
