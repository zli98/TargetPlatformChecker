using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetPlatformChecker
{
    public class TargetPlatformInfo
    {
        public string FilePath { get; set; }
        public string BinaryPlatform { get; set; }
        public string ProcessorArchitecture { get; set; }
    }
}
