using System;
using System.Collections.Generic;

namespace ZenDoodles
{
    class LineSection
    {
        private const float FillLengthCuttoff = 15;

        private readonly List<LineSegment> _lineSegments;

        public LineSection(List<DVector2> vertices)
        {
            _lineSegments = new List<LineSegment>();

            for (int i = 0; i < vertices.Count - 1; i++)
            {
                _lineSegments.Add(new LineSegment
                {
                    Start = vertices[i],
                    End = vertices[i + 1]
                });
            }

            _lineSegments.Add(new LineSegment
            {
                Start = vertices[vertices.Count - 1],
                End = vertices[0]
            });
        }

        /// <summary>
        /// Fills the given set of vertices with lines according to the zen doodle rules
        /// </summary>
        public List<LineSegment> Fill()
        {
            int currentIndex = 0;
            double localLineDistance = double.PositiveInfinity;

            DVector2 lastEndPoint = _lineSegments[currentIndex].Start;

            // Stop filling lines once the line segment distance drops below the threshold
            while (localLineDistance > FillLengthCuttoff && currentIndex < _lineSegments.Count)
            {
                LineSegment currentSegment = _lineSegments[currentIndex];

                double startingAngle = currentSegment.Angle() + Math.PI * 0.014;

                DVector2 endPoint = DVector2.FromAngle(startingAngle) * currentSegment.Length() * 2;

                var fullSegment = new LineSegment
                {
                    Start = lastEndPoint,
                    End = endPoint + currentSegment.Start
                };

                for (int i = currentIndex + 1; i < _lineSegments.Count; i++)
                {
                    DVector2 intersection;

                    if (LineSegment.Intersects(fullSegment, _lineSegments[i], out intersection))
                    {
                        var realSegment = new LineSegment
                        {
                            Start = lastEndPoint,
                            End = intersection
                        };

                        _lineSegments.Add(realSegment);

                        lastEndPoint = intersection;
                        break;
                    }
                }

                localLineDistance = currentSegment.Length();
                int localSamples = Math.Min(3, currentIndex);

                // Check the local line distance for the past few segements
                for (int i = 1; i < localSamples; i++)
                {
                    localLineDistance += _lineSegments[currentIndex - i].Length();
                }

                // Take the average
                localLineDistance /= (localSamples + 1);

                currentIndex++;
            }

            return _lineSegments;
        }
    }
}
