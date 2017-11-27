using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace AutoFixtureCollectionSpecimenBuilder
{
    [TestFixture]
    public class AutoFixtureSpecimenBuilderTests
    {
        private static void TestCreationOfTasks(Func<IFixture, ICollection<Item>> creator)
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new ItemCollectionSpecimenBuilder());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var tasks = creator(fixture);

            Assert.AreEqual(3, tasks.Count);
            Assert.AreEqual(2, tasks.GroupBy(t => t.Week).Count());
            Assert.IsTrue(tasks.GroupBy(t => t.Week).Select(g => g.Select(t => t.Name).Distinct()).All(distinctNames => distinctNames.Count() == 1));
            var task = tasks.GroupBy(t => t.Week).OrderBy(g => g.Count()).First().OrderBy(t => t.ItemId).First();

        }

        [Test]
        public void CreateMany() => TestCreationOfTasks(fixture => fixture.CreateMany<Item>().ToList());

        [Test]
        public void CreateWithProperty() => TestCreationOfTasks(fixture => fixture.Create<TodoList>().Tasks);

        [Test]
        public void CreateAsList() => TestCreationOfTasks(fixture => fixture.Create<IList<Item>>());
    }
}
