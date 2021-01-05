using NUnit.Framework;
using SnippetBuilder.Commands;
using SnippetBuilder.Models;

namespace SnippetBuilder.Test.Commands
{
    public class ExtensionsCommandTests
    {
        [Test]
        public void ValidateTest()
        {
            var sut = new ExtensionsCommand();
            var actual = sut.Validate();
            Assert.That(actual, Is.True);
        }

        [Test]
        public void ApplyToTest()
        {
            const string expected = ".cs";
            var sut = new ExtensionsCommand();
            sut.Append(expected);
            var recipe = new Recipe();
            sut.ApplyTo(recipe);

            var actual = recipe.Extensions;

            Assert.That(actual, Is.EqualTo(new[] {expected}));
        }
    }
}