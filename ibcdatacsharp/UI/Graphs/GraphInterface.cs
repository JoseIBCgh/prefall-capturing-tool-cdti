using System;

namespace ibcdatacsharp.UI.Graphs
{
    // Interfaz que deberian implementar todos los grafos
    public interface GraphInterface
    {
        // Se ejecuta cuando hay que añadir nuevos puntos al grafo
        public void onTick(object sender, EventArgs e);
        // Borra los datos
        public void clearData();
        // Se ejecuta cuando hay que actualizar el renderizado
        public void onRender(object sender, EventArgs e);
    }
}
