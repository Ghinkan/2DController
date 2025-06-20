using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;
namespace UnityUtils.Timers
{
    public static class PlayerLoopInterface
    {
        private static readonly List<PlayerLoopSystem> InsertedSystems = new List<PlayerLoopSystem>();

        public static UnityAction SystemsCleared;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            //Systems are not automatically removed from the PlayerLoop, so we need to clean up the ones that have been added in play mode, as they'd otherwise
            // keep running when outside play mode, and in the next play mode if we don't have assembly reload turned on.
            SystemsCleared = null;
            EditorApplication.playModeStateChanged -= ClearInsertedSystems;
            EditorApplication.playModeStateChanged += ClearInsertedSystems;
        }

        private static void ClearInsertedSystems(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                foreach (PlayerLoopSystem playerLoopSystem in new List<PlayerLoopSystem>(InsertedSystems))
                    RemoveSystem(ref currentPlayerLoop, playerLoopSystem);

                InsertedSystems.Clear();
                SystemsCleared?.Invoke();
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                EditorApplication.playModeStateChanged -= ClearInsertedSystems;
            }
        }
  #endif

        private static bool HandleSubSystemLoopForInsert<T>(ref PlayerLoopSystem playerLoopSystem, in PlayerLoopSystem systemToInsert, int index)
        {
            if (playerLoopSystem.subSystemList == null) return false;

            for (int i = 0; i < playerLoopSystem.subSystemList.Length; ++i)
            {
                if (!InsertSystem<T>(ref playerLoopSystem.subSystemList[i], in systemToInsert, index)) continue;
                return true;
            }

            return false;
        }

        public static bool InsertSystem<T>(ref PlayerLoopSystem playerLoopSystem, in PlayerLoopSystem systemToInsert, int index)
        {
            if (playerLoopSystem.type != typeof(T)) return HandleSubSystemLoopForInsert<T>(ref playerLoopSystem, systemToInsert, index);

            List<PlayerLoopSystem> playerLoopSystemList = new List<PlayerLoopSystem>();
            if (playerLoopSystem.subSystemList != null) playerLoopSystemList.AddRange(playerLoopSystem.subSystemList);
            playerLoopSystemList.Insert(index, systemToInsert);
            playerLoopSystem.subSystemList = playerLoopSystemList.ToArray();

            InsertedSystems.Add(systemToInsert);
            Debug.Log($"<b>Inserted system</b>: {systemToInsert.type}");
            return true;
        }

        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            List<PlayerLoopSystem> playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (int i = 0; i < playerLoopSystemList.Count; ++i)
                if (playerLoopSystemList[i].type == systemToRemove.type && playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate)
                {
                    playerLoopSystemList.RemoveAt(i);
                    loop.subSystemList = playerLoopSystemList.ToArray();

                    InsertedSystems.Remove(systemToRemove);
                    Debug.Log($"<b>Removed system</b>: {systemToRemove.type}");
                }

            HandleSubSystemLoopForRemoval<T>(ref loop, systemToRemove);
        }

        private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            for (int i = 0; i < loop.subSystemList.Length; ++i)
                RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
        }

        public static void RemoveSystem(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            List<PlayerLoopSystem> playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (int i = 0; i < playerLoopSystemList.Count; ++i)
                if (playerLoopSystemList[i].type == systemToRemove.type && playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate)
                {
                    playerLoopSystemList.RemoveAt(i);
                    loop.subSystemList = playerLoopSystemList.ToArray();

                    InsertedSystems.Remove(systemToRemove);
                    Debug.Log($"<b>Removed system</b>: {systemToRemove.type}");
                }

            HandleSubSystemLoopForRemoval(ref loop, systemToRemove);
        }

        private static void HandleSubSystemLoopForRemoval(ref PlayerLoopSystem loop, PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            for (int i = 0; i < loop.subSystemList.Length; ++i)
                RemoveSystem(ref loop.subSystemList[i], systemToRemove);
        }

        public static void PrintPlayerLoop(PlayerLoopSystem loop)
        {
            StringBuilder playerLoopSystemText = new StringBuilder();

            playerLoopSystemText.AppendLine("Unity Player Loop");
            foreach (PlayerLoopSystem subSystem in loop.subSystemList)
                PrintSubsystem(subSystem, playerLoopSystemText, 0);

            Debug.Log(playerLoopSystemText.ToString());
        }

        private static void PrintSubsystem(PlayerLoopSystem system, StringBuilder playerLoopSystemText, int level)
        {
            playerLoopSystemText.Append(' ', level * 2).AppendLine(system.type.ToString());
            if (system.subSystemList == null || system.subSystemList.Length == 0) return;

            foreach (PlayerLoopSystem subSystem in system.subSystemList)
                PrintSubsystem(subSystem, playerLoopSystemText, level + 1);
        }
    }
}