using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 环形缩放组件
/// 支持2d、3d旋转
/// </summary>
public class CHoop : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum HoopMode
    {
        FixCenter,//以中心点固定
        FreeDrag,//自由滑动
    }
    public bool is3D = true;
    public int rotationX = 30; //绕X轴角度 0-90 度
    public CBaseRender itemRenderPrefab;
    public float radius = 100;//半径
    public float minScale = 0.5f; //最小处缩放比例
    public float minAlpha = 0.5f; //最小透明度
    public HoopMode hoopMode = HoopMode.FixCenter;
    public Type itemType = typeof(BaseRender);
    public RectTransform content;


    public float backSpeed = 2; //归位速度，角度
    [NonSerialized]
    private float _angle;

    private int _count = 0;

    private Action<object> _onCenter;
    private Action<object> _onSelect;

    private List<object> _dataProvider;
    private List<float> _angleList = new List<float>();
    private Stack<BaseRender> _renderPool = new Stack<BaseRender>();
    private List<BaseRender> _beRenderRool = new List<BaseRender>();
    private List<BaseRender> _renderIndex = new List<BaseRender>();
    private Dictionary<BaseRender, float> _mapAngle = new Dictionary<BaseRender, float>();
    private bool _isBack = false;
    private float _backAngle = 0;
    private Vector2 _beginDragPos;
    private BaseRender _curRender;
    private List<BaseRender> _tempSortLs = new List<BaseRender>();

    /////////////////////////////////////////////////////////////
    public List<object> DataProvider
    {
        get { return _dataProvider; }
        set
        {
            _dataProvider = value;
            _count = _dataProvider.Count;
            if (_count > 0)
            {
                InitHoopParam();
                RecoverRender();
                InitItemRender();
            }
        }
    }

   
    public float ItemWidth
    {
        get { return itemRenderPrefab.Width; }
    }

    public float ItemHeight
    {
        get { return itemRenderPrefab.Height; }
    }

    protected override void Awake()
    {
        if (!is3D)
        {
            rotationX = 0;
        }
    }

    public void AddOnCenter(Action<object> callBack)
    {
        _onCenter = callBack;
    }

    public void AddSelect(Action<object> callBack)
    {
        _onSelect = callBack;
    }

    public void ClearSelect()
    {
        _onSelect = null;
    }

    private void InitHoopParam()
    {
        float intervalAngle = 360f / _count;
        _angleList.Clear();
        _tempSortLs.Clear();
        _renderIndex.Clear();
        for (int i = 0; i < _count; i++)
        {
            float angle = 270 + (i * intervalAngle);
            if (angle > 360)
            {
                angle = angle - 360;
            }
            _angleList.Add(angle);
        }
    }

    private float GetAlpha(float angle)
    {
        return GetScale(angle);
    }
    private float GetScale(float angle)
    {
        if (Math.Abs(minScale - 1) < 0.00001) return 1f;
        float scale = 1;
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        if (y < 0)
        {
            y = Mathf.Abs(y);
            scale = (0.5f + minScale / 2) + (y / radius) * ((1 - minScale) / 2);
        }
        else
        {
            scale = (0.5f + minScale / 2) - (y / radius) * ((1 - minScale) / 2);
        }
        return scale;
    }

    private Vector2 GetPos(float angle)
    {
        Vector2 pos = Vector2.one;
        pos.x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.y = radius * Mathf.Sin(angle * Mathf.Deg2Rad) * Mathf.Sin(rotationX * Mathf.Deg2Rad);
        return pos;
    }

    private float GetAngleByDistance(float distance)
    {
        return distance * (90 / radius);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _beginDragPos = eventData.position;
        _isBack = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (Math.Abs(eventData.delta.x) > 0.00001)
        {
            float angle = GetAngleByDistance(eventData.delta.x);
            Rotate(angle);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        EndDrag(eventData);
        SortItemTopClass();
    }


    public void EndDrag(PointerEventData eventData)
    {
        BaseRender curRender = _beRenderRool[_count - 1];
        if (_count > 1 && _curRender == curRender)
        {
            float posDis = eventData.position.x - _beginDragPos.x;
            float width = (ItemWidth / 3);
            if ((Mathf.Abs(curRender.Render.rectTransform.anchoredPosition.x) > width) && Math.Abs(posDis) < width+1)
            {
                curRender = GetPlusMinusRender(curRender, -posDis);
            }
        }
        float lastAngle = _mapAngle[curRender];
        _backAngle = 270 - lastAngle;
        if (Math.Abs(_backAngle) > 0.0001)
        {
            _isBack = true;
        }
        else
        {
            _onCenter(curRender.data);
        }
    }

    public void OnRenderClick(PointerEventData eventData)
    {
        CBaseRender cbRender = eventData.pointerPress.GetComponent<CBaseRender>();
        foreach (KeyValuePair<BaseRender, float> map in _mapAngle)
        {
            if (map.Key.Render == cbRender)
            {
                float angle = map.Value;
                if (_onSelect != null)
                {
                    _onSelect(map.Key.data);
                }
                if (_isBack) return;
                float realAngle = 270 - angle;
                if (angle >= 0 && angle < 90)
                {
                    realAngle = -(90 + angle);
                }
                _backAngle = realAngle;
                _isBack = true;
                return;
            }
        }
    }

    private BaseRender GetPlusMinusRender(BaseRender render, float dir)
    {
        if (_count <= 1) return render;
        _tempSortLs.Sort((a, b) =>
        {
            float angleA = _mapAngle[a];
            float angleB = _mapAngle[b];
            return (int)(angleA - angleB);
        });
        int index = _tempSortLs.IndexOf(render);
        if (dir > 0)
        {
            if (index == _count - 1)
            {
                return _tempSortLs[0];
            }
            return _tempSortLs[index + 1];
        }
        else
        {
            if (index != 0)
            {
                return _tempSortLs[index - 1];
            }
            return _tempSortLs[_count - 1];
        }
    }

    public virtual void Update()
    {
        if (_isBack)
        {
            float angle = backSpeed;
            float lastRemainAngel = _backAngle;
            if (_backAngle > 0)
            {
                _backAngle = _backAngle - backSpeed;
                if (_backAngle <= 0)
                {
                    angle = lastRemainAngel;
                    _isBack = false;
                }
            }
            else
            {
                angle = -backSpeed;
                _backAngle = (_backAngle + backSpeed);
                if (_backAngle >= 0)
                {
                    angle = lastRemainAngel;
                    _isBack = false;
                }
            }
            Rotate(angle);
            if (_isBack == false)
            {
                BaseRender curRender = _beRenderRool[_count-1];
                if (_onCenter != null)
                {
                    _onCenter(curRender.data);
                }
            }
        }
    }
    public void GotoIndex(int index)
    {
        BaseRender render = _renderIndex[index];
        float angle = _mapAngle[render];
        Rotate(270 - angle);
        if (_onCenter != null)
        {
            _onCenter(_beRenderRool[_count-1].data);
        }
    }

    public void GotoLast()
    {
        if(_isBack ) return;
        _backAngle = -90;
        _isBack = true;
    }
    public void GotoNext()
    {
        if (_isBack) return;
        _backAngle = 90;
        _isBack = true;
    }

    private float GetRealAngle(BaseRender render, float angle)
    {
        float lastAngle = _mapAngle[render];
        float realAngle = lastAngle + angle;
        if (realAngle > 360)
        {
            realAngle = realAngle - 360;
            return realAngle;
        }
        if (realAngle < 0)
        {
            realAngle = 360 + realAngle;
        }
        return realAngle;
    }

    public void Rotate(float dragAngle)
    {
        for (int i = 0; i < _count; i++)
        {
            BaseRender itemRender = _beRenderRool[i];
            float angle = GetRealAngle(itemRender, dragAngle);
            SetItemPos(itemRender, angle);
        }
        SortItemTopClass();

    }

    public void InitItemRender()
    {
        for (int i = 0; i < _count; i++)
        {
            BaseRender itemRender = GetItemRender();
            _beRenderRool.Add(itemRender);
            _tempSortLs.Add(itemRender);
            _renderIndex.Add(itemRender);
            float angle = _angleList[i];
            itemRender.Render.gameObject.name = itemRenderPrefab.name + i;
            SetItemPos(itemRender, angle);
            object data = _dataProvider[i];
            itemRender.SetData(data);
        }
        SortItemTopClass();
    }

    private float getCompareRate(float angle)
    {
        if (angle >= 90 && angle < 270)
        {
            return angle;
        }
        else if (angle >= 0 && angle < 90)
        {
            return 180 - angle;
        }
        else
        {
            return 540 - angle;
        }
    }

    private void SortItemTopClass()
    {
        _beRenderRool.Sort((a, b) =>
        {
            int indexA = (int)getCompareRate(_mapAngle[a]);
            int indexB = (int)getCompareRate(_mapAngle[b]);
            return indexA - indexB;
        });
        for (int i = 0; i < _beRenderRool.Count; i++)
        {
            BaseRender itemRender = _beRenderRool[i];
            itemRender.Render.rectTransform.SetSiblingIndex(i);
        }
        _curRender = _beRenderRool[_count - 1];
    }

    private void SetItemPos(BaseRender itemRender, float angle)
    {
        _mapAngle[itemRender] = angle;
        Vector2 pos = GetPos(angle);
        float scale = GetScale(angle);
        itemRender.Render.rectTransform.localScale = Vector3.one * scale;
        itemRender.Render.rectTransform.localPosition = Vector3.zero;
        itemRender.Render.rectTransform.anchoredPosition = pos;
        if (!is3D && angle > 5 && angle < 175) //由于精度问题，小于5度
        {
            itemRender.Render.gameObject.SetActive(false);
        }
        else
        {
            itemRender.Render.gameObject.SetActive(true);
            float alpha = GetAlpha(angle);
            if (Math.Abs(alpha - 1f) > 0.00001)
            {
                MingUIUtil.SetGroupAlpha(itemRender.Render.gameObject,alpha);
            }
        }
    }

    private void RecoverRender()
    {
        int lenght = _beRenderRool.Count;
        for (int i = 0; i < lenght; i++)
        {
            BaseRender itemRender = _beRenderRool[0];
            _beRenderRool.RemoveAt(0);
            itemRender.Render.gameObject.SetActive(false);
            _renderPool.Push(itemRender);
        }
    }

    protected virtual BaseRender GetItemRender()
    {
        if (_renderPool.Count > 0)
        {
            return _renderPool.Pop();
        }
        return CreateItemRender();
    }

    protected virtual BaseRender CreateItemRender()
    {
        BaseRender render = Activator.CreateInstance(itemType) as BaseRender;
        if (render != null)
        {
            GameObject ob = Instantiate(itemRenderPrefab.gameObject);
            CBaseRender baseRender = ob.GetComponent<CBaseRender>();
            AddChild(baseRender.rectTransform, Vector2.zero);
          
            baseRender.AddClick(OnRenderClick);
            render.Render = baseRender;
        }
        return render;
    }

    public virtual void AddChild(RectTransform trans, Vector2 pos)
    {
        trans.SetParent(content);
        trans.anchorMin = trans.anchorMax = new Vector2(0.5f, 0.5f);
        trans.localScale = Vector3.one;
        trans.localEulerAngles = Vector3.zero;
        trans.anchoredPosition = pos;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
       
        _mapAngle.Clear();
        _tempSortLs.Clear();
        foreach (BaseRender baseRender in _renderPool)
        {
            baseRender.OnDestroy();
        }
        foreach (BaseRender baseRender in _beRenderRool)
        {
            baseRender.OnDestroy();
        }
    }
}
