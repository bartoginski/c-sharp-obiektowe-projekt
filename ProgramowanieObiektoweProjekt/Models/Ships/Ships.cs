namespace ProgramowanieObiektoweProjekt.Models.Ships
{
    internal class BattleShip : ShipBase
    {
        public BattleShip()
        {
            Name = "Battleship";
            Length = 4;
            NumberOfShips = 1;
        }
    }
    internal class Cruiser : ShipBase
    {
        public Cruiser()
        {
            Name = "Cruiser";
            Length = 3;
            NumberOfShips = 2;
        }
    }
    internal class Destroyer : ShipBase
    {
        public Destroyer()
        {
            Name = "Destroyer";
            Length = 2;
            NumberOfShips = 3;
        }
    }
    internal class Submarine : ShipBase
    {
        public Submarine()
        {
            Name = "Submarine";
            Length = 1;
            NumberOfShips = 4;
        }
    }
}
