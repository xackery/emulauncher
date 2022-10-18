﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQEmu_Launcher.Manage
{
    internal class QueryServ
    {
        static readonly StatusType status = StatusType.QueryServ;

        public static void Check()
        {
            Process[] pname = Process.GetProcessesByName("QueryServ");
            if (pname.Length > 0)
            {
                StatusLibrary.SetText(status, $"QueryServ is running");
                StatusLibrary.SetIsFixNeeded(status, false);
                return;
            }
            StatusLibrary.SetText(status, "QueryServ is not running");
            StatusLibrary.SetIsFixNeeded(status, true);
        }

        public static void Start()
        {
            try
            {
                StatusLibrary.SetStatusBar($"starting QueryServ");
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = $"{Application.StartupPath}\\server\\queryserv.exe",
                        Arguments = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                Check();
            } catch (Exception e)
            {
                string result = $"failed QueryServ start \"server\\queryserv.exe\": {e.Message}";
                StatusLibrary.SetStatusBar(result);
                MessageBox.Show(result, "QueryServ Start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Stop()
        {
            int QueryServCount = 0;
            try
            {

                Process[] workers = Process.GetProcessesByName("QueryServ");
                Console.WriteLine($"found {workers.Length} QueryServ instances");
                foreach (Process worker in workers)
                {
                    Console.WriteLine($"stopping QueryServ pid {worker.Id}");
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                    QueryServCount++;
                }
                if (QueryServCount == 0) StatusLibrary.SetStatusBar("QueryServ not found to stop");
                else StatusLibrary.SetStatusBar($"stopped {QueryServCount} QueryServ instances");
            }
            catch (Exception e)
            {
                string result = $"failed to stop QueryServ: {e.Message}";
                StatusLibrary.SetStatusBar("QueryServ stop failed");
                MessageBox.Show(result, "QueryServ Stop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
