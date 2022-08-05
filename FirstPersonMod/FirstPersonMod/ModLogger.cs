using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;

namespace FirstPersonMod
{
    class ModLogger
    {
        public static void Log(string in_msg)
        {
            if (ModManager.m_isDebugActive)
                MelonLogger.Msg(in_msg);
        }
    }
}
