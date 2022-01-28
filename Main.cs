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

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace DSPMergeStorage
{

    [BepInPlugin("Appun.DSP.plugin.MergeStorage", "DSPMergeStorage", "0.1.0")]
    [BepInProcess("DSPGAME.exe")]

    public class Main : BaseUnityPlugin
    {

        public static int maxSize = 480;
        public static int maxRow = 15;

        public static bool enableMerge = true;

        public void Start()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            UI.LoadIcon();
            UI.MergeButtonCreate();
            UI.ScrollAreaCreate();
        }
    }
}