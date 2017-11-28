using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace AutoFixtureCollectionSpecimenBuilder
{
    public class ItemCollectionSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!IsApplicable(request))
            {
                return new NoSpecimen();
            }

            var items = new List<Item>(3);
            var week1 = context.Create<Week>();
            var week2 = context.Create<Week>();

            items.Add(CreateItem(context, week1));
            items.Add(CreateItem(context, week1));
            items.Add(CreateItem(context, week2));

            items.GroupBy(t => t.Week).ToList().ForEach(ConfigureNames);
            ConfigureSubItems(context, items);

            return items;
        }

        private static bool IsApplicable(object request)
        {
            bool IsManyItemsType(Type type) => typeof(IEnumerable<Item>).IsAssignableFrom(type);
            bool IsItemsType(Type type) => type != null && typeof(Item) == type;

            switch (request)
            {
                case PropertyInfo pInfo:
                    return IsManyItemsType(pInfo.PropertyType);
                case Type type:
                    return IsManyItemsType(type);
                case MultipleRequest multipleRequest:
                    if (!(multipleRequest.Request is SeededRequest seededRequest))
                    {
                        return false;
                    }
                    return IsItemsType(seededRequest.Request as Type);
                default:
                    return false;
            }
        }

        private static Item CreateItem(ISpecimenContext context, Week week)
        {
            var item = context.Create<Item>();
            item.Week = week;
            return item;
        }

        private static void ConfigureNames(IEnumerable<Item> items)
        {
            string name = null;
            foreach (var item in items)
            {
                if (name == null)
                {
                    name = item.Name;
                }
                else
                {
                    item.Name = name;
                }
            }
        }

        private static void ConfigureSubItems(ISpecimenContext context, IEnumerable<Item> items)
        {
            foreach (var group in items.GroupBy(item => item.Week.WeekId))
            {
                var subItemTemplates = context.CreateMany<SubItem>().ToList();
                foreach (var item in group)
                {
                    item.SubItems.Clear();
                    foreach (var subItem in context.CreateMany<SubItem>().Zip(subItemTemplates,
                        (model, subItem) =>
                        {
                            subItem.Item = item;
                            subItem.Name = model.Name;
                            subItem.SortOrder = model.SortOrder;
                            subItem.HelpText = model.HelpText;
                            return subItem;
                        }))
                    {
                        item.SubItems.Add(subItem);
                    }
                }
            }
        }
    }
}