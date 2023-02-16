using System.Linq;
using UnityEngine;

namespace Assets.Script {
    public enum ViewType {
        DisplayObj,
        BaseView,
        BasePanel,
        PopupPanel,
        BaseRender,
        None,
        TabNavPanel,
        TabNavMainView,
        TabNavSubView,
        自定义,
        NotifyPanel
    }


    public class ModuleCreateOptions : MonoBehaviour {
        public bool isFullPanel;
        public bool isUnderResidentPanel;
        public ViewType viewType;
        public bool autoLoad = true;
        public string 自定义基类 = "";
        public bool genCreateModulePanelFunc = true;
        public string 自定义bindkey = "";

        private static ViewType[] NOT_LOAD_TYPES = { ViewType.BaseRender, ViewType.TabNavMainView, ViewType.TabNavSubView };
        private static ViewType[] PANEL_FAMILY = { ViewType.BasePanel, ViewType.PopupPanel, ViewType.TabNavPanel, ViewType.NotifyPanel };

        public bool ForceNotLoad() {
            return NOT_LOAD_TYPES.Contains(viewType);
        }

        public bool IsPanel() {
            return PANEL_FAMILY.Contains(viewType);
        }

        public string GetViewTypeStr() {
            if (viewType == ViewType.自定义) {
                return 自定义基类;
            }
            if (viewType == ViewType.NotifyPanel) {
                return "BasePanel";
            }
            return viewType.ToString();
        }

        protected virtual void Start() {
            //脚本初始化时，不会执行enable
            MingUIAgent.OnEnable(this, true);
        }

        protected virtual void OnEnable() {
            MingUIAgent.OnEnable(this, true);
        }

        protected virtual void OnDisable() {
            MingUIAgent.OnEnable(this, false);

        }
    }
}
