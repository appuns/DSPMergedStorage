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
            //viewport.GetComponent<Image>().enabled = true;

            viewport.transform.localPosition = new Vector3(-285, 510, 0);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 0);

            GameObject vbar = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/scrollArea/v-bar");
            vbar.transform.localPosition = new Vector3(235, 510, 0);
            vbar.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 0);


        }



    }
}
