using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UITaskItem))]
	public class UITaskItemOnCreateSystem : OnCreateSystem<UITaskItem>
	{

		public override void OnCreate(UITaskItem self)
		{
			self.lblTaskName = self.AddUIComponent<UITextmesh>("lblTaskName");
			self.lblDesc = self.AddUIComponent<UITextmesh>("lblDesc");
			self.lblProgress = self.AddUIComponent<UITextmesh>("lblProgress");
			self.lblReward = self.AddUIComponent<UITextmesh>("lblReward");
			self.btnGetReward = self.AddUIComponent<UIButton>("btnGetReward");
			self.lblTaskStatue = self.AddUIComponent<UITextmesh>("btnGetReward/lblTaskStatue");
			self.btnGetReward.SetOnClick(()=>{self.OnClickbtnGetReward();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UITaskItem))]
	public class UITaskItemLoadSystem : LoadSystem<UITaskItem>
	{

		public override void Load(UITaskItem self)
		{
			self.btnGetReward.SetOnClick(()=>{self.OnClickbtnGetReward();});
		}

	}
	[FriendClass(typeof(UITaskItem))]
	public static class UITaskItemSystem
	{
		public static void OnClickbtnGetReward(this UITaskItem self)
		{

		}
	}

}
