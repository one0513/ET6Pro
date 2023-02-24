using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIRoleView : Entity, IAwake, ILoad, IOnCreate, IOnEnable<Scene>
	{
		public static string PrefabPath => "UI/UIRole/Prefabs/UIRoleView.prefab";
		public UITextmesh lblCE;
		public UIButton btnUpLevel;
		public UISlider SliderLevel;
		public UITextmesh lblLevel;
		public UITextmesh lblExpProgress;
		public UITextmesh lblNowAddPoint;
		public UITextmesh lblAtk;
		public UIButton btnUpAtk;
		public UITextmesh lblDef;
		public UIButton btnUpDef;
		public UITextmesh lblHp;
		public UIButton btnUpHp;
		public UITextmesh lblDmg;
		public UIButton btnUpDmg;
		public UIButton btnUpTip;
		public UIButton btnClose;
		public Scene scene;


	}
}
