using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[ChildOfAttribute(typeof(UIMyRoomView))]

	public class UIMyRoomItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UIRoom/Prefabs/UIMyRoomItem.prefab";
		public UIImage spHeader;
		public UITextmesh lblName;
		public UITextmesh lblLV;
		public UITextmesh lblState;

	}
}
