using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulatedAutomation.UiPathProfiler.Activities
{
    public class Profiler
    {
        private readonly string outputDir;

        public Profiler(string outputDir)
        {
            this.outputDir = outputDir;
        }

        public class OutputFiles
        {
            public string ProcessListPath { get; set; }
            public string SoftwareListPath { get; set; }
        }

        public OutputFiles WriteProfile()
        {
            DateTime dt = DateTime.Now;
            String dateStr = dt.ToString("s", DateTimeFormatInfo.InvariantInfo);
            dateStr = dateStr.Replace(":", ".");

            string installedOutputFile = Path.Combine(outputDir, String.Format("installed-{0}.csv", dateStr));
            string processesOutputFile = Path.Combine(outputDir, String.Format("processes-{0}.csv", dateStr));

            WriteProcessProfile(processesOutputFile);
            WriteSoftwareProfile(installedOutputFile);

            return new OutputFiles
            {
                ProcessListPath = processesOutputFile,
                SoftwareListPath = installedOutputFile
            };

        }

        public int WriteProcessProfile(string path)
        {
            SystemProfiler systemProfiler = new SystemProfiler();
            List<SystemProfiler.ProcessDetail> systemProcesses = systemProfiler.GetRunningProcesses();

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(systemProcesses);
            }
            return systemProcesses.Count;
        }

        public int WriteSoftwareProfile(string path)
        {
            SystemProfiler systemProfiler = new SystemProfiler();
            List<SystemProfiler.InstalledProgram> systemSoftware = systemProfiler.GetInstalledApps();

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(systemSoftware);
            }
            return systemSoftware.Count;
        }

    }
}
