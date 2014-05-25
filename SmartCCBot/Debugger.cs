using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
namespace HREngine.Bots
{
   public class Debugger
    {
        public static byte[] Serialize(Object o)
        {

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat
                = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Serialize(stream, o);
            return stream.ToArray();
        }

        public static Object BinaryDeSerialize(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat

                = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Binder

                = new VersionConfigToNamespaceAssemblyObjectBinder();
            Object obj = (Object)formatter.Deserialize(stream);
            return obj;
        }

        internal sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                try
                {
                    string ToAssemblyName = assemblyName.Split(',')[0];
                    List<Assembly> Assemblies = new List<Assembly>();

                    Assemblies.Add(Assembly.LoadFile(CardTemplate.DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Profile.dll"));
                    foreach (Assembly ass in Assemblies)
                    {
                        foreach(Type t in ass.GetTypes())
                        {
                            if(t.FullName.Contains(typeName))
                            {
                                typeToDeserialize = ass.GetType(typeName);
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
                return typeToDeserialize;
            }
        }

    }
}
