# Changelog
## v1.0.0
### Añadido
He cambiado cada componente por una carpeta, dentro de la carpeta esta el fichero del componente y sus dependencias. El fichero del componente tiene el mismo nombre que la carpeta.<br/>
Añadida la carpeta Common. Contiene los ficheros que comparten mas de un componente.<br/>
Añadido el File Browser.<br/>
Añadido el Device List.<br/> 
Añadido el boton de Scan. (Cuando se clica el botón scan se ejecuta la funcion onScanFunction. Esta en el fichero MainWindow.xaml.cs)<br/>
Añadido el botón Connect. También se puede conectar haciendo doble click en el TreeView. (En ambos casos se llama a las funciones connectIMU o connectCamera. Estan en el fichero DeviceList.xaml.cs)<br/>
Añadido el botón Disconnect. (Llama a la función disconnectIMU. Esta en el fichero DeviceList.xaml.cs)<br/>
Añadido el botón Open Camera. (Cuando se clica el botón se ejecuta la función onOpenCameraFunction. Esta en el fichero MainWindow.xaml.cs)<br/>
## v1.1.0
### Añadido
Añadido los graficos del acelerometro, magnetometro y giroscopio (GraphWindow)<br/>
Añadido el botón Capture. (Cuando se clica el botón se ejecuta la función play. Esta en el fichero GraphWindow.xaml.cs)<br/>
Añadido el botón Pause.  (Cuando se clica el botón se ejecuta la función pause. Esta en el fichero GraphWindow.xaml.cs)<br/>  
Añadido el botón Stop.  (Cuando se clica el botón se ejecuta la función stop. Esta en el fichero GraphWindow.xaml.cs)<br/>
Añadida funcion que plotea datos inventados. <br/>
## v1.1.1
### Modificado
Arreglados problemas del scrolling de los graficos.<br/>
## v1.2.0
### Añadido
Añadido el menú.<br/>
## v1.2.1
### Modificado
Cambiado mensaje de version<br/>
## v1.2.2
### Modificado
Arreglado bug del boton pause del menú<br/>
Simplificado codigo de los graficos<br/>
## v1.3.0
### Añadido
Añadidos los graficos de angulos(AngleGraph)<br/>