using System;
using System.Collections;
using System.Globalization;
using YooAsset;
using System.Linq;
using System.Reflection;
using System.Threading;
using UniFramework.Event;
using UniFramework.Module;
using UnityEngine;
namespace ET
{

	
	public class Init: MonoBehaviour
	{
		
		public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
		
		private void Awake()
		{
			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Log.Error(e.ExceptionObject.ToString());
			};
			
			SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
			DontDestroyOnLoad(gameObject);
			LitJson.UnityTypeBindings.Register();

			ETTask.ExceptionHandler += Log.Error;

			Log.ILog = new UnityLogger();

			Options.Instance = new Options();
			Options.Instance.Develop = 1;
			Options.Instance.LogLevel = 0;
			InitUnitySetting();
			
		}
		

		private void Start()
		{
			// 初始化BetterStreaming
			BetterStreamingAssets.Initialize();

			// 初始化事件系统
			UniEvent.Initalize();

			// 初始化管理系统
			UniModule.Initialize();

			// 初始化资源系统
			YooAssets.Initialize();
			YooAssets.SetOperationSystemMaxTimeSlice(30);

			// 创建补丁管理器
			UniModule.CreateModule<PatchManager>();

			// 开始补丁更新流程
			PatchManager.Instance.Run(PlayMode);
		}

		private void Update()
		{
			
			if (CodeLoader.Instance.IsInit)
			{
				CodeLoader.Instance.Update();
			}
		}

		private void LateUpdate()
		{
			if (CodeLoader.Instance.IsInit)
			{
				CodeLoader.Instance.LateUpdate();
			}
		}

		private void OnApplicationQuit()
		{
			CodeLoader.Instance?.OnApplicationQuit();
			CodeLoader.Instance?.Dispose();
		}
		
		// 一些unity的设置项目
		void InitUnitySetting()
		{
			Input.multiTouchEnabled = false;
			//设置帧率
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
			Application.runInBackground = true;
		}
	}
}