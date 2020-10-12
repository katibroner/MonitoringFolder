using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace MonitoringFolder
{
    class Program
    {
        private int TimeoutMillis = 2000;
        FileSystemWatcher fsw = new FileSystemWatcher(@"C:\Users\migkbron\Desktop\test");
        System.Threading.Timer m_timer = null;
        List<String> files = new List<string>();

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Initial();
        }

        public void Initial()
        {
            fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

           // fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.Created += new FileSystemEventHandler(OnChanged);
            //fsw.Deleted += new FileSystemEventHandler(OnChanged);
            fsw.EnableRaisingEvents = true;

            if (m_timer == null)
            {
                m_timer = new System.Threading.Timer(new System.Threading.TimerCallback(OnWatchedFileChange), null, Timeout.Infinite, Timeout.Infinite);
            }

            Console.WriteLine("Press \'Enter\' to quit the sample.");
            Console.ReadLine();
        

        }

        void OnChanged(Object s, FileSystemEventArgs e)
        {
            List<string> Txt = new List<string>();
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            if (!files.Contains(e.Name))
            {
                files.Add(e.Name);
                
            }
        
            mutex.ReleaseMutex();
            m_timer.Change(TimeoutMillis, Timeout.Infinite);

           // WatcherChangeTypes wct = e.ChangeType;

           // Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
            /* string FullPathTxt = e.FullPath;
             Console.WriteLine(FullPathTxt);
             if(FullPathTxt.EndsWith(".txt"))
             {
                 Txt.Add(FullPathTxt);
                 System.IO.File.WriteAllLines(@"C:\Users\migkbron\Desktop\TxtLines.txt", Txt);

           }*/

        }

        private void OnWatchedFileChange(object state)
        {
            List<String> backup = new List<string>();
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            backup.AddRange(files);

            files.Clear();
            mutex.ReleaseMutex();
            //System.IO.File.WriteAllLines(@"C:\Users\migkbron\Desktop\WriteLines.txt", backup);


            foreach (string file in backup)
            {
                if (file.EndsWith(".xml"))
                {
                    File.WriteAllLines(@"C:\Users\migkbron\Desktop\WriteLines.txt", backup);
                    Console.WriteLine(file + " changed");    
                }
                else
                {
                    File.WriteAllLines(@"C:\Users\migkbron\Desktop\TxtLines.txt", backup);
                    Console.WriteLine(file + " changed");
                }

                if (File.Exists(@"C:\Users\migkbron\Desktop\TxtLines.txt") && File.Exists(@"C:\Users\migkbron\Desktop\WriteLines.txt"))
                {
                    
                    CreateFileSpecificID();
                }
            }
        }

        static void CreateFileSpecificID()
        {
            string text1 = File.ReadAllText(@"C:\Users\migkbron\Desktop\WriteLines.txt");
            string fullPath = @"C:\Users\migkbron\Desktop\test\" + text1;
            Console.WriteLine(fullPath);
            XDocument doc = XDocument.Load(fullPath);
            List<string> accessDeniedList = new List<string>();
            accessDeniedList = GetIdPackaging();
            var pageInfo = (from xml2 in doc.Descendants("Packaging")
                            where accessDeniedList.Contains((xml2.Attribute("IdPackaging")).Value)
                            select xml2);

            pageInfo.Remove();
            doc.Save("C:/Users/migkbron/Desktop/400strace1.xml");
        }

        static List<string> GetIdPackaging()
        {
            string text2 = File.ReadAllText("C:/Users/migkbron/Desktop/TxtLines.txt");
            string fullPathtxt = @"C:\Users\migkbron\Desktop\test\" + text2;
            Console.WriteLine(fullPathtxt);
            fullPathtxt = fullPathtxt.Replace("\n","");
            fullPathtxt = fullPathtxt.Replace("\r", "");
            string[] lines = File.ReadAllLines(fullPathtxt);

            List<string> text = new List<string>();
            List<string> idPackaging = new List<string>();
            foreach (string line in lines)
            {
                text.Add(line);
            }
            for (int i = 0; i < text.Count; i++)
            {
                // Part B: access element with index.
                Console.WriteLine($"{i} = {text[i]}");
                string subString = "unit";
                int indexOfSubstring = text[i].IndexOf(subString);
                string subStringFromTo = text[i].Substring(indexOfSubstring + 6);
                string idPackaging1 = subStringFromTo.Substring(0, 10);
                idPackaging.Add(idPackaging1);

            }
            foreach (string id in idPackaging)
            {
                Console.WriteLine(id);
            }
            return idPackaging;
        }
    }
}
