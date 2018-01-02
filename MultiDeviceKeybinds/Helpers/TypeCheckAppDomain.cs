using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MultiDeviceKeybinds
{
    internal class TypeCheckAppDomain : MarshalByRefObject
    {
        private static int _counter = 0;

        public TypeCheckAppDomain()
        {
        }

        private void LoadAssembly(byte[] asm)
        {
            Assembly.Load(asm);
        }

        private AppDomain _appdomain { get { return AppDomain.CurrentDomain; } }

        private string[] InternalGetAssembliesWithType(string[] paths, Type type)
        {
            List<string> files = new List<string>(), assemblies = new List<string>();

            foreach (string path in paths) if (Directory.Exists(path)) assemblies.AddRange(Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly).Concat(Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly)));

            foreach (string file in assemblies)
            {
                try
                {
                    AssemblyName asmname = AssemblyName.GetAssemblyName(file);

                    Assembly asm = Assembly.LoadFile(file);

                    if (asm.GetTypes().Any(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))) files.Add(file);
                }
                catch (BadImageFormatException)
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return files.ToArray();
        }

        private static TypeCheckAppDomain CreateInstance()
        {
            string id = $"MultiDeviceKeybinds_{++_counter}";
            string shadowCopyFolderName = $"{id}_ShadowCopy";
            string shadowCopyFolderPath = CreateShadowCopyFolder(shadowCopyFolderName);

            AppDomainSetup setup = new AppDomainSetup
            {
                CachePath = shadowCopyFolderPath
            };

            string domainName = $"{id}_AppDomain";
            AppDomain domain = AppDomain.CreateDomain(domainName, AppDomain.CurrentDomain.Evidence, setup);

            Type typeToCreate = typeof(TypeCheckAppDomain);

            TypeCheckAppDomain instance = (TypeCheckAppDomain)domain.CreateInstanceAndUnwrap(typeToCreate.Assembly.FullName, typeToCreate.FullName, false, BindingFlags.CreateInstance, null, new object[] { }, CultureInfo.InvariantCulture, null);

            return instance;
        }

        public static string[] GetAssembliesWithType(string path, Type type)
        {
            return GetAssembliesWithType(new[] { path }, type);
        }

        public static string[] GetAssembliesWithType(string[] paths, Type type)
        {
            TypeCheckAppDomain instance = CreateInstance();

            string[] files = instance.InternalGetAssembliesWithType(paths, type);

            AppDomain.Unload(instance._appdomain);

            return files;
        }

        private static string CreateShadowCopyFolder(string shadowCopyFolderName)
        {
            string shadowCopyPath = Path.Combine(Path.GetTempPath(), shadowCopyFolderName);

            if (!Directory.Exists(shadowCopyPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(shadowCopyPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return shadowCopyPath;
        }
    }
}