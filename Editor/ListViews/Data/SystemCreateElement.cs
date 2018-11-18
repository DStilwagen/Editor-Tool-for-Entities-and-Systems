using System;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data {
    public class SystemCreateElement : TreeViewItem
    {
        private Type type;
        private int  worldSelection;
        public int selectWorld
        {
            get => worldSelection;
            set => worldSelection = value;
        }
        public SystemCreateElement(Type type, int id = 0, int depth = 1) : base(id, depth, type?.Name)
        {
            this.type = type;
        }

        public void Create()
        {
            var world = World.AllWorlds[worldSelection];
            world.GetOrCreateManager(type);

            //Updates the playerloop for this systems world only,
            //switch which line is commented to update all worlds loops on creation
            if (ECSTools.UpdatePlayerLoopChoice)
                ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
            //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.AllWorlds.ToArray());
        }
    }
}