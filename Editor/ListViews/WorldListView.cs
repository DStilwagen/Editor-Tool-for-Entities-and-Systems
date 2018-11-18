using System;
using System.Collections.Generic;
using System.Linq;
using ECSTools.ListViews.Data;
using Unity.Entities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews
{
    public class WorldListView : BaseListView
    {
        public WorldListView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            columnIndexForTreeFoldouts = 1;
            multiColumnHeader.height = 20f;
            getNewSelectionOverride = (item, selection, shift) => new List<int>();
        }

        private Dictionary<WorldElement, SystemListViewElement> systemListByWorld =
            new Dictionary<WorldElement, SystemListViewElement>();
        protected override TreeViewItem BuildRoot()
        {
            systemListByWorld.Clear();

            LastPlayerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

            var root = new TreeViewItem(-1, -1, "Root");

            var expandedIDs = new List<int>();

            foreach (var world in World.AllWorlds)
            {
                var worldElement = new WorldElement(id++, world);
                
                expandedIDs.Add(worldElement.id);
                
                var systemList = new SystemListViewElement(id++, 1, world);
                systemListByWorld.Add(worldElement, systemList);

                worldElement.AddChild(systemList);
                if (worldElement.hasChildren)
                    root.AddChild(worldElement);
            }

            state.expandedIDs = expandedIDs;
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        public static MultiColumnHeaderState BuildWorldMultiColumnHeaderState()
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
            if (item is SystemListViewElement)
                return ((SystemListViewElement) item).SystemListView.totalHeight;
            return base.GetCustomRowHeight(row, item);
        }

        protected override void RowGUI (RowGUIArgs args)
        {
            if (args.item is SystemListViewElement item)
            {
                var rect = args.GetCellRect(1);
                rect.xMin += GetContentIndent(args.item);
                item.SystemListView.UpdateIfNecessary();
                item.SystemListView.OnGUI(rect);
                return;
            }
            if (args.item is WorldElement world)
            {
                var rect = args.GetCellRect(1);
                rect.xMin += GetContentIndent(args.item);
                GUILayout.BeginArea(rect);
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(world.World.Name);
                if (GUILayout.Button(new GUIContent("Update All",
                                                    $"Updates all the systems for {world.World.Name} in the order they appear,"), GUILayout.Height(rowHeight - 3)))
                    world.UpdateAll();
                if (GUILayout.Button(new GUIContent("Update Selected",
                                                    $"Updates selected systems for {world.World.Name} in the order they appear,"),
                                     GUILayout.Height(rowHeight - 3)))
                {
                    foreach (var systems in systemListByWorld[world].SystemListView.GetRows().Where(a => systemListByWorld[world].SystemListView.GetSelection().Contains(a.id)).ToArray())
                    {
                        ((SystemElement) systems).Update();

                    }
                }
                if (GUILayout.Button(new GUIContent("Update PlayerLoop",
                                                    $"Update the PlayerLoop for {world.World.Name}"), GUILayout.Height(rowHeight - 3)))
                    world.UpdatePlayerLoop();
                if (GUILayout.Button(new GUIContent("Dispose", $"Dispose of {world.World.Name}"), GUILayout.Height(rowHeight - 3)))
                    world.Dispose();

                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
                return;
                
            }
        }
    }
}