/*---------------------------------
 *Title:UI自动化组件查找代码生成工具
 *Date:2024/4/15 15:38:24
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件查找脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace ZMUIFrameWork
{
	public class TestWindowUIComponent
	{
		public   Button  LoginButton;

		public   Text  DesText;

		public   InputField  AccountInputField;

		public   InputField  PassInputField;

		public  void InitComponent(WindowBase target)
		{
		     //组件查找
		     LoginButton =target.transform.Find("UIContent/[Button]Login").GetComponent<Button>();
		     DesText =target.transform.Find("UIContent/[Text]Des").GetComponent<Text>();
		     AccountInputField =target.transform.Find("UIContent/[InputField]Account").GetComponent<InputField>();
		     PassInputField =target.transform.Find("UIContent/[InputField]Pass").GetComponent<InputField>();
	
	
		     //组件事件绑定
		     TestWindow mWindow=(TestWindow)target;
		     target.AddButtonClickListener(LoginButton,mWindow.OnLoginButtonClick);
		     target.AddInputFieldListener(AccountInputField,mWindow.OnAccountInputChange,mWindow.OnAccountInputEnd);
		     target.AddInputFieldListener(PassInputField,mWindow.OnPassInputChange,mWindow.OnPassInputEnd);
		}
	}
}
