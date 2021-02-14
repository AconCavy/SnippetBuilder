using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SnippetBuilder.IO;
using SnippetBuilder.Models;
using SnippetBuilder.Snippets;
using SnippetBuilder.Test.Fakes;

namespace SnippetBuilder.Test.Snippets
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
        public async Task BuildAsyncByPathsTest()
        {
            var recipe = new Recipe
            {
                Name = "HelloSample",
                Output = "./output",
                Paths = new[] { "HelloSample.cs", "directory" }
            };
            var fakeFileBroker = new FakeFileBroker();
            var fakeFileStreamBroker = new FakeFileStreamBroker();

            var sut = new VisualStudioCodeSnippet(fakeFileBroker, fakeFileStreamBroker);

            var actual = (await sut.BuildAsync(recipe.Paths!)).First();
            const string expected = @"{
  ""HelloSample"": {
    ""prefix"": [
      ""hellosample"",
      ""hs""
    ],
    ""body"": [
      ""using System;"",
      ""public string Greet(string name)"",
      ""{"",
      ""    return $\u0022Hello {name}\u0022"",
      ""}""
    ]
  }
}";
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}