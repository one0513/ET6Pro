using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class UnitDBSaveComponent : Entity, IAwake,IDestroy
    {
        public HashSet<Type> EntityChangeTypeSet { get; } = new HashSet<Type>();
        
        public long Timer;
    }
}