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
using HarmonyLib;
using System.Reflection;
using xiaoye97;
using System.Security;
using System.Security.Permissions;


namespace DSPMergeStorage
{
    public class UI : MonoBehaviour
    {
        public static GameObject mergeButton;
        public static Sprite mergeIcon;
        //public static Sprite purgeIcon;

        public static void ScrollAreaCreate()
        {
            GameObject scrollView = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Statistics Window/product-bg/scroll-view");
            GameObject scrollArea = Instantiate(scrollView, GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/").transform);
            scrollArea.name = "scrollArea";

            //位置調整テスト
            scrollArea.transform.localPosition = new Vector3(25, -570, 0);
            scrollArea.GetComponent<RectTransform>().sizeDelta = new Vector2(-120, -120);

            GameObject storage = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/storage");
            GameObject viewport = scrollArea.transform.Find("viewport").gameObject;
            GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/storage").transform.SetParent(viewport.transform);
            scrollArea.GetComponent<ScrollRect>().content = storage.GetComponent<RectTransform>();
            
            Destroy(viewport.transform.Find("content").gameObject);////////////////////////
            
            viewport.GetComponent<Image>().enabled = false;
            viewport.GetComponent<Mask>().enabled = false;

            viewport.transform.localPosition = new Vector3(-285, 510, 0);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 0);

            GameObject vbar = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/scrollArea/v-bar");
            vbar.transform.localPosition = new Vector3(235, 510, 0);
            vbar.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 0);
        }

        public static void MergeButtonCreate()
        {
            GameObject titleText = GameObject.Find("UI Root/Overlay Canvas/In Game/Game Menu");
            mergeButton = Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Game Menu/detail-func-group/dfunc-1"), titleText.transform) as GameObject;
            mergeButton.name = "mergeButton";
            mergeButton.transform.localPosition = new Vector3(-55, 270, 0);

            mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storage".Translate();
            mergeButton.GetComponent<UIButton>().tips.tipText = "Click to turn ON / OFF Merge Storage.".Translate();
            mergeButton.GetComponent<UIButton>().tips.corner = 4;
            mergeButton.GetComponent<UIButton>().tips.offset = new Vector2(0, 20);

            mergeButton.transform.Find("icon").GetComponent<Image>().sprite = mergeIcon;
            mergeButton.GetComponent<UIButton>().highlighted = true;
            //ボタンイベントの作成
            mergeButton.GetComponent<UIButton>().button.onClick.AddListener(new UnityAction(onClick));
        }

        //アイコンのロード
        public static void LoadIcon()
        {
            try
            {
                var assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DSPMergeStorage.mergestorageicon"));
                if (assetBundle == null)
                {
                    LogManager.Logger.LogInfo("Icon loaded.");
                }
                else
                {
                    mergeIcon = assetBundle.LoadAsset<Sprite>("merge");
                    //purgeIcon = assetBundle.LoadAsset<Sprite>("purge");
                    assetBundle.Unload(false);
                }
            }
            catch (Exception e)
            {
                LogManager.Logger.LogInfo("e.Message " + e.Message);
                LogManager.Logger.LogInfo("e.StackTrace " + e.StackTrace);
            }
        }

        //ボタンイベント
        public static void onClick()
        {
            if (Main.enableMerge)
            {
                //LogManager.Logger.LogInfo("--------------------------------------------------------UIWindowDrag_OnDisable_Patch : " + __instance.gameObject);
            //    if (MergedComponent.merged)
            //    {
            //        MergedComponent.Split();
            //        GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);
            //    }
            //}
            //else
            //{
            //    if (UIRoot.instance.uiGame.storageWindow.storageUI.active)
            //    {
            //        MergedComponent.Merge(UIRoot.instance.uiGame.storageWindow.storageUI.storage.bottom);
            //    }
            //}
            Main.enableMerge = !Main.enableMerge;
            UI.mergeButton.GetComponent<UIButton>().highlighted = Main.enableMerge;

        }


    }
}
