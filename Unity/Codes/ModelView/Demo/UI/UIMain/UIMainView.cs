using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIMainView : Entity, IAwake, ILoad, IOnCreate, IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UIMain/Prefabs/UIMainView.prefab";
		public UITextmesh lblRoleName;
		public UITextmesh lblLevel;
		public UITextmesh lblExp;
		public UITextmesh lblCoin;
		public UITextmesh lblCurLevel;
		public UIButton btnRole;
		public UIButton btnAdventure;
		public UIButton btnNextAdventure;
		public UIButton btnTask;
		
		public UIButton btnCreateRoom;
		public UIButton btnRoomList;
		public UIButton btnOutRoom;
		public UIButton btnMyRoom;
		public UIButton btnChangeMap;
		public Scene scene;

	}
}
