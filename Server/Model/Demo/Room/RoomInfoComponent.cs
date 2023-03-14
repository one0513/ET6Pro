using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class RoomInfoComponent: Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, RoomInfo> RoomInfos = new Dictionary<long, RoomInfo>();
    }
}