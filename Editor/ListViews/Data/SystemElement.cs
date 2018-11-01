using System;
using ECSTools.ListViews.Data.Base;
using Unity.Entities;

namespace ECSTools.ListViews.Data
{
    public class SystemElement : BaseSystemElement
    {
        private World world;
        private Type type;
        private ScriptBehaviourManager  Manager;
        private static Type GetName(ScriptBehaviourManager manager = null, Type type = null)
        {
            if (manager == null && type == null)
                throw new ArgumentException("Can't Have Both Manager and Type Equal Null");
            return type ?? manager.GetType();
        }

        public SystemElement(bool exists, World world, ScriptBehaviourManager manager = null, Type type = null, int id = 0, int depth = 1) : base(id, depth, GetName(manager, type).Name)
        {
            if (manager == null && type == null)
                throw new ArgumentException("Can't Have Both Manager and Type Equal Null");
            Exists  = exists;
            Manager = manager;
            this.world   = world ?? World.Active;
            this.type    = type ?? Manager.GetType();
        }
        public override void Create()
        {
            Manager = world.CreateManager(type);
            Exists = true;

            //Updates the playerloop for this systems world only,
            //switch which line is commented to update all worlds loops on creation
            if (ECSTools.UpdatePlayerLoopChoice)
                ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
                //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.AllWorlds.ToArray());
        }
        public override void Update() { Manager.Update(); }

        public override void Dispose()
        {
            if (!Exists)
                return;
            world.DestroyManager(Manager);
            Exists = false;

            //Always updates for this systems world only
            //switch which line is commented to update all worlds loops on dispose
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
            //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.AllWorlds.ToArray());
        }
    }
}