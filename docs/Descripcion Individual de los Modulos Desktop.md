# AHRS
### Descripción general y propósito
Filtros Madgwick y Mahony
### Dependencias
No tiene
### Implementacion
- MadgwickAHRS.cs
- MAhonyAHRS.cs
# EKF
### Descripción general y propósito
Filtro EKF
### Dependencias
- ibcdatacsharp.UI.Common
- MathNet.Numerics.LinearAlgebra
### Implementacion
- EKF.cs
# Login
### Descripción general y propósito
Pantalla inicial de login
### Dependencias
- ibcdatacsharp.UI
- MySql.Data.MySqlClient
### Implementacion
- Connection.cs
- Dashboard.xaml
- Dashboard.xaml.cs
- LoginInfo.cs
# CamaraViewport
### Descripción general y propósito
Viewport de la camara
### Dependencias
- OpenCvSharp
- OpenCvSharp.WpfExtensions
### Implementacion
- CamaraViewport.xaml
- CamaraViewport.xaml.cs
# Commands
### Descripción general y propósito
Algunos comandos de la aplicacion
### Dependencias
- ibcdatacsharp.UI.Pacientes
- ibcdatacsharp.Login
### Implementacion
- SeleccionarPacienteCommand
- SubirTestCommand
# Common
### Descripción general y propósito
Funciones y clases de utilidad de la aplicación que se usan en varias partes
### Dependencias
No tiene
### Implementacion
- BaseObject.cs
- Helpers.cs
- PropertyNotifier.cs
# Device
### Descripción general y propósito
Sirve para generar datos aleatorios. (para testear)
### Dependencias
- ibcdatacsharp.UI.ToolBar.Enums
- ibcdatacsharp.UI.Timer
### Implementacion
- AngleArgs.cs
- Device.cs
- DeviceArgs.cs
- RawArgs
# DeviceList
### Descripción general y propósito
Componente de la lista de dispositivos (camaras y sensores)
### Dependencias
- ibcdatacsharp.DeviceList.TreeClasses
### Implementacion
- DeviceList.xaml
- DeviceList.xaml.cs
## DeviceList.Converters
### Descripción general y propósito
Sirve para adornar los datos a mostrar
### Dependencias
No tiene
### Implementacion
- BatteryConverter.cs
- ConnectedConverter.cs
- JAEnabledConverter.cs
## DeviceList.Enums
### Descripción general y propósito
Conjunto de enums que usa el paquete de la lista de dispositivos
### Dependencias
No tiene
### Implementacion
- Joint.cs
## DeviceList.TreeClasses
### Descripción general y propósito
Objetos que se muestran en la lista de dispositivos
### Dependencias
- ibcdatacsharp.Common
- ibcdatacsharp.UI.DeviceList.Enums
### Implementacion
- CameraInfo.cs
- DeviceListInfo.cs
- IMUInfo.cs
- InsolesInfo.cs
# FileBrowser
### Descripción general y propósito
Buscador de ficheros
### Dependencias
- ibcdatacsharp.Common
### Implementacion
- FileBrowser.xaml
- FileBrowser.xaml.cs
- FileManager.cs
- FolderManager.cs
- Interop.cs
- ShellManager.cs
## FileBrowser.Enums
### Descripción general y propósito
Conjunto de enums que usa el paquete FileBrowser
### Dependencias
No tiene
### Implementacion
- FileAttribute.cs
- IconSize.cs
- ItemState.cs
- ItemType.cs
- ShellAttribute.cs
## FileBrowser.ShellClasses
### Descripción general y propósito
Almacena todos los drives, carpetas y ficheros. Se encarga de la interacción entre el usuario y la UI.
### Dependencias
- ibcdatacsharp.Common
- ibcdatacsharp.UI.FileBrowser.Enums
### Implementacion
- DummyFileSystemObjectInfo.cs
- FileSystemObjectInfo.cs
## FileBrowser.ShellClasses
### Descripción general y propósito
Almacena la información extraida de la shell
### Dependencias
No tiene
### Implementacion
- ShellFileInfo.cs
# FileSaver
### Descripción general y propósito
Se encarga de manejar los buffers para guardar tanto el video como audio
### Dependencias
- ibcdatacsharp.UI.Device
- ibcdatacsharp.UI.ToolBar;
- ibcdatacsharp.UI.ToolBar.Enums;
- ibcdatacsharp.Login
- OpenCvSharp
- Newtonsoft.Json
### Implementacion
- DeviceList.xaml
- DeviceList.xaml.cs
# Filters
### Descripción general y propósito
Se encarga de escoger el filtro que se quiere usar.
### Dependencias
- AHRS
- ibcdatacsharp.UI.Common
- ibcdatacsharp.EKF.EKF
- WisewalkSDK
- MathNet.Numerics.LinearAlgebra
### Implementacion
- EKF.cs
- Filter.cs
- FilterManager.cs
- Madgwick.cs
- Mahoney.cs
- None.cs
# Graphs
### Descripción general y propósito
Se encarga de todo lo relacionado con los graficos
### Dependencias
- ibcdatacsharp.UI.ToolBar
- ibcdatacsharp.UI.ToolBar.Enums
- ibcdatacsharp.UI.Common
- ibcdatacsharp.DeviceList.TreeClasses
- ibcdatacsharp.UI.Filters
### Implementacion
- GraphData.cs
- GraphInterface.cs
- GraphManager.cs
## Graphs.Models
### Descripción general y propósito
Modelos de Scottplot de los graficos
### Dependencias
- ScottPlot;
- ScottPlot.Plottable;
### Implementacion
- Model1S.cs
- Model3S.cs
- Model4S.cs
- ModelSagital.cs
## Graphs.OneIMU
### Descripción general y propósito
Vistas de los graficos para 1 IMU
### Dependencias
- ibcdatacsharp.UI.Device
- ibcdatacsharp.UI.Graphs.Models
### Implementacion
- GraphAccelerometer.xaml
- GraphAccelerometer.xaml.cs
- GraphGyroscope.xaml
- GraphGyroscope.xaml.cs
- GraphLinAcc.xaml
- GraphLinAcc.xaml.cs
- GraphMagnetometer.xaml
- GraphMagnetometer.xaml.cs
- GraphQuaternion.xaml
- GraphQuaternion.xaml.cs
## Graphs.Sagital
### Descripción general y propósito
Vistas de los graficos para 4 IMUs (Angulos sagitales)
### Dependencias
- ibcdatacsharp.UI.Graphs.Models;
### Implementacion
- GraphAnkle.xaml
- GraphAnkle.xaml.cs
- GraphHip.xaml
- GraphHip.xaml.cs
- GraphKnee.xaml
- GraphKnee.xaml.cs
## Graphs.TwoIMU
### Descripción general y propósito
Vistas de los graficos para 2 IMUs
### Dependencias
- ibcdatacsharp.UI.Device;
- ibcdatacsharp.UI.Graphs.Models;
### Implementacion
- AngleGraphX.xaml
- AngleGraphX.xaml.cs
- AngleGraphY.xaml
- AngleGraphY.xaml.cs
- AngleGraphZ.xaml
- AngleGraphZ.xaml.cs
- GraphAngularAcceleration.xaml
- GraphAngularAcceleration.xaml.cs
- GraphAngularVelocity.xaml
- GraphAngularVelocity.xaml.cs
# MenuBar
### Descripción general y propósito
Se encarga de la barra de menús de arriba
### Dependencias
- ibcdatacsharp.UI.ToolBar
- ibcdatacsharp.UI.ToolBar.Enums
### Implementacion
- MenuBar.xaml
- MenuBar.xaml.cs
- Version.xaml
- Version.xaml.cs
## MenuBar.View
### Descripción general y propósito
Clases para manejar la recuperacion de vistas al cerrarlas
### Dependencias
- ibcdatacsharp.Common
- AvalonDock.Layout
### Implementacion
- ViewInfo.cs
- WindowInfo.cs
- WindowInfoTitle.cs
# Pacientes
### Descripción general y propósito
Vista de centros y usuarios de la base de datos
### Dependencias
- ibcdatacsharp.Login
- ibcdatacsharp.UI.Pacientes.Models
- sign_in_dotnet_wpf
- MySql.Data.MySqlClient
- Microsoft.Win32
- Newtonsoft.Json
- Syncfusion.UI.Xaml.TreeView.Engine
### Implementacion
- AuxiliarDataTemplateSelector.cs
- MedicoDataTemplateSelector.cs
- MessageRecorded.xaml
- MessageRecorded.xaml.cs
- Pacientes.xaml
- Pacientes.xaml.cs
- RemoteTransactions.cs
## Pacientes.Models
### Descripción general y propósito
Modelos para el MVVM de la vista Pacientes
### Dependencias
No tiene
### Implementacion
- Auxiliar.cs
- Centro.cs
- CentroRoot.cs
- Medico.cs
- Paciente.cs
- Test.cs
# SagitalAngles
### Descripción general y propósito
Calculos de los angulos sagitales (4 IMUs)
### Dependencias
- ibcdatacsharp.DeviceList.TreeClasses
- ibcdatacsharp.UI.Common
- ibcdatacsharp.UI.Graphs.Models
- ibcdatacsharp.UI.Graphs.Sagital
- ibcdatacsharp.UI.ToolBar
- ibcdatacsharp.UI.ToolBar.Enums
### Implementacion
- SagitalAngles.cs
- Utils.cs
# Subjects
### Descripción general y propósito
No se usa (es como el modulo Pacientes pero no esta completo)
# TimeLine
### Descripción general y propósito
Vista de la linea de tiempo (de abajo)
### Dependencias
- ibcdatacsharp.UI.ToolBar
- ScottPlot
- ScottPlot.Plottable
### Implementacion
- Model.cs
- TimeLine.xaml
- TimeLine.xaml.cs
# Timer
### Descripción general y propósito
Timer sacado de un code project no se usa
# ToolBar
### Descripción general y propósito
Se encarga del manejo de la barra de herramientas. (arriba debajo del menu)
### Dependencias
- ibcdatacsharp.UI.Common
- ibcdatacsharp.UI.Filters
- ibcdatacsharp.UI.SagitalAngles
- ibcdatacsharp.DeviceList.TreeClasses
- ibcdatacsharp.UI.FileSaver
- ibcdatacsharp.Login
- ibcdatacsharp.UI.Graphs
- Microsoft.WindowsAPICodePack.Shell
- Microsoft.WindowsAPICodePack.Shell.PropertySystem
### Implementacion
- ToolBar.xaml
- ToolBar.xaml.cs
- VirtualToolBar.cs
- VirtualToolBarProperties.cs
- Enums/PauseState.cs
- Enums/RecordState.cs
