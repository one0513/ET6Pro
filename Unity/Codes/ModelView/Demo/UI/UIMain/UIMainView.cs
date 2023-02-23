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
		public UIButton btnRole;
		public UIButton btnAdventure;
		public UIButton btnTask;
		public Scene scene;

	}
}
