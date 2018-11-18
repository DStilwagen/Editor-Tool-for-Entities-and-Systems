using Unity.Entities;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Experimental.LowLevel;

namespace ECSTools.ListViews
{
    public abstract class BaseListView : TreeView
    {
        protected int id;
        protected BaseListView(TreeViewState state) : base(state) { }
        protected BaseListView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader) { }
        
        private PlayerLoopSystem lastPlayerLoop;
        public PlayerLoopSystem LastPlayerLoop
        {
            get => lastPlayerLoop;
            
            set => lastPlayerLoop = value;
        }

        private bool PlayerLoopsMatch(PlayerLoopSystem a, PlayerLoopSystem b)
        {
            if (a.type != b.type)
                return false;
            if (a.subSystemList == b.subSystemList)
                return true;
            if (a.subSystemList == null || b.subSystemList == null)
                return false;
            if (a.subSystemList.Length != b.subSystemList.Length)
                return false;
            for (var i = 0; i < a.subSystemList.Length; ++i)
            {
                if (!PlayerLoopsMatch(a.subSystemList[i], b.subSystemList[i]))
                    return false;
            }
            
            return true;
        }

        public virtual void UpdateIfNecessary()
        {
            if (!PlayerLoopsMatch(lastPlayerLoop, ScriptBehaviourUpdateOrder.CurrentPlayerLoop))
            {
                //if (rootItem != null)
                    Reload();
            }
        }
    }
}