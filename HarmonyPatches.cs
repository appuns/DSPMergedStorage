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
using UnityEngine.Rendering;
using Steamworks;
using rail;
using xiaoye97;


namespace DSPMergeStorage
{
    [HarmonyPatch]
    class HarmonyPatches
    {

        //アイテム数のリセットを最後まで
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
            if (Main.enableMerge)
            {
                if (objId != 0 && __instance.player.factory != null && objType == EObjectType.Entity)
                {
                    int StorageId0 = __instance.player.factory.entityPool[objId].storageId;
                    //LogManager.Logger.LogInfo($"StorageId0 {StorageId0}");
                    if (StorageId0 != 0)
                    {
                        int bottomId = 0;
                        if (__instance.player.factory.factoryStorage.storagePool[StorageId0].previous == 0)
                        {
                            bottomId = StorageId0;
                        }
                        else
                        {
                            bottomId = __instance.player.factory.factoryStorage.storagePool[StorageId0].bottom;
                            objId = __instance.player.factory.factoryStorage.storagePool[bottomId].entityId;
                        }
                        if (MergedComponent.merged)
                        {
                            MergedComponent.Split();
                            MergedComponent.Merge(bottomId);
                        }
                        else
                        {
                            MergedComponent.Merge(bottomId);
                        }

                    }
                }
            }
        }

        //アレイのサイズ再設定
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        public static void VFPreload_PreloadThread_Patch()

        {
            ref int[] numbers = ref AccessTools.FieldRefAccess<UIStorageGrid, int[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numbers");
            ref Text[] numTexts = ref AccessTools.FieldRefAccess<UIStorageGrid, Text[]>(UIRoot.instance.uiGame.storageWindow.storageUI, "numTexts");
            Array.Resize(ref numTexts, Main.maxSize);
            Array.Resize(ref numbers, Main.maxSize);
        }

        //ウインドウサイズの調整
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
            if (Main.enableMerge)
            {
                //LogManager.Logger.LogInfo("--------------------------------------------------------UIWindowDrag_OnDisable_Patch : " + __instance.gameObject);
                if (MergedComponent.merged)
                {
                    if (__instance.gameObject == GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/"))
                    {
                        MergedComponent.Split();
                    }
                }
            }
        }


    }
}
