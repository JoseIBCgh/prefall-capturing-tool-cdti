He cambiado cada componente por una carpeta, dentro de la carpeta esta el fichero del componente y sus dependencias. El fichero del componente tiene el mismo nombre que la carpeta.<br/>
Añadida la carpeta Common. Contiene los ficheros que comparten mas de un componente.<br/>
Añadido el File Browser.<br/>
Añadido el Device List y el boton de Scan. Cuando se clica el botón scan se ejecuta la funcion onScanFunction. Esta en el fichero MainWindow.xaml.cs<br/>
Añadido el botón Connect. También se puede conectar haciendo doble click en el TreeView. En ambos casos se llama a las funciones connectIMU o connectCamera. Estan en el fichero DeviceList.xaml.cs<br/>