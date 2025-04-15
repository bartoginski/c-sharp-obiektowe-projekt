using Spectre.Console;

namespace BoardNamespace
{
    public class Board
    {
        var table = new Table();
        table.ShowRowSeparators = true;

        table.AddColumns("", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J");

        var r1 = new (string, string)[]
        {
            ("empty", "1"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r2 = new (string, string)[]
        {
            ("empty", "2"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r3 = new (string, string)[]
        {
            ("empty", "3"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r4 = new (string, string)[]
        {
            ("empty", "4"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r5 = new (string, string)[]
        {
            ("empty", "5"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r6 = new (string, string)[]
        {
            ("empty", "6"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r7 = new (string, string)[]
        {
            ("empty", "7"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r8 = new (string, string)[]
        {
            ("empty", "8"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r9 = new (string, string)[]
        {
            ("empty", "9"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };
        var r10 = new (string, string)[]
        {
            ("empty", "10"),
            ("A", "~"),
            ("B", "~"),
            ("C", "~"),
            ("D", "~"),
            ("E", "~"),
            ("F", "~"),
            ("G", "~"),
            ("H", "~"),
            ("I", "~"),
            ("J", "~")
        };

        var rTuple = new (string, string)[][]
        {
            r1,r2,r3,r4,r5,r6,r7,r8,r9,r10
        };

        public Board() 
        {
            for (int i = 0; i < 10; i++)
            {
                string[] t2 = new string[11];
                for (int j = 0; j < 11; j++)
                {
                    t2[j] = rTuple[i][j].Item2;
                }
                table.AddRow(t2);
            }

            AnsiConsole.Write(table);
        }
    }
}