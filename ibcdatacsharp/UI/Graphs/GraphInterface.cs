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
        // Borra los datos
        public void clearData();
    }
}
