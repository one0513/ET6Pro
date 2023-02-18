using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UILobbyView : Entity, IAwake, ILoad, IOnCreate, IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UILobby/Prefabs/UILobbyView.prefab";
		public UIImage Panel;
		public UIButton btnClienTest;
		public UIButton btnServerTest;

		public Scene scene;

	}
}
