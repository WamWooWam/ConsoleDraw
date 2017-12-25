namespace ConsoleDraw
{
    public class Rectangle
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public int X => x;
        public int Y => y;
        public int Width => width;
        public int Height => height;

        internal Rectangle() { }

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        
        public static implicit operator System.Drawing.Rectangle(Rectangle c) => new System.Drawing.Rectangle(c.x, c.y, c.width, c.height);
        public static implicit operator Rectangle(System.Drawing.Rectangle c) => new Rectangle(c.X, c.Y, c.Width, c.Height);
    }
}