using BoardNamespace;
using StatisticTableNamespace;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        var board1 = new Board();
        var board2 = new Board();
        var statistic = new StatisticTable();

        // Wyświetlanie każdej tabeli osobno
        AnsiConsole.Write(board1.GetTable());
        AnsiConsole.Write(board2.GetTable());
        AnsiConsole.Write(statistic.GetTable());
    }
}