using SHDocVw;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MultiDeviceKeybinds
{
    //public enum ExplorerViewMode
    //{
    //    Icons = 1,
    //    SmallIcons = 2,
    //    List = 3,
    //    Details = 4,
    //    Tiles = 6,
    //    Content = 8
    //}

    //public enum ExplorerIconSize
    //{
    //    Small = 16,
    //    Medium = 48,
    //    Large = 96,
    //    ExtraLage = 256
    //}

    //public enum FileDetailInfo
    //{
    //    Retrieves_the_info_tip_inf = -1,
    //    Name = 0,
    //    Size = 1,
    //    Year = 15,
    //    Type = 2,
    //    Date_Modified = 3,
    //    Date_Created = 4,
    //    Date_Accessed = 5,
    //    Attributes = 6,
    //    Status = 7,
    //    Owner = 8,
    //    Author = 9,
    //    Title = 10,
    //    Subject = 11,
    //    Category = 12,
    //    Pages = 13,
    //    Comments = 14,
    //    Copyright = 15,
    //    Artist = 16,
    //    Album_Title = 17,
    //    Track_Number = 19,
    //    Genre = 20,
    //    Protected = 23,
    //    Camera_Model = 24,
    //    Date_Picture_Taken = 25,
    //    Dimensions = 26,
    //    Duration = 27,
    //    Bit_Rate = 28,
    //    /*Not_used = 27,
    //    Not_used_file = 28,*/
    //    //Not_used = 29,
    //    Company = 30,
    //    Description = 31,
    //    File_Version = 32,
    //    Product_Name_Chapter = 33,
    //    //Scripting Quicktest Profess11ional Page 63 
    //    Product_Version = 34,
    //}

    //public class FolderWrapper
    //{
    //    private InternetExplorer folder;

    //    public string Path
    //    {
    //        get
    //        {
    //            return new Uri(folder.LocationURL).LocalPath;
    //        }
    //        set
    //        {
    //            //folder.LocationURL = new Uri(value).AbsoluteUri;
    //            folder.Navigate(new Uri(value).AbsoluteUri);
    //        }
    //    }

    //    public ExplorerViewMode ViewMode
    //    {
    //        get
    //        {
    //            return (ExplorerViewMode)(uint)folder.Document.CurrentViewMode;
    //        }
    //        set
    //        {
    //            folder.Document.CurrentViewMode = (int)value;
    //        }
    //    }

    //    public ExplorerIconSize IconSize
    //    {
    //        get
    //        {
    //            return (ExplorerIconSize)(uint)folder.Document.IconSize;
    //        }
    //        set
    //        {
    //            folder.Document.IconSize = (int)value;
    //        }
    //    }

    //    public string SortColumns
    //    {
    //        get
    //        {
    //            return folder.Document.SortColumns;
    //        }
    //        set
    //        {
    //            folder.Document.SortColumns = value;
    //        }
    //    }

    //    public FolderWrapper(InternetExplorer ie)
    //    {
    //        folder = ie;
    //    }

    //    public override string ToString()
    //    {
    //        return $"{Path}|{ViewMode}|{IconSize}|{SortColumns}";
    //    }
    //}

    static class ExplorerHelper
    {
        public static IEnumerable<InternetExplorer> GetOpenFolders()
        {
            List<InternetExplorer> folders = new List<InternetExplorer>();

            ShellWindows shellWindows = new ShellWindows();

            foreach (InternetExplorer ie in shellWindows)
            {
                if (!Path.GetFileNameWithoutExtension(ie.FullName).ToLower().Equals("explorer")) continue;

                folders.Add(ie);

                // Save the location off to your application
                //Console.WriteLine($"Explorer location : {ie.LocationURL}");
                //Console.WriteLine($"Path: {new Uri(ie.LocationURL).LocalPath}");
                //string path = new Uri(ie.LocationURL).LocalPath;

                //Console.WriteLine(ie.LocationName);

                // Setup a trigger for when the user navigates
                //ie.NavigateComplete2 += new DWebBrowserEvents2_NavigateComplete2EventHandler(handlerMethod);

                //Console.WriteLine((int)ie.Document.CurrentViewMode);
                //Console.WriteLine((int)ie.Document.IconSize);
                //Console.WriteLine((string)ie.Document.SortColumns);
                /*if((string)ie.Document.SortColumns == "prop:-System.ItemNameDisplay;")
                {
                    Console.WriteLine(new Uri(ie.LocationURL).LocalPath);
                    ie.Document.SortColumns = "prop:System.ItemNameDisplay;";
                    break;
                }*/
                /*FolderWrapper f = new FolderWrapper(ie);
                Console.WriteLine(f);*/
                //Console.WriteLine(ie.Document.GetType().GetProperties().Length);
                //Console.WriteLine(ie.Document.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Length);
                //Console.WriteLine(ie.Document.GetType());
                /*Console.WriteLine(ie.Document.ViewOptions);
                Console.WriteLine(ie.Document.GroupBy);
                Console.WriteLine(ie.Document.FolderFlags);*/
                /*ShellFolderView folderview = (ShellFolderView)ie.Document;
                Console.WriteLine(folderview.Folder.Items().Count);*/
                /*Console.WriteLine(folderview.SelectedItems().Count);
                if (folderview.SelectedItems().Count > 0)
                {
                    FolderItem item = folderview.SelectedItems().Item(0);
                    Console.WriteLine(item.Path);
                    //Console.WriteLine(folderview.Folder.GetDetailsOf(item, (int)FileDetailInfo.Name)); // works
                    //Console.WriteLine(GetShell32NameSpace(new Uri(ie.LocationURL).LocalPath).GetDetailsOf(item, (int)FileDetailInfo.Size));
                }*/
                //break;
            }

            return folders;
        }

        /*public static Folder GetShell32NameSpace(object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Folder)shellAppType.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }*/

        public static IEnumerable<string> GetOpenFolderPaths()
        {
            return GetFolderPaths(GetOpenFolders());
        }

        public static IEnumerable<string> GetFolderPaths(IEnumerable<InternetExplorer> folders)
        {
            List<string> paths = new List<string>();

            foreach (InternetExplorer ie in folders)
            {
                try
                {
                    paths.Add(new Uri(ie.LocationURL).LocalPath);
                }
                catch
                {
                }
            }

            return paths;
        }

        public static bool IsFolderPathOpen(string path)
        {
            return IsFolderPathOpen(GetOpenFolderPaths(), path);
        }

        public static bool IsFolderPathOpen(IEnumerable<string> openpaths, string path)
        {
            return openpaths.Any(p => p.Length == path.Length && p.Equals(path, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}