
namespace gui.Model.Utils
{
    /// <summary>
    /// Provides utility functions for working with lists.
    /// </summary>
    public static class ListUtils
    {
        /// <summary>
        /// Swaps two elements in a list at the given indices.
        /// </summary>
        public static void Swap<T>(List<T> list, int i, int j)
        {
            if (i != j && i >= 0 && j >= 0 && i < list.Count && j < list.Count)
                (list[i], list[j]) = (list[j], list[i]);
        }

        public static List<T> EnumToList<TEnum, T>(Func<TEnum, T> initializer)
            where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                       .Select(initializer)
                       .ToList();
        }

    }
}
