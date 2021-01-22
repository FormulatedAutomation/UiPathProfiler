using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace FormulatedAutomation.UiPathProfiler.Activities
{

    public class SystemProfiler
    {
        public class SystemSnapshot
        {
            public DateTimeOffset CreatedAt { get; set; }
            public string WindowsVersion { get; set; }
            public List<InstalledProgram> InstalledPrograms { get; set; }
            public List<ProcessDetail> ProcessDetails { get; set; }
            public string RunUUID { get; set; }
        }

        public class InstalledProgram
        {
            public string Name { get; set; }
            public string InstallDate { get; set; }
            public string Version { get; set; }
            public string Source { get; set; }
        }

        public class ProcessDetail
        {
            public string ProcessName { get; set; }
            public int Id { get; internal set; }
            public string ExecutablePath { get; internal set; }
            public string CommandLine { get; internal set; }
            public string CreationDate { get; internal set; }
            public int ParentProcessId { get; internal set; }
            public int WorkingSetSize { get; internal set; }
        }

        public SystemProfiler()
        {
        }

        public List<InstalledProgram> GetInstalledApps()
        {
            String key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            String wow64key = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            List<InstalledProgram> progList = FromUninstallRegisty("hklm", key);
            progList.AddRange(FromUninstallRegisty("hklm", wow64key));
            progList.AddRange(FromUninstallRegisty("hkcu", key));
            progList.Sort((a, b) => a.Name.CompareTo(b.Name));
            return progList;
        }

        private List<InstalledProgram> FromUninstallRegisty(string root, string key)
        {
            RegistryKey rk;
            List<InstalledProgram> progList = new List<InstalledProgram>();
            if (root == "hklm")
            {
                rk = Registry.LocalMachine.OpenSubKey(key);
            }
            else
            {
                rk = Registry.CurrentUser.OpenSubKey(key);
            }
            foreach (string skName in rk.GetSubKeyNames())
            {
                using (RegistryKey sk = rk.OpenSubKey(skName))
                {
                    try
                    {
                        progList.Add(new InstalledProgram
                        {
                            Name = RegistryValue(sk, "DisplayName"),
                            Version = RegistryValue(sk, "DisplayVersion"),
                            InstallDate = RegistryValue(sk, "InstallDate"),
                            Source = "Registry - " + root + ":" + key,
                        });
                    }
                    catch (Exception ex)
                    {
                        // Ignore errors accessing the registry, usually due to security constraints
                        Debug.WriteLine("Exception: {0}", ex.Message);
                    }
                }
            }
            List<InstalledProgram> cleanedProgList = progList.FindAll(p => p.Name.Length > 0);
            return cleanedProgList;
        }

        public List<ProcessDetail> GetRunningProcesses()
        {
            List<ProcessDetail> procList = new List<ProcessDetail>();

            var wmiQueryString = "SELECT ProcessId, Name, ExecutablePath, CommandLine, CreationDate, ParentProcessId, WorkingSetSize FROM Win32_Process";
            //var wmiQueryString = "SELECT * FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Id = (int)(uint)mo["ProcessId"],
                                Path = (string)mo["ExecutablePath"],
                                Name = (string)mo["Name"],
                                CommandLine = (string)mo["CommandLine"],
                                CreationDate = (string)mo["CreationDate"],
                                ParentProcessId = (int)(uint)mo["ParentProcessId"],
                                WorkingSetSize = (int)(UInt64)mo["WorkingSetSize"],
                            };
                foreach (var item in query)
                {
                    Process process = item.Process;
                    try
                    {
                        ProcessDetail processDetail = new ProcessDetail
                        {
                            Id = item.Id,
                            ParentProcessId = item.ParentProcessId,
                            ProcessName = item.Name,
                            ExecutablePath = item.Path,
                            CommandLine = item.CommandLine,
                            CreationDate = item.CreationDate,
                            WorkingSetSize = item.WorkingSetSize,
                        };
                        procList.Add(processDetail);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(process.Id);
                    }
                }
            }
            procList.Sort((a, b) => a.ProcessName.CompareTo(b.ProcessName));
            return procList;
        }

        public SystemSnapshot GetInitialProfile()
        {
            SystemSnapshot systemReport = new SystemSnapshot
            {
                InstalledPrograms = GetInstalledApps(),
                ProcessDetails = GetRunningProcesses()
            };
            return systemReport;
        }

        private string RegistryValue(RegistryKey rk, string key)
        {
            string val = rk.GetValue(key) as string;
            if (String.IsNullOrEmpty(val))
            {
                return "";
            }
            return val;
        }
    }

}


