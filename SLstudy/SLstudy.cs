using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace SLstudy
{
    // Token: 0x02000002 RID: 2
    public class Settings : UnityModManager.ModSettings
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
    }

    // Token: 0x02000003 RID: 3
    public static class Main
    {
        // Token: 0x06000003 RID: 3 RVA: 0x00002064 File Offset: 0x00000264
        public static void CheckPerFrame()
        {
            if (!Main.enabled)
            {
                return;
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                if (Main.injuryList.Count > 0 && Main.mianqi.Count > 0)
                {
                    if (DateFile.instance.actorInjuryDate.ContainsKey(DateFile.instance.mianActorId))
                    {
                        if (Main.injuryList[0].Count == 0)
                        {
                            DateFile.instance.actorInjuryDate[DateFile.instance.mianActorId].Clear();
                        }
                        else
                        {
                            DateFile.instance.actorInjuryDate[DateFile.instance.mianActorId] = Main.injuryList[0];
                        }
                    }
                    if (GameData.Characters.HasCharProperty(DateFile.instance.mianActorId, 39))
                    {
                        GameData.Characters.SetCharProperty(DateFile.instance.mianActorId, 39, Main.mianqi[0]);
                        DateFile.instance.ChangeMianQi(DateFile.instance.mianActorId, 0, 5);
                    }
                }
                Main.injuryList = new List<Dictionary<int, int>>();
                Main.mianqi = new List<string>();
                StudyWindow.instance.StartCoroutine((IEnumerator)Main.ShowStudyWindow.Invoke(StudyWindow.instance, new object[0]));
                return;
            }
            if (Input.GetKeyDown(KeyCode.Z) && Main.studyDiskIdList.Count > 0 && Main.studyDiskOkList.Count > 0)
            {
                StudyWindow.instance.studyDiskId = Main.studyDiskIdList[Main.studyDiskIdList.Count - 1];
                StudyWindow.instance.studyDiskOk = Main.studyDiskOkList[Main.studyDiskOkList.Count - 1];
                if (DateFile.instance.actorInjuryDate.ContainsKey(DateFile.instance.mianActorId))
                {
                    if (Main.injuryList[Main.injuryList.Count - 1].Count == 0)
                    {
                        DateFile.instance.actorInjuryDate[DateFile.instance.mianActorId].Clear();
                    }
                    else
                    {
                        DateFile.instance.actorInjuryDate[DateFile.instance.mianActorId] = Main.injuryList[Main.injuryList.Count - 1];
                    }
                }
                if (GameData.Characters.HasCharProperty(DateFile.instance.mianActorId, 39))
                {
                    GameData.Characters.SetCharProperty(DateFile.instance.mianActorId, 39, Main.mianqi[Main.mianqi.Count - 1]);
                    DateFile.instance.ChangeMianQi(DateFile.instance.mianActorId, 0, 5);
                }
                Main.studyDiskIdList.RemoveAt(Main.studyDiskIdList.Count - 1);
                Main.studyDiskOkList.RemoveAt(Main.studyDiskOkList.Count - 1);
                Main.injuryList.RemoveAt(Main.injuryList.Count - 1);
                Main.mianqi.RemoveAt(Main.mianqi.Count - 1);
                Main.UpdateStudyGameobject.Invoke(StudyWindow.instance, new object[0]);
                Main.UpdateStudy.Invoke(StudyWindow.instance, new object[0]);
                TipsWindow.instance.SetTips(0, new string[]
                {
                    "回退到上一步，重新思考进路。"
                }, 60, -755f, -380f, 450, 100);
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000023D4 File Offset: 0x000005D4
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
            Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Main.logger = modEntry.Logger;
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
            modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
            if (Main._looper == null)
            {
                Main._looper = (new GameObject().AddComponent(typeof(Main.Looper)) as Main.Looper);
                UnityEngine.Object.DontDestroyOnLoad(Main._looper);
            }
            return true;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000247D File Offset: 0x0000067D
        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value)
            {
                return false;
            }
            Main.enabled = value;
            return true;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x0000248B File Offset: 0x0000068B
        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("按退格键←悔棋", new GUILayoutOption[0]);
            GUILayout.Label("左ctrl+退格键初始化所有格子", new GUILayoutOption[0]);
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000024AD File Offset: 0x000006AD
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Main.settings.Save(modEntry);
        }

        // Token: 0x04000001 RID: 1
        public static bool enabled;

        // Token: 0x04000002 RID: 2
        public static UnityModManager.ModEntry.ModLogger logger;

        // Token: 0x04000003 RID: 3
        public static Settings settings;

        // Token: 0x04000004 RID: 4
        public static List<List<int>> studyDiskOkList = new List<List<int>>();

        // Token: 0x04000005 RID: 5
        public static List<List<int[]>> studyDiskIdList = new List<List<int[]>>();

        // Token: 0x04000006 RID: 6
        public static List<Dictionary<int, int>> injuryList = new List<Dictionary<int, int>>();

        // Token: 0x04000007 RID: 7
        public static List<string> mianqi = new List<string>();

        // Token: 0x04000008 RID: 8
        public static MethodInfo UpdateStudyGameobject = typeof(StudyWindow).GetMethod("UpdateStudyGameobject", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

        // Token: 0x04000009 RID: 9
        public static MethodInfo UpdateStudy = typeof(StudyWindow).GetMethod("UpdateStudy", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

        // Token: 0x0400000A RID: 10
        public static MethodInfo ShowStudyWindow = typeof(StudyWindow).GetMethod("ShowStudyWindow", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

        // Token: 0x0400000B RID: 11
        private static Main.Looper _looper = null;

        // Token: 0x02000004 RID: 4
        public class Looper : MonoBehaviour
        {
            // Token: 0x06000009 RID: 9 RVA: 0x00002548 File Offset: 0x00000748
            private void LateUpdate()
            {
                Main.CheckPerFrame();
            }
        }

        // Token: 0x02000005 RID: 5
        [HarmonyPatch(typeof(StudyWindow), "SetStudyButton")]
        public static class SetStudyButton_patch
        {
            // Token: 0x0600000B RID: 11 RVA: 0x00002558 File Offset: 0x00000758
            public static void Prefix()
            {
                if (!Main.enabled)
                {
                    return;
                }
                List<int[]> list = new List<int[]>();
                foreach (int[] array in StudyWindow.instance.studyDiskId)
                {
                    list.Add((int[])array.Clone());
                }
                List<int> item = new List<int>(StudyWindow.instance.studyDiskOk);
                Main.studyDiskIdList.Add(list);
                Main.studyDiskOkList.Add(item);
                if (DateFile.instance.actorInjuryDate.ContainsKey(DateFile.instance.mianActorId))
                {
                    Main.injuryList.Add(new Dictionary<int, int>(DateFile.instance.actorInjuryDate[DateFile.instance.mianActorId]));
                }
                else
                {
                    Main.injuryList.Add(new Dictionary<int, int>());
                }
                if (GameData.Characters.HasCharProperty(DateFile.instance.mianActorId, 39))
                {
                    Main.mianqi.Add(GameData.Characters.GetCharProperty(DateFile.instance.mianActorId, 39));
                    return;
                }
                Main.mianqi.Add("0");
            }
        }

        // Token: 0x02000006 RID: 6
        [HarmonyPatch(typeof(StudyWindow), "SetStudyWindow")]
        public static class SetStudyWindow_patch
        {
            // Token: 0x0600000C RID: 12 RVA: 0x000026A0 File Offset: 0x000008A0
            public static void Prefix()
            {
                if (!Main.enabled)
                {
                    return;
                }
                Main.studyDiskIdList = new List<List<int[]>>();
                Main.studyDiskOkList = new List<List<int>>();
                Main.injuryList = new List<Dictionary<int, int>>();
                Main.mianqi = new List<string>();
            }
        }
    }
}
