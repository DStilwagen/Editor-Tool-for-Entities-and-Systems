using Unity.Entities;
using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data
{
    public class SystemElement : TreeViewItem
    {
        private ComponentSystemBase Manager;
        private World world;
        public SystemElement(World world, ScriptBehaviourManager manager, int id = 0, int depth = 1) : base(id, depth, manager.GetType().Name)
        {
            Manager = manager as ComponentSystemBase;
            this.world = world;
        }
        public bool Enabled
        {
            get => Manager?.Enabled ?? false;
            set {
                if(Manager == null)
                    return;
                Manager.Enabled = value;
            }
        }
        public void Update()
        {
            if (Manager.Enabled)
                Manager.Update();
            else if(ECSTools.ForceUpdateChoice)
            {
                Manager.Enabled = true;
                Manager.Update();
                Manager.Enabled = false;
            }
        }

        public void Dispose()
        {
            world.DestroyManager(Manager);
            //Always updates for this systems world only
            //switch which line is commented to update all worlds loops on dispose
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
            //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.AllWorlds.ToArray());
        }
    }
}