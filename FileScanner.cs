using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChkdiskFileScanner
{
    class FileScanner
    {
        // Массив очередей
        List<FileInfoExt> fileQueue;

        static Stat stat;
        static Log log;

        public FileScanner()
        {
            fileQueue = new List<FileInfoExt>();
        }

        //----------------------------------------------------------
        // Функция поиска по всем дискам с указанными настройками поиска
        public void CheckAllDrives()
        {
            stat.Start();

            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir);
            }

            stat.Stop();
        }

        //------------------------------------------------------------------
        // Рекурсивная функция поиска файлов по указанным настройками поиска
        public void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetFiles();
            }

            catch (UnauthorizedAccessException e)
            {

                //log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // предварительная проверка расширения 
                    FileInfoExt fiExt = new FileInfoExt(fi);

                    if ((fiExt.IsFileImage() == false) && (fiExt.IsFileVideo() == false)) continue;

                    // предварительная проверка размера
                    if (fi.Length < 30_000 || fi.Length > 4_000_000_000) continue;

  

                    if (fiExt.level > 0)
                        AddFileToList(fi, level);

                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Глобально исключаем папки
                    // Windows "Program Files" "Program Files (x86)" ProgramData
                    // эти папки уже наверное поштучно будем исключать AppData Downloads Загрузки

                    if (dirInfo.Name == "Windows" ||
                        dirInfo.Name == "Program Files" ||
                        dirInfo.Name == "Program Files (x86)" ||
                        dirInfo.Name == "ProgramData" ||
                        dirInfo.Name == "AppData" // ||
                                                  //
                                                  // dirInfo.Name == "Adult" 

                    ) continue;

                    WalkDirectoryTree(dirInfo);
                }
            }
        }

    }
}
