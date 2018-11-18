using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews.Data
{
    public class SystemListViewElement : TreeViewItem
    {
        public SystemListViewElement(int id, int depth, World world) : base(id, depth)
        {
            var headerState = BuildHeaderState(false);

            if (MultiColumnHeaderState.CanOverwriteSerializedFields(systemMultiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(systemMultiColumnHeaderState, headerState);
            systemMultiColumnHeaderState = headerState;

            var systemMultiColumnHeader = new MultiColumnHeader(headerState);

            if (systemTreeViewState == null)
                systemTreeViewState = new TreeViewState();

            systemMultiColumnHeader.ResizeToFit();

            SystemListView = new SystemListView(systemTreeViewState, systemMultiColumnHeader, world);
            SystemListView.UpdateIfNecessary();
        }

        public  BaseListView           SystemListView;
        private TreeViewState          systemTreeViewState;
        private MultiColumnHeaderState systemMultiColumnHeaderState;
        private World                  world;

        public static MultiColumnHeaderState BuildHeaderState(bool worldSelectionColumn)
        {
            var columns = new[ ]
                          {
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent =
                                      new GUIContent("Enabled", "Toogle for quickly enabling or disabling systems"),
                                  headerTextAlignment   = TextAlignment.Left,
                                  sortedAscending       = false,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 55,
                                  minWidth              = 55,
                                  maxWidth              = 55,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              },
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent =
                                      new GUIContent("System Name", "Name of a Component/JobComponent System"),
                                  headerTextAlignment   = TextAlignment.Left,
                                  sortedAscending       = false,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 300,
                                  minWidth              = 300,
                                  maxWidth              = 300,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              },
                              //Timing is currently disabled because it does not work properly.
                              //new MultiColumnHeaderState.Column
                              //{
                              //headerContent =
                              //new GUIContent("main (ms)", "Runtime of a systems OnUpdate()."),
                              ////contextMenuText = "Asset",
                              //headerTextAlignment   = TextAlignment.Center,
                              //sortedAscending       = false,
                              //sortingArrowAlignment = TextAlignment.Right,
                              //width                 = 75,
                              //minWidth              = 75,
                              //maxWidth              = 75,
                              //autoResize            = false,
                              //allowToggleVisibility = false
                              //},
                              //Create now handled in SystemCreateListView.
                              //new MultiColumnHeaderState.Column
                              //{
                              //    headerContent         = new GUIContent("Create", "Create system in selected world."),
                              //    headerTextAlignment   = TextAlignment.Center,
                              //    sortedAscending       = false,
                              //    sortingArrowAlignment = TextAlignment.Right,
                              //    width                 = 90,
                              //    minWidth              = 90,
                              //    autoResize            = false,
                              //    allowToggleVisibility = false
                              //},
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent         = new GUIContent("Update", "Call update on system."),
                                  headerTextAlignment   = TextAlignment.Center,
                                  sortedAscending       = false,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 90,
                                  minWidth              = 90,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              },
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent         = new GUIContent("Dispose", "Call dispose on system."),
                                  headerTextAlignment   = TextAlignment.Center,
                                  sortedAscending       = false,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 90,
                                  minWidth              = 90,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              }
                          };
            
            var state = new MultiColumnHeaderState(columns);
            return state;
        }

    }
}