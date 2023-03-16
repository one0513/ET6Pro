using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIMyRoomItem))]
	public class UIMyRoomItemOnCreateSystem : OnCreateSystem<UIMyRoomItem>
	{

		public override void OnCreate(UIMyRoomItem self)
		{
			self.spHeader = self.AddUIComponent<UIImage>("sp/spHeader");
			self.lblName = self.AddUIComponent<UITextmesh>("lblName");
			self.lblLV = self.AddUIComponent<UITextmesh>("lblLV");
			self.lblState = self.AddUIComponent<UITextmesh>("lblState");
			
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIMyRoomItem))]
	public class UIMyRoomItemLoadSystem : LoadSystem<UIMyRoomItem>
	{

		public override void Load(UIMyRoomItem self)
		{
		}

	}
	[FriendClass(typeof(UIMyRoomItem))]
	public static class UIMyRoomItemSystem
	{
		public static void UpdateItem(this UIMyRoomItem self,PlayerInfoProto infoProto)
		{
			self.lblName.SetText(infoProto.Name);
			self.lblLV.SetText($"LV.{infoProto.Level}");
			string state = infoProto.IsOnline == 0? "在线" : "离线";
			self.lblState.SetText(state);
		}
	}

}
