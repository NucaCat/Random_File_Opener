using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    public interface ISoftDeleteable
    {
        bool IsDeleted { get; set; }
    }

    public static class SoftDeleteableExtensions
    {
        public static void SoftDelete(this ISoftDeleteable item)
            => item.IsDeleted = true;
        public static void SoftDelete(this IEnumerable<ISoftDeleteable> items)
        {
            foreach (var item in items)
            {
                item.SoftDelete();
            }
        }

        public static IEnumerable<T> ExcludeDeleted<T>(this IEnumerable<T> items) where T : ISoftDeleteable
            => items.Where(u => !u.IsDeleted);
    }
}