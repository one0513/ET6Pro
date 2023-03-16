using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIMyRoomView : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIRoom/Prefabs/UIMyRoomView.prefab";
		public UIButton btnClose;
		public UIImage spRoomPlayer;

		public List<UIMyRoomItem> RoomItems = new List<UIMyRoomItem>();

	}
}
