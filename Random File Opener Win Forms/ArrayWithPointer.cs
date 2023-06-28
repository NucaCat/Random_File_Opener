using System;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class ArrayWithPointer<T> where T : class
    {
        public T[] Entities { get; set; } = Array.Empty<T>();
        private int _currentIndex = 0;

        public T GetCurrentAndMoveNext()
        {
            if (Entities.Length == 0)
                return null;

            var current = Entities[_currentIndex];

            _currentIndex++;

            if (_currentIndex == Entities.Length)
                _currentIndex = 0;

            return current;
        }
    }
}