namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// ConcurrentOperation allows us to get exclusive thread access for a given string value.
    /// </summary>
    internal class ConcurrentKeyOperation
    {
        public delegate void ExecuteProc();
        private readonly Dictionary<string, LockReference> _locks = new();

        private class LockReference
        {
            private int _count = 0;

            public long Count
                => Volatile.Read(ref _count);

            public void Increment()
                => Interlocked.Increment(ref _count);

            public void Decrement()
                => Interlocked.Decrement(ref _count);
        }

        public void Execute(string key, ExecuteProc proc)
        {
            LockReference? lockKey;

            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out lockKey))
                {
                    lockKey = new LockReference();
                    _locks.Add(key, lockKey);
                }

                lockKey.Increment();
            }

            lock (lockKey)
            {
                proc();
            }

            lock (_locks)
            {
                lockKey.Decrement();
                if (lockKey.Count == 0)
                {
                    _locks.Remove(key);
                }
            }
        }
    }
}
