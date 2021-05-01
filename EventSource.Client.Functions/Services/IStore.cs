using System;
using System.Collections.Generic;

namespace EventSource.Client.Functions.Services
{
    public interface IStore<TData>
    {
        IEnumerable<TData> Get(Func<TData, bool> filter);

        IEnumerable<TData> GetAll();

        void Add(TData data);
    }
}