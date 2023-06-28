namespace Random_File_Opener_Win_Forms
{
    public interface IDeletable
    {
        bool IsDeleted { get; set; }
    }

    public static class DeletableExtensions
    {
        public static void Delete(this IDeletable x)
        {
            x.IsDeleted = true;
        }
    }
}