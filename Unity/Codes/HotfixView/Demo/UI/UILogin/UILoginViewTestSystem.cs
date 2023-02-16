using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UILoginViewTest))]
	public class UILoginViewTestOnCreateSystem : OnCreateSystem<UILoginViewTest>
	{

		public override void OnCreate(UILoginViewTest self)
		{
			self.lbl = self.AddUIComponent<UITextmesh>("Panel/lbl");
			self.sp = self.AddUIComponent<UIImage>("Panel/sp");
			self.sp_blue = self.AddUIComponent<UIImage>("Panel/sp_blue");
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UILoginViewTest))]
	public class UILoginViewTestLoadSystem : LoadSystem<UILoginViewTest>
	{

		public override void Load(UILoginViewTest self)
		{
		}

	}
	[FriendClass(typeof(UILoginViewTest))]
	public static class UILoginViewTestSystem
	{
	}

}
