using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[ChildOfAttribute(typeof(UIFrontTipComponent))]
	public class UIFrontTip : Entity, IAwake, ILoad, IOnCreate, IOnEnable,IOnEnable<string>
	{
		public static string PrefabPath => "UI/UIComTip/Prefabs/UIFrontTip.prefab";
		public UITextmesh lblContent;
		 

	}
}
