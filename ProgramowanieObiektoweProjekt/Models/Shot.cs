using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramowanieObiektoweProjekt.Models
{
    internal class Shot
    {
        public int X { get; }
        public int Y { get; }
        public bool IsHit { get; }

        public Shot(int x, int y, bool isHit)
        {
            X = x;
            Y = y;
            IsHit = isHit;
        }
    }
}
