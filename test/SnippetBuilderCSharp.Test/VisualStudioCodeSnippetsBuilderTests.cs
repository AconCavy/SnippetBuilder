using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SnippetBuilderCSharp.IO;
using SnippetBuilderCSharp.Models;
using SnippetBuilderCSharp.Snippets;
using SnippetBuilderCSharp.Test.Fakes;

namespace SnippetBuilderCSharp.Test
{
    public class VisualStudioCodeSnippetsBuilderTests
    {
        [Test]
        public void InitializeTest()
        {
            var baseRecipe = CreateRecipe();
            var mockFileStreamBroker = new Mock<IFileStreamBroker>().Object;
            var mockFileBroker = new Mock<IFileBroker>().Object;

            Assert.Throws<ArgumentNullException>(() =>
                _ = new VisualStudioCodeSnippet(
                    new Recipe {Output = baseRecipe.Output, Paths = baseRecipe.Paths},
                    mockFileStreamBroker,
                    mockFileBroker));

            Assert.Throws<ArgumentNullException>(() =>
                _ = new VisualStudioCodeSnippet(
                    new Recipe {Name = baseRecipe.Name, Paths = baseRecipe.Paths},
                    mockFileStreamBroker,
                    mockFileBroker));

            Assert.Throws<ArgumentNullException>(() =>
                _ = new VisualStudioCodeSnippet(
                    new Recipe {Name = baseRecipe.Name, Output = baseRecipe.Output},
                    mockFileStreamBroker,
                    mockFileBroker));
        }

        [Test]
        public void InitializeWithCreateDirectoryTest()
        {
            var recipe = CreateRecipe();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            var mockFileBroker = new Mock<IFileBroker>();
            mockFileBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
            mockFileBroker.Setup(x => x.ExistsDirectory(It.IsAny<string>())).Returns(false);

            Assert.DoesNotThrow(() =>
                _ = new VisualStudioCodeSnippet(recipe, fakeFileStreamBroker, mockFileBroker.Object));
            mockFileBroker.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BuildAsyncTest()
        {
            var recipe = CreateRecipe();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            var fakeFileBroker = new FakeFileBroker();

            var sut = new VisualStudioCodeSnippet(recipe, fakeFileStreamBroker, fakeFileBroker);
            Assert.DoesNotThrowAsync(async () => await sut.BuildAsync());
        }

        [Test]
        public async Task BuildSnippetsAsyncTest()
        {
            var recipe = CreateRecipe();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            var fakeFileBroker = new FakeFileBroker();

            var sut = new VisualStudioCodeSnippet(recipe, fakeFileStreamBroker, fakeFileBroker);

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