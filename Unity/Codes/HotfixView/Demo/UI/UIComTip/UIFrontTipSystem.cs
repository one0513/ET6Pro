using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	

	[UISystem]
	[FriendClass(typeof(UIFrontTip))]
	public class UIFrontTipOnEnableSystem : OnEnableSystem<UIFrontTip, string>
	{
		public override void OnEnable(UIFrontTip self, string param1)
		{
			self.lblContent.SetText(param1);
		}
	}
	
	[UISystem]
	[FriendClass(typeof(UIFrontTip))]
	public class UIFrontTipOnCreateSystem : OnCreateSystem<UIFrontTip>
	{

		public override void OnCreate(UIFrontTip self)
		{
			self.lblContent = self.AddUIComponent<UITextmesh>("lblContent");
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIFrontTip))]
	public class UIFrontTipLoadSystem : LoadSystem<UIFrontTip>
	{

		public override void Load(UIFrontTip self)
		{
		}

	}
	

}
