using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class ArrayWithPointer<T> where T : class
    {
        public int CurrentIndex { get; private set; } = 0;
        public IReadOnlyList<T> All => _entities;

        private List<T> _entities = new List<T>(capacity: 0);

        public void Initialize(List<T> sequence)
        {
            _entities = sequence;
            CurrentIndex = 0;
        }

        public T Current => _entities[CurrentIndex];

        public T GetCurrent()
        {
            if (_entities.IsEmpty())
                return null;

            var current = Current;

            return current;
        }

        private void AdvanceToIndex(int targetIndex)
        {
            CurrentIndex = targetIndex;

            if (CurrentIndex >= _entities.Count)
                CurrentIndex = 0;
        }

        public void ForAll(Action<T> action)
        {
            _entities.ForAll(action);
        }

        public void Delete(T item)
        {
            var indexOfDeletedFile = _entities.IndexOf(item); 
            
            _entities.Remove(item);
            if (CurrentIndex != 0 && indexOfDeletedFile <= CurrentIndex)
                CurrentIndex--;
        }

        public void Delete(T[] items)
        {
            _entities.RemoveAll(items.Contains);
            CurrentIndex = 0;
        }

        public void SelectFile(T file)
        {
            var index = _entities.IndexOf(file);
            AdvanceToIndex(index + 1);
        }

        public void MoveItemToFirstPosition(T selectedFile)
        {
            _entities.Remove(selectedFile);
            _entities.Insert(0, selectedFile);
        }
    }
}