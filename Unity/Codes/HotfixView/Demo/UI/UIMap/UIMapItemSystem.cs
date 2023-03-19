using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	
	
	[UISystem]
	[FriendClass(typeof(UIMapItem))]
	public class UIMapItemOnCreateSystem : OnCreateSystem<UIMapItem>
	{

		public override void OnCreate(UIMapItem self)
		{
			self.lblMapName = self.AddUIComponent<UITextmesh>("lblMapName");
			self.lblMonsterNum = self.AddUIComponent<UITextmesh>("lblMonsterNum");
			self.btnJoin = self.AddUIComponent<UIButton>("btnJoin");
			self.btnJoin.SetOnClick(()=>{self.OnClickbtnJoin().Coroutine();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIMapItem))]
	public class UIMapItemLoadSystem : LoadSystem<UIMapItem>
	{

		public override void Load(UIMapItem self)
		{
			self.btnJoin.SetOnClick(()=>{self.OnClickbtnJoin().Coroutine();});
		}
	}
	[FriendClass(typeof(UIMapItem))]
	public static class UIMapItemSystem
	{
		public static async ETTask OnClickbtnJoin(this UIMapItem self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.scene.CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
			if (numericComponent.GetAsLong(NumericType.RoomID) == 0)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "请先加入小队" }).Coroutine();
			}
			else
			{
				int error = await AdventureHelper.RequestStartGameLevel(self.scene, self.data.Id);
				if (error != ErrorCode.ERR_Success)
				{
					Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "当前无法切换地图" }).Coroutine();
				}
			}
			UIManagerComponent.Instance.CloseWindow<UIMapView>().Coroutine();
		}
		
		public static void SetData(this UIMapItem self,BattleLevelConfig config)
		{
			self.data = config;
			self.lblMapName.SetText(config.Name);
			self.lblMonsterNum.SetText($"怪物数量:{config.MonsterIds.Length}");
		}
	}

}
