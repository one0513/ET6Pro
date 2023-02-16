using UnityEngine;

namespace MingUI.Script.Core {
    /*
     * 很挫的通过clone一个label来实现加粗效果
     */
    class CBold : MonoBehaviour {
        private CMeshProLabel oriLabel;
        private CMeshProLabel cloneLabel;

        void Start() {
            oriLabel = oriLabel ?? GetComponent<CMeshProLabel>();
            if (oriLabel != null) {
                if (cloneLabel == null) {
                    cloneLabel = Instantiate(oriLabel);
                    cloneLabel.name = "boldClone";
                    var cbd = cloneLabel.GetComponent<CBold>();
                    Destroy(cbd);
                    cloneLabel.transform.parent = oriLabel.transform.parent;
                    cloneLabel.transform.localScale = Vector3.one;
                    cloneLabel.transform.localPosition = oriLabel.transform.localPosition;
                    //oriLabel.AddSizeChange(OnChange);
                }
            }
        }

        private void OnChange(Vector2 vec) {
            if (cloneLabel) {
                cloneLabel.text = oriLabel.text;
            }
        }
    }
}
