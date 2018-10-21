using System;

namespace LyricsGame
{
    public class Clock
    {
        private Counter _sec;
        private Counter _min;
        private Counter _hour;

        public string CurrentTime
        {
            get
            {
                string output = String
                    .Concat(_hour.Count.ToString(), ":", _min.Count.ToString(), ":", _sec.Count.ToString());

                return output;
            }
        }

        public Clock()
        {
            _sec = new Counter();
            _min = new Counter();
            _hour = new Counter();
        }

        public void Tick()
        {
            _sec.Increment();
            if (_sec.Count == 60)
            {
                _sec.Reset();
                _min.Increment();

                if (_min.Count == 60)
                {
                    _min.Reset();
                    _hour.Increment();
                }
            }
        }
    }
}

