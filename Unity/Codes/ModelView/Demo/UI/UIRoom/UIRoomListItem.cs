using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	[ChildOfAttribute(typeof(UIRoomListView))]
	public class UIRoomListItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIRoom/Prefabs/UIRoomListItem.prefab";
		public UITextmesh lblRoomName;
		public UITextmesh lblDesc;
		public UITextmesh lblRoomPlayerNum;
		public UITextmesh lblRoomPower;
		public UIButton btnJoin;
		
		public Scene scene;
		public RoomInfo info;

	}
}
