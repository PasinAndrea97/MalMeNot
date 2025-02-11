﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MalMeNOT
{
    class DllChekcs
    {
        public static void AbandonedComKeysBuildUp()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_ClassicCOMClassSetting");

            List<string> inprocsvr32 = new List<string>();

            //Query all objects for their InProcSvr32 value and if not null, check that the file still exists
            foreach (ManagementObject queryObj in searcher.Get())
            {
                object inprocsvrVal = queryObj["InprocServer32"];
                string inprocsvrStr = Convert.ToString(inprocsvrVal);
                string resolvedEnvVars = Environment.ExpandEnvironmentVariables(inprocsvrStr);
                string path = resolvedEnvVars.Trim('"');

                if (path != null)
                {
                    if (!File.Exists(path))
                    {
                        object clsidVal = queryObj["ComponentID"];
                        string clsidStr = Convert.ToString(clsidVal);
                        string missingKey = path + "," + clsidStr;
                        if (missingKey.StartsWith("C:")) //This filters out things like combase.dll
                            inprocsvr32.Add(missingKey);
                    }
                }
            }

            List<string> distinct = inprocsvr32.Distinct().ToList();
            List<string> cleanList = distinct.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            foreach (string dll in cleanList)
            {
                //Console.WriteLine(dll);File.AppendAllText(System.Environment.MachineName + @"\Resultdll.txt", dll + Environment.NewLine); 
            }

        }
    }
}
