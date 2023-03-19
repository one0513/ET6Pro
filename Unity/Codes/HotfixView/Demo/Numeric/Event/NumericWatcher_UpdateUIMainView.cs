using ET.EventType;
using UnityEngine;

namespace ET
{
    // [NumericWatcher(NumericType.Lv)]
    // [NumericWatcher(NumericType.CurLevel)]
    [NumericWatcher(NumericType.Exp)]
    public class NumericWatcher_UpdateUIMainView : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (args.New > args.Old && args.Old != 0)
            {
                string content = $"经验：+{args.New - args.Old}";
                Game.EventSystem.PublishAsync(new UIEventType.ShowFront() { Text = content }).Coroutine();
            }
          
            
            //UIManagerComponent.Instance.GetWindow<UIMainView>()?.UpdateView();
        }
    }
}