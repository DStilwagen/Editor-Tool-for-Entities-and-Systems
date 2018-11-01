using System.Reflection;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data
{
    public class FieldElement : TreeViewItem
    {
        public static string Name(FieldInfo field, object obj)
        {
            return $"{field.FieldType.Name} {field.Name} = {field.GetValue(obj)}";
        }

        public int ExtraRows; //Number of extra rows that are needed to display editors properly
        private FieldInfo field;
        private object obj;
        public FieldElement(int id, int depth, FieldInfo field, ref object obj) : base(id, depth, Name(field, obj))
        {
            this.field = field;
            this.obj   = obj;
            ExtraRows = StructDrawer.StructDrawer.GetHeight(Value);
        }
        public void BuildUI(World world)
        {
            StructDrawer.StructDrawer.GetType(field, ref obj, Value, world);
        }
        public object Value
        {
            get => field.GetValue(obj);
            set => field.SetValue(obj, value);
        }
    }
}