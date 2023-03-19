using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIMapView : Entity, IAwake, ILoad, IOnCreate, IOnEnable,IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UIMap/Prefabs/UIMapView.prefab";
		public UILoopListView2 ScrollView;
		public UIButton btnClose;
		public Scene scene;
		public List<BattleLevelConfig> listdatas = BattleLevelConfigCategory.Instance.GetAllList();

	}
}
