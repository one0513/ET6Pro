using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UIRoomListItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public UITextmesh lblRoomName;
		public UITextmesh lblDesc;
		public UITextmesh lblRoomPlayerNum;
		public UITextmesh lblRoomPower;
		public UIButton btnJoin;
		
		public Scene scene;
		 

	}
}
