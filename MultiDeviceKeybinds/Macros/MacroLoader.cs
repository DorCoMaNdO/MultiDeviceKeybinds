using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MultiDeviceKeybinds
{
    static class MacroLoader
    {
        private static IEnumerable<Type> GetTypes()
        {
            Type imacro = typeof(IMacro);

            List<Type> types = new List<Type>();
            
            foreach (string file in TypeCheckAppDomain.GetAssembliesWithType(new[] { Program.Location, Path.Combine(Program.Location, "plugins"), Path.Combine(Program.Location, "plugin"), Path.Combine(Program.Location, "addons"), Path.Combine(Program.Location, "addon") }, imacro))
            {
                Assembly asm = Assembly.LoadFile(file);

                foreach (Type t in asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && imacro.IsAssignableFrom(t))) types.Add(t);
            }

            return types;
        }

        public static Dictionary<string, IMacro> GetMacros(IEnumerable<Type> types)
        {
            Type imacro = typeof(IMacro);

            Dictionary<string, IMacro> macros = new Dictionary<string, IMacro>();

            Assembly asm = null;
            string guid = null;

            foreach (Type t in types)
            {
                if (!t.IsClass || !imacro.IsAssignableFrom(t)) continue;

                if (asm != t.Assembly)
                {
                    asm = t.Assembly;
                    
                    guid = Marshal.GetTypeLibGuidForAssembly(asm).ToString();
                }

                string typename = $"{guid}.{t}";
                
                Console.Write($"Loading IMacro type \"{typename}\"... ");

                try
                {
                    IMacro macro = (IMacro)Activator.CreateInstance(t);

                    if(string.IsNullOrWhiteSpace(macro.Name) || string.IsNullOrWhiteSpace(macro.Description))
                    {
                        Console.WriteLine("FAILED - Missing name or description");

                        continue;
                    }

                    macros.Add(typename, macro);

                    Console.WriteLine("DONE");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FAILED:\r\n{e}");
                }
            }

            Console.WriteLine();

            return macros;
        }

        public static Dictionary<string, IMacro> GetMacros()
        {
            return GetMacros(GetTypes());
        }
    }
}