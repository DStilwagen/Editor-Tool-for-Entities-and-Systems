using System;
using System.Collections.Generic;
using System.Linq;
using ECSTools.ListViews.Data;
using Unity.Entities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

namespace ECSTools.ListViews
{
    public class SystemListView : BaseListView
    {
        //TODO Fix timing
        //Recorder taken from Unity's Entity Debugger window to offer the same information but it does not work correctly.
        private readonly Dictionary<ScriptBehaviourManager, AverageRecorder> recordersByManager = new Dictionary<ScriptBehaviourManager, AverageRecorder>();
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

        private readonly World world;
        private int lastTimedFrame;

        public SystemListView(TreeViewState state, MultiColumnHeader multiColumnHeader, World world) :
            base(state, multiColumnHeader)
        {
            this.world               = world;
            multiColumnHeader.height = 20f;
        }

        protected override TreeViewItem BuildRoot()
        {
            LastPlayerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

            var root = new TreeViewItem(-1, -1, "Root");
            id = 0;

            foreach (var manager in world.BehaviourManagers)
            {
                root.AddChild(new SystemElement(world, manager, id++, 0));
                //CreateSystemRecorder(manager);
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            //If this row contains data for a System render it below.
            if (args.item is SystemElement sys)
            {
                int column = 0;
                if(sys.Enabled)
                    sys.Enabled = GUI.Toggle(args.GetCellRect(column++), sys.Enabled, GUIContent.none);
                else
                {
                    column++;
                }
                    GUI.Label(args.GetCellRect(column++), sys.displayName);
                //sys.Enabled = EditorGUI.Toggle(args.GetCellRect(column++), sys.Enabled);

                //Disabled because it does not update properly.
                //Timings don't update as quickly as in the Entity Debugger window.
                //if (sys.Manager is ComponentSystemBase compBase)
                //{
                //    var timingRect = args.GetCellRect(column++);
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

                //If the system exists enable update and dispose.
                if (GUI.Button(args.GetCellRect(column++), "Update"))
                    sys.Update();
                if (GUI.Button(args.GetCellRect(column++), "Dispose"))
                    sys.Dispose();
            }
        }
    }
}