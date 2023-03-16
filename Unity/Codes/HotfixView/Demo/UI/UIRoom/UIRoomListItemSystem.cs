using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	
	
	[UISystem]
	[FriendClass(typeof(UIRoomListItem))]
	public class UIRoomListItemOnCreateSystem : OnCreateSystem<UIRoomListItem>
	{

		public override void OnCreate(UIRoomListItem self)
		{
			self.lblRoomName = self.AddUIComponent<UITextmesh>("lblRoomName");
			self.lblDesc = self.AddUIComponent<UITextmesh>("lblDesc");
			self.lblRoomPlayerNum = self.AddUIComponent<UITextmesh>("lblRoomPlayerNum");
			self.lblRoomPower = self.AddUIComponent<UITextmesh>("lblRoomPower");
			self.btnJoin = self.AddUIComponent<UIButton>("btnJoin");
			self.btnJoin.SetOnClick(()=>{self.OnClickbtnJoin().Coroutine();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIRoomListItem))]
	public class UIRoomListItemLoadSystem : LoadSystem<UIRoomListItem>
	{

		public override void Load(UIRoomListItem self)
		{
			self.btnJoin.SetOnClick(()=>{self.OnClickbtnJoin().Coroutine();});
		}

	}
	[FriendClass(typeof(UIRoomListItem))]
	[FriendClass(typeof(RoomInfo))]
	public static class UIRoomListItemSystem
	{
		public static async ETTask OnClickbtnJoin(this UIRoomListItem self)
		{
			M2C_JoinBattleRoom m2CJoinBattleRoom= (M2C_JoinBattleRoom) await self.scene.GetComponent<SessionComponent>().Session.Call(new C2M_JoinBattleRoom() {RoomId = self.info.RoomId});
			if (m2CJoinBattleRoom.Error == ErrorCode.ERR_Success)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "已加入该小队" }).Coroutine();
				UIManagerComponent.Instance.CloseWindow<UIRoomListView>().Coroutine();
			}
			else if(m2CJoinBattleRoom.Error == ErrorCode.ERR_AreadyHasRoom)
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "请先退出当前所在队伍" }).Coroutine();
			}
			else
			{
				Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "当前队伍已满员或解散" }).Coroutine();
			}
		}
		
		public static void SetData(this UIRoomListItem self,RoomInfo info)
		{
			self.lblRoomPlayerNum.SetText($"小队人数:{info.RoomPlayerNum}/3");
			self.lblRoomName.SetText(info.RoomName);
			self.info = info;
		}
	}

}
