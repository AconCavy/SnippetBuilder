using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SnippetBuilderCSharp.IO;
using SnippetBuilderCSharp.Models;
using SnippetBuilderCSharp.Snippets;
using SnippetBuilderCSharp.Test.Fakes;

namespace SnippetBuilderCSharp.Test.Snippets
{
    public class VisualStudioCodeSnippetTests
    {
        [Test]
        public void InitializeTest()
        {
            var mockFileBroker = new Mock<IFileBroker>().Object;
            var mockFileStreamBroker = new Mock<IFileStreamBroker>().Object;

            Assert.DoesNotThrow(() => _ = new VisualStudioCodeSnippet(mockFileBroker, mockFileStreamBroker));
        }

        [Test]
        public void BuildAsyncWithCreateDirectoryTest()
        {
            var recipe = CreateRecipe();
            var mockFileBroker = new Mock<IFileBroker>();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            mockFileBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
            mockFileBroker.Setup(x => x.ExistsDirectory(It.IsAny<string>())).Returns(false);

            var sut = new VisualStudioCodeSnippet(mockFileBroker.Object, fakeFileStreamBroker);

            Assert.DoesNotThrow(() => sut.BuildAsync(recipe));
            mockFileBroker.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BuildAsyncByRecipeTest()
        {
            var recipe = CreateRecipe();
            var fakeFileBroker = new FakeFileBroker();
            var fakeFileStreamBroker = new FakeFileStreamBroker();

            var sut = new VisualStudioCodeSnippet(fakeFileBroker, fakeFileStreamBroker);
            Assert.DoesNotThrowAsync(async () => await sut.BuildAsync(recipe));
        }

        [Test]
        public async Task BuildAsyncByPathsTest()
        {
            var recipe = CreateRecipe();
            var fakeFileBroker = new FakeFileBroker();
            var fakeFileStreamBroker = new FakeFileStreamBroker();

            var sut = new VisualStudioCodeSnippet(fakeFileBroker, fakeFileStreamBroker);

            var actual = (await sut.BuildAsync(recipe.Paths!)).First();
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

        [Test]
        public void BuildAsyncThrowsExceptionTest()
        {
            var recipe = CreateRecipe();
            var mockFileBroker = new Mock<IFileBroker>().Object;
            var mockFileStreamBroker = new Mock<IFileStreamBroker>().Object;
            var sut = new VisualStudioCodeSnippet(mockFileBroker, mockFileStreamBroker);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.BuildAsync(new Recipe {Output = recipe.Output, Paths = recipe.Paths}));
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.BuildAsync(new Recipe {Name = recipe.Name, Paths = recipe.Paths}));
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.BuildAsync(new Recipe {Name = recipe.Name, Output = recipe.Output}));
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