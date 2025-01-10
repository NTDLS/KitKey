namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// ConcurrentOperation allows us to get exclusive thread access for a given string value.
    /// </summary>
    internal class ConcurrentKeyOperation
    {
        public delegate void ConcurrentOperationFunction();
        public delegate T ConcurrentOperationFunction<T>();

        private readonly Dictionary<string, VolatileReferenceCounter> _locks = new();

        private class VolatileReferenceCounter
        {
            private int _count = 0;

            public long Count
                => Volatile.Read(ref _count);

            public void Increment()
                => Interlocked.Increment(ref _count);

            public void Decrement()
                => Interlocked.Decrement(ref _count);
        }

        public T Execute<T>(string key, ConcurrentOperationFunction<T> function)
        {
            VolatileReferenceCounter? referenceCounter;

            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out referenceCounter))
                {
                    referenceCounter = new VolatileReferenceCounter();
                    _locks.Add(key, referenceCounter);
                }

                referenceCounter.Increment();
            }

            T result;

            lock (referenceCounter)
            {
                result = function();
            }

            lock (_locks)
            {
                referenceCounter.Decrement();
                if (referenceCounter.Count == 0)
                {
                    _locks.Remove(key);
                }
            }

            return result;
        }

        public void Execute(string key, ConcurrentOperationFunction function)
        {
            VolatileReferenceCounter? referenceCounter;

            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out referenceCounter))
                {
                    referenceCounter = new VolatileReferenceCounter();
                    _locks.Add(key, referenceCounter);
                }

                referenceCounter.Increment();
            }

            lock (referenceCounter)
            {
                function();
            }

            lock (_locks)
            {
                referenceCounter.Decrement();
                if (referenceCounter.Count == 0)
                {
                    _locks.Remove(key);
                }
            }
        }
    }
}
