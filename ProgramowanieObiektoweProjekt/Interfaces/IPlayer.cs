using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramowanieObiektoweProjekt.Models;

namespace ProgramowanieObiektoweProjekt.Interfaces
{
    internal interface IPlayer
    {
        string Name { get; }
        IBoard Board { get; }
        List<Shot> ShotsFired { get; }
    }
}
