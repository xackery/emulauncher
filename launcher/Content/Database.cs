﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQEmu_Launcher
{
    internal class Database
    {
        private readonly static StatusType status = StatusType.Database;
        public static Task FixTask { get; private set; }

        public static int Check()
        {
            StatusLibrary.SetIsFixNeeded(status, true);

            string path = Application.StartupPath + "\\db\\mariadb-10.6.10-winx64";
            if (!Directory.Exists(path))
            {
                StatusLibrary.SetText(status, "mariadb not found");
                return 0;
            }

            if (!File.Exists(Application.StartupPath + "\\db\\heidi\\heidisql.exe"))
            {
                StatusLibrary.SetText(status, "heidi not found");
                return 20;
            }

            if (!File.Exists(Application.StartupPath + "\\db\\heidi\\portable_settings.txt"))
            {
                StatusLibrary.SetText(status, "heidi settings not found");
                return 40;
            }

            StatusLibrary.SetIsFixNeeded(status, false);
            StatusLibrary.SetText(status, "Database found");
            return 100;
        }

        public static void FixCheck()
        {
            StatusLibrary.Log("Running fix check");
            FixTask = Task.Run(() => 
            { 
                StatusLibrary.LockUI(); 
                Fix(false); 
                Check(); 
                StatusLibrary.UnlockUI(); 
            });
        }

        public static async void Fix(bool fixAll)
        {
            int startStage = Check();
            int stage;

            stage = await UtilityLibrary.Download(40, 80, "https://archive.mariadb.org/mariadb-10.6.10/winx64-packages/mariadb-10.6.10-winx64.zip", "cache", "mariadb-10.6.10-winx64.zip", 136);
            if (stage == -1) { return; }
            if (!fixAll && stage > startStage) { return; }

            stage = await UtilityLibrary.Extract(40, 80, "cache", "mariadb-10.6.10-winx64.zip", "db", $"{Application.StartupPath}\\db\\mariadb-10.6.10-winx64\bin\\mysqld.exe", 136);
            if (stage == -1) { return; }
            if (!fixAll && stage > startStage) { return; }

            stage = await UtilityLibrary.Download(40, 80, "https://www.heidisql.com/downloads/releases/HeidiSQL_12.1_64_Portable.zip", "cache", "HeidiSQL_12.1_64_Portable.zip", 17);
            if (stage == -1) { return; }
            if (!fixAll && stage > startStage) { return; }

            stage = await UtilityLibrary.Extract(40, 80, "cache", "HeidiSQL_12.1_64_Portable.zip", "db\\heidi", $"{Application.StartupPath}\\db\\heidi\\heidisql.exe", 17);
            if (stage == -1) { return; }
            if (!fixAll && stage > startStage) { return; }

            stage = await UtilityLibrary.Download(40, 80, "https://raw.githubusercontent.com/xackery/emulauncher/main/launcher/Assets/portable_settings.txt", "db\\heidi", "portable_settings.txt", 1);
            if (stage == -1) { return; }
            if (!fixAll && stage > startStage) { return; }
        }

        public static void FixAll()
        {
            StatusLibrary.Log("Fixing all database issues");
            FixTask = Task.Run(() => { StatusLibrary.LockUI(); Fix(true); Check(); StatusLibrary.UnlockUI(); });
        }

    }
}
