using NUnit.Framework;
using SnippetBuilder.Commands;
using SnippetBuilder.Models;

namespace SnippetBuilder.Test.Commands
{
    public class OutputCommandTests
    {
        [Test]
        public void ValidateTest()
        {
            var sut = new OutputCommand();
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
            var sut = new OutputCommand();
            sut.Append(expected);
            var recipe = new Recipe();
            sut.ApplyTo(recipe);

            var actual = recipe.Output;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}