using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobraMono.Performance
{
    public class FrameCounter
    {
        public FrameCounter(int maximumSamples = 100)
        {
            this.maximumSamples = maximumSamples;
        }
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }
        public int maximumSamples { get; }
        private readonly Queue<float> _sampleBuffer = new Queue<float>();
        public void Update(float deltaTimeSeconds)
        {
            CurrentFramesPerSecond = 1.0f / deltaTimeSeconds;
            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > this.maximumSamples)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTimeSeconds;
        }
        public void Update(TimeSpan t)
        {
            Update((float) t.TotalSeconds);
        }
    }
}
