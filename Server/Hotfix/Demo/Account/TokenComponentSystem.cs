namespace ET
{
    [FriendClass(typeof(TokenComponent))]
    public static class TokenComponentSystem
    {

        public static void Add(this TokenComponent self, long key, string token)
        {
            self.tokenDict.Add(key,token);
            self.TimeOutRemoveKey(key,token).Coroutine();
        }

        public static string Get(this TokenComponent self, long key)
        {
            string value = null;
            self.tokenDict.TryGetValue(key, out value);
            return value;
        }

        public static void Remove(this TokenComponent self, long key)
        {
            if (self.tokenDict.ContainsKey(key))
            {
                self.tokenDict.Remove(key);
            }
        }

        private static async ETTask TimeOutRemoveKey(this TokenComponent self, long key, string token)
        {
            await TimerComponent.Instance.WaitAsync(600000);
            string onlineToken = self.Get(key);

            if (!string.IsNullOrEmpty(onlineToken) && onlineToken ==token)
            {
                self.Remove(key);
            }
        }
    }
}