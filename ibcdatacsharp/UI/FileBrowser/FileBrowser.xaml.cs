using System.Windows.Controls;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ibcdatacsharp.UI.FileBrowser.ShellClasses;
using System.Diagnostics;

namespace ibcdatacsharp.UI.FileBrowser
{
    public partial class FileBrowser : Page
    {
        private const string INITIAL_PATH = "C:\\Temp";
        public FileBrowser()
        {
            InitializeComponent();
            InitializeFileSystemObjects();
        }
        #region Events

        private void FileSystemObject_AfterExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void FileSystemObject_BeforeExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Wait;
        }
        // Funcion que se ejecuta al hacer doble click sobre un fichero
        private void onItemDoubleClick(object sender, EventArgs e)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            FileSystemObjectInfo fileSystemObjectInfo = ((TreeViewItem)sender).DataContext as FileSystemObjectInfo;
            FileSystemInfo fileSystemInfo = fileSystemObjectInfo.FileSystemInfo;
            if(fileSystemInfo is DirectoryInfo)
            {
                return;
            }
            string path = fileSystemInfo.FullName; // Usar este path para acceder al fichero
            Trace.WriteLine(path);
        }

        #endregion

        #region Methods
        // Inicializa los drives y expande el Escritorio
        private void InitializeFileSystemObjects()
        {
            var drives = DriveInfo.GetDrives();
            DriveInfo
                .GetDrives()
                .ToList()
                .ForEach(drive =>
                {
                    var fileSystemObject = new FileSystemObjectInfo(drive);
                    fileSystemObject.BeforeExplore += FileSystemObject_BeforeExplore;
                    fileSystemObject.AfterExplore += FileSystemObject_AfterExplore;
                    treeView.Items.Add(fileSystemObject);
                });
            PreSelect(INITIAL_PATH);
        }
        // Carpeta seleccionada por defecto
        private void PreSelect(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            var driveFileSystemObjectInfo = GetDriveFileSystemObjectInfo(path);
            driveFileSystemObjectInfo.IsExpanded = true;
            PreSelect(driveFileSystemObjectInfo, path);
        }
        // Funcion auxiliar recursiva de Preselect
        private void PreSelect(FileSystemObjectInfo fileSystemObjectInfo,
            string path)
        {
            foreach (var childFileSystemObjectInfo in fileSystemObjectInfo.Children)
            {
                var isParentPath = IsParentPath(path, childFileSystemObjectInfo.FileSystemInfo.FullName);
                if (isParentPath)
                {
                    if (string.Equals(childFileSystemObjectInfo.FileSystemInfo.FullName, path))
                    {
                        childFileSystemObjectInfo.IsExpanded = true;
                        /* Fichero encontrado */
                    }
                    else
                    {
                        childFileSystemObjectInfo.IsExpanded = true;
                        PreSelect(childFileSystemObjectInfo, path);
                    }
                }
            }
        }

        #endregion

        #region Helpers
        // Obtiene el drive de un path
        private FileSystemObjectInfo GetDriveFileSystemObjectInfo(string path)
        {
            var directory = new DirectoryInfo(path);
            var drive = DriveInfo
                .GetDrives()
                .Where(d => d.RootDirectory.FullName == directory.Root.FullName)
                .FirstOrDefault();
            return GetDriveFileSystemObjectInfo(drive);
        }
        // Devuelve el FileSystemObjectInfo de un drive a partir del DriveInfo
        private FileSystemObjectInfo GetDriveFileSystemObjectInfo(DriveInfo drive)
        {
            foreach (var fso in treeView.Items.OfType<FileSystemObjectInfo>())
            {
                if (fso.FileSystemInfo.FullName == drive.RootDirectory.FullName)
                {
                    return fso;
                }
            }
            return null;
        }

        private bool IsParentPath(string path,
            string targetPath)
        {
            return path.StartsWith(targetPath);
        }

        #endregion
    }
}

