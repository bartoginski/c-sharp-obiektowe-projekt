using ProgramowanieObiektoweProjekt.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramowanieObiektoweProjekt.Models.Ships
{
    /// <summary>
    /// Abstract base class for ships.
    /// Contains common properties and methods for different ship types.
    /// </summary>
    abstract class ShipBase : IShip
    {
        public int Length { get; protected set; }

        /// <summary>
        /// The number of hits the ship has received.
        /// </summary>
        protected int Hits;

        /// <summary>
        /// Indicates whether the ship was sunk (hit in all segments).
        /// </summary>
        public bool IsSunk => Hits >= Length;

        /// <summary>
        /// Records a ship hit, incrementing the hit counter.
        /// </summary>
        public virtual void Hit()
        {
            Hits++;
        }
    }
}
