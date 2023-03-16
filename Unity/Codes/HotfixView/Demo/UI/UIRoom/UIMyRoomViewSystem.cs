using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIMyRoomView))]
	public class UIMyRoomViewOnCreateSystem : OnCreateSystem<UIMyRoomView>
	{

		public override void OnCreate(UIMyRoomView self)
		{
			self.btnClose = self.AddUIComponent<UIButton>("spBg/btnClose");
			self.spRoomPlayer = self.AddUIComponent<UIImage>("spBg/spRoomPlayer");
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIMyRoomView))]
	public class UIMyRoomViewLoadSystem : LoadSystem<UIMyRoomView>
	{

		public override void Load(UIMyRoomView self)
		{
			self.btnClose.SetOnClick(()=>{self.OnClickbtnClose();});
		}

	}
	[FriendClass(typeof(UIMyRoomView))]
	public static class UIMyRoomViewSystem
	{
		public static void OnClickbtnClose(this UIMyRoomView self)
		{
			self.CloseSelf().Coroutine();
		}
		
		public static async ETTask UpdateView(this UIMyRoomView self ,List<PlayerInfoProto> infoProtos)
		{
			for (int i = 0; i < self.RoomItems.Count; i++)
			{
				self.RoomItems[i].SetActive(false);
			}

			for (int i = 0; i < infoProtos.Count; i++)
			{
				if (self.RoomItems.Count >= i+1)
				{
					self.RoomItems[i].UpdateItem(infoProtos[i]);
					self.RoomItems[i].SetActive(true);
				}
				else
				{
					GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIMyRoomItem.PrefabPath);
					UIMyRoomItem ui = self.AddChild<UIMyRoomItem>();
					var transform = gameObject.transform;
					ui.AddUIComponent<UITransform,Transform>("", transform);
					transform = gameObject.transform;
					transform.SetParent(self.spRoomPlayer.GetTransform());
					transform.localPosition = new Vector3(360*i-360,0,0);
					transform.localRotation = Quaternion.identity;
					transform.localScale = new Vector3(1, 1, 1);
					UIWatcherComponent.Instance.OnCreate(ui);
					UIWatcherComponent.Instance.OnEnable(ui);
					self.RoomItems.Add(ui);
					self.RoomItems[i].UpdateItem(infoProtos[i]);
					self.RoomItems[i].SetActive(true);
				}
			}
		}
	}

}
