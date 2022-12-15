using System;

namespace ibcdatacsharp.UI.Graphs
{
    // Interfaz que deberian implementar todos los grafos
    public interface GraphInterface
    {
        // Prepara el grafo para hacer capture
        public void initCapture();
        // Dibuja todo los datos (para hacer el replay)
        public void drawData(GraphData data);
        // Modifica los datos a mostrar segun el timeline (para el replay)
        public void onUpdateTimeLine(object sender, int frame);
        // Se ejecuta cuando hay que añadir nuevos puntos al grafo

        //public void drawRealTimeData(double accX, double accY, double accZ);
        public void onTick(object sender, EventArgs e);
        // Borra los datos
        public void clearData();
        // Se ejecuta cuando hay que actualizar el renderizado
        public void onRender(object sender, EventArgs e);
    }
}
