namespace Grid
{
    public class Cell : ICell
    {
        public Cell()
        {
            X = 0;
            Y = 0;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }
}