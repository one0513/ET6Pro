using System;
using System.Collections.Generic;

namespace ET
{
    public class HashSetComponent<T>: HashSet<T>, IDisposable
    {
        public static HashSetComponent<T> Create()
        {
            return MonoPool.Instance.Fetch(TypeInfo<HashSetComponent<T>>.Type) as HashSetComponent<T>;
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool.Instance.Recycle(this);
        }
    }
}