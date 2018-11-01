using System.Collections.Generic;
using ECSTools.ListViews.Data;
using Unity.Entities;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ECSTools.ListViews
{
    public class EntityListView : BaseListView
    {
        public delegate void SelectionChange(Entity entity, World world, ComponentType type);
        private SelectionChange selectionChanged;

        private List<TreeViewItem> rows = new List<TreeViewItem>();
        private World world;
        private EntityManager entityManager;
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
                                  headerContent         = new GUIContent("Entities"),
                                  headerTextAlignment   = TextAlignment.Left,
                                  sortedAscending       = true,
                                  sortingArrowAlignment = TextAlignment.Right,
                                  //    //width                 = 90,
                                  //    //minWidth              = 90,
                                  autoResize            = false,
                                  allowToggleVisibility = false
                              }
                              //These have been left in for the possibility of adding buttons later
                              //new MultiColumnHeaderState.Column
                              //{
                              //    headerContent         = new GUIContent("Duplicate", "Create a copy of this entity on selected world."),
                              //    headerTextAlignment   = TextAlignment.Left,
                              //    sortedAscending       = true,
                              //    sortingArrowAlignment = TextAlignment.Right,
                              //    //width                 = 90,
                              //    //minWidth              = 90,
                              //    autoResize            = false,
                              //    allowToggleVisibility = false
                              //},
                              //new MultiColumnHeaderState.Column
                              //{
                                                                            //This will require get from type variable overload for all the different GetComponentDatas
                                                                            //or an extension method using reflection.
                              //    headerContent         = new GUIContent("Copy to World", "Copy this entity to selected world."),
                              //    headerTextAlignment   = TextAlignment.Left,
                              //    sortedAscending       = true,
                              //    sortingArrowAlignment = TextAlignment.Right,
                              //    //width                 = 90,
                              //    //minWidth              = 90,
                              //    autoResize            = false,
                              //    allowToggleVisibility = false
                              //},
                              //new MultiColumnHeaderState.Column
                              //{
                              //    headerContent         = new GUIContent("Destroy", "Destroy selected entity."),
                              //    headerTextAlignment   = TextAlignment.Left,
                              //    sortedAscending       = true,
                              //    sortingArrowAlignment = TextAlignment.Right,
                              //    //width                 = 90,
                              //    //minWidth              = 90,
                              //    autoResize            = false,
                              //    allowToggleVisibility = false
                              //}
                          };
            var multiColumnHeaderState = new MultiColumnHeaderState(columns);

            return multiColumnHeaderState;
        }
        public EntityListView(TreeViewState state, out MultiColumnHeaderState headerState, World world, SelectionChange selectionChanged) : base(state, BuilMultiColumnHeader(out headerState))
        {
            this.selectionChanged = selectionChanged;
            this.world = world;
            entityManager = world.GetExistingManager<EntityManager>();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if(selectionChanged != null)
            {
                var row = rows[selectedIds[0]];
                if (row is EntityElement entityElement)
                    selectionChanged.Invoke(entityElement.Entity, world, ComponentType.Create<Entity>());
                else if (row is ComponentElement componentElement)
                {
                    entityElement = componentElement.parent as EntityElement;
                    if (entityElement != null) selectionChanged.Invoke(entityElement.Entity, world, componentElement.Type);
                }


            }
        }

        public override void UpdateIfNecessary()
        {
            base.UpdateIfNecessary();
        }

        //Disable MultiSelect since we don't want people selecting multiple entities for editing
        protected override bool CanMultiSelect(TreeViewItem item) { return false; }
        private int ids;

        protected override TreeViewItem BuildRoot()
        {
            //Reset variables and create Root node.
            ids = 0;
            rows.Clear();
            var root = new TreeViewItem(-1, -1, "Root");
            if (entityManager.IsCreated)
            {
                var entities = entityManager.GetAllEntities().ToArray();
                foreach (var entity in entities)
                {
                    var entityItem = new EntityElement(ids++, entity);
                    rows.Add(entityItem);
                    foreach (var type in entityManager.GetComponentTypes(entity))
                    {
                        var ctype = new ComponentElement(type, ids++, 1);
                        rows.Add(ctype);
                        entityItem.AddChild(ctype);
                    }
                    root.AddChild(entityItem);
                }
            }
            root.AddChild(new TreeViewItem(1));
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        
    }
}