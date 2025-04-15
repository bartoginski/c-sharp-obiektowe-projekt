using Spectre.Console;

namespace StatisticTableNamespace
{
    public class StatisticTable
    {
        private Table table;

        public StatisticTable()
        {
            table = new Table();

            
            

            table.AddColumn(new TableColumn("Statistic Table").Centered());
            table.AddRow("Enemy: A10 MISS!!");
            table.AddRow("+-------------------+");
            table.AddRow("You: B5 GOOD SHOT!!");
            table.AddRow("+-------------------+");
            table.AddRow("Enemy: C4 MISS!!");
            table.AddRow("+-------------------+");
            table.AddRow("Enemy: A10 MISS!!");
            table.AddRow("+-------------------+");
            table.AddRow("You: B5 GOOD SHOT!!");
            table.AddRow("+-------------------+");
            table.AddRow("Enemy: C4 MISS!!");
            table.AddRow("+-------------------+");
            table.AddRow("Enemy: A10 MISS!!");
            table.AddRow("+-------------------+");
            table.AddRow("You: B5 GOOD SHOT!!");
            table.AddRow("+-------------------+");
            table.AddRow("Enemy: C4 MISS!!");
            table.AddRow("+-------------------+");
            table.AddRow("Enemy: C4 MISS!!");
        }
        
        public void GetStat(string stat)
        {
            table.AddRow(stat);
        }

        public Table GetTable()
        {
            return table;
        }
    }
}
