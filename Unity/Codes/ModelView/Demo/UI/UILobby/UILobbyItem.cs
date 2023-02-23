using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UILobbyItem : Entity, IAwake, ILoad, IOnCreate, IOnEnable
	{
		public UITextmesh lblServerName;

		public UIButton buttonClick;

		public ServerInfo itemData;

		public Scene scene;

	}
}
