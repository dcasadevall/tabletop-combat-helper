namespace Grid
{
    public class GridCoordinates : IGridCoordinates
    {
        public GridCoordinates()
        {
            X = 0;
            Y = 0;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }
}