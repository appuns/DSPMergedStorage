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
        //public static StorageComponent backupSTR = new StorageComponent(0);　　
        //public static StorageComponent backupSTR = new StorageComponent(40);
        public static StorageComponent[] newSTRs = new StorageComponent[10];
        public static int[] cID = new int[10];
        public static int[] entityId = new int[10];
        
        public static bool merged = false;
        public static int realSizeSum;
        public static int STRCount;
        public static int originSize;
        public static int originBans;

        public static void Merge(int bottomId)
        {
             UIStorageWindow storageWindow = UIRoot.instance.uiGame.storageWindow;

            storageWindow.eventLock = true;

            int storageLevel = GameMain.data.history.storageLevel;
            var storagePool = GameMain.data.localPlanet.factory.factoryStorage.storagePool;
            //int bansSum = 0;

            //元のコンポーネントIDの配列を作成
            //LogManager.Logger.LogInfo($"bottomId : {bottomId} ");

            int selectedID = storagePool[bottomId].id;

            cID[0] = storagePool[bottomId].bottom;
            entityId[0] = storagePool[bottomId].entityId;
            originSize = storagePool[bottomId].size;
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
                //LogManager.Logger.LogInfo("Array.Copy  : " + storagePool[cID[0]].grids.Length);
            }
            //storagePool[cID[0]].Sort();
            storagePool[cID[0]].CutNext();
            //LogManager.Logger.LogInfo("CutNext");
            //結合したものをストレージにアタッチ
            for (i = 1; i < STRCount; i++)
            {
                //LogManager.Logger.LogInfo(GameMain.data.localPlanet.factory.entityPool[entityId[i]].storageId + "  = " + storagePool[cID[0]].id);
                GameMain.data.localPlanet.factory.entityPool[entityId[i]].storageId = storagePool[cID[0]].id; //すべてにbottomを割り当て
            }
            //LogManager.Logger.LogInfo("------------------ --------------------------------------merged");
            storageWindow.storageUI.OnStorageContentChanged();
            GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(false);
            GameObject scrollArea = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/scrollArea");
            scrollArea.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

            storageWindow.eventLock = false;
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
            GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);

            merged = false;

        }



    }
}
