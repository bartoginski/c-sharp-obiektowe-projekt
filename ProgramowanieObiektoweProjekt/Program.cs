using ProgramowanieObiektoweProjekt.Models.Menu;

namespace ProgramowanieObiektoweProjekt
{
    class Program
    {
        static void Main(string[] args)
        {
            // Pętla główna programu, która zapewnia powrót do menu
            while (true)
            {
                Console.Clear(); // Czyszczenie konsoli przed wyświetleniem menu
                Menu.MenuDisplay();
            }
        }
    }
}
