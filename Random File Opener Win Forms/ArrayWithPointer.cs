using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class ArrayWithPointer<T> where T : class, ISoftDeleteable
    {
        public int CurrentIndex { get; private set; } = 0;
        public bool IsAllGenerated { get; private set; } = false;
        public IEnumerable<T> All => _entities.ExcludeDeleted();

        private List<T> _entities = new List<T>(capacity: 0);

        public void Initialize(List<T> sequence)
        {
            _entities = sequence;
            CurrentIndex = 0;
        }

        public T Current => _entities[CurrentIndex];

        public T GetCurrent()
        {
            if (CurrentIndex >= _entities.Count)
                return null;

            if (_entities.IsEmpty())
                return null;

            var current = Current;

            return current;
        }

        private void AdvanceToIndex(int targetIndex)
        {
            while (true)
            {
                CurrentIndex = targetIndex;

                if (CurrentIndex >= _entities.Count)
                {
                    CurrentIndex = 0;
                    IsAllGenerated = true;
                    return;
                }

                if (Current.IsDeleted)
                {
                    targetIndex = CurrentIndex + 1;
                    continue;
                }

                break;
            }
        }

        public void ForAll(Action<T> action)
        {
            _entities.ForAll(action);
        }

        public void Delete(T item)
        {
            var indexOfDeletedFile = _entities.IndexOf(item); 
            
            item.SoftDelete();
            if (CurrentIndex != 0 && indexOfDeletedFile <= CurrentIndex)
                CurrentIndex--;
        }

        public void Delete(T[] items)
        {
            items.SoftDelete();
            CurrentIndex = 0;
        }

        public void SelectFile(T file)
        {
            var index = _entities.IndexOf(file);
            AdvanceToIndex(index + 1);
        }
    }
}