using System.Collections.Generic;

namespace AutoFixtureCollectionSpecimenBuilder
{
    public class TodoList
    {
        public ICollection<Item> Tasks { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public Week Week { get; set; }
        public ICollection<SubItem> SubItems { get; set; }
        public int ItemId { get; set; }
        public TodoList TodoList { get; set; }
    }

    public class SubItem
    {
        public Item Item { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string HelpText { get; set; }
    }

    public class Week
    {
        public int WeekId { get; set; }
    }
}