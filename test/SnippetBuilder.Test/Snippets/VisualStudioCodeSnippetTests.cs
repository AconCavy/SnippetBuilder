using Moq;
using NUnit.Framework;
using SnippetBuilder.IO;
using SnippetBuilder.Models;
using SnippetBuilder.Snippets;
using SnippetBuilder.Test.Fakes;

namespace SnippetBuilder.Test.Snippets;

public class VisualStudioCodeSnippetTests
{
    [Test]
    public void InitializeTest()
    {
        var mockFileProvider = new Mock<IFileProvider>().Object;
        var mockFileStreamProvider = new Mock<IFileStreamProvider>().Object;

        Assert.DoesNotThrow(() => _ = new VisualStudioCodeSnippet(mockFileStreamProvider, mockFileProvider));
    }

    [Test]
    public async Task BuildAsyncByPathsTest()
    {
        var recipe = new Recipe
        {
            Name = "HelloSample",
            Output = "./output",
            Input = new[] { "HelloSample.cs", "directory" }
        };
        var fakeFileProvider = new FakeFileProvider();
        var fakeFileStreamProvider = new FakeFileStreamProvider();

        var sut = new VisualStudioCodeSnippet(fakeFileStreamProvider, fakeFileProvider);

        var actual = (await sut.BuildAsync(recipe.Input!)).First();
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