using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO: записывать в лог-файл


namespace ChkdiskFileScanner
{
    class FileInfoExt
    {
        public FileInfo fi;
        public int level;
    }

    public partial class MainForm : Form
    {
        // Массив очередей
        List<FileInfoExt> fileQueue;
        int[] totalFiles = {0, 0, 0, 0, 0, 0, 0, 0, 0 };
        long[] totalFileSize = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //----------------------------------------------------------
        public MainForm()
        {
            InitializeComponent();
            fileQueue = new List<FileInfoExt>(); 
        }

        //----------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {

            DateTime start = DateTime.Now;
            // Сканируем все диски
            CheckAllDrives();

            DateTime end = DateTime.Now;
            TimeSpan scanTime = end - start;


            // Считываем из списка в текстовый лог-файл


            for (int lvl = 1; lvl <= 8; lvl++)
            {
                foreach (FileInfoExt element in fileQueue)
                {
                  
                    if (element.level == lvl)
                        Console.WriteLine($"{lvl} - {element.fi.FullName}");

                }

                Console.WriteLine($"Группа {lvl} - Файлов: {totalFiles[lvl]}");
                Console.WriteLine($"Группа {lvl} - Размером: {totalFileSize[lvl]}");
            }

            Console.WriteLine($"Время сканирования: {scanTime}");
            Console.WriteLine($"Файлов: {totalFiles[0]}");
            Console.WriteLine($"Размером: {totalFileSize[0]}");


        }


        //----------------------------------------------------------
        // Функция поиска по всем дискам с указанными настройками поиска
        void CheckAllDrives()
        {
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
        }


        // Рекурсивная функция поиска файлов по указанным настройками поиска
        //------------------------------------------------------------------
        void WalkDirectoryTree(System.IO.DirectoryInfo root)
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

                    if ((IsFileImage(fi) == false) && (IsFileVideo(fi) == false)) continue;

                    // предварительная проверка размера
                    if (fi.Length < 30_000 || fi.Length > 4_000_000_000) continue;

                    int level = GetFileLevel(fi);

                    if (level > 0)
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
                        dirInfo.Name == "AppData" ||
                        //
                        dirInfo.Name == "Adult" 

                    ) continue;
                 
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        //------------------------------------------------------------------
        static bool IsFileImage(FileInfo fi)
        {
            return (fi.Extension == ".jpg" || fi.Extension == ".jpeg") ? true : false; 
        }        
        
        //------------------------------------------------------------------
        static bool IsFileVideo(FileInfo fi)
        {
            return (fi.Extension == ".mov" ||
                fi.Extension == ".mp4" ||
                fi.Extension == ".mpg" ||
                fi.Extension == ".avi" ||
                fi.Extension == ".mts" ||
                fi.Extension == ".3gp") ? true : false; 
        }

        //------------------------------------------------------------------
        // Определяем к какому уровню относится этот файл
        // 1 - jpg
        // 2 - видео < 100 Mb 
        // 3 - видео 100-200 Mb
        // 4 - видео 200-300 Mb
        // 5 - видео 300-400 Mb
        // 6 - видео 400-500 Mb
        // 7 - видео 500-1000 Mb
        // 8 - видео 1-4 Gb
        static int GetFileLevel(FileInfo fi)
        {
            // проверяем 1 уровень
            if (IsFileImage(fi)) 
                return 1;

            // проверяем 2 уровень
            if (fi.Length <= 100_000_000 && IsFileVideo(fi)) 
                return 2;

            // проверяем 3 уровень
            if (fi.Length > 100_000_000 && 
                fi.Length <= 200_000_000 && 
                IsFileVideo(fi)
            ) return 3;
                
            // проверяем 4 уровень
            if (fi.Length > 200_000_000 &&
                fi.Length <= 300_000_000 &&
                IsFileVideo(fi)
            ) return 4;            
            
            // проверяем 5 уровень
            if (fi.Length > 300_000_000 &&
                fi.Length <= 400_000_000 && 
                IsFileVideo(fi)
            ) return 5;
            
            // проверяем 6 уровень
            if (fi.Length > 400_000_000 &&
                fi.Length <= 500_000_000 &&
                IsFileVideo(fi)
            ) return 6;  
            
            // проверяем 7 уровень
            if (fi.Length > 500_000_000 &&
                fi.Length <= 1_000_000_000 &&
                IsFileVideo(fi)
            ) return 7;
            
            // проверяем 8 уровень
            if (fi.Length > 500_000_000 &&
                fi.Length <= 1_000_000_000 &&
                IsFileVideo(fi)
            ) return 8;

            // в остальных случаях возвращаем 0
            return 0;
        }

        //------------------------------------------------------------------
        void AddFileToList(FileInfo fi, int level)
        {
            FileInfoExt fiExt = new FileInfoExt();
            fiExt.fi = fi;
            fiExt.level = level;
            totalFiles[0]++;
            totalFiles[level]++;
            totalFileSize[0] += fi.Length;
            totalFileSize[level] += fi.Length;
            fileQueue.Add(fiExt);
        }


    }
}
