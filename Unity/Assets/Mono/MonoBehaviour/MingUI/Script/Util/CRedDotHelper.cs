using DG.Tweening;
using UnityEngine;

public class CRedDotHelper
{
    private static float redDotTweenTime = 0.25f;
    public static void ShowRedDot(CSprite redTip,bool isShow,ref string tweenID)
    {
        if (redTip == null)return;
        if (redTip.gameObject.activeSelf != isShow) redTip.gameObject.SetActive(isShow);
        if (isShow)
        {
            if (DOTween.IsTweening(tweenID) == false) {
                var seq = DOTween.Sequence();
                
                seq.Append(redTip.DOFade(.5f, redDotTweenTime)).Append(redTip.DOFade(1f, redDotTweenTime)).Append(redTip.DOFade(1, redDotTweenTime)).SetLoops(-1).SetId(tweenID).Goto(Time.time, true);
            }
        }
        else
        {
            DOTween.Kill(tweenID);
            tweenID = null;
        }
    }
    public static void SetRedDotTweenTime(float tweenTime)
    {
        redDotTweenTime = tweenTime;
    }

}