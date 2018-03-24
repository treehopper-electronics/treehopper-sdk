using System.Collections.Generic;

namespace Treehopper
{
    /// <summary>
    ///     This comparer is used to compare Treehopper boards
    /// </summary>
    public class TreehopperBoardEqualityComparer : EqualityComparer<TreehopperUsb>
    {
        /// <summary>
        ///     Determines if two treehopper boards are the same
        /// </summary>
        /// <param name="x">The first board to compare</param>
        /// <param name="y">The second board to compare</param>
        /// <returns>Whether the boards are equal</returns>
        public override bool Equals(TreehopperUsb x, TreehopperUsb y)
        {
            if (x == null || y == null)
                return false;
            return x.Connection.DevicePath == y.Connection.DevicePath;
        }

        /// <summary>
        ///     Returns the Hash of the Treehopper board
        /// </summary>
        /// <param name="obj">The Treehopper board to calculate the hash for</param>
        /// <returns>The calculated hash value</returns>
        public override int GetHashCode(TreehopperUsb obj)
        {
            if (obj != null)
                if (obj.Name != null)
                    return obj.Connection.DevicePath.GetHashCode();
                else
                    return 0;
            return 0;
        }
    }
}