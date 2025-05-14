using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThemeInstaller
{
    public class Wow64ToSys32IntPtr : IDisposable
    {
        private readonly IntPtr _ptr;

        public Wow64ToSys32IntPtr()
        {
            if (Is64BitOs)
            {
                _ptr = new IntPtr();
                External.Wow64DisableWow64FsRedirection(ref _ptr);
            }
        }

        public void Dispose()
        {
            if (Is64BitOs)
            {
                External.Wow64RevertWow64FsRedirection(_ptr);
            }
        }

        public static bool Is64BitOs
        {
            get { return Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null; }
        }
    }
}
