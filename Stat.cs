using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChkdiskFileScanner
{
    class Stat
    {
        int[] totalFiles = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        long[] totalFileSize = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        DateTime start;
        DateTime end;

        internal void Start()
        {
            start = DateTime.Now;
        }

        //---------------------------------------------------------------------
        internal void Stop()
        {
            end = DateTime.Now;
            TimeSpan scanTime = end - start;
        }
    }
}
