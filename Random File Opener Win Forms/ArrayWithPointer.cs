using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class ArrayWithPointer<T> where T : class, IDeletable
    {
        private List<T> _entities { get; set; } = new List<T>(capacity: 0);
        private int _currentIndex = 0;

        public void Initialize(IEnumerable<T> sequence)
        {
            _entities = sequence.ToList();
            _currentIndex = 0;
        }

        public T GetCurrentAndMoveNext()
        {
            if (_entities.Count == 0)
                return null;

            var current = _entities[_currentIndex];

            _currentIndex++;

            if (_currentIndex == _entities.Count)
                _currentIndex = 0;

            return current;
        }

        public void FlushDeleted()
        {
            if (_entities.Count == 0) 
                return;

            var deletedCount = _entities.RemoveAll(u => u.IsDeleted);

            _currentIndex -= deletedCount;
        }

        public void ForAll(Action<T> action)
        {
            _entities.ForAll(action);
        }
    }
}