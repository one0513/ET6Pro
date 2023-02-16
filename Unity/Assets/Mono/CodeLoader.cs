using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using HybridCLR;
using UnityEngine;
using YooAsset;

namespace ET
{
	public class CodeLoader
	{
		[IgnoreDataMember]//防裁剪
		public static CodeLoader Instance = new CodeLoader();

		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;
		
		private Assembly assembly;
		
		public bool IsInit = false;

		// 所有mono的类型
		private readonly Dictionary<string, Type> monoTypes = new Dictionary<string, Type>();

		// 热更层的类型
		private readonly Dictionary<string, Type> hotfixTypes = new Dictionary<string, Type>();

		public static List<string> AllAotDllList
		{
			get
			{
				var res = new List<string>();
				res.AddRange(SystemAotDllList);
				res.AddRange(UserAotDllList);
				return res;
			}
		}
		public static string[] SystemAotDllList = {
			"mscorlib.dll", 
			"System.dll", 
			"System.Core.dll"
		};
		public static string[] UserAotDllList = {
			"Unity.ThirdParty.dll",
			"Unity.Mono.dll"
		};
		/// <summary>
		/// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
		/// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
		/// </summary>
		public void LoadMetadataForAOTAssembly()
		{


		}
		private CodeLoader()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly ass in assemblies)
			{
				foreach (Type type in ass.GetTypes())
				{
					this.monoTypes[type.FullName] = type;
					this.monoTypes[type.AssemblyQualifiedName] = type;
				}
			}
		}
		
		public Type GetMonoType(string fullName)
		{
			this.monoTypes.TryGetValue(fullName, out Type type);
			return type;
		}
		
		public Type GetHotfixType(string fullName)
		{
			this.hotfixTypes.TryGetValue(fullName, out Type type);
			return type;
		}

		public void Dispose()
		{

		}
		
		public void Start()
		{
			string[] strings = new[] { "Code.dll","Code.pdb","mscorlib.dll","System.Core.dll","System.dll","Unity.Mono.dll","Unity.ThirdParty.dll" };
			Dictionary<string, UnityEngine.Object> dictionary = AssetsBundleHelper.LoadSomeAssetSync(strings);
			LoadMetadataForAOTAssemblies(dictionary);
			byte[] assBytes = ((TextAsset)dictionary["Code.dll"]).bytes;
			byte[] pdbBytes = ((TextAsset)dictionary["Code.pdb"]).bytes;
			assembly = Assembly.Load(assBytes,pdbBytes);
			foreach (Type type in this.assembly.GetTypes())
			{
				this.monoTypes[type.FullName] = type;
				this.hotfixTypes[type.FullName] = type;
			}
			IStaticAction start = new MonoStaticAction(assembly, "ET.Entry", "Start");
			start.Run();
			IsInit = true;
		}

		

		/// <summary>
		/// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
		/// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
		/// </summary>
		private static void LoadMetadataForAOTAssemblies(Dictionary<string,UnityEngine.Object> dictionary)
		{
			/// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
			/// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
			/// 
			HomologousImageMode mode = HomologousImageMode.SuperSet;
			foreach (var aotDllName in LoadDll.AOTMetaAssemblyNames)
			{
				byte[] dllBytes = ((TextAsset)dictionary[aotDllName]).bytes;
				// 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
				LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
				Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
			}
		}
		
		// 热重载调用下面三个方法
		// CodeLoader.Instance.LoadLogic();
		// Game.EventSystem.Add(CodeLoader.Instance.GetTypes());
		// Game.EventSystem.Load();
		public void LoadLogic()
		{
			// if (this.CodeMode != CodeMode.Reload)
			// {
			// 	throw new Exception("CodeMode != Reload!");
			// }
			
			// 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
			string[] logicFiles = Directory.GetFiles(Define.BuildOutputDir, "Logic_*.dll");
			if (logicFiles.Length != 1)
			{
				throw new Exception("Logic dll count != 1");
			}

			string logicName = Path.GetFileNameWithoutExtension(logicFiles[0]);
			byte[] assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.dll"));
			byte[] pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.pdb"));

			Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);
			
			foreach (Type type in this.assembly.GetTypes())
			{
				this.monoTypes[type.FullName] = type;
				this.hotfixTypes[type.FullName] = type;
			}
			
			foreach (Type type in hotfixAssembly.GetTypes())
			{
				this.monoTypes[type.FullName] = type;
				this.hotfixTypes[type.FullName] = type;
			}
		}

		public Dictionary<string, Type> GetHotfixTypes()
		{
			return this.hotfixTypes;
		}

		public bool isReStart = false;


		public Dictionary<string, EntityView> GetAllEntitys()
		{
			IStaticFunc<Dictionary<string, EntityView>> GetAllEntitys = 
					new MonoStaticFunc<Dictionary<string, EntityView>>(assembly, "ET.ViewEditorHelper", "GetAllEntitys");
			return GetAllEntitys.Run();
		}

		public EntityData GetEntityData()
		{
			IStaticFunc<EntityData> GetEntityData = new MonoStaticFunc<EntityData>(assembly, "ET.ViewEditorHelper", "GetEntityData");
			return GetEntityData.Run();
		}
	}
}