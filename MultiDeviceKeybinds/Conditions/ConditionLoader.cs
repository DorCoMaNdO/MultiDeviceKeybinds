using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MultiDeviceKeybinds
{
    static class ConditionLoader
    {
        private static IEnumerable<Type> GetTypes()
        {
            Type icondition = typeof(ICondition);

            List<Type> types = new List<Type>();

            foreach (string file in TypeCheckAppDomain.GetAssembliesWithType(new[] { Program.Location, Path.Combine(Program.Location, "plugins"), Path.Combine(Program.Location, "plugin"), Path.Combine(Program.Location, "addons"), Path.Combine(Program.Location, "addon") }, icondition))
            {
                Assembly asm = Assembly.LoadFile(file);

                foreach (Type t in asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && icondition.IsAssignableFrom(t))) types.Add(t);
            }

            return types;
        }

        public static Dictionary<string, ICondition> GetConditions(IEnumerable<Type> types)
        {
            Type icondition = typeof(ICondition);

            Dictionary<string, ICondition> conditions = new Dictionary<string, ICondition>();

            Assembly asm = null;
            string guid = null;

            foreach (Type t in types)
            {
                if (!t.IsClass || !icondition.IsAssignableFrom(t)) continue;

                if (asm != t.Assembly)
                {
                    asm = t.Assembly;

                    guid = Marshal.GetTypeLibGuidForAssembly(asm).ToString();
                }

                string typename = $"{guid}.{t}";

                Console.Write($"Loading ICondition type \"{typename}\"... ");

                try
                {
                    ICondition condition = (ICondition)Activator.CreateInstance(t);

                    if (string.IsNullOrWhiteSpace(condition.Name) || string.IsNullOrWhiteSpace(condition.Description))
                    {
                        Console.WriteLine("FAILED - Missing name or description");

                        continue;
                    }

                    conditions.Add(typename, condition);

                    Console.WriteLine("DONE");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FAILED:\r\n{e}");
                }
            }

            Console.WriteLine();

            return conditions;
        }

        public static Dictionary<string, ICondition> GetConditions()
        {
            return GetConditions(GetTypes());
        }
    }
}