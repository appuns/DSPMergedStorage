using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HarmonyLib;
using System.Reflection;
using xiaoye97;
using System.Security;
using System.Security.Permissions;


namespace DSPMergeStorage
{
    public class MergedComponent : MonoBehaviour
    {
        //public static StorageComponent mergedSTR = new StorageComponent(0);  　
        //public static StorageComponent displaySTR = new StorageComponent(0); 
        public static StorageComponent backupSTR = new StorageComponent(0);　　
        //public static StorageComponent backupSTR = new StorageComponent(40);
        public static StorageComponent[] newSTRs = new StorageComponent[10];
        public static int[] cID = new int[10];
        public static int[] entityId = new int[10];
        
        public static bool merged = false;
        public static int realSizeSum;
        public static int STRCount;
        public static int originSize;
        public static int originBans;

        public static void Merge()
        {
             UIStorageWindow storageWindow = UIRoot.instance.uiGame.storageWindow;
            ref bool eventLock = ref AccessTools.FieldRefAccess<UIStorageWindow, bool>(storageWindow, "eventLock");

            //Text[] numTexts = AccessTools.FieldRefAccess<UIStorageGrid, Text[]>(storageWindow.storageUI, "numTexts");
            // LogManager.Logger.LogInfo($"numTexts  : {numTexts.Length}");
            // int[] numbers = AccessTools.FieldRefAccess<UIStorageGrid, int[]>(storageWindow.storageUI, "numbers ");
            // LogManager.Logger.LogInfo($"numbers   : {numbers.Length}");

            eventLock=true;

            int storageLevel = GameMain.data.history.storageLevel;
            //storageComponents = new StorageComponent[storageLevel];
            var storagePool = storageWindow.factoryStorage.storagePool;
            //int bansSum = 0;

            //元のコンポーネントIDの配列を作成
            int selectedID = storagePool[storageWindow.storageId].id;

            cID[0] = storagePool[storageWindow.storageId].bottom;
            entityId[0] = storagePool[storageWindow.storageId].entityId;
            originSize = storagePool[storageWindow.storageId].size;
            int i = 1;
            while (storagePool[cID[i-1]].next != 0)
            {
                //LogManager.Logger.LogInfo($"storagePool[cID[i-1]].next : {storagePool[cID[i - 1]].next} ");
                cID[i] = storagePool[cID[i - 1]].next;
                entityId[i] = storagePool[cID[i]].entityId;
                i++;
            }
            STRCount = i;

            //LogManager.Logger.LogInfo("STRCount  : " + STRCount);

            //配列をコピーしてbottomに結合
            storagePool[cID[0]].SetSize(originSize * STRCount);

            for (i = 1; i < STRCount; i++)
            {
                Array.Copy(storagePool[cID[i]].grids, 0, storagePool[cID[0]].grids, originSize * i, originSize);
            }
            storagePool[cID[0]].Sort();
            storagePool[cID[0]].CutNext();
            //結合したものをストレージにアタッチ
            for (i = 1; i < STRCount; i++)
            {
                //LogManager.Logger.LogInfo("mergedSTR.id : " + newSTRs[0].id);
                storageWindow.factory.entityPool[entityId[i]].storageId = storagePool[cID[0]].id; //すべてにbottomを割り当て
                //if (cID[i] == selectedID)
                //{
                //    storageWindow.storageId = selectedID;
                //}
            }
            //storageWindow.storageId = 0;
            //storageWindow.storageId = cID[0];
            //storageWindow.storageUI.OnStorageDataChanged();
            eventLock = false;
            LogManager.Logger.LogInfo("--------------------------------------------------------merged");

            merged = true;
        }

        //元のストレージにもどす ボタン
        public static void Split()
        {

            var storagePool = GameMain.data.localPlanet.factory.factoryStorage.storagePool;

            storagePool[cID[0]].Sort();
            for (int i = 1; i < STRCount; i++)
            {
                Array.Copy(storagePool[cID[0]].grids, originSize * i, storagePool[cID[i]].grids, 0, originSize);
                GameMain.data.mainPlayer.factory.entityPool[entityId[i]].storageId = cID[i];
            }
            storagePool[cID[0]].SetSize(originSize);
            storagePool[cID[0]].next = cID[1];
            UIRoot.instance.uiGame.storageWindow.storageUI.OnStorageContentChanged();
            Array.Clear(cID, 0, cID.Length);
            Array.Clear(entityId, 0, entityId.Length);
            merged = false;

        }

        //元のストレージにもどす ウインドウを閉じる
        public static void SplitOnClose()
        {
            var storagePool = GameMain.data.localPlanet.factory.factoryStorage.storagePool;
            //
            storagePool[cID[0]].Sort();
            for (int i = 1; i < STRCount; i++)
            {
                Array.Copy(storagePool[cID[0]].grids, originSize * i, storagePool[cID[i]].grids, 0, originSize);
                GameMain.data.mainPlayer.factory.entityPool[entityId[i]].storageId = cID[i];
            }
            storagePool[cID[0]].SetSize(originSize);
            storagePool[cID[0]].next = cID[1];
            Array.Clear(cID, 0, cID.Length);
            Array.Clear(entityId, 0, entityId.Length);
            merged = false;
        }


        //public static void SetSignPool(StorageComponent targetComponent)
        //{
        //    targetComponent.SetSkimSign(GameMain.data.mainPlayer.factory.entitySignPool);
        //    GameMain.data.mainPlayer.factory.entityAnimPool[targetComponent.entityId].state = ((targetComponent.searchStart != -1) ? 1U : 0U);
        //}

    }
}
