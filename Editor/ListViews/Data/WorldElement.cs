using Unity.Entities;
using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data
{
    public class WorldElement : TreeViewItem
    {
        public World World => world ?? (world = World.Active);
        
        private World world;
        private EntityManager entityManager;
        //World is always at the 0 depth node
        public WorldElement(int id, World world) : base(id, 0, world.Name)
        {
            this.world    = world;
            entityManager = world.GetExistingManager<EntityManager>();
        }
        public void Dispose() { World.Dispose(); }
        
        //Update player loop for this world.
        public void UpdatePlayerLoop()
        {
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        }
        //Updates all managers;
        public void UpdateAll()
        {
            foreach (var manager in World.BehaviourManagers)
            {
                manager.Update();
            }
        }
    }
}