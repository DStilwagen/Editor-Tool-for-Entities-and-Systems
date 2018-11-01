﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECSTools.ListViews;
using ECSTools.ListViews.Data;
using ECSTools.ListViews.StructDrawer;
using Unity.Entities;
using Unity.Entities.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using ComponentTypeListView = ECSTools.ListViews.ComponentTypeListView;
using EntityListView = ECSTools.ListViews.EntityListView;
using SystemListView = Unity.Entities.Editor.SystemListView;

namespace ECSTools
{
    public class ECSTools : EditorWindow
    {
        [MenuItem("Tools/Entity Tools")]
        private static ECSTools OpenWindow()
        {
            var window = GetWindow<ECSTools>();
            window.titleContent = new GUIContent("System & Entity Tools");
            window.Focus();
            window.minSize = new Vector2(650, 310);
            window.Repaint();
            return window;

        }

        [NonSerialized] private bool initialized;
        [SerializeField] private TreeViewState worldTreeViewState;
        [SerializeField] private MultiColumnHeaderState worldMultiColumnHeaderState;
        private WorldListView worldListView;
        private SearchField systemSearchField;
        
        private SystemListView test;

        [SerializeField] private TreeViewState      entityTreeViewState;
        [SerializeField] private MultiColumnHeaderState entityListMultiColumnHeaderState;
        private EntityListView     entityList;
        
        [SerializeField] private TreeViewState      componentTypesTreeViewState;
        [SerializeField] private MultiColumnHeaderState componentTypeListMultiColumnHeaderState;
        private ComponentTypeListView componentTypeList;
        
        private List<Type> allSystems = new List<Type>();
        private float lastUpdate;
        private Vector2 scrollPosition = Vector2.zero;
        private EntitySelectionProxy selectionEntityProxy;
        
        // Currently in development as a possible way to display editors for components see Note 6 from the README.md
        //private ObjectSelectionProxy selectionObjectProxy;

        void InitIfNeeded()
        {

            if (!initialized)
            {
                if (worldTreeViewState == null)
                    worldTreeViewState = new TreeViewState();

                var firstInit   = worldMultiColumnHeaderState == null;
                var headerState = WorldListView.CreateDefaultMultiColumnHeaderState();
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(worldMultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(worldMultiColumnHeaderState, headerState);
                worldMultiColumnHeaderState = headerState;

                var multiColumnHeader = new MultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                worldListView = new WorldListView(worldTreeViewState, multiColumnHeader, allSystems);
                worldListView.UpdateIfNecessary();

                //Not yet full implemented
                //systemSearchField                         =  new SearchField();
                //systemSearchField.downOrUpArrowKeyPressed += worldListView.SetFocusAndEnsureSelectedItem;

                if (componentTypesTreeViewState == null)
                    componentTypesTreeViewState = new TreeViewState();

                componentTypeList =
                    new ComponentTypeListView(componentTypesTreeViewState, out componentTypeListMultiColumnHeaderState, ObjectSelectionChanged, () => World);
                componentTypeList.UpdateIfNecessary();


                if (entityTreeViewState == null)
                    entityTreeViewState = new TreeViewState();

                entityList = new EntityListView(entityTreeViewState, out entityListMultiColumnHeaderState, World, EntitySelectionChanged);
                entityList.UpdateIfNecessary();


                initialized = true;
            }
        }

        private void OnEnable()
        {
            this.selectionEntityProxy           = CreateInstance<EntitySelectionProxy>();
            this.selectionEntityProxy.hideFlags = HideFlags.HideAndDontSave;

            //Not yet full implemented. See the comment at the selectionObjectProxy declaration on line 53
            //this.selectionObjectProxy           = CreateInstance<ObjectSelectionProxy>();
            //this.selectionObjectProxy.hideFlags = HideFlags.HideAndDontSave;

            allSystems.Clear();
            IEnumerable<Type> allTypes;
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    allTypes = ass.GetTypes();

                }
                catch (ReflectionTypeLoadException e)
                {
                    allTypes = e.Types.Where(t => t != null);
                    Debug.LogWarning("DefaultWorldInitialization failed loading assembly: " + ass.Location);
                }

                var systemTypes = allTypes.Where(t =>
                                                     t.IsSubclassOf(typeof(ComponentSystemBase)) &&
                                                     !t.IsAbstract &&
                                                     !t.ContainsGenericParameters);
                allSystems.AddRange(systemTypes);
            }
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;

            //Used internally for help in setting position values like height and width
            //PositioningWindow.Add = this;
        }
        private void OnDisable()
        {
            //Used internally for help in setting position values like height and width
            //PositioningWindow.Remove = this;

            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
            if (selectionEntityProxy)
                DestroyImmediate(selectionEntityProxy);
            //EntityList.selectionChanged -= EntitySelectionChanged;
        }
        void EntitySelectionChanged(Entity entity, World world, ComponentType type)
        {
            if (type.GetManagedType() == typeof(Entity))
            {
                if (world.GetExistingManager<EntityManager>() != null)
                    selectionEntityProxy.SetEntity(world, entity);
                if (Selection.activeObject != selectionEntityProxy)
                    Selection.activeObject = selectionEntityProxy;
            }
            else
            {
                var entityManager = world.GetExistingManager<EntityManager>();
                if (entityManager != null)
                    if (type.GetManagedType() != typeof(MonoBehaviour) ||
                        type.GetManagedType() != typeof(ScriptableObject))
                    {
                        var obj = entityManager.GetComponentFromType(entity, type);
                        ObjectSelectionChanged(type, ref obj);
                    }
            }
        }

        //Temporarily used for holding reference to selected components object and ComponentType. See the comment at the selectionObjectProxy declaration on line 53 for why this exists.
        private object selectedComponentTypeObject;
        private ComponentType selectedComponentType;
        void ObjectSelectionChanged(ComponentType type, ref object objs)
        {
            selectedComponentType = type;
            selectedComponentTypeObject = objs;
            ////Not yet full implemented. See the comment at the selectionObjectProxy declaration on line 53
            //if (objs != null)
            //    selectionObjectProxy.SetObject(ref objs, type);
            //if (Selection.activeObject != selectionObjectProxy)
            //    Selection.activeObject = selectionObjectProxy;
            //Selection.selectionChanged.Invoke();
        }

        private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
        {
            string str = "";
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    worldListView?.Repaint();
                    componentTypeList?.Repaint();
                    entityList?.Repaint();
                    //str = "Entered Edit Mode";
                    break;
                //Kept for use later
                //case PlayModeStateChange.EnteredPlayMode:
                //    str = "Entered Play Mode";
                //    break;
                //case PlayModeStateChange.ExitingEditMode:
                //    str = "Exiting Edit Mode";
                //    break;
                //case PlayModeStateChange.ExitingPlayMode:
                //    str = "Exiting Play Mode";
                //    break;
            }

            //Debug.Log(str);
        }

        private void Update()
        {
            //Disabled system timing because it doesn't work properly.
            //WorldSystemTreeView.systems.ForEach(a => a.systemListView.UpdateTimings());
            if (Time.realtimeSinceStartup > lastUpdate + 0.5f)
                Repaint();
        }

        //Selected World Toolbar
        private int world;
        //Selected Page (Current Pages: Systems, and Entities)
        private int page;
        //Get Selected World
        public World World => World.AllWorlds[world];
        void OnGUI ()
        {
            //For use in setting position values like height and width
            //PositioningWindow.Update = this;
            if(World.AllWorlds.Count <= 0)
            {
                GUILayout.Label("No Worlds press Play or have at least 1 GameObjectEntity in the scene");
                return;
            }
            InitIfNeeded();
            worldListView.UpdateIfNecessary();
            componentTypeList.UpdateIfNecessary();
            entityList.UpdateIfNecessary();

            page = GUILayout.Toolbar(page, new[ ] { "Systems", "Entities" }, GUILayout.Width(300));
                BuildWindow();

            lastUpdate = Time.realtimeSinceStartup;
        }
        void BuildWindow()
        {
            switch (page)
            {
                case 0:
                    SystemsTreeView();
                    break;
                case 1:
                    EntitiesView();
                    break;
            }
        }

        private static GUIStyle Box
        {
            get
            {
                if (box == null)
                {
                    box = new GUIStyle(GUI.skin.box)
                          {
                              margin   = new RectOffset(),
                              padding  = new RectOffset(1, 0, 1, 0),
                              overflow = new RectOffset(0, 1, 0, 1)
                          };
                }

                return box;
            }
        }

        private static GUIStyle box;
        
        void EntitiesView()
        {   
            world = EditorGUILayout.Popup(world, World.AllWorlds.Select(w => w.Name).ToArray(), GUILayout.Width(250));

            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(position.height / 2));
            
            var entityTypeListRect = GUIHelpers.GetExpandingRect();
            componentTypeList.OnGUI(entityTypeListRect);
            
            var entityListRect = GUIHelpers.GetExpandingRect();
            entityList.OnGUI(entityListRect);
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.BeginVertical(GUILayout.MinHeight(minSize.y * .25f));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, Box);

            //Not yet full implemented See the comment at the selectionObjectProxy declaration on line 53
            //var obj = selectionObjectProxy.Object;
            var obj = selectedComponentTypeObject;
            if (obj != null)
                foreach (var field in obj.GetType().GetFields())
                    StructDrawer.GetType(field, ref obj, field.GetValue(obj), World);
            GUILayout.EndScrollView();
            //EditorGUILayout.EndVertical();
            if (Selection.activeObject is EntitySelectionProxy proxy)
                selectionEntityProxy = proxy;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Component"))
            {
                EntityManager em         = World.Active.GetExistingManager<EntityManager>();
                var           selection  = componentTypeList.GetSelection();
                var           objectList = componentTypeList.GetRows().Where(a => selection.Contains(a.id)).ToArray();

                //TODO: Finish Multi-Component Selection adding and setting
                //var typeList = new List<ComponentType>();
                //foreach (var item in objectList)
                //{
                //    var ele = item as ComponentElement;
                //    typeList.Add(ele.Type);
                //}
                //var comptypes = new ComponentTypes(typeList.ToArray());
                //em.AddComponents(selectionEntityProxy.Entity, comptypes);
                if (objectList[0] is ComponentElement element)
                {
                    if(!em.HasComponent(selectionEntityProxy.Entity, element.Type))
                    {
                        em.AddComponent(selectionEntityProxy.Entity, element.Type);
                        em.SetComponentWithType(selectionEntityProxy.Entity, element.Type, obj);
                    }
                }
            }
            if (GUILayout.Button("Set Component"))
            {
                EntityManager em = World.Active.GetExistingManager<EntityManager>();
                var selection = entityList.GetSelection();
                var objectList = entityList.GetRows().Where(a => selection.Contains(a.id)).ToArray();

                if (objectList[0] is EntityElement entityElementSelected)
                    if(!em.HasComponent(selectionEntityProxy.Entity, selectedComponentType))
                        em.SetComponentWithType(entityElementSelected.Entity, selectedComponentType, obj);
                    //See the comments for selectedComponentType an ObjectSelectionChanged function
                    //if(!em.HasComponent(selectionEntityProxy.Entity, selectionObjectProxy.ComponentType))
                    //    em.SetComponentWithType(entityElementSelected.Entity, selectionObjectProxy.ComponentType, obj);

                else if (objectList[0] is ComponentElement element)
                    if (element.parent is EntityElement entityElement)
                        if(!em.HasComponent(selectionEntityProxy.Entity, element.Type))
                        em.SetComponentWithType(entityElement.Entity, element.Type, obj);
            }
            if (GUILayout.Button("Remove Component"))
            {
                EntityManager em        = World.Active.GetExistingManager<EntityManager>();
                var           selection = componentTypeList.GetSelection();
                var           typeList  = componentTypeList.GetRows().Where(a => selection.Contains(a.id));
                
                foreach (var type in typeList)
                    if(type is ComponentElement entityType)
                        if (!em.HasComponent(selectionEntityProxy.Entity, entityType.Type))
                            em.RemoveComponent(selectionEntityProxy.Entity, entityType.Type);
            }
            if (GUILayout.Button("Destroy Entity"))
            {
                if(selectionEntityProxy.Exists)
                {
                    var entityManager = World.GetExistingManager<EntityManager>();
                    entityManager.DestroyEntity(selectionEntityProxy.Entity);
                }
            }
            if (GUILayout.Button("Create Entity With Components"))
            {
                EntityManager em        = World.GetExistingManager<EntityManager>();
                var           selection = componentTypeList.GetSelection();
                var           typeList  = componentTypeList.GetRows().Where(a => selection.Contains(a.id));

                var entity = em.CreateEntity();
                foreach (var type in typeList)
                    if(type is ComponentElement entityType)
                        if (!em.HasComponent(entity, entityType.Type))
                            em.SetComponentWithType(entity, entityType.Type, obj);

            }
            GUILayout.EndHorizontal();
        }

        private static bool updatePlayerLoopChoice;
        public static bool UpdatePlayerLoopChoice => updatePlayerLoopChoice;
        void SystemsTreeView()
        {
            //if (GUILayout.Button("Update Player Loop", GUILayout.Width(200)))
            //    ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.AllWorlds.ToArray());
            updatePlayerLoopChoice = EditorGUILayout.ToggleLeft(new GUIContent("Update Player Loop",
                                                      "Update the player loop when a system is created"), updatePlayerLoopChoice);
            
            worldListView.OnGUI(GUIHelpers.GetExpandingRect());
        }
        //void SearchBar (Rect rect) => WorldSystemTreeView.searchString = systemSearchField.OnGUI (rect, WorldSystemTreeView.searchString);
    }
}