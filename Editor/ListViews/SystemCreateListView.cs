using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews.Data
{
    public class SystemCreateListView : BaseListView
    {
        private List<Type> systemList;
        private int id;
        private int worldSelector;

        public static MultiColumnHeaderState BuildHeaderState()
        {
            var columns = new[ ]
                              {
                                  new MultiColumnHeaderState.Column
                                  {
                                      //headerContent =
                                          //new GUIContent("System Name", "Name of a Component/JobComponent System"),
                                      headerTextAlignment   = TextAlignment.Left,
                                      sortedAscending       = false,
                                      sortingArrowAlignment = TextAlignment.Right,
                                      width                 = 300,
                                      minWidth              = 300,
                                      autoResize            = true,
                                      allowToggleVisibility = false
                                  },
                                  new MultiColumnHeaderState.Column
                                  {
                                      headerContent =
                                          new GUIContent("World Selection",
                                                         "Select the world to create system in."),
                                      headerTextAlignment   = TextAlignment.Left,
                                      sortedAscending       = false,
                                      sortingArrowAlignment = TextAlignment.Right,
                                      width                 = 100,
                                      minWidth              = 100,
                                      maxWidth              = 150,
                                      autoResize            = false,
                                      allowToggleVisibility = false
                                  },
                                  new MultiColumnHeaderState.Column
                                  {
                                      headerContent =
                                          new GUIContent("Create", "Create system in selected world."),
                                      headerTextAlignment   = TextAlignment.Center,
                                      sortedAscending       = false,
                                      sortingArrowAlignment = TextAlignment.Right,
                                      width                 = 90,
                                      minWidth              = 90,
                                      autoResize            = false,
                                      allowToggleVisibility = false
                                  },
                                  //new MultiColumnHeaderState.Column
                                  //{
                                  //    headerContent         = new GUIContent("Update", "Call update on system."),
                                  //    headerTextAlignment   = TextAlignment.Center,
                                  //    sortedAscending       = false,
                                  //    sortingArrowAlignment = TextAlignment.Right,
                                  //    width                 = 90,
                                  //    minWidth              = 90,
                                  //    autoResize            = false,
                                  //    allowToggleVisibility = false
                                  //},
                                  //new MultiColumnHeaderState.Column
                                  //{
                                  //    headerContent         = new GUIContent("Dispose", "Call dispose on system."),
                                  //    headerTextAlignment   = TextAlignment.Center,
                                  //    sortedAscending       = false,
                                  //    sortingArrowAlignment = TextAlignment.Right,
                                  //    width                 = 90,
                                  //    minWidth              = 90,
                                  //    autoResize            = false,
                                  //    allowToggleVisibility = false
                                  //}
                              };
                var state = new MultiColumnHeaderState(columns);
                return state;
        }

        public static SystemCreateListView CreateListView(MultiColumnHeaderState systemMultiColumnHeaderState, TreeViewState systemTreeViewState, List<Type> allSystems)
        {
            var headerState = BuildHeaderState();
            
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(systemMultiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(systemMultiColumnHeaderState, headerState);
            systemMultiColumnHeaderState = headerState;

            var systemMultiColumnHeader = new MultiColumnHeader(headerState);
            
            if (systemTreeViewState == null)
                systemTreeViewState = new TreeViewState();

            systemMultiColumnHeader.ResizeToFit();

            var systemCreateListView = new SystemCreateListView(systemTreeViewState, systemMultiColumnHeader, allSystems);
            systemCreateListView.UpdateIfNecessary();

            return systemCreateListView;
        }
        public SystemCreateListView(TreeViewState state, MultiColumnHeader multiColumnHeader, List<Type> allSystems) :
            base(state, multiColumnHeader)
        {
            //Make list unselectable.
            getNewSelectionOverride = (item, selection, shift) => new List<int>();;
            multiColumnHeader.height = 0f;
            systemList = new List<Type>(allSystems);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }
        protected override TreeViewItem BuildRoot()
        {
            LastPlayerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

            var root = new TreeViewItem(-1, -1, "Root");
            id = 0;
            var firstElement = new SystemTitleElement(id++, 0, "System Name");
            
            foreach (var system in systemList)
                firstElement.AddChild(new SystemCreateElement(system, id++, 1));
            
            state.expandedIDs.Add(firstElement.id);
            if (firstElement.hasChildren)
                root.AddChild(firstElement);
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            switch (args.item)
            {
                case SystemTitleElement systemName:
                {
                    var rect = args.GetCellRect(0);
                    rect.xMin += GetContentIndent(systemName);

                    GUI.Label(rect, args.item.displayName);
                    break;
                }
                case SystemCreateElement systemCreate:
                {
                    var rect = args.GetCellRect(0);
                    rect.xMin += GetContentIndent(systemCreate);

                    GUI.Label(rect, systemCreate.displayName);

                    rect      =  args.GetCellRect(1);
                    rect.xMin += GetContentIndent(systemCreate);

                    systemCreate.selectWorld = EditorGUI.Popup(args.GetCellRect(1), systemCreate.selectWorld,
                                                               World.AllWorlds.Select(w => w.Name).ToArray());
                    rect      =  args.GetCellRect(2);
                    rect.xMin += GetContentIndent(systemCreate);

                    if (GUI.Button(args.GetCellRect(2), "Create"))
                        systemCreate.Create();
                    break;
                }
            }
        }

        
    }
}