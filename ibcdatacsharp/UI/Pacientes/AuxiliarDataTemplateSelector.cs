using ibcdatacsharp.UI.Pacientes.Models;
using Microsoft.VisualBasic.ApplicationServices;
using Syncfusion.UI.Xaml.TreeView.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ibcdatacsharp.UI.Pacientes
{
    public class AuxiliarDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AuxiliarTemplate { get; set; }
        public DataTemplate CentroTemplate { get; set; }
        public DataTemplate PacienteTemplate { get; set; }
        public DataTemplate TestTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TreeViewNode)
            {
                var node = (TreeViewNode)item;
                if (node.Content is Auxiliar)
                {
                    return AuxiliarTemplate;
                }
                else if (node.Content is CentroRoot)
                {
                    return CentroTemplate;
                }
                else if (node.Content is Paciente)
                {
                    return PacienteTemplate;
                }
                else if (node.Content is Test)
                {
                    return TestTemplate;
                }
            }

            // Return a default template if the item type is not recognized
            return base.SelectTemplate(item, container);
        }
    }
}
