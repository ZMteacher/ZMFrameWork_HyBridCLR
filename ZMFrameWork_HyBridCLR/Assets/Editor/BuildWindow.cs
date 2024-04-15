﻿/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAssetFrameWork
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;


public class BuildWindows : OdinMenuEditorWindow
{
    [SerializeField]
    public BuildBundleWindow buildBundleWindow = new BuildBundleWindow();

    [SerializeField]
    public BuildHotPatchWindow buildHotWindow = new BuildHotPatchWindow();

    [SerializeField]
    public BundleSettings settingWindow ;
    [MenuItem("ZMFrame/AssetBundle")]
    public static void ShowAssetBundleWindow()
    {
        BuildWindows window = GetWindow<BuildWindows>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(985,612);
        window.ForceMenuTreeRebuild();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        buildBundleWindow.Initzation();
        buildHotWindow.Initzation();
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Build",null,EditorIcons.House},
            { "Build/AssetBundle",buildBundleWindow,EditorIcons.UnityLogo},
            { "Build/HotPatch",buildHotWindow,EditorIcons.UnityLogo},
            { "Bundle Setting",BundleSettings.Instance,EditorIcons.SettingsCog},
        };
        return menuTree;
    }
}
#endif