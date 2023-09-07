using Syncfusion.UI.Xaml.TreeView.Engine;
using System.Windows.Controls;
using System.Windows;
using ibcdatacsharp.UI.Pacientes.Models;

namespace ibcdatacsharp.UI.Pacientes
{
    public class PacientesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate CentroTemplate { get; set; }
        public DataTemplate PacienteTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TreeViewNode)
            {
                var node = (TreeViewNode)item;
                if (node.Content is User)
                {
                    return UserTemplate;
                }
                else if (node.Content is Centro)
                {
                    return CentroTemplate;
                }
                else if (node.Content is Paciente)
                {
                    return PacienteTemplate;
                }
            }

            // Return a default template if the item type is not recognized
            return base.SelectTemplate(item, container);
        }
    }
}
