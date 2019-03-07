using UnityEngine;

namespace Grid
{
    /// <summary>
    /// A data structure that represents a single Cell in a 2D Grid using integers as x and y.
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// The position of the cell in the x axis of the grid.
        /// </summary>
        int X { get; set; }
        /// <summary>
        /// The position of the cell in the y axis of the grid.
        /// </summary>
        int Y { get; set; }
    }
}