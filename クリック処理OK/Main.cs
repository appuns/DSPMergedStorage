using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;
using System.IO;
using HarmonyLib;
using System.Reflection;
using xiaoye97;
using System.Security;
using System.Security.Permissions;

namespace DSPMergeStorage
{

    [BepInPlugin("Appun.DSP.plugin.MergeStorage", "DSPMergeStorage", "0.1.0")]
    [BepInProcess("DSPGAME.exe")]

    public class main : BaseUnityPlugin
    {



        public void Start()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            UI.LoadIcon();
            UI.MergeButtonCreate();
            UI.ScrollAreaCreate();
            //GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Storage Window/").gameObject.AddListener(new UnityAction(OnCargoButtonClick));


        }


    }



}