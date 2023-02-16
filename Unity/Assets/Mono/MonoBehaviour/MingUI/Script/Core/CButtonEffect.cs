using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 按钮点击缩放效果
/// </summary>
public class CButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 orginScale = Vector3.one; //正常大小

    public float pressDownTime = 0.08f;
    public float pressDownScale = 0.95f; //相对于源缩放
    public Ease pressDownEase = Ease.Linear;

    public float pressUpTime = 0.5f;
    public float pressUpScale = 1f; //相对于源缩放
    public Ease pressUpEase = Ease.OutElastic;

    private Tweener _tweener;
    private void Awake()
    {
        orginScale = transform.localScale;
    }

    private void OnDisable()
    {
        ClearTween();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        ClearTween();
        _tweener = transform.DOScale(orginScale * pressDownScale, pressDownTime);
        _tweener.SetEase(pressDownEase);
        _tweener.SetId(GetInstanceID());
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        ClearTween();
        _tweener = transform.DOScale(orginScale * pressUpScale, pressUpTime);
        _tweener.SetEase(pressUpEase);
        _tweener.SetId(GetInstanceID());
    }

    private void ClearTween()
    {
        if (_tweener != null)
        {
            _tweener.Kill();
        }
        transform.localScale = orginScale;
    }

    public void DoCustomizeTweenScal(float scalTime, float durationTime, Ease ease = Ease.Unset)
    {
        ClearTween();
        ease = ease == Ease.Unset ? pressUpEase : ease;
        _tweener = transform.DOScale(orginScale * scalTime, durationTime);
        _tweener.SetEase(ease);
        _tweener.SetId(GetInstanceID());
    }

}