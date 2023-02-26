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
	[FriendClass(typeof(TaskInfo))]
	public static class UITaskItemSystem
	{
		public static void OnClickbtnGetReward(this UITaskItem self)
		{
			if (self.canGetReward)
			{
				TaskHelper.GetTaskReward(self.scene, self.configId).Coroutine();
			}
		}
		
		public static void SetData(this UITaskItem self,TaskInfo info)
		{
			var config = TaskConfigCategory.Instance.Get(info.ConfigId);
			self.configId = info.ConfigId;
			if (info.TaskState == (int)TaskState.Complete)
			{
				self.canGetReward = true;
				self.lblTaskStatue.SetText("领取奖励");
			}
			else
			{
				self.lblTaskStatue.SetText("未完成");
			}
			self.lblTaskName.SetText(config.TaskName);
			self.lblReward.SetText($"奖励:{config.RewardExpCount}经验");
			self.lblDesc.SetText(config.TaskDesc);
			self.lblProgress.SetText($"进度:{info.TaskPogress}/{config.TaskTargetCount}");
		}
	}

}
