using System;

namespace ConsoleDraw.UI
{
    public class ConsoleKeyPressEventArgs : EventArgs
    {
        public ConsoleKeyPressEventArgs(ConsoleKeyInfo inf)
        {
            KeyInfo = inf;
        }

        public ConsoleKeyInfo KeyInfo {get;set;}
    }
}