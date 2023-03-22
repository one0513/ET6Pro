using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIForgeView : Entity, IAwake, ILoad, IOnCreate, IOnEnable,IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UIForge/Prefabs/UIForgeView.prefab";
		public UIButton btnClose;
		public UIButton btnWeapon;
		public UIButton btnDeputy;
		public UIButton btnCasque;
		public UIButton btnArmour;
		public UIButton btnStart;
		public UIEmptyGameobject Mar1;
		public UIEmptyGameobject Mar2;
		public UIEmptyGameobject Mar3;
		public UIEmptyGameobject Result;
		public UITextmesh lblResult;
		public UILoopListView2 ScrollView;

		public UIImage spSelect;

		public Scene scene;
		public int curSelectPos = (int)EquipPosition.Weapon;//当前选择的装备类型 EquipPosition
		public int curSelectMar = 0;//当前选择的材料位置
		public List<Item> listData = new List<Item>();

		public Dictionary<int, UIBagItem> MarDit = new Dictionary<int, UIBagItem>();
		public UIBagItem ResultItem = new UIBagItem();
	}
}
