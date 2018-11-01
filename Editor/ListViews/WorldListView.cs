using System;
using System.Collections.Generic;
using ECSTools.ListViews.Data;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews
{
    public class WorldListView : BaseListView
    {
        public WorldListView(TreeViewState state, MultiColumnHeader multiColumnHeader, List<Type> allSystems) : base(state, multiColumnHeader)
        {
            columnIndexForTreeFoldouts = 1;
            this.allSystems = allSystems;
        }

        private int currentId;
        private List<SystemListViewItem> systems = new List<SystemListViewItem>();
        private List<WorldElement> worlds = new List<WorldElement>();
        private List<Type> allSystems;
        protected override TreeViewItem BuildRoot()
        {
            systems.Clear();
            worlds.Clear();

            LastPlayerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

            var root = new TreeViewItem(-1, -1, "Root");

            var expandedIDs = new List<int>();
            foreach (var child in World.AllWorlds)
            {
                var world = new WorldElement(currentId++, child);
                
                worlds.Add(world);
                expandedIDs.Add(world.id);
                
                var systemList = new SystemListViewItem(currentId++, 1, child, allSystems);
                systems.Add(systemList);
                
                world.AddChild(systemList);
                if (world.hasChildren)
                    root.AddChild(world);
            }
            state.expandedIDs = expandedIDs;
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            var columns = new[ ]
                          {
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent = new GUIContent("Id", "Id number."),
                                  //contextMenuText = "Asset",
                                  headerTextAlignment   = TextAlignment.Center,
                                  sortedAscending       = false,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 0,
                                  minWidth              = 1,
                                  maxWidth              = 2500,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              },
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent = new GUIContent("World", "The World."),
                                  //contextMenuText = "World",
                                  headerTextAlignment   = TextAlignment.Left,
                                  sortedAscending       = false,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 625,
                                  minWidth              = 625,
                                  autoResize            = true,
                                  allowToggleVisibility = false,

                              }
                          };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            if (item is SystemListViewItem)
                return ((SystemListViewItem) item).SystemListView.totalHeight;
            return base.GetCustomRowHeight(row, item);
        }

        protected override void RowGUI (RowGUIArgs args)
        {
            var item = args.item is SystemListViewItem header ? header : null;

            if (item != null)
            {
                var rect = args.GetCellRect(1);
                rect.xMin += GetContentIndent(args.item);
                item.SystemListView.UpdateIfNecessary();
                item.SystemListView.OnGUI(rect);
                return;
            }

            args.rowRect = args.GetCellRect(1);
            base.RowGUI(args);
        }
    }
}