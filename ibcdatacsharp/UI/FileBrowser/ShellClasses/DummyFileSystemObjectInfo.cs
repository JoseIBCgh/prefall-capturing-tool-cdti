using System.IO;

namespace ibcdatacsharp.UI.FileBrowser.ShellClasses
{
    // Clase que se añade a un directorio si no se han explorado sus ficheros y subdirectorios aun.
    internal class DummyFileSystemObjectInfo : FileSystemObjectInfo
    {
        public DummyFileSystemObjectInfo()
            : base(new DirectoryInfo("DummyFileSystemObjectInfo"))
        {
        }
    }
}
