﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
    {
        return new AsyncEnumerable<T>(enumerable);
    }

    private class AsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public AsyncEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator(_enumerable.GetEnumerator());
        }

        private class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public AsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public T Current => _enumerator.Current;

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }
        }
    }
}
