using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace ECSTools.ListViews.StructDrawer
{
    //Class for drawing ComponentDataEditors
    //Hopefully this won't be needed as I look into different ways to display editor information
    
    //Currently supports all int and float structs from Unity.Mathematics along with all primitives as defined by unity
    public static class StructDrawer
    {
        //Last used Field Name
        private static string name;
        //Last used Entity Manager
        private static EntityManager entityManager;
        public static void GetType(FieldInfo field, ref object parent, object value, World world)
        {
            entityManager = world.GetExistingManager<EntityManager>();
            
            var wideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            CheckType(field, ref parent, value);
            EditorGUIUtility.wideMode = wideMode;
        }

        //TODO Get rid of the huge switch statements at the very least split them
        public static int GetHeight(object obj)
        {
            switch (obj)
            {
                case float2x2 f2x2:
                case float3x2 f3x2:
                case float4x2 f4x2:
                    return 2;
                case int2x2 i2x2:
                case int3x2 i3x2:
                case int4x2 i4x2:
                    return 2;
                case float2x3 f2x3:
                case float3x3 f3x3:
                case float4x3 f4x3:
                    return 4;
                case int2x3 i2x3:
                case int3x3 i3x3:
                case int4x3 i4x3:
                    return 4;
                case float2x4 f2x4:
                case float3x4 f3x4:
                case float4x4 f4x4:
                    return 6;
                case int2x4 i2x4:
                case int3x4 i3x4:
                case int4x4 i4x4:
                    return 6;
                default:
                    return 1;
            }
        }
        private static void CheckType(FieldInfo field, ref object parent, object value)
        {
            name = field.Name;
            //Change this to show type and field name.
            //name = field.FieldType.Name + " : " + field.Name;

            if (field.FieldType.IsPrimitive)
            {
                HandlePrimitives(field, ref parent, value);
                return;
            }
            if(HandleIntFloat(field, ref parent, value))
                return;

            switch (value)
            {
                case Entity entity:
                    CustomVisit(ref entity);
                    field.SetValue(parent, entity);
                    break;
                case quaternion q:
                    CustomVisit(ref q);
                    field.SetValue(parent, q);
                    break;
                case Enum e:
                    CustomVisit(ref e);
                    field.SetValue(parent, e);
                    break;
                case Mesh mesh:
                    CustomVisit(ref mesh);
                    field.SetValue(parent, mesh);
                    break;
                case Material mat:
                    CustomVisit(ref mat);
                    field.SetValue(parent, mat);
                    break;
                default:
                    DefaultVisit(ref value);
                    field.SetValue(parent, value);
                    break;
            }
        }

        private static bool HandleIntFloat(FieldInfo field, ref object parent, object value)
        {
            switch (value)
            {
                case float2 f2:
                    CustomVisit(ref f2);
                    field.SetValue(parent, f2);
                    break;
                case float3 f3:
                    CustomVisit(ref f3);
                    field.SetValue(parent, f3);
                    break;
                case float4 f4:
                    CustomVisit(ref f4);
                    field.SetValue(parent, f4);
                    break;
                case float2x2 f2X2:
                    CustomVisit(ref f2X2);
                    field.SetValue(parent, f2X2);
                    break;
                case float2x3 f2X3:
                    CustomVisit(ref f2X3);
                    field.SetValue(parent, f2X3);
                    break;
                case float2x4 f2X4:
                    CustomVisit(ref f2X4);
                    field.SetValue(parent, f2X4);
                    break;
                case float3x2 f3x2:
                    CustomVisit(ref f3x2);
                    field.SetValue(parent, f3x2);
                    break;
                case float3x3 f3x3:
                    CustomVisit(ref f3x3);
                    field.SetValue(parent, f3x3);
                    break;
                case float3x4 f3x4:
                    CustomVisit(ref f3x4);
                    field.SetValue(parent, f3x4);
                    break;
                case float4x2 f4x2:
                    CustomVisit(ref f4x2);
                    field.SetValue(parent, f4x2);
                    break;
                case float4x3 f4x3:
                    CustomVisit(ref f4x3);
                    field.SetValue(parent, f4x3);
                    break;
                case float4x4 f4x4:
                    CustomVisit(ref f4x4);
                    field.SetValue(parent, f4x4);
                    break;

                case int2 i2:
                    CustomVisit(ref i2);
                    field.SetValue(parent, i2);
                    break;
                case int3 i3:
                    CustomVisit(ref i3);
                    field.SetValue(parent, i3);
                    break;
                case int4 i4:
                    CustomVisit(ref i4);
                    field.SetValue(parent, i4);
                    break;
                case int2x2 i2X2:
                    CustomVisit(ref i2X2);
                    field.SetValue(parent, i2X2);
                    break;
                case int2x3 i2X3:
                    CustomVisit(ref i2X3);
                    field.SetValue(parent, i2X3);
                    break;
                case int2x4 i2X4:
                    CustomVisit(ref i2X4);
                    field.SetValue(parent, i2X4);
                    break;
                case int3x2 i3x2:
                    CustomVisit(ref i3x2);
                    field.SetValue(parent, i3x2);
                    break;
                case int3x3 i3x3:
                    CustomVisit(ref i3x3);
                    field.SetValue(parent, i3x3);
                    break;
                case int3x4 i3x4:
                    CustomVisit(ref i3x4);
                    field.SetValue(parent, i3x4);
                    break;
                case int4x2 i4x2:
                    CustomVisit(ref i4x2);
                    field.SetValue(parent, i4x2);
                    break;
                case int4x3 i4x3:
                    CustomVisit(ref i4x3);
                    field.SetValue(parent, i4x3);
                    break;
                case int4x4 i4x4:
                    CustomVisit(ref i4x4);
                    field.SetValue(parent, i4x4);
                    break;
                default:
                    return false;
            }
            return true;
        }
        private static void HandlePrimitives(FieldInfo field, ref object parent, object value)
        {
            switch (value)
            {
                case sbyte sb:
                    CustomVisit(ref sb);
                    field.SetValue(parent, sb);
                    break;
                case short sh:
                    CustomVisit(ref sh);
                    field.SetValue(parent, sh);
                    break;
                case int i:
                    CustomVisit(ref i);
                    field.SetValue(parent, i);
                    break;
                case long l:
                    CustomVisit(ref l);
                    field.SetValue(parent, l);
                    break;
                case byte b:
                    CustomVisit(ref b);
                    field.SetValue(parent, b);
                    break;
                case ushort ush:
                    CustomVisit(ref ush);
                    field.SetValue(parent, ush);
                    break;
                case uint ui:
                    CustomVisit(ref ui);
                    field.SetValue(parent, ui);
                    break;
                case ulong ul:
                    CustomVisit(ref ul);
                    field.SetValue(parent, ul);
                    break;
                case float f:
                    CustomVisit(ref f);
                    field.SetValue(parent, f);
                    break;
                case double d:
                    CustomVisit(ref d);
                    field.SetValue(parent, d);
                    break;
                case char c:
                    CustomVisit(ref c);
                    field.SetValue(parent, c);
                    break;
                case string str:
                    CustomVisit(ref str);
                    field.SetValue(parent, str);
                    break;
                case bool bo:
                    CustomVisit(ref bo);
                    field.SetValue(parent, bo);
                    break;
            }
        }
        //Should each visit be sent to console window.
        private static readonly bool logVisit = false;
        private static void LogVisit<TValue>(TValue value)
        {
            if(logVisit)
                Debug.Log($"Custom Visit {typeof(TValue)} and Value is : {value}");

        }
        
        private static int selectedEntity;
        private static int selectedEnum;

        private static void CustomVisit<T>(ref T obj) where T : struct
        {
            
        }

        private static Dictionary<object, bool> nestedTypeFoldout = new Dictionary<object, bool>();

        private static void DefaultVisit(ref object obj)
        {
            if(obj == null)
            {
                GUI.enabled = false;
                GUILayout.Label("Object Null");
                GUI.enabled = true;
                return;
            }
            var type = obj.GetType();
            if (!type.IsValueType || type.IsClass) return;
            if(!nestedTypeFoldout.ContainsKey(obj))
                nestedTypeFoldout.Add(obj, true);
            EditorGUI.indentLevel++;
            nestedTypeFoldout[obj] = EditorGUILayout.Foldout(nestedTypeFoldout[obj], type.Name);
            if(!nestedTypeFoldout[obj])
                return;
            EditorGUI.indentLevel++;
            foreach (var field in type.GetFields())
                CheckType(field, ref obj, field.GetValue(obj));
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref Mesh obj)
        {
            GUI.enabled = false;
            var m = EditorGUILayout.ObjectField(name, obj, typeof(Mesh), true);
            obj = (Mesh)m;
            GUI.enabled = true;
        }
        private static void CustomVisit(ref Material obj)
        {
            GUI.enabled = false;
            var m = EditorGUILayout.ObjectField(name, obj, typeof(Material), true);
            obj = (Material)m;
            GUI.enabled = true;
        }
        private static void CustomVisit(ref Enum obj)
        {
            var t       = obj.GetType();
            var options = Enum.GetNames(t);
            var objCopy = obj;
            selectedEnum = Array.FindIndex(options, name => name == objCopy.ToString());
            selectedEnum = EditorGUILayout.Popup(t.Name, selectedEnum, options);

            obj = Enum.GetValues(t).GetValue(selectedEnum) as Enum;
        }
        private static void CustomVisit(ref Entity entity)
        {
            var entities = entityManager.GetAllEntities().ToArray().Reverse().ToArray();
            var entityNames = entities.Select(entity1 => $"Entity {entity1.Index}").ToArray();
            selectedEntity = EditorGUILayout.Popup(name, entity.Index, entityNames);
            entity = entities[selectedEntity];
        }
        private static void CustomVisit(ref quaternion q)
        {
            LogVisit(q);
            q.value = EditorGUILayout.Vector4Field(name, q.value);
        }

        #region Visit Float types from Unity.Mathmatics

        private static void CustomVisit(ref float2 f)
        {
            LogVisit(f);
            f = EditorGUILayout.Vector2Field(name, f);
        }

        private static void CustomVisit(ref float3 f)
        {
            LogVisit(f);
            f = EditorGUILayout.Vector3Field(name, f);
        }

        private static void CustomVisit(ref float4 f)
        {
            LogVisit(f);
            f = EditorGUILayout.Vector4Field(name, f);
        }

        private static void CustomVisit(ref float2x2 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector2Field("", (Vector2)f[0]);
            f[1] = EditorGUILayout.Vector2Field("", (Vector2)f[1]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float2x3 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector2Field("", (Vector2)f[0]);
            f[1] = EditorGUILayout.Vector2Field("", (Vector2)f[1]);
            f[2] = EditorGUILayout.Vector2Field("", (Vector2)f[2]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float2x4 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector2Field("", (Vector2) f[0]);
            f[1] = EditorGUILayout.Vector2Field("", (Vector2) f[1]);
            f[2] = EditorGUILayout.Vector2Field("", (Vector2) f[2]);
            f[3] = EditorGUILayout.Vector2Field("", (Vector2) f[3]);
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref float3x2 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector3Field("", (Vector3) f[0]);
            f[1] = EditorGUILayout.Vector3Field("", (Vector3) f[1]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float3x3 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector3Field("", (Vector3) f[0]);
            f[1] = EditorGUILayout.Vector3Field("", (Vector3) f[1]);
            f[2] = EditorGUILayout.Vector3Field("", (Vector3) f[2]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float3x4 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector3Field("", (Vector3) f[0]);
            f[1] = EditorGUILayout.Vector3Field("", (Vector3) f[1]);
            f[2] = EditorGUILayout.Vector3Field("", (Vector3) f[2]);
            f[3] = EditorGUILayout.Vector3Field("", (Vector3) f[3]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float4x2 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector4Field("", (Vector4) f[0]);
            f[1] = EditorGUILayout.Vector4Field("", (Vector4) f[1]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float4x3 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector4Field("", (Vector4) f[0]);
            f[1] = EditorGUILayout.Vector4Field("", (Vector4) f[1]);
            f[2] = EditorGUILayout.Vector4Field("", (Vector4) f[2]);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref float4x4 f)
        {
            LogVisit(f);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            f[0] = EditorGUILayout.Vector4Field("", (Vector4) f[0]);
            f[1] = EditorGUILayout.Vector4Field("", (Vector4) f[1]);
            f[2] = EditorGUILayout.Vector4Field("", (Vector4) f[2]);
            f[3] = EditorGUILayout.Vector4Field("", (Vector4) f[3]);
            EditorGUI.indentLevel--;
        }

        #endregion

        #region Visit Int types from Unity.Mathmatics

        #region Converters from and to Vector2Int, Vector3Int, Vector4

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2Int ConvertToVector2Int(int2 i) => new Vector2Int(i.x, i.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3Int ConvertToVector3Int(int3 i) => new Vector3Int(i.x, i.y, i.z);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector4 ConvertToVector4(int4 i) => new Vector4(i.x, i.y, i.z, i.w); //Using Vector4 since there is no Vector4Int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int2 ConvertVector2IntToInt2(Vector2Int vi) => new int2(vi.x, vi.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int3 ConvertVector3IntToInt3(Vector3Int vi) => new int3(vi.x, vi.y, vi.z);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int4 ConvertVector4ToInt4(Vector4 v) => new int4((int)v.x, (int)v.y, (int)v.z, (int)v.w); //Using Vector4 since there is no Vector4Int

        #endregion
        private static void CustomVisit(ref int2 i)
        {
            LogVisit(i);
            var field = EditorGUILayout.Vector2IntField(name, ConvertToVector2Int(i));
            i = ConvertVector2IntToInt2(field);
        }

        private static void CustomVisit(ref int3 i)
        {
            LogVisit(i);
            var field = EditorGUILayout.Vector3IntField(name, ConvertToVector3Int(i));
            i = ConvertVector3IntToInt3(field);
        }

        private static void CustomVisit(ref int4 i)
        {
            LogVisit(i);
            var field = EditorGUILayout.Vector4Field(name, ConvertToVector4(i));
            i = ConvertVector4ToInt4(field);
        }

        private static void CustomVisit(ref int2x2 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[0]));
            var field1 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[1]));

            i[0] = ConvertVector2IntToInt2(field0);
            i[1] = ConvertVector2IntToInt2(field1);
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref int2x3 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[0]));
            var field1 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[1]));
            var field2 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[2]));

            i[0] = ConvertVector2IntToInt2(field0);
            i[1] = ConvertVector2IntToInt2(field1);
            i[2] = ConvertVector2IntToInt2(field2);
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref int2x4 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[0]));
            var field1 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[1]));
            var field2 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[2]));
            var field3 = EditorGUILayout.Vector2IntField("", ConvertToVector2Int(i[3]));

            i[0] = ConvertVector2IntToInt2(field0);
            i[1] = ConvertVector2IntToInt2(field1);
            i[2] = ConvertVector2IntToInt2(field2);
            i[3] = ConvertVector2IntToInt2(field3);
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref int3x2 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[0]));
            var field1 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[1]));

            i[0] = ConvertVector3IntToInt3(field0);
            i[1] = ConvertVector3IntToInt3(field1);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref int3x3 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[0]));
            var field1 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[1]));
            var field2 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[2]));

            i[0] = ConvertVector3IntToInt3(field0);
            i[1] = ConvertVector3IntToInt3(field1);
            i[2] = ConvertVector3IntToInt3(field2);
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref int3x4 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[0]));
            var field1 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[1]));
            var field2 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[2]));
            var field3 = EditorGUILayout.Vector3IntField("", ConvertToVector3Int(i[3]));

            i[0] = ConvertVector3IntToInt3(field0);
            i[1] = ConvertVector3IntToInt3(field1);
            i[2] = ConvertVector3IntToInt3(field2);
            i[3] = ConvertVector3IntToInt3(field3);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref int4x2 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[0]));
            var field1 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[1]));

            i[0] = ConvertVector4ToInt4(field0);
            i[1] = ConvertVector4ToInt4(field1);
            EditorGUI.indentLevel--;
        }

        private static void CustomVisit(ref int4x3 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[0]));
            var field1 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[1]));
            var field2 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[2]));

            i[0] = ConvertVector4ToInt4(field0);
            i[1] = ConvertVector4ToInt4(field1);
            i[2] = ConvertVector4ToInt4(field2);
            EditorGUI.indentLevel--;
        }
        private static void CustomVisit(ref int4x4 i)
        {
            LogVisit(i);
            GUILayout.Label(name);
            EditorGUI.indentLevel++;
            var field0 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[0]));
            var field1 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[1]));
            var field2 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[2]));
            var field3 = EditorGUILayout.Vector4Field("", ConvertToVector4(i[3]));

            i[0] = ConvertVector4ToInt4(field0);
            i[1] = ConvertVector4ToInt4(field1);
            i[2] = ConvertVector4ToInt4(field2);
            i[3] = ConvertVector4ToInt4(field3);
            EditorGUI.indentLevel--;
        }
        #endregion

        #region ICustomVisitPrimitives

        private static void CustomVisit(ref sbyte sb)
        {
            LogVisit(sb);
            sb = (sbyte)Mathf.Clamp(EditorGUILayout.IntField(name, sb), sbyte.MinValue, sbyte.MaxValue);
        }

        private static void CustomVisit(ref short s)
        {
            LogVisit(s);
            s = (short)Mathf.Clamp(EditorGUILayout.IntField(name, s), short.MinValue, short.MaxValue);
        }

        private static void CustomVisit(ref int i)
        {
            LogVisit(i);
            i = EditorGUILayout.IntField(name, i);
        }

        private static void CustomVisit(ref long l)
        {
            LogVisit(l);
           l = EditorGUILayout.LongField(name, l);
        }

        private static void CustomVisit(ref byte by)
        {
            LogVisit(by);
            by = (byte)Mathf.Clamp(EditorGUILayout.IntField(name, by), byte.MinValue, byte.MaxValue);
        }

        private static void CustomVisit(ref ushort us)
        {
            LogVisit(us);
            us = (ushort)Mathf.Clamp(EditorGUILayout.IntField(name, us), ushort.MinValue, ushort.MaxValue);
        }

        private static void CustomVisit(ref uint ui)
        {
            LogVisit(ui);
           ui = (uint)Mathf.Clamp(EditorGUILayout.LongField(name, ui), uint.MinValue, uint.MaxValue);
        }

        private static void CustomVisit(ref ulong ul)
        {
            LogVisit(ul);
            var text = EditorGUILayout.TextField(name, ul.ToString());
            ulong.TryParse(text, out var num);
            ul = num;
        }

        private static void CustomVisit(ref float f)
        {
            LogVisit(f);
            f = EditorGUILayout.FloatField(name, f);
        }

        private static void CustomVisit(ref double d)
        {
            LogVisit(d);
            d = EditorGUILayout.DoubleField(name, d);
        }

        private static void CustomVisit(ref bool b)
        {
            LogVisit(b);
            b = EditorGUILayout.Toggle(name, b);
        }

        private static void CustomVisit(ref char c)
        {
            LogVisit(c);

            var text = EditorGUILayout.TextField(name, c.ToString());
            c = (string.IsNullOrEmpty(text) ? '\0' : text[0]);
        }

        private static void CustomVisit(ref string str)
        {
            LogVisit(str);
            if (IsTypeIdMarker(name))
            {
                return;
            }

            str = EditorGUILayout.TextField(name, str.ToString());
        }
        #endregion

        private static bool IsTypeIdMarker(string s)
        {
            return s == "$TypeId";
        }
    }
}