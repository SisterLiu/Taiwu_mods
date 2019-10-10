using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace Customize
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
    }

    public static class Main
    {
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
            Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Main.logger = modEntry.Logger;
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
            modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Main.enabled = value;
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("自定义", new GUILayoutOption[0]);
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Main.settings.Save(modEntry);
        }

        public static bool enabled;
        public static UnityModManager.ModEntry.ModLogger logger;
        public static Settings settings;
    }

    // 移动需求 -1
    [HarmonyPatch(typeof(DateFile), "GetMapMoveNeed", new Type[] { typeof(int), typeof(int) , typeof(int)})]
    public static class GetMapMoveNeed_Patch
    {
        private static void Postfix(ref int __result)
        {
            if (!Main.enabled) return;
            __result = __result - 1;
            if (__result < 0) __result = 0;
        }
    }

    // 移动需求 -1
    [HarmonyPatch(typeof(DateFile), "GetMapMoveNeed", new Type[] { typeof(int), typeof(int) })]
    public static class GetMapMoveNeed_2_Patch
    {
        private static void Postfix(ref int __result)
        {
            if (!Main.enabled) return;
            __result = __result - 1;
            if (__result < 0) __result = 0;
        }
    }
    // 移动需求显示 -1
    [HarmonyPatch(typeof(Pathfinding), "FindPath", new Type[] { typeof(int), typeof(int) , typeof(int), typeof(bool), typeof(bool) })]
    public static class Pathfinding_Patch
    {
        private static void Postfix(ref List<Node> __result)
        {
            if (!Main.enabled) return;
            for(int i = 0; i <= __result.Count; i++) {
                __result[i].costToHere -= (i + 1);
            }
        }
    }

    // 移动需求显示 -1
    [HarmonyPatch(typeof(Pathfinding), "FindPath", new Type[] { typeof(int), typeof(int), typeof(bool), typeof(bool) })]
    public static class Pathfinding_2_Patch
    {
        private static void Postfix(ref List<Node> __result)
        {
            if (!Main.enabled) return;
            for (int i = 0; i < __result.Count; i++)
            {
                __result[i].costToHere -= (i + 1);
            }
        }
    }
}
