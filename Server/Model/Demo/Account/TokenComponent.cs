using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class TokenComponent : Entity ,IAwake
    {
        public readonly Dictionary<long, string> tokenDict = new Dictionary<long, string>();
    }
}