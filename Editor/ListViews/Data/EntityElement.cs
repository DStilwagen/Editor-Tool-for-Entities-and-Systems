using Unity.Entities;
using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data
{
    public class EntityElement : TreeViewItem
    {
        public Entity Entity;
        public EntityElement(int id, Entity entity) : base(id, 0, entity.ToString()) { Entity = entity; }
    }
}