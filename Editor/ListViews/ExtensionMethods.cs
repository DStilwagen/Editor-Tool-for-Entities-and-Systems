using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace ECSTools.ListViews
{
    public static class ExtensionMethods
    {
        public static object GetComponentFromType(this EntityManager EntityManager, Entity entity, ComponentType type)
        {
            object componentData = new object();
            if (type.GetManagedType().GetInterface("IComponentData") != null)
            { if(!type.IsZeroSized)
                {
                    MethodInfo methodInfo        = typeof(EntityManager).GetMethod("GetComponentData");
                    MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type.GetManagedType());
                    var        parameters        = new object[ ] { entity };
                    componentData = genericMethodInfo.Invoke(EntityManager, parameters);
                }
                //else if (type.IsZeroSized) //Zero sized components should not be passed
                //    return null;
            }
            else if (type.GetManagedType().GetInterface("ISharedComponentData", true) != null)
            {
                MethodInfo methodInfo =
                    typeof(EntityManager).GetMethod("GetSharedComponentData", new[ ] { typeof(Entity) });
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type.GetManagedType());
                var        parameters        = new object[ ] { entity };
                componentData = genericMethodInfo.Invoke(EntityManager, parameters);
            }
            else if(type.GetManagedType() == typeof(Mesh) || type.GetManagedType() == typeof(Material))
            {
                MethodInfo methodInfo =
                    typeof(EntityManagerExtensions).GetMethod("GetComponentObject", new[] { typeof(EntityManager), typeof(Entity) });
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type.GetManagedType());
                var        parameters        = new object[] { EntityManager, entity };
                var        data              = genericMethodInfo.Invoke(EntityManager, parameters);
                if (data != null)
                    componentData = data;
            }
            return componentData;
        }
        public static bool SetComponentWithType(this EntityManager entityManager, Entity entity, ComponentType type, object obj)
        {
            if(type.IsZeroSized)
                Debug.Assert(type.IsZeroSized, "Zero-sized Components can not be set only added or removed");
            else if (type.GetManagedType().GetInterface("IComponentData") != null)
            {
                MethodInfo methodInfo        = typeof(EntityManager).GetMethod("SetComponentData");
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type.GetManagedType());
                var        parameters        = new object[ ] { entity, obj };
                if (genericMethodInfo.Invoke(entityManager, parameters) != null)
                    return true;
            }
            else if (type.GetManagedType().GetInterface("ISharedComponentData", true) != null)
            {
                MethodInfo methodInfo =
                    typeof(EntityManager).GetMethod("SetSharedComponentData", new[] { typeof(Entity) });
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type.GetManagedType());
                var        parameters        = new object[] { entity, obj };
                if (genericMethodInfo.Invoke(entityManager, parameters) != null)
                    return true;
            }
            //This is highly Experimental and is here only for completeness sake. This method is normally internal and not exposed
            //else if(!type.GetManagedType().IsValueType)
            //{
            //    MethodInfo methodInfo =
            //        typeof(EntityManager).GetMethod("SetComponentObject", 
            //                                        BindingFlags.NonPublic | BindingFlags.Instance);//, 
            //                                        //null, 
            //                                        //new[ ] { typeof(Entity), typeof(ComponentType), typeof(object) },
            //                                        //null);
            //    //internal void SetComponentObject(Entity entity, ComponentType componentType, object componentObject)
            //    MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type.GetManagedType());
            //    var        parameters        = new object[ ] { entity, type, obj };
            //    if (genericMethodInfo.Invoke(EntityManager, parameters) != null)
            //        return true;
            //}
            return false;
        }

        //See Comment In ComponentTypeListView.cs constructor
        public static bool IsZeroSized(this ComponentType componentType)
        {
            return (componentType.IsZeroSized &&
                    !(componentType.IsSharedComponent || componentType.IsSystemStateSharedComponent));
        }
    }
}