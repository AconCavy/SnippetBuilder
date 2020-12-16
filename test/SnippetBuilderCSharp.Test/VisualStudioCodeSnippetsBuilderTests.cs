using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SnippetBuilderCSharp.Test.Fakes;

namespace SnippetBuilderCSharp.Test
{
    public class VisualStudioCodeSnippetsBuilderTests
    {
        [Test]
        public void InitializeTest()
        {
            var baseRecipe = CreateRecipe();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            var fakeFileBroker = new FakeFileBroker();

            Assert.Throws<ArgumentNullException>(() =>
                _ = new VisualStudioCodeSnippetsBuilder(
                    new Recipe {Output = baseRecipe.Output, Paths = baseRecipe.Paths},
                    fakeFileStreamBroker,
                    fakeFileBroker));

            Assert.Throws<ArgumentNullException>(() =>
                _ = new VisualStudioCodeSnippetsBuilder(
                    new Recipe {Name = baseRecipe.Name, Paths = baseRecipe.Paths},
                    fakeFileStreamBroker,
                    fakeFileBroker));

            Assert.Throws<ArgumentNullException>(() =>
                _ = new VisualStudioCodeSnippetsBuilder(
                    new Recipe {Name = baseRecipe.Name, Output = baseRecipe.Output},
                    fakeFileStreamBroker,
                    fakeFileBroker));
        }

        [Test]
        public void BuildAsyncTest()
        {
            var recipe = CreateRecipe();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            var fakeFileBroker = new FakeFileBroker();

            var sut = new VisualStudioCodeSnippetsBuilder(recipe, fakeFileStreamBroker, fakeFileBroker);
            Assert.DoesNotThrowAsync(async () => await sut.BuildAsync());
        }

        [Test]
        public async Task BuildSnippetsAsyncTest()
        {
            var recipe = CreateRecipe();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            var fakeFileBroker = new FakeFileBroker();

            var sut = new VisualStudioCodeSnippetsBuilder(recipe, fakeFileStreamBroker, fakeFileBroker);

            var actual = (await sut.BuildSnippetsAsync()).First();
            const string expected = @"{
  ""HelloSample"": {
    ""scope"": ""csharp"",
    ""prefix"": [
      ""hellosample"",
      ""hs""
    ],
    ""body"": [
      ""public string Greet(string name)"",
      ""{"",
      ""    return $\u0022Hello {name}\u0022"",
      ""}""
    ]
  }
}";
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static Recipe CreateRecipe()
        {
            return new Recipe
            {
                Name = "HelloSample",
                Output = "./output",
                Paths = new[]
                {
                    "HelloSample.cs",
                    "directory"
                }
            };
        }
    }
}