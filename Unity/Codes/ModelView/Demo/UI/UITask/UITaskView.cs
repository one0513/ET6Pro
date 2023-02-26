using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UITaskView : Entity, IAwake, ILoad, IOnCreate, IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UITask/Prefabs/UITaskView.prefab";
		public UILoopListView2 ScrollView;
		public UIButton btnClose;
		public Scene scene;
		public List<TaskInfo> listData;
	}
}
