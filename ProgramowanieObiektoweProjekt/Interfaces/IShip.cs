using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramowanieObiektoweProjekt.Interfaces
{
    /// <summary>
    /// Interface defining ships.
    /// </summary>
    internal interface IShip
    {
        /// <summary>
        /// Length of ship min 1 max 4
        /// </summary>
        int Length { get; }
        bool IsSunk { get; }
        void Hit();
    }
}
