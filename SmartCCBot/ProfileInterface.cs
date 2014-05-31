using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace HREngine.Bots
{
    public static class ProfileInterface
    {
        static Assembly assembly = Assembly.LoadFile(CardTemplate.DatabasePath + "/Bots/SmartCC/Profile.dll");

        public static ProfileBehavior Behavior = null;
        public static void LoadBehavior()
        {
            Type type = assembly.GetType("HREngine.Bots.bProfileBehavior");

            Behavior = (ProfileBehavior)Activator.CreateInstance(type);
        }
    }
}
