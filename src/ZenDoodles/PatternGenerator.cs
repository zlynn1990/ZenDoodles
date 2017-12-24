using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZenDoodles
{
    static class PatternGenerator
    {
        public static List<LineSection> Simple(int width, int height)
        {
            return new List<LineSection>
            {
                new LineSection(new List<DVector2>
                {
                    DVector2.Zero,
                    new DVector2(width * 0.23, 0),
                    new DVector2(width * 0.76, height * 0.4),
                    new DVector2(0, height * 0.66)
                }),
                new LineSection(new List<DVector2>
                {
                    new DVector2(width * 0.76, height * 0.4),
                    new DVector2(width * 0.58, height),
                    new DVector2(0, height),
                    new DVector2(0, height * 0.66)
                }),
                new LineSection(new List<DVector2>
                {
                    new DVector2(width * 0.76, height * 0.4),
                    new DVector2(width * 0.23, 0),
                    new DVector2(width, 0),
                    new DVector2(width, height * 0.21),
                }),
                new LineSection(new List<DVector2>
                {
                    new DVector2(width * 0.58, height),
                    new DVector2(width * 0.76, height * 0.4),
                    new DVector2(width, height * 0.21),
                    new DVector2(width, height),
                })
            };
        }
    }
}
