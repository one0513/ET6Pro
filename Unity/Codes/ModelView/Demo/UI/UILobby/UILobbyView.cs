using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UILobbyView : Entity, IAwake, ILoad, IOnCreate, IOnEnable<Scene>,IOnEnable<List<ServerInfo>,Scene>
	{
		public static UILobbyView Instance;
		public Scene scene;
		public static string PrefabPath => "UI/UILobby/Prefabs/UILobbyView.prefab";
		public UIButton btnStart;
		public UILoopListView2 ScrollView;
		public UITextmesh lblServer { get; set; }

		public List<ServerInfo> listData;


	}
}
