using NUnit.Framework;
using SnippetBuilder.Extensions;
using SnippetBuilder.Models;

namespace SnippetBuilder.Test.Extensions
{
    public class RecipeExtensionsTests
    {
        [Test]
        public void ValidationIsTrueTest()
        {
            var sut = CreateRecipe();
            Assert.That(sut.Validate(), Is.True);
        }

        [Test]
        public void ValidationIsFalseByNameTest()
        {
            var baseRecipe = CreateRecipe();
            var sut = new Recipe {Output = baseRecipe.Output, Paths = baseRecipe.Paths};
            Assert.That(sut.Validate(), Is.False);

            sut = new Recipe {Name = string.Empty, Output = baseRecipe.Output, Paths = baseRecipe.Paths};
            Assert.That(sut.Validate(), Is.False);
        }

        [Test]
        public void ValidationIsFalseByOutputTest()
        {
            var baseRecipe = CreateRecipe();
            var sut = new Recipe {Name = baseRecipe.Name, Paths = baseRecipe.Paths};
            Assert.That(sut.Validate(), Is.False);

            sut = new Recipe {Name = baseRecipe.Name, Output = string.Empty, Paths = baseRecipe.Paths};
            Assert.That(sut.Validate(), Is.False);
        }

        [Test]
        public void ValidationIsFalseByPathsTest()
        {
            var baseRecipe = CreateRecipe();
            var sut = new Recipe {Name = baseRecipe.Name, Output = baseRecipe.Output};
            Assert.That(sut.Validate(), Is.False);

            sut = new Recipe {Name = baseRecipe.Name, Output = baseRecipe.Output, Paths = new[] {string.Empty}};
            Assert.That(sut.Validate(), Is.False);
        }

        private static Recipe CreateRecipe() =>
            new Recipe {Name = "HelloSample", Output = "./output", Paths = new[] {"HelloSample.cs", "directory"}};
    }
}