/*---------------------------------
 *Title:UI自动化组件查找代码生成工具
 *Date:2024/4/14 4:15:33
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件查找脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace ZMUIFrameWork
{
	public class YZTWindowUIComponent
	{
		public   Button  CloseButton;

		public   Button  LoadSpriteButton;

		public   Button  LoadSpriteAsyncButton;

		public   Button  LoadAltasSpriteButton;

		public   Button  LoadTextureButton;

		public   Button  LoadTextureAsyncButton;

		public   Text  LoadTextAssetsText;

		public   Transform  InstantiateRootTransform;

		public   Transform  InstantiateAsyncRootTransform;

		public   Transform  PreLoadObjTransform;

		public  void InitComponent(WindowBase target)
		{
		     //组件查找
		     CloseButton =target.transform.Find("UIContent/[Button]Close").GetComponent<Button>();
		     LoadSpriteButton =target.transform.Find("UIContent/LoadSprite/[Button]LoadSprite").GetComponent<Button>();
		     LoadSpriteAsyncButton =target.transform.Find("UIContent/LoadSpriteAsync/[Button]LoadSpriteAsync").GetComponent<Button>();
		     LoadAltasSpriteButton =target.transform.Find("UIContent/LoadAltasSprite/[Button]LoadAltasSprite").GetComponent<Button>();
		     LoadTextureButton =target.transform.Find("UIContent/LoadTexture/[Button]LoadTexture").GetComponent<Button>();
		     LoadTextureAsyncButton =target.transform.Find("UIContent/LoadTextureAsync/[Button]LoadTextureAsync").GetComponent<Button>();
		     LoadTextAssetsText =target.transform.Find("UIContent/LoadTextAssets/[Text]LoadTextAssets").GetComponent<Text>();
		     InstantiateRootTransform =target.transform.Find("UIContent/Instantiate/[Transform]InstantiateRoot").transform;
		     InstantiateAsyncRootTransform =target.transform.Find("UIContent/InstantiateAsync/[Transform]InstantiateAsyncRoot").transform;
		     PreLoadObjTransform =target.transform.Find("UIContent/PreLoadObj/[Transform]PreLoadObj").transform;
	
	
		     //组件事件绑定
		     YZTWindow mWindow=(YZTWindow)target;
		     target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
		     target.AddButtonClickListener(LoadSpriteButton,mWindow.OnLoadSpriteButtonClick);
		     target.AddButtonClickListener(LoadSpriteAsyncButton,mWindow.OnLoadSpriteAsyncButtonClick);
		     target.AddButtonClickListener(LoadAltasSpriteButton,mWindow.OnLoadAltasSpriteButtonClick);
		     target.AddButtonClickListener(LoadTextureButton,mWindow.OnLoadTextureButtonClick);
		     target.AddButtonClickListener(LoadTextureAsyncButton,mWindow.OnLoadTextureAsyncButtonClick);
		}
	}
}
