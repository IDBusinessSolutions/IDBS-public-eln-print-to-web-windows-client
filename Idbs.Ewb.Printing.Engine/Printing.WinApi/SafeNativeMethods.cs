// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SafeNativeMethods.cs" company="ID Business Solutions Ltd.">
//
//    Copyright (C) 2014
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Idbs.Ewb.Printing.WinApi
{
    using System.Text;

    /// <summary>
    /// Internal class containing the external Win API methods
    /// </summary>
    internal class SafeNativeMethods
    {
        /// <summary> All access </summary>
        internal const uint PRINTER_ALL_ACCESS = 0x000F000C;

        /// <summary> Insufficient buffer error </summary>
        internal const int ERROR_INSUFFICIENT_BUFFER = 122;

        /// <summary> Copy new files flag </summary>
        internal const uint APD_COPY_NEW_FILES = 0x00000008;

        /// <summary> Copy all files flag </summary>
        internal const uint APD_COPY_ALL_FILES = 0x00000004;

        [DllImport("winspool.drv", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr AddPrinter(string pName, uint Level, [In] ref PRINTER_INFO_2 pPrinter);

        [DllImport("winspool.drv")]
        public static extern int ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, ref PRINTER_DEFAULTS pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool DeletePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int AddPortEx(string pName, int pLevel, ref PORT_INFO_1 lpBuffer, string pMonitorName);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DeletePort(string pName, IntPtr hWnd, string pPortName);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumPorts(string pName, uint level, IntPtr lpbPorts, uint cbBuf, ref uint pcbNeeded, ref uint pcReturned);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddPrinterDriverExW(
            [In, Optional, MarshalAs(UnmanagedType.LPWStr)] string pName, 
            uint Level, 
            [In] IntPtr pPrinter, 
            uint Flags);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumPrinterDrivers(string pName, string pEnvironment, uint level, IntPtr pDriverInfo, uint cdBuf, ref uint pcbNeeded, ref uint pcReturned);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPrinterDriverDirectory(string pName, string pEnvironment, uint Level, StringBuilder pDriverDirectory, uint cbBuf, out uint pcbNeeded);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pServerName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrinterName;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pShareName;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPortName;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverName;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pComment;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pLocation;
            
            public IntPtr pDevMode;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pSepFile;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrintProcessor;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDatatype;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pParameters;

            public IntPtr pSecurityDescriptor;

            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PORT_INFO_1
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string szPortName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_DEFAULTS
        {
            public IntPtr pDatatype;
            public IntPtr pDevMode;
            public uint DesiredAccess;
        }

        /// <summary>
        /// structure contains printer driver information
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DRIVER_INFO_6
        {
            /// <summary>
            /// The operating system version for which the driver was written. The supported value is 3.
            /// </summary>
            public uint cVersion;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the name of the driver (for example, QMS 810).
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pName;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the environment for which the driver was written (for example, Windows NT x86, Windows IA64, and Windows x64.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pEnvironment;

            /// <summary>
            /// Pointer to a null-terminated string that specifies a file name or a full path and file name for the file that contains the device driver (for example, C:\DRIVERS\Pscript.dll).
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverPath;

            /// <summary>
            /// Pointer to a null-terminated string that specifies a file name or a full path and file name for the file that contains driver data (for example, C:\DRIVERS\Qms810.ppd).
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDataFile;

            /// <summary>
            /// Pointer to a null-terminated string that specifies a file name or a full path and file name for the device driver's configuration dynamic-link library (for example, C:\DRIVERS\Pscrptui.dll).
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pConfigFile;

            /// <summary>
            /// Pointer to a null-terminated string that specifies a file name or a full path and file name for the device driver's help file (for example, C:\DRIVERS\Pscrptui.hlp).
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pHelpFile;

            /// <summary>
            /// A pointer to a MultiSZ buffer that contains a sequence of null-terminated strings. Each null-terminated string in the buffer contains the name of a file the driver depends on. The sequence of strings is terminated by an empty, zero-length string. If pDependentFiles is not NULL and does not contain any file names, it will point to a buffer that contains two empty strings.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDependentFiles;

            /// <summary>
            /// A pointer to a null-terminated string that specifies a language monitor (for example, "PJL monitor"). This member can be NULL and should be specified only for printers capable of bidirectional communication.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pMonitorName;

            /// <summary>
            /// A pointer to a null-terminated string that specifies the default data type of the print job (for example, "EMF").
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDefaultDataType;

            /// <summary>
            /// A pointer to a null-terminated string that specifies previous printer driver names that are compatible with this driver. For example, OldName1\0OldName2\0\0.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszzPreviousNames;

            /// <summary>
            /// The date of the driver package, as coded in the driver files.
            /// </summary>
            System.Runtime.InteropServices.ComTypes.FILETIME ftDriverDate;

            /// <summary>
            /// Version number of the driver. This comes out of the version structure of the driver.
            /// </summary>
            UInt64 dwlDriverVersion;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the manufacturer's name.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszMfgName;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the URL for the manufacturer.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszOEMUrl;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the hardware ID for the printer driver.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHardwareID;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the provider of the printer driver (for example, "Microsoft Windows 2000")
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszProvider;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DRIVER_INFO_3
        {
            public uint cVersion;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pName;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pEnvironment;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDriverPath;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataFile;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pConfigFile;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pHelpFile;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDependentFiles;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pMonitorName;
            
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDefaultDataType;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DRIVER_INFO_2
        {
            public uint cVersion;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pName;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pEnvironment;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverPath;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDataFile;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pConfigFile;
        }
    }
}
