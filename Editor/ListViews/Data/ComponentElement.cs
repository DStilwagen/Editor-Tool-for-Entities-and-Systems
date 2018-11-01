using Unity.Entities;
using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data
{
    public class ComponentElement : TreeViewItem
    {
        private static string Name(ComponentType type)
        {
            var str = $"{type.ToString()}";
            foreach (var @interface in type.GetManagedType().GetInterfaces())
            {
                str += $": {@interface.Name}";
            }
            return str + " : " + type.TypeIndex;
        }

        public ComponentElement(ComponentType type, int id, int depth = 0) : base(id, depth, Name(type)) { this.type = type; }
        public ComponentType Type => type;
        private ComponentType type;
    }
}