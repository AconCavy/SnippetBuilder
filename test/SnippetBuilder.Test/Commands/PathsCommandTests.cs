using NUnit.Framework;
using SnippetBuilder.Commands;
using SnippetBuilder.Models;
using SnippetBuilder.Test.Fakes;

namespace SnippetBuilder.Test.Commands
{
    public class PathsCommandTests
    {
        [Test]
        public void ValidateTest()
        {
            var sut = new PathsCommand(new FakeFileBroker());
            var actual = sut.Validate();
            Assert.That(actual, Is.False);

            sut.Append("Sample.cs");
            actual = sut.Validate();
            Assert.That(actual, Is.True);
        }

        [Test]
        public void ApplyToTest()
        {
            const string expected = "Sample.cs";
            var sut = new PathsCommand(new FakeFileBroker());
            sut.Append(expected);
            var recipe = new Recipe();
            sut.ApplyTo(recipe);

            var actual = recipe.Paths;

            Assert.That(actual, Is.EqualTo(new[] {expected}));
        }
    }
}