namespace ConsoleDraw
{
    public struct Point
    {
        private int x;
        private int y;

        public int X => x;
        public int Y => y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

#if NET35 || NET40 || NET452 || NETSTANDARD2_0
        public static implicit operator System.Drawing.Point(Point c) => new System.Drawing.Point(c.x, c.y);

        public static implicit operator Point(System.Drawing.Point c) => new Point(c.X, c.Y);
#endif
    }
}