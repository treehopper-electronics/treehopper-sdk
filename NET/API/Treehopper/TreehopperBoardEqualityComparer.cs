namespace Treehopper
{
    using System.Collections.Generic;

    /// <summary>
    /// This comparer is used to compare Treehopper boards
    /// </summary>
    public class TreehopperBoardEqualityComparer : EqualityComparer<TreehopperUsb>
    {
        /// <summary>
        /// Determines if two treehopper boards are the same
        /// </summary>
        /// <param name="x">The first board to compare</param>
        /// <param name="y">The second board to compare</param>
        /// <returns>Whether the boards are equal</returns>
        public override bool Equals(TreehopperUsb x, TreehopperUsb y)
        {
            if (x == null || y == null)
                return false;
            return x.ToString() == y.ToString();
        }

        /// <summary>
        /// Returns the Hash of the Treehopper board
        /// </summary>
        /// <param name="obj">The Treehopper board to calculate the hash for</param>
        /// <returns>The calculated hash value</returns>
        public override int GetHashCode(TreehopperUsb obj)
        {
            if (obj != null)
                if (obj.Name != null)
                    return obj.ToString().GetHashCode();
                else
                    return 0;
            else
                return 0;
        }
    }
}
