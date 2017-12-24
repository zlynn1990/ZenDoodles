using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZenDoodles
{
    class LineRenderer
    {
        public double LineSpeed { get; private set; }

        private Pen _linePen;

        private List<LineSegment> _segments;
        private Queue<LineSegment> _segmentsToCache;

        private int _currentIndex;
        private double _currentDrawDistance;

        public LineRenderer(double lineSpeed, List<LineSegment> segments)
        {
            LineSpeed = lineSpeed;

            _segments = segments;
            _linePen = new Pen(Color.Black, 2.0f);

            _segmentsToCache = new Queue<LineSegment>();
        }

        public void Reset()
        {
            _currentIndex = 0;
            _currentDrawDistance = 0;
        }

        public void Cache(Graphics graphics)
        {
            while (_segmentsToCache.Count > 0)
            {
                LineSegment segment = _segmentsToCache.Dequeue();

                graphics.DrawLine(_linePen, (float)segment.Start.X, (float)segment.Start.Y,
                                            (float)segment.End.X, (float)segment.End.Y);
            }
        }

        public void Draw(Graphics graphics, double elapsedTime)
        {
            if (_currentIndex == _segments.Count)
            {
                return;
            }

            LineSegment currentSegment = _segments[_currentIndex];

            double segmentLength = currentSegment.Length();

            if (segmentLength > 0)
            {
                double currentLinePercent = Math.Min(_currentDrawDistance / segmentLength, 1);

                DVector2 slope = currentSegment.End - currentSegment.Start;

                DVector2 drawEnd = currentSegment.Start + slope * currentLinePercent;

                graphics.DrawLine(_linePen, (float)currentSegment.Start.X, (float)currentSegment.Start.Y,
                                            (float)drawEnd.X, (float)drawEnd.Y);

                graphics.FillEllipse(new SolidBrush(Color.Black), (float)drawEnd.X - 5f, (float)drawEnd.Y - 5f, 10f, 10f);

                _currentDrawDistance += LineSpeed * elapsedTime;

                if (_currentDrawDistance >= segmentLength)
                {
                    _currentDrawDistance = 0;
                    _currentIndex++;

                    _segmentsToCache.Enqueue(currentSegment);
                }
            }
            else
            {
                _currentDrawDistance = 0;
                _currentIndex++;
            }
        }
    }
}
