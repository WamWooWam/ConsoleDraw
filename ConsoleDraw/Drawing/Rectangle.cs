namespace ConsoleDraw
{
    public struct Rectangle
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public int X => x;
        public int Y => y;
        public int Width => width;
        public int Height => height;

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }


#if NET35 || NET40 || NET452 || NETSTANDARD2_0
        public static implicit operator System.Drawing.Rectangle(Rectangle c) => new System.Drawing.Rectangle(c.x, c.y, c.width, c.height);

        public static implicit operator Rectangle(System.Drawing.Rectangle c) => new Rectangle(c.X, c.Y, c.Width, c.Height);
#endif
    }
}