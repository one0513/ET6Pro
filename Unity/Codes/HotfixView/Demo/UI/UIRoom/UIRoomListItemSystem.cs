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
			self.btnJoin.SetOnClick(()=>{self.OnClickbtn();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIRoomListItem))]
	public class UIRoomListItemLoadSystem : LoadSystem<UIRoomListItem>
	{

		public override void Load(UIRoomListItem self)
		{
			self.btnJoin.SetOnClick(()=>{self.OnClickbtn();});
		}

	}
	[FriendClass(typeof(UIRoomListItem))]
	[FriendClass(typeof(RoomInfo))]
	public static class UIRoomListItemSystem
	{
		public static void OnClickbtn(this UIRoomListItem self)
		{

		}
		
		public static void SetData(this UIRoomListItem self,RoomInfo info)
		{
			self.lblRoomPlayerNum.SetText($"小队人数:{info.RoomPlayerNum}/3");
			self.lblRoomName.SetText(info.RoomName);
		}
	}

}
