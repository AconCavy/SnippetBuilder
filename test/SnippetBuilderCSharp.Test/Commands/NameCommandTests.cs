using System.Linq;
using NUnit.Framework;
using SnippetBuilderCSharp.Commands;

namespace SnippetBuilderCSharp.Test.Commands
{
    public class NameCommandTests
    {
        [Test]
        public void AppendTest()
        {
            var sut = new NameCommand();
            var actual = sut.Parameters.Count();
            Assert.That(actual, Is.Zero);

            sut.Append("Sample");
            actual = sut.Parameters.Count();
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void ParameterTest()
        {
            const string expected = "Sample";
            var sut = new NameCommand();
            var actual = sut.Parameter;
            Assert.That(actual, Is.EqualTo(string.Empty));

            sut.Append(expected);
            actual = sut.Parameter;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ValidateTest()
        {
            var sut = new NameCommand();
            var actual = sut.Validate();
            Assert.That(actual, Is.False);

            sut.Append("Sample");
            actual = sut.Validate();
            Assert.That(actual, Is.True);
        }

        [Test]
        public void ApplyToTest()
        {
            const string expected = "Sample";
            var sut = new NameCommand();
            sut.Append(expected);
            var recipe = new Recipe();
            sut.ApplyTo(recipe);

            var actual = recipe.Name;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}