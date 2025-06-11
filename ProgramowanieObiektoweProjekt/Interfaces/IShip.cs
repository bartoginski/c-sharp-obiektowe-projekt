namespace ProgramowanieObiektoweProjekt.Interfaces
{
    internal interface IShip
    {
        int Length { get; }
        bool IsSunk { get; }
        void Hit();
    }
}
