using ECSTools.ListViews.Data.Base;
using Unity.Entities;

namespace ECSTools.ListViews.Data
{
    public class WorldElement : BaseSystemElement
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

        public override void Create() { world = new World("New World" + id); }

        //Updates all managers;
        public override void Update()
        {
            foreach (var manager in World.BehaviourManagers)
            {
                manager.Update();
            }
        }

        public override void Dispose() { World.Dispose(); }
    }
}