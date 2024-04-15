/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Date:2024/4/14 3:18:00
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;

	public class YZTWindow:WindowBase
	{
	
		 public YZTWindowDataComponent dataCompt;
	public void Start()
    {
		Debug.Log("232");

	}
		 #region 声明周期函数
		 //调用机制与Mono Awake一致
		 public override void OnAwake()
		 {
			 dataCompt=gameObject.GetComponent<YZTWindowDataComponent>();
			 dataCompt.InitComponent(this);
			 base.OnAwake();
		 }
		 //物体显示时执行
		 public override void OnShow()
		 {
			 base.OnShow();
		 }
		 //物体隐藏时执行
		 public override void OnHide()
		 {
			 base.OnHide();
		 }
		 //物体销毁时执行
		 public override void OnDestroy()
		 {
			 base.OnDestroy();
		 }
		 #endregion
		 #region API Function
		    
		 #endregion
		 #region UI组件事件
		 public void OnCloseButtonClick()
		 {
		Debug.Log("232");
			HideWindow();
		 }
		 public void OnLoadSpriteButtonClick()
		 {
		Debug.Log("2322");
	}
		 public void OnLoadSpriteAsyncButtonClick()
		 {
		Debug.Log("2323");
	}
		 public void OnLoadAltasSpriteButtonClick()
		 {
		Debug.Log("2324");

	}
		 public void OnLoadTextureButtonClick()
		 {
		Debug.Log("2325");
		HideWindow();
	}
		 public void OnLoadTextureAsyncButtonClick()
		 {
		Debug.Log("2326");
	}
		 #endregion
	}
