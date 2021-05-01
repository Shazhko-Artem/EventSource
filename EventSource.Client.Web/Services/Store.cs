using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSource.Client.Web.Services
{
    public class Store<TData> : IStore<TData>
    {
        private readonly List<TData> store = new List<TData>();
        public IEnumerable<TData> Get(Func<TData, bool> filter)
        {
            return store.Where(filter);
        }

        public IEnumerable<TData> GetAll()
        {
            return this.store.ToList();
        }

        public void Add(TData data)
        {
            this.store.Add(data);
        }
    }
}