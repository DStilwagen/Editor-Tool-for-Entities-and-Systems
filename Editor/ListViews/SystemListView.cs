using System;
using System.Collections.Generic;
using ECSTools.ListViews.Data;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

namespace ECSTools.ListViews
{
    public class SystemListView : BaseListView
    {
        //TODO Fix timing
        //Recorder taken from Unity's Entity Debugger window to offer the same information but it does not work correctly.
        private class AverageRecorder
        {
            private readonly Recorder recorder;
            private          int      frameCount;
            private          int      totalNanoseconds;
            private          float    lastReading;

            public AverageRecorder(Recorder recorder)
            {
                this.recorder = recorder;
            }

            public void Update()
            {
                ++frameCount;
                totalNanoseconds += (int)recorder.elapsedNanoseconds;
            }

            public float ReadMilliseconds()
            {
                if (frameCount > 0)
                {
                    lastReading = (totalNanoseconds/1e6f) / frameCount;
                    frameCount  = totalNanoseconds = 0;
                }

                return lastReading;
            }
        }
        public void UpdateTimings()
        {
            if (Time.frameCount == this.lastTimedFrame)
                return;
            foreach (AverageRecorder averageRecorder in this.recordersByManager.Values)
                averageRecorder.Update();
            this.lastTimedFrame = Time.frameCount;
        }
        void CreateSystemRecorder(ScriptBehaviourManager manager)
        {
            var recorder = Recorder.Get($"{world.Name} {manager.GetType().FullName}");
            recordersByManager.Add(manager, new AverageRecorder(recorder));
            recorder.enabled = true;
        }

        private readonly Dictionary<ScriptBehaviourManager, AverageRecorder> recordersByManager = new Dictionary<ScriptBehaviourManager, AverageRecorder>();
        private readonly World world;
        private List<Type> inactiveSystems;
        private int lastTimedFrame;
        private int id;

        public SystemListView(TreeViewState state, MultiColumnHeader multiColumnHeader, World world, List<Type> allSystems) :
            base(state, multiColumnHeader)
        {
            this.world = world;
            multiColumnHeader.height = 20f;
            inactiveSystems = new List<Type>(allSystems);
        }
        protected override TreeViewItem BuildRoot()
        {
            LastPlayerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

            var root = new TreeViewItem(-1, -1, "Root");
            id = 0;
            foreach (var manager in world.BehaviourManagers)
            {
                root.AddChild(new SystemElement(true, world, manager, id: id++, depth: 0));
                //CreateSystemRecorder(manager);
                if(inactiveSystems != null)
                    inactiveSystems.Remove(manager.GetType());
            }
            foreach (var inactiveSystem in inactiveSystems)
            {
                root.AddChild(new SystemElement(false, world, type: inactiveSystem, id: id++, depth: 0));
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        protected override void RowGUI(RowGUIArgs args)
        {
            //If this row contains data for a System render it below.
            if (args.item is SystemElement)
            {
                var sys = args.item as SystemElement;
                
                GUI.Label(args.GetCellRect(0), sys.displayName);

                //Disabled because it does not update properly.
                //Timings don't update as quickly as in the Entity Debugger window.
                //if (sys.Manager is ComponentSystemBase compBase)
                //{
                //    var timingRect = args.GetCellRect(1);
                //    if (compBase.ShouldRunSystem())
                //    {
                //        var recorder = recordersByManager[sys.Manager];
                //        GUI.Label(timingRect, recorder.ReadMilliseconds().ToString("f3"));
                //    }
                //    else
                //    {
                //        GUI.enabled = false;
                //        GUI.Label(timingRect, "not run");
                //        GUI.enabled = true;
                //    }
                //}

                //If the system exists enable update and dispose. If it doesn't exists disable update and dispose and enable create
                if (sys.Exists)
                {
                    if (GUI.Button(args.GetCellRect(2), "Update"))
                        sys.Update();
                    if (GUI.Button(args.GetCellRect(3), "Dispose"))
                        sys.Dispose();
                    GUI.enabled = false;
                    if (GUI.Button(args.GetCellRect(1), "Create"))
                        sys.Create();
                    GUI.enabled = true;
                }
                else
                {
                    if (GUI.Button(args.GetCellRect(1), "Create"))
                        sys.Create();
                    GUI.enabled = false;
                    if (GUI.Button(args.GetCellRect(2), "Update"))
                        sys.Update();
                    if (GUI.Button(args.GetCellRect(3), "Dispose"))
                        sys.Dispose();
                    GUI.enabled = true;
                }
            }
        }
    }
}