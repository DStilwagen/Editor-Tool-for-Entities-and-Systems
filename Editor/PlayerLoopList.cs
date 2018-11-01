using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

namespace ECSTools
{
    public class CPlayerLoop : OdinEditorWindow
    {
        [MenuItem("Tools/PlayerLoop Tools")]
        private static CPlayerLoop OpenWindow()
        {
            var window = GetWindow<CPlayerLoop>();
            window.titleContent = new GUIContent("PlayerLoop Tools");
            window.Focus();
            //window.minSize = new Vector2(625, 310);
            window.Repaint();
            return window;
        }

        //private void OnEnable()
        //{   
        //}

        private void GetAllSystems(PlayerLoopSystem playerLoop)
        {
            GetSubSystem(playerLoop);
            void GetSubSystem(PlayerLoopSystem playerloop)
            {
                if (playerloop.type == null)
                    return;
                if (playerloop.subSystemList.Length <= 0)
                    FullPlayerLoop.Add(playerloop, playerloop.subSystemList);
                foreach (var system in playerloop.subSystemList)
                    GetSubSystem(system);
            }
        }

        public void GetSystems(PlayerLoopSystem playerLoop, List<string> names = null)
        {
            if (names == null)
                names = new List<string>();
            if(playerLoop.subSystemList == null)
            {
                Debug.Log(playerLoop + " : " + playerLoop.type);
                names.Add(playerLoop.ToString());
            }
            else if (playerLoop.subSystemList.Length > 0)
            {
                Debug.Log(playerLoop + " : " + playerLoop.subSystemList.Length);
                names.Add(playerLoop.ToString());
                FullPlayerLoop.Add(playerLoop, playerLoop.subSystemList);
                FullPlayerLoopNames.Add(playerLoop.ToString(), names);
                foreach (var system in playerLoop.subSystemList)
                {
                    if (system.type != null)
                        GetSystems(system, names);
                }
            }
        }

        [Button]
        public void Button()
        {
            //Debug.Log(ScriptBehaviourUpdateOrder.CurrentPlayerLoop.subSystemList.Length);
            Debug.Log(PlayerLoop.GetDefaultPlayerLoop().subSystemList.Length);
            //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.Active);
            //Debug.Log(ScriptBehaviourUpdateOrder.CurrentPlayerLoop.subSystemList.Length);
            foreach (var system in PlayerLoop.GetDefaultPlayerLoop().subSystemList)
                GetSystems(system);
        }

        //[SerializeField]
        private Dictionary<PlayerLoopSystem, PlayerLoopSystem[ ]> FullPlayerLoop = new Dictionary<PlayerLoopSystem, PlayerLoopSystem[ ]>();
        //private PlayerLoopSystem playerLoop;
        protected override void OnGUI()
        {
            base.OnGUI();

            //SirenixEditorGUI.BeginVerticalList();
            //if(FullPlayerLoop.Count > 0)
            //    foreach (var loopSystem in FullPlayerLoop)
            //    {
            //        BuildLoop(loopSystem);
            //    }
            //SirenixEditorGUI.EndVerticalList();
        }

        public Dictionary<string, List<string>> FullPlayerLoopNames = new Dictionary<string, List<string>>();
        private void BuildLoop(KeyValuePair<PlayerLoopSystem, PlayerLoopSystem[]> loopSystem)
        {
            var playerLoop = loopSystem.Key;

            if(playerLoop.subSystemList == null)
            {
                SirenixEditorGUI.BeginListItem();
                GUILayout.Label(loopSystem.Key + " : " + loopSystem.Value.Length);
                SirenixEditorGUI.EndListItem();
            }
            else
            {
                SirenixEditorGUI.BeginListItem();
                GUILayout.Label(loopSystem.Key + " : " + loopSystem.Value.Length);
                SirenixEditorGUI.EndListItem();

                SirenixEditorGUI.BeginVerticalList();
                if (playerLoop.subSystemList.Length > 0)
                {
                    foreach (var system in playerLoop.subSystemList)
                    {
                        if (system.type != null)
                            GetSystems(system);
                    }
                }
                SirenixEditorGUI.EndVerticalList();
            }
        }
    }
}
