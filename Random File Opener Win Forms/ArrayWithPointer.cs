using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class ArrayWithPointer<T> where T : class
    {
        private List<T> _entities = new List<T>(capacity: 0);
        private int _currentIndex = 0;
        public bool IsAllGenerated { get; private set; } = false;

        public void Initialize(IEnumerable<T> sequence)
        {
            _entities = sequence.ToList();
            _currentIndex = 0;
        }

        public T GetCurrentAndMoveNext()
        {
            if (_entities.IsEmpty())
                return null;

            var current = _entities[_currentIndex];

            _currentIndex++;

            if (_currentIndex == _entities.Count)
            {
                _currentIndex = 0;
                IsAllGenerated = true;
            }

            return current;
        }

        public void ForAll(Action<T> action)
        {
            _entities.ForAll(action);
        }

        public void Delete(T item)
        {
            _entities.Remove(item);
            if (_currentIndex != 0)
                _currentIndex--;
        }
    }
}