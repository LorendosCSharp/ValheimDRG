using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;


namespace FirstValheimMod
{
    public static class MyLogger
    {
        public static ManualLogSource Logger { get; private set; }


        public static void Init(string name)
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(name);
            
        }
        public static void Info(string msg) => Logger?.LogInfo(msg);
        public static void Warn(string msg) => Logger?.LogWarning(msg);
        public static void Error(string msg) => Logger?.LogError(msg);
    }
}
