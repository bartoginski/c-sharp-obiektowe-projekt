using BoardNamespace;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        var board1 = new Board();
        var board2 = new Board();

        var tabela = new Table();

        tabela.Border = TableBorder.None;


        tabela.AddColumn(new TableColumn(board1.GetTable()));
        tabela.AddColumn(new TableColumn(board2.GetTable()));

        AnsiConsole.Write(tabela);
    }
}