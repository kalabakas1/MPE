using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Models;
using MPE.Models.Interfaces;

namespace MPE.MongoDB.Logic
{
    public class Initializer : IInitializer
    {
        private ProcessStartInfo _processStartInfo;
        private Process _process;

        public void Start()
        {
            try
            {
                _processStartInfo = new ProcessStartInfo();
                _processStartInfo.FileName = @"C:\Data\Projects\MPE\External\MongoDB\bin\mongod.exe";
                _processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _processStartInfo.UseShellExecute = false;
                _processStartInfo.Arguments = @"--quiet --dbpath " + EnsureDataDirecturyExists();

                _process = Process.Start(_processStartInfo);
            }
            catch
            {
                
            }
        }

        public void Stop()
        {
            try
            {
                _process.Kill();
            }
            catch
            {
                
            }
        }

        private string EnsureDataDirecturyExists()
        {
            var dataDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            return dataDirectory;
        }
    }
}
