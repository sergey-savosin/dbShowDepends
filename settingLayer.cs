using System;
using Microsoft.Win32;

namespace dbShowDepends
{
    class SettingLayer
    {
        //корневой путь к настройкам в реестре
        const string RegistryRoot = @"HKEY_CURRENT_USER\Software\LK\DBShowDepends";
        const string OcsBranch = @"\DBSettings";

        // генерирует исключение в случае ошибки
        public static void SaveDbParams (DbParams par)
        {
            const string regPath = RegistryRoot + OcsBranch + "\\" + "default";
            try
            {
                Registry.SetValue(regPath, "serverName", par.ServerName);
                Registry.SetValue(regPath, "serverWinAuth", par.ServerWinAuth);
                Registry.SetValue(regPath, "serverLogin", par.ServerLogin);
                Registry.SetValue(regPath, "serverPassword", par.ServerPassword);
                Registry.SetValue(regPath, "dbName", par.DbName);
            }
            catch (Exception e)
            {
                throw;
                //return "Error saving setting: " + e.Message;
            }
        }

        // генерирует исключение в случае ошибки
        public static DbParams LoadDefaultParams()
        {
            const string regPath = RegistryRoot + OcsBranch + "\\" + "default";
            string serverName, serverLogin, serverPassword, dbName;
            bool serverWinAuth;

            try
            {
                string s = (string)Registry.GetValue(regPath, "serverName", "") ?? "";
                serverName = s;

                s = (string)Registry.GetValue(regPath, "serverWinAuth", "True") ?? "True";
                serverWinAuth = s == "True";

                s = (string)Registry.GetValue(regPath, "serverLogin", "") ?? "";
                serverLogin = s;

                s = (string)Registry.GetValue(regPath, "serverPassword", "") ?? "";
                serverPassword = s;

                s = (string)Registry.GetValue(regPath, "dbName", "") ?? "";
                dbName = s;
            }
            catch (Exception e)
            {
                throw;// "Error loading setting: " + e.Message;
            }

            return new DbParams(serverName, serverWinAuth, serverLogin, serverPassword, dbName);
        }

    }
}
