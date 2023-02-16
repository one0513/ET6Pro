using UnityEngine.EventSystems;

public class CStandaloneInputModule: StandaloneInputModule
{
    public void ResetPressEvent()
    {
        foreach (var kv in m_PointerData)
        {
            ProcessTouchPress(kv.Value,true,false);
        }
    }
}
