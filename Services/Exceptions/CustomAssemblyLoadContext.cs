using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDllFromPath(absolutePath);
        }

        // Bắt buộc override, nhưng không cần implement
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return IntPtr.Zero;
        }

        protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
        {
            return null;
        }
    }
}
