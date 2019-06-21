using System;
using System.Collections.Generic;
using System.IO;

namespace SeekAndArchive
{
    class Program
    {
        public static void Main(string[] args)
        {
            string fileName = args[0];
            string directoryName = args[1];
            FoundFiles = new List<FileInfo>();

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

            Console.ReadKey();
        }

        public static List<FileInfo> FoundFiles { get; set; }

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
    }
}
