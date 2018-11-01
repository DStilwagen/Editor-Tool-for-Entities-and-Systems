using System;
using System.Collections.Generic;
using System.Linq;
using ECSTools.ListViews.Data;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews
{
    public class ComponentTypeListView : BaseListView
    {
        public Dictionary<ComponentType, object> Objects = new Dictionary<ComponentType, object>();
        public delegate void SelectionChange<in T1, T2>(T1 type, ref T2 obj);

        
        private SelectionChange<ComponentType, object> selectionChanged;
        private Func<World> worldChanged;
        private bool shouldUpdate;

        public override void UpdateIfNecessary()
        {
            if (shouldUpdate)
                Reload();
            base.UpdateIfNecessary();
        }
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if(FindItem(selectedIds[0], rootItem) is ComponentElement element)
            {
                if(Objects.TryGetValue(element.Type, out var obj) == false)
                    return;
                selectionChanged?.Invoke(element.Type, ref obj);
                Objects[element.Type] = obj;
            }
            shouldUpdate = true;
        }
        private static MultiColumnHeader BuilMultiColumnHeader(out MultiColumnHeaderState headerState)
        {
            headerState = BuildColumnHeaderState();
            var multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.ResizeToFit();
            multiColumnHeader.height = 18;
            return multiColumnHeader;
        }
        private static MultiColumnHeaderState BuildColumnHeaderState()
        {
            var columns = new[ ]
                          {
                              new MultiColumnHeaderState.Column
                              {
                                  headerContent         = new GUIContent("Components"),
                                  headerTextAlignment   = TextAlignment.Left,
                                  sortedAscending       = true,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  width                 = 85,
                                  minWidth              = 85,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              }
                          };
            var multiColumnHeaderState = new MultiColumnHeaderState(columns);

            return multiColumnHeaderState;
        }

        public ComponentTypeListView(TreeViewState state, out MultiColumnHeaderState headerState, SelectionChange<ComponentType, object> selectionChange, Func<World> worldChanged) : base(state, BuilMultiColumnHeader(out headerState))
        {
            selectionChanged = selectionChange;
            this.worldChanged = worldChanged;

            foreach (var type in TypeManager.AllTypes())
            {
                if (type.Type == typeof(Entity)) continue;
                var typeIndex     = TypeManager.GetTypeIndex(type.Type);
                var componentType = ComponentType.FromTypeIndex(typeIndex);
                if (componentType.GetManagedType() == null) continue;
                
                //Currently IsZeroSized is broken and returns true for both SharedComponents and SystemStateSharedComponents //Case #1086719 @ 
                if(componentType.IsZeroSized())
                {
                    //Debug.Log($"{componentType} : {componentType.IsSharedComponent} : {componentType.IsSystemStateComponent} : {componentType.IsSystemStateSharedComponent} : {componentType.IsZeroSized} : {componentType.BufferCapacity}");
                    Objects.Add(componentType, null);
                }
                else
                {
                    var obj = Activator.CreateInstance(type.Type, true);
                    Objects.Add(componentType, obj);
                    if (componentType.IsSharedComponent || componentType.IsSystemStateSharedComponent)
                    {
                        foreach (var fieldInfo in obj.GetType().GetFields().Where(f => f.FieldType.IsClass))
                        {
                            //if(fieldInfo.FieldType != typeof(Material))
                            if (fieldInfo.FieldType.GetConstructor(Type.EmptyTypes) != null)
                            {
                                var value = Activator.CreateInstance(fieldInfo.FieldType);
                                fieldInfo.SetValue(obj, value);
                            }
                        }
                    }
                }
            }
        }
       protected override bool CanMultiSelect(TreeViewItem item)
        {
            //MultiSelect is currently off because adding multiple components and setting them to a value is not currently working
            //if (item is ComponentElement obj)
            //    return base.CanMultiSelect(item);
            return false;
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            if (item is FieldElement element)
                return rowHeight * element.ExtraRows;
            return base.GetCustomRowHeight(row, item);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is FieldElement element)
            {
                var rec = args.rowRect;
                rec.xMin = GetContentIndent(element);
                GUILayout.BeginArea(rec);
                element.BuildUI(worldChanged.Invoke());
                GUILayout.EndArea();
            }
            else
                base.RowGUI(args);
        }
        private int ids;

        private void AddTo(TreeViewItem item, ComponentType type)
        {
            var child = new ComponentElement(type, ids++, 1);
            if(Objects.TryGetValue(type, out var obj) && obj != null)
                if (!type.IsZeroSized)
                    foreach (var fieldInfo in obj.GetType().GetFields())
                    {
                        var field = new FieldElement(ids++, 2, fieldInfo, ref obj);
                
                        child.AddChild(field);
                    }
            item.AddChild(child);
        }
        protected override TreeViewItem BuildRoot()
        {
            LastPlayerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

            TreeViewItem root = new TreeViewItem(-1, -1, "Root");
            ids = 0;

            var original = new List<ComponentType>(Objects.Keys);
			var copy = new List<ComponentType>(original);
			
            var Shared = new TreeViewItem(ids++, 0, "Shared Components");
            foreach (var type in Objects.Keys.Where(type => type.IsSharedComponent))
            {
                AddTo(Shared, type);
                copy.Remove(type);
            }
            root.AddChild(Shared);

            var SharedSystemState = new TreeViewItem(ids++, 0, "Shared System State Components");
            foreach (var type in Objects.Keys.Where(type => type.IsSystemStateSharedComponent))
            {
                AddTo(SharedSystemState, type);
                copy.Remove(type);
            }
            root.AddChild(SharedSystemState);

            var SystemState = new TreeViewItem(ids++, 0, "System State Components");
            foreach (var type in Objects.Keys.Where(type => type.IsSystemStateComponent))
            {
                AddTo(SystemState, type);
                copy.Remove(type);
            }
            root.AddChild(SystemState);
            
            var Tag = new TreeViewItem(ids++, 0, "Tag (Zero Sized) Components");
            foreach (var type in original.Where(type => (type.IsZeroSized && !type.IsSharedComponent)))
            {
                AddTo(Tag, type);
                copy.Remove(type);
            }
            root.AddChild(Tag);

            var BufferArray = new TreeViewItem(ids++, 0, "Buffer Array Components");
            foreach (var type in Objects.Keys.Where(type => type.IsFixedArray))
            {
                AddTo(BufferArray, type);
                copy.Remove(type);
            }
            root.AddChild(BufferArray);

            var Component = new TreeViewItem(ids++, 0, "Components (IComponentData)");
            foreach (var type in copy)
                AddTo(Component, type);
            
            root.AddChild(Component);
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
    }
}