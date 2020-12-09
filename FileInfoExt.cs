using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChkdiskFileScanner
{
    class FileInfoExt 
    {
        private FileInfo fi;
        public int level;

        public FileInfoExt(FileInfo _fi)
        {
            fi = _fi;
            level = GetFileLevel();
        }

        //------------------------------------------------------------------
        public bool IsFileImage()
        {
            return (fi.Extension == ".jpg" || fi.Extension == ".jpeg") ? true : false;
        }

        //------------------------------------------------------------------
        public bool IsFileVideo()
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
        private int GetFileLevel()
        {
            // проверяем 1 уровень
            if (IsFileImage())
                return 1;

            // проверяем 2 уровень
            if (fi.Length <= 100_000_000 && IsFileVideo())
                return 2;

            // проверяем 3 уровень
            if (fi.Length > 100_000_000 &&
                fi.Length <= 200_000_000 &&
                IsFileVideo()
            ) return 3;

            // проверяем 4 уровень
            if (fi.Length > 200_000_000 &&
                fi.Length <= 300_000_000 &&
                IsFileVideo()
            ) return 4;

            // проверяем 5 уровень
            if (fi.Length > 300_000_000 &&
                fi.Length <= 400_000_000 &&
                IsFileVideo()
            ) return 5;

            // проверяем 6 уровень
            if (fi.Length > 400_000_000 &&
                fi.Length <= 500_000_000 &&
                IsFileVideo()
            ) return 6;

            // проверяем 7 уровень
            if (fi.Length > 500_000_000 &&
                fi.Length <= 1_000_000_000 &&
                IsFileVideo()
            ) return 7;

            // проверяем 8 уровень
            if (fi.Length > 500_000_000 &&
                fi.Length <= 1_000_000_000 &&
                IsFileVideo()
            ) return 8;

            // в остальных случаях возвращаем 0
            return 0;
        }

        //------------------------------------------------------------------
    }
}
