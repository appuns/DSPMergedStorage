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
    [HarmonyPatch]
    class HarmonyPatches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIStorageGrid), "DeactiveAllGridGraphics")]
        public static void UIStorageGrid_DeactiveAllGridGraphics_Postfix(UIStorageGrid __instance)
        {
            //LogManager.Logger.LogInfo("UIStorageGrid_DeactiveAllGridGraphics_Postfix start!");

            ref int[] numbers = ref AccessTools.FieldRefAccess<UIStorageGrid, int[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numbers");
            ref Text[] numTexts = ref AccessTools.FieldRefAccess<UIStorageGrid, Text[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numTexts");

            for (int i = 400; i < Main.maxSize; i++)
            {
                if (numTexts[i] != null && numTexts[i].gameObject.activeSelf)
                {
                    numTexts[i].text = "";
                    numTexts[i].gameObject.SetActive(false);
                }
            }
            Array.Clear(numbers, 400, Main.maxSize - 400);

            //LogManager.Logger.LogInfo("UIStorageGrid_DeactiveAllGridGraphics_Postfix end");
        }

        //bottom以外を選択してもbottomが開かれる
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerAction_Inspect), "SetInspectee")]
        public static void PlayerAction_Inspect_SetInspectee_Prefix(PlayerAction_Inspect __instance, EObjectType objType, ref int objId)
        {
            if (objId != 0 && __instance.player.factory != null && objType == EObjectType.Entity)
            {
                int StorageId0 = __instance.player.factory.entityPool[objId].storageId;
                //LogManager.Logger.LogInfo($"StorageId0 {StorageId0}");
                if (StorageId0 != 0)
                {
                    if (__instance.player.factory.factoryStorage.storagePool[StorageId0].previous != 0)
                    {
                        int bottomId = __instance.player.factory.factoryStorage.storagePool[StorageId0].bottom;
                        //int newObjId = __instance.player.factory.factoryStorage.storagePool[bottomId].entityId;

                        //LogManager.Logger.LogInfo($"newID {newObjId}");

                        objId = __instance.player.factory.factoryStorage.storagePool[bottomId].entityId;


                        ref bool eventLock = ref AccessTools.FieldRefAccess<UIStorageWindow, bool>(UIRoot.instance.uiGame.storageWindow, "eventLock");
                        eventLock = true;

                        //LogManager.Logger.LogInfo("//////////////////////////////Inspectee Changed     eventLock : " + eventLock );

                        if (MergedComponent.merged)
                        {
                            MergedComponent.Split();
                            //UI.mergeButton.transform.Find("icon").GetComponent<Image>().sprite = UI.mergeIcon;
                            //UI.mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storages".Translate();
                            //UI.mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Merge Storages.".Translate();
                            //GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);
                            //MergedComponent.merged = false;
                            ////LogManager.Logger.LogInfo("--------------------------------------------------------splited in SetInspectee");
                        }else
                        {
                            MergedComponent.Merge();

                        }
                        eventLock = false;
                    }
                }
            }

        }


        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        //[HarmonyPatch(typeof(UIStorageGrid), "_OnCreate")]

        public static void VFPreload_PreloadThread_Patch()

        {
            ref int[] numbers = ref AccessTools.FieldRefAccess<UIStorageGrid, int[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numbers");
            ref Text[] numTexts = ref AccessTools.FieldRefAccess<UIStorageGrid, Text[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numTexts");

            Array.Resize(ref numTexts, Main.maxSize);
            Array.Resize(ref numbers, Main.maxSize);


            //LogManager.Logger.LogInfo($"/////////////////////////////////////////////////////numbers   : {numbers.Length}");
            //LogManager.Logger.LogInfo($"/////////////////////////////////////////////////////numTexts   : {numTexts.Length}");

        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIStorageWindow), "_OnUpdate")]
        public static void UIStorageWindow_OnUpdate_Patch(UIStorageWindow __instance)
        {
            float rowCount = __instance.storageUI.rowCount;
            if (rowCount > Main.maxRow)
            {
                rowCount = Main.maxRow;
            }
            float width = __instance.windowTrans.sizeDelta.x; // + 200;
            float height = (float)(rowCount * 50 + 130);
            __instance.windowTrans.sizeDelta = new Vector2(608, height); // 588,630 
        }

        //ストレージウインドウを閉じたら分割
        [HarmonyPostfix, HarmonyPatch(typeof(UIWindowDrag), "OnDisable")]
        public static void UIStorageWindow_OnDisable_Patch(UIWindowDrag __instance)
        {
            //LogManager.Logger.LogInfo("--------------------------------------------------------UIWindowDrag_OnDisable_Patch : " + __instance.gameObject);
            if (MergedComponent.merged)
            {
                if (__instance.gameObject == GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/"))
                {
                    //LogManager.Logger.LogInfo("--------------------------------------------------------ok");
                    MergedComponent.Split();
                    //UI.mergeButton.transform.Find("icon").GetComponent<Image>().sprite = UI.mergeIcon;
                    //UI.mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storages".Translate();
                    //UI.mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Merge Storages.".Translate();
                    //GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);
                    //MergedComponent.merged = false;
                }
            }

        }

        //ストレージウインドウを開いたら
        //[HarmonyPostfix, HarmonyPatch(typeof(UIWindowDrag), "OnEnable")]
        //public static void UIStorageWindow_OnEnable_Patch(UIWindowDrag __instance)
        //{
        //    //LogManager.Logger.LogInfo("--------------------------------------------------------UIWindowDrag_OnDisable_Patch : " + __instance.gameObject);
        //    //if (!MergedComponent.merged)
        //    //{
        //    if (__instance.gameObject == GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/"))
        //    {
        //        //LogManager.Logger.LogInfo("--------------------------------------------------------ok");

        //        if (__instance.gameObject. __instance.player.factory.factoryStorage.storagePool[StorageId0].previous == 0)
        //        {

        //            MergedComponent.Merge();
        //        }
        //    }

        //}

    }
}
