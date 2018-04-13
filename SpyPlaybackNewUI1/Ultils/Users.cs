using System;
using System.IO;

namespace SpyandPlaybackTestTool.Ultils
{
    internal class Users
    {
        public string Username { get; set; }
        public string JsonPath { get; set; }

        public string GetUsername()
        {
            return Username = Environment.UserName;
        }

        public string CreateScriptFolder()
        {
            try
            {
                string sysdrive = Path.GetPathRoot(Environment.SystemDirectory);
                string scriptFolder = sysdrive + @"Users\" + GetUsername() + @"\AppData\Roaming\Botsina\Scripts\";

                if (!Directory.Exists(scriptFolder))
                {
                    Directory.CreateDirectory(scriptFolder);
                }

                return scriptFolder;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}