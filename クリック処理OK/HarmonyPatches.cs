using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using static UnityEngine.GUILayout;
using UnityEngine.Rendering;
using Steamworks;
using rail;
using xiaoye97;


namespace DSPMergeStorage
{
    class HarmonyPatches
    {

        [HarmonyPatch(typeof(UIStorageWindow), "_OnOpen")]
        public static class UIStorageWindow_OnOpens_Prefix
        {
            [HarmonyPrefix]
            public static void patch()
            {
                LogManager.Logger.LogInfo("****************************UIStorageWindow_OnOpens_Prefix");
            }
        }

        //数値の書き換え1
        //[HarmonyPatch(typeof(UIStorageGrid), "DeactiveAllGridGraphics")]
        //public static class UIStorageGridn_DeactiveAllGridGraphics_Prefix
        //{
        //    [HarmonyPrefix]
        //    public static bool patch(UIStorageGrid __instance, Text[] numTexts, int[] numbers, uint[] iconIndexArray, uint[] stateArray)
        //    {
        //        LogManager.Logger.LogInfo("****************************UIStorageGridn_DeactiveAllGridGraphics_Prefix");
        //        for (int i = 0; i < 400; i++)
        //        {
        //            if (numTexts[i] != null && numTexts[i].gameObject.activeSelf)
        //            {
        //                numTexts[i].text = "";
        //                numTexts[i].gameObject.SetActive(false);
        //            }
        //        }
        //        Array.Clear(numbers, 0, 400);
        //        Array.Clear(stateArray, 0, stateArray.Length);
        //        Array.Clear(iconIndexArray, 0, iconIndexArray.Length);
        //        return false;
        //    }

        //}

        //bottom以外を選択してもbottomが開かれる
        [HarmonyPatch(typeof(PlayerAction_Inspect), "SetInspectee")]
        public static class PlayerAction_Inspect_SetInspectee_Prefix
        {
            [HarmonyPrefix]
            public static void patch(PlayerAction_Inspect __instance, EObjectType objType, ref int objId)
            {

                //LogManager.Logger.LogInfo("SetInspectee");
                //LogManager.Logger.LogInfo($"oldID {objId}");

                //if(UIRoot.instance.uiGame.storageWindow.gameObject.activeSelf)
                //{
                //    UIRoot.instance.uiGame.storageWindow.gameObject.SetActive(false);
                //    LogManager.Logger.LogInfo("//////////////////////////////storageWindow.gameObject.SetActive(false)");
                //}




                if (objId != 0 && __instance.player.factory != null && objType == EObjectType.Entity)
                {
                    int StorageId0 = __instance.player.factory.entityPool[objId].storageId;
                    //LogManager.Logger.LogInfo($"StorageId0 {StorageId0}");
                    if (StorageId0 != 0)
                    {
                        if(__instance.player.factory.factoryStorage.storagePool[StorageId0].previous != 0)
                        {
                             int bottomId = __instance.player.factory.factoryStorage.storagePool[StorageId0].bottom;
                           //int newObjId = __instance.player.factory.factoryStorage.storagePool[bottomId].entityId;

                            //LogManager.Logger.LogInfo($"newID {newObjId}");

                            objId = __instance.player.factory.factoryStorage.storagePool[bottomId].entityId;


                            ref bool eventLock = ref AccessTools.FieldRefAccess<UIStorageWindow, bool>(UIRoot.instance.uiGame.storageWindow, "eventLock");

                            LogManager.Logger.LogInfo("//////////////////////////////Inspectee Changed     eventLock : " + eventLock
                                );



                            if (MergedComponent.merged)
                            {
                                MergedComponent.Split();
                                UI.mergeButton.transform.Find("icon").GetComponent<Image>().sprite = UI.mergeIcon;
                                UI.mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storages".Translate();
                                UI.mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Merge Storages.".Translate();
                                GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);
                                MergedComponent.merged = false;
                                  LogManager.Logger.LogInfo("--------------------------------------------------------splited in SetInspectee");
                          }
                            eventLock = false;
                            //objId = newObjId;
                            //UIRoot.instance.uiGame.storageWindow.storageId = 0;
                            //UIRoot.instance.uiGame.storageWindow.storageId = StorageId0;


                        }
                    }
                }
            }

        }


        //[HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        //[HarmonyPatch(typeof(UIStorageGrid), "_OnCreate")]

        public static class VFPreload_PreloadThread_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()

            {
                ref int[] numbers = ref AccessTools.FieldRefAccess<UIStorageGrid, int[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numbers");
                ref Text[] numTexts = ref AccessTools.FieldRefAccess<UIStorageGrid, Text[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numTexts");

                Array.Resize(ref numTexts, 600);
                Array.Resize(ref numbers, 600);

                //for (int i = 0; i < 400; i++)
                //{

                //    CreateGridGraphic(i);

                //}

                LogManager.Logger.LogInfo($"/////////////////////////////////////////////////////numbers   : {numbers.Length}");
                LogManager.Logger.LogInfo($"/////////////////////////////////////////////////////numTexts   : {numTexts.Length}");

            }
        }

        [HarmonyPatch(typeof(UIStorageWindow), "_OnUpdate")]
        public static class UIStorageWindow_OnUpdate_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(UIStorageWindow __instance)
            {
                float rowCount = __instance.storageUI.rowCount;
                float x = __instance.windowTrans.sizeDelta.x;
                if (rowCount > 18)
                {
                    rowCount = 18;
                    x += 20;
                }
                __instance.windowTrans.sizeDelta = new Vector2(__instance.windowTrans.sizeDelta.x, (float)(rowCount * 50 + 130));

                GameObject infoText = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/panel-bg/infoText");

                int pre = __instance.factoryStorage.storagePool[__instance.storageId].previous;
                int id = __instance.factoryStorage.storagePool[__instance.storageId].id;
                int next = __instance.factoryStorage.storagePool[__instance.storageId].next;
                int top = __instance.factoryStorage.storagePool[__instance.storageId].top;
                int bottom = __instance.factoryStorage.storagePool[__instance.storageId].bottom;

                infoText.GetComponent<Text>().text = $"pre: {pre,3} ID: {id,3} next: {next,3}  top: {top} bottom: {bottom}    ";
            }
        }

        //[HarmonyPatch(typeof(UIWindowDrag), "OnEnable")]
        public static class UIStorageWindow_onEnable_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(UIWindowDrag __instance)
            {
                LogManager.Logger.LogInfo("--------------------------------------------------------UIWindowDrag_onEnable_Patch : " + __instance.gameObject);
                if (__instance.gameObject == GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/"))
                {
                    UIStorageWindow storageWindow = UIRoot.instance.uiGame.storageWindow;


                    uint[] stateArray = AccessTools.FieldRefAccess<UIStorageGrid, uint[]>(storageWindow.storageUI, "stateArray");
                    LogManager.Logger.LogInfo($"stateArray   : {stateArray.Length}");
                    uint[] iconIndexArray = AccessTools.FieldRefAccess<UIStorageGrid, uint[]>(storageWindow.storageUI, "iconIndexArray");
                    LogManager.Logger.LogInfo($"iconIndexArray   : {iconIndexArray.Length}");
                    ref int[] numbers = ref AccessTools.FieldRefAccess<UIStorageGrid, int[]>(storageWindow.storageUI, "numbers");
                    Array.Resize(ref numbers, 600);


                    LogManager.Logger.LogInfo($"numbers   : {numbers.Length}");
                    ref Text[] numTexts = ref AccessTools.FieldRefAccess<UIStorageGrid, Text[]>(storageWindow.storageUI, "numTexts");
                    Array.Resize(ref numTexts, 600);
                    LogManager.Logger.LogInfo($"numTexts  : {numTexts.Length}");
                }
            }
        }


        //[HarmonyPatch(typeof(PlayerAction_Inspect), "GetObjectSelectDistance")]
        //public static class PlayerAction_Inspect_GetObjectSelectDistancee_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix()
        //    {
        //        LogManager.Logger.LogInfo("GetObjectSelectDistance");
        //    }
        //}

        //[HarmonyPostfix, HarmonyPatch(typeof(PlayerAction_Inspect), "GetObjectSelectDistance"), HarmonyPriority(Priority.VeryLow)]
        //public static void GetObjectSelectDistance_Patch_Postfix(ref float __result, EObjectType objType, int objid)
        //{
        //    LogManager.Logger.LogInfo("GetObjectSelectDistance");

        //}

        //ストレージウインドウを閉じたら分割
        [HarmonyPatch(typeof(UIWindowDrag), "OnDisable")]
        public static class UIStorageWindow_OnDisable_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(UIWindowDrag __instance)
            {
                //LogManager.Logger.LogInfo("--------------------------------------------------------UIWindowDrag_OnDisable_Patch : " + __instance.gameObject);
                if (MergedComponent.merged)
                {
                    if (__instance.gameObject == GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/"))
                    {
                        //LogManager.Logger.LogInfo("--------------------------------------------------------ok");
                        MergedComponent.Split();
                        UI.mergeButton.transform.Find("icon").GetComponent<Image>().sprite = UI.mergeIcon;
                        UI.mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storages".Translate();
                        UI.mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Merge Storages.".Translate();
                        GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);
                        MergedComponent.merged = false;
                    }
                }

            }
        }
        ////[HarmonyPatch(typeof(ManualBehaviour), "_OnClose")]
        //public static class ManualBehaviou_OnClose_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix()
        //    {
        //        LogManager.Logger.LogInfo("--------------------------------------------------------ManualBehaviou_OnClose_Patch");

        //    }
        //}


        ////[HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        //public static class VFPreload_PreloadThread_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix()
        //    {
        //        LogManager.Logger.LogInfo("--------------------------------------------------------VFPreload_PreloadThread_Patch");

        //    }
        //}




        ////仮想ストレージを閉じるさいに分割
        ////[HarmonyPrefix, HarmonyPatch(typeof(UIStorageWindow), "_OnClose")]
        ////public static bool UIStorageWindow_OnClose_Prefix(UIStorageWindow __instance)
        ////{
        ////    LogManager.Logger.LogInfo("UIStorageWindow_OnClose_Prefix");
        ////    return true;

        ////}
        //cursorPointsaとcursorIndicesの配列サイズの変更

        //[HarmonyPatch(typeof(UIStorageGrid), "DeactiveAllGridGraphics")]
        class Transpiler_replace1
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> UIStorageGrid_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                LogManager.Logger.LogInfo("++++++++++++++++++++++++++++++++++++++++++++++UIStorageGrid_Transpiler1");
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4 && (int)codes[i].operand == 400)
                    {
                        LogManager.Logger.LogInfo("++++++++++++++++++++++++++++++++++++++++++++++UIStorageGrid_Transpiler1　　changed " + codes[i].operand);
                        codes[i].operand = 600;
                        LogManager.Logger.LogInfo("++++++++++++++++++++++++++++++++++++++++++++++UIStorageGrid_Transpiler1　　changed " + codes[i].operand);
                    }
                }
                return codes.AsEnumerable();
            }
        }

        //[HarmonyPatch(typeof(UIStorageGrid), "_OnCreate")]
        //class Transpiler_replace2
        //{
        //    [HarmonyTranspiler]
        //    static IEnumerable<CodeInstruction> UIStorageGrid_Transpiler(IEnumerable<CodeInstruction> instructions)
        //    {
        //        LogManager.Logger.LogInfo("++++++++++++++++++++++++++++++++++++++++++++++UIStorageGrid_Transpiler2");
        //        var codes = new List<CodeInstruction>(instructions);
        //        for (int i = 0; i < codes.Count; i++)
        //        {
        //            if (codes[i].opcode == OpCodes.Ldc_I4 && (int)codes[i].operand == 400)
        //            {
        //                LogManager.Logger.LogInfo("++++++++++++++++++++++++++++++++++++++++++++++UIStorageGrid_Transpiler2　　changed " + codes[i].operand);
        //                codes[i].operand = 600;
        //                LogManager.Logger.LogInfo("++++++++++++++++++++++++++++++++++++++++++++++UIStorageGrid_Transpiler2　　changed " + codes[i].operand);
        //            }
        //        }
        //        return codes.AsEnumerable();
        //    }
        //}
    }
}
