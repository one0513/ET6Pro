using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIRoomListView : Entity, IAwake, ILoad, IOnCreate, IOnEnable,IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UIRoom/Prefabs/UIRoomListView.prefab";
		public UILoopListView2 ScrollView;
		public UIButton btnClose;
		public UIButton btnRefresh;
		public Scene scene;
		public List<RoomInfo> listData;

	}
}
