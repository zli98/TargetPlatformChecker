using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TargetPlatformChecker
{
    public class TargetPlatform
    {
        public enum MachineType : ushort
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
            IMAGE_FILE_MACHINE_AM33 = 0x1d3,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664,
            IMAGE_FILE_MACHINE_ARM = 0x1c0,
            IMAGE_FILE_MACHINE_EBC = 0xebc,
            IMAGE_FILE_MACHINE_I386 = 0x14c,
            IMAGE_FILE_MACHINE_IA64 = 0x200,
            IMAGE_FILE_MACHINE_M32R = 0x9041,
            IMAGE_FILE_MACHINE_MIPS16 = 0x266,
            IMAGE_FILE_MACHINE_MIPSFPU = 0x366,
            IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466,
            IMAGE_FILE_MACHINE_POWERPC = 0x1f0,
            IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1,
            IMAGE_FILE_MACHINE_R4000 = 0x166,
            IMAGE_FILE_MACHINE_SH3 = 0x1a2,
            IMAGE_FILE_MACHINE_SH3DSP = 0x1a3,
            IMAGE_FILE_MACHINE_SH4 = 0x1a6,
            IMAGE_FILE_MACHINE_SH5 = 0x1a8,
            IMAGE_FILE_MACHINE_THUMB = 0x1c2,
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169,
        }

        public TargetPlatformInfo GetTargetPlatformInfoFromNativeBinary(string filePath)
        {
            var is64bit = UnmanagedDllIs64Bit(filePath);
            TargetPlatformInfo targetPlatformInfo = new TargetPlatformInfo();
            targetPlatformInfo.FilePath = filePath;
            targetPlatformInfo.BinaryPlatform = "Native Binary";
            if (is64bit == null)
            {
                targetPlatformInfo.ProcessorArchitecture = "unknown";
            }
            else
            {
                targetPlatformInfo.ProcessorArchitecture = (bool)is64bit ? "x64" : "x86";
            }

            return targetPlatformInfo;
        }

        public TargetPlatformInfo GetTargetPlatformInfoFromManagedBinary(string filePath)
        {
            AssemblyName assemblyName = System.Reflection.AssemblyName.GetAssemblyName(filePath);
            TargetPlatformInfo targetPlatformInfo = new TargetPlatformInfo();
            targetPlatformInfo.FilePath = filePath;
            targetPlatformInfo.BinaryPlatform = "Managed Binary";
            switch (assemblyName.ProcessorArchitecture)
            {
                case ProcessorArchitecture.Amd64:
                case ProcessorArchitecture.IA64:
                    targetPlatformInfo.ProcessorArchitecture = "x64";
                    break;
                case ProcessorArchitecture.X86:
                    targetPlatformInfo.ProcessorArchitecture = "x86";
                    break;
                case ProcessorArchitecture.MSIL:
                    targetPlatformInfo.ProcessorArchitecture = "anycpu";
                    break;
                case ProcessorArchitecture.Arm:
                    targetPlatformInfo.ProcessorArchitecture = "arm";
                    break;
                default:
                    targetPlatformInfo.ProcessorArchitecture = "unknown";
                    break;
            }

            return targetPlatformInfo;
        }

        public TargetPlatformInfo GetTargetPlatformInfo(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception(string.Format("File {0} doesn't exist.", filePath));

            if (!IsManagedBinary(filePath))
            {
                return GetTargetPlatformInfoFromNativeBinary(filePath);
            }
            else
            {
                return GetTargetPlatformInfoFromManagedBinary(filePath);
            }
        }

        private bool IsManagedBinary(string filePath)
        {
            bool ret = true;
            try
            {
                System.Reflection.AssemblyName.GetAssemblyName(filePath);
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        public MachineType GetDllMachineType(string dllPath)
        {
            // See http://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx
            // Offset to PE header is always at 0x3C.
            // The PE header starts with "PE\0\0" =  0x50 0x45 0x00 0x00,
            // followed by a 2-byte machine type field (see the document above for the enum).
            //
            FileStream fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x3c, SeekOrigin.Begin);
            Int32 peOffset = br.ReadInt32();
            fs.Seek(peOffset, SeekOrigin.Begin);
            UInt32 peHead = br.ReadUInt32();

            if (peHead != 0x00004550) // "PE\0\0", little-endian
                throw new Exception("Can't find PE header");

            MachineType machineType = (MachineType)br.ReadUInt16();
            br.Close();
            fs.Close();
            return machineType;
        }

        // Returns true if the dll is 64-bit, false if 32-bit, and null if unknown
        public bool? UnmanagedDllIs64Bit(string dllPath)
        {
            switch (GetDllMachineType(dllPath))
            {
                case MachineType.IMAGE_FILE_MACHINE_AMD64:
                case MachineType.IMAGE_FILE_MACHINE_IA64:
                    return true;
                case MachineType.IMAGE_FILE_MACHINE_I386:
                    return false;
                default:
                    return null;
            }
        }

    }
}
