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
    public partial class MainForm : Form
    {
        static FileScanner fileScanner;
        
        //----------------------------------------------------------
        public MainForm()
        {
            InitializeComponent();
            fileScanner = new FileScanner();
        }

        //----------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            // Сканируем все диски
            fileScanner.CheckAllDrives();

            

            

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
