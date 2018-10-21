using System;

namespace LyricsGame
{
    public class Counter
    {
        public int Count;

        public Counter()
        {
            Count = 0;
        }

        public void Increment()
        {
            Count++;
        }

        public void Reset()
        {
            Count = 0;
        }
    }
}

