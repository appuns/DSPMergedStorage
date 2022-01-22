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
        //public static GameObject storageWindow;
        public static Sprite mergeIcon;
        public static Sprite purgeIcon;


        //public static GameObject Scrollbar;


        public static void ScrollAreaCreate()
        {
            GameObject scrollView = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Statistics Window/product-bg/scroll-view");
            GameObject scrollArea = Instantiate(scrollView, GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/").transform) as GameObject;
            scrollArea.name = "scrollArea";
            GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/storage").transform.SetParent(scrollArea.transform);



        }
        public static void MergeButtonCreate()
        {
            //GameObject bansBar = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar");
            //mergeButton = Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Game Menu/detail-func-group/dfunc-1"), bansBar.transform) as GameObject;
            GameObject titleText = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/panel-bg/title-text");

            //デバッグ用テキストの作成
            GameObject infoText = Instantiate(titleText, GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/panel-bg").transform) as GameObject;
            infoText.name = "infoText";
            infoText.transform.localPosition = new Vector3(-75, 190, 0);
            infoText.GetComponent<RectTransform>().sizeDelta = new Vector3(500, 36, 0);

            mergeButton = Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Game Menu/detail-func-group/dfunc-1"), titleText.transform) as GameObject;
            mergeButton.name = "mergeButton";
            //float y = bansBar.GetComponent<RectTransform>().sizeDelta.y;
            //float y = storageWindow.GetComponent<RectTransform>().sizeDelta.y;
            mergeButton.transform.localPosition = new Vector3(114, 7, 0);

            mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storages".Translate();
            mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Merge Storages.".Translate();
            mergeButton.transform.Find("icon").GetComponent<Image>().sprite = mergeIcon;
            mergeButton.GetComponent<UIButton>().highlighted = true;
            //ボタンイベントの作成
            mergeButton.GetComponent<UIButton>().button.onClick.AddListener(new UnityAction(onClick));

            //Scrollbar = Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Statistics Window/product-bg/scroll-view/v-bar"), GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/panel-bg").transform) as GameObject;
        }

        //ボタンイベント
        public static void onClick()
        {
            if (MergedComponent.merged)
            {
                MergedComponent.Split();
                mergeButton.transform.Find("icon").GetComponent<Image>().sprite = mergeIcon;
                mergeButton.GetComponent<UIButton>().tips.tipTitle = "Merge Storage".Translate();
                mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Merge Storages.".Translate();
                GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(true);

            }
            else
            {
                MergedComponent.Merge();
                mergeButton.transform.Find("icon").GetComponent<Image>().sprite = mergeIcon;
                mergeButton.GetComponent<UIButton>().tips.tipTitle = "Split Storage".Translate();
                mergeButton.GetComponent<UIButton>().tips.tipText = "Click to Split Storage.".Translate();
                GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/bans-bar").SetActive(false);

            }
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
                    purgeIcon = assetBundle.LoadAsset<Sprite>("purge");
                    assetBundle.Unload(false);
                }
            }
            catch (Exception e)
            {
                LogManager.Logger.LogInfo("e.Message " + e.Message);
                LogManager.Logger.LogInfo("e.StackTrace " + e.StackTrace);
            }
        }


    }
}
