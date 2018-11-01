using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews.Data.Base
{
    public abstract class BaseSystemElement : TreeViewItem
    {
        [SerializeField] private bool systemExists;
        public bool Exists
        {
            get => systemExists;
            set => systemExists = value;
        }

        public BaseSystemElement() : base(-1, -1, "Root") { }

        public BaseSystemElement(int id, int depth, string name)
        {
            this.id = id;
            this.depth = depth;
            this.displayName = name;
        }

        public abstract void Create();
        public abstract void Update();
        public abstract void Dispose();
    }
}