using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SeekAndArchive
{
    class Program
    {
        public static void Main(string[] args)
        {
            string fileName = args[0];
            string directoryName = args[1];
            FoundFiles = new List<FileInfo>();
            Watchers = new List<FileSystemWatcher>();

            DirectoryInfo rootDir = new DirectoryInfo(directoryName);

            if (!rootDir.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }

            RecursiveSearch(FoundFiles, fileName, rootDir);
            Console.WriteLine("Found {0} files.", FoundFiles.Count);

            foreach (FileInfo file in FoundFiles)
            {
                Console.WriteLine("{0}", file.FullName);
            }

            foreach (FileInfo fil in FoundFiles)
            {
                FileSystemWatcher newWatcher = new FileSystemWatcher(fil.DirectoryName, fil.Name);
                newWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                newWatcher.Renamed += new RenamedEventHandler(WatcherChanged);
                newWatcher.Deleted += new FileSystemEventHandler(WatcherChanged);
                newWatcher.EnableRaisingEvents = true;
                Watchers.Add(newWatcher);
            }

            ArchiveDirs = new List<DirectoryInfo>();
            for (int i = 0; i < FoundFiles.Count; i++)
            {
                ArchiveDirs.Add(Directory.CreateDirectory("archive" + i.ToString()));
            }

            Console.ReadKey();
        }

        public static List<FileInfo> FoundFiles { get; set; }
        public static List<FileSystemWatcher> Watchers { get; set; }
        public static List<DirectoryInfo> ArchiveDirs { get; set; }

        static void RecursiveSearch(List<FileInfo> foundFiles, string fileName, DirectoryInfo currentDirectory)
        {
            foreach (FileInfo file in currentDirectory.GetFiles())
            {
                if (file.Name == fileName) foundFiles.Add(file);
            }

            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                RecursiveSearch(foundFiles, fileName, dir);
            }
        }

        static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
            int index = Watchers.IndexOf(senderWatcher);

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed!", e.FullPath);
                ArchiveFile(ArchiveDirs[index], FoundFiles[index]);
                Console.WriteLine("Archived to {0}", ArchiveDirs[index].GetFiles()[0].FullName);
            }
            else if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                Console.WriteLine("{0} has been renamed!", e.FullPath);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Console.WriteLine("{0} has been deleted!", e.FullPath);
                Watchers.Remove(senderWatcher);
            }
        }

        static void ArchiveFile(DirectoryInfo archiveDir, FileInfo fileToArchive)
        {
            FileStream input = new FileStream(fileToArchive.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            FileStream output = File.Create(archiveDir.FullName + "\\" + fileToArchive.Name + ".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte();

            while (b != -1)
            {
                Compressor.WriteByte((byte)b);
                b = input.ReadByte();
            }

            Compressor.Close();
            input.Close();
            output.Close();
        }
    }
}
