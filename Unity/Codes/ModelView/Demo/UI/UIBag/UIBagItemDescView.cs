using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UIComponent]
	public class UIBagItemDescView : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIBag/Prefabs/UIBagItemDescView.prefab";
		public UITextmesh lblName;
		public UIButton btnClose;
		public UITextmesh lblDesc;
		public UIEmptyGameobject CanUseTransform;
		public UITextmesh lblUseNum;
		public UISlider SlideChangeNum;
		public UIButton btnUse;

		public Scene scene;
		public Item Data;
		public int selectNum;

	}
}
