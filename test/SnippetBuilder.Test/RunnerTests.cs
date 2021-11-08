using Moq;
using NUnit.Framework;
using SnippetBuilder.IO;
using SnippetBuilder.Models;
using SnippetBuilder.Snippets;
using SnippetBuilder.Test.Utilities;

namespace SnippetBuilder.Test;

public class RunnerTests
{
    [Test]
    public void RunAsyncWithInputTest()
    {
        var mockSnippet = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        mockRecipeSerializer.Setup(x => x.DeserializeAsync(It.IsAny<string[]>()))
            .Returns(Array.Empty<Recipe>().ToAsyncEnumerable);

        var sut = new Runner(new[] { mockSnippet.Object }, mockRecipeSerializer.Object);

        const string line = "--input ./sample.cs";
        var args = line.Split(" ");

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void RunAsyncWithRecipeTest()
    {
        var mockSnippet = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        mockRecipeSerializer.Setup(x => x.DeserializeAsync(It.IsAny<string[]>()))
            .Returns((new[] { new Recipe { Input = new[] { "sample.cs" } } }).ToAsyncEnumerable);

        var sut = new Runner(new[] { mockSnippet.Object }, mockRecipeSerializer.Object);

        const string line = "--recipes sample.json";
        var args = line.Split(" ");

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
        mockRecipeSerializer.Verify(x => x.DeserializeAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Test]
    public void RunAsyncWithDuplicatedNameTest()
    {
        var recipes = new[]
        {
            new Recipe { Input = new[] { "sample1.cs" } }, new Recipe { Input = new[] { "sample2.cs" } }
        };
        var mockSnippet = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        mockRecipeSerializer.Setup(x => x.DeserializeAsync(It.IsAny<string[]>()))
            .Returns(recipes.ToAsyncEnumerable());

        var sut = new Runner(new[] { mockSnippet.Object }, mockRecipeSerializer.Object);

        const string line = "--recipes sample1.json sample2.json";
        var args = line.Split(" ");

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        mockRecipeSerializer.Verify(x => x.DeserializeAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Test]
    public void RunAsyncWithInputAndRecipeTest()
    {
        var mockSnippet = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        mockRecipeSerializer.Setup(x => x.DeserializeAsync(It.IsAny<string[]>()))
            .Returns((new[] { new Recipe { Input = new[] { "sample.cs" } } }).ToAsyncEnumerable);

        var sut = new Runner(new[] { mockSnippet.Object }, mockRecipeSerializer.Object);

        const string line = "--input ./sample.cs --recipes sample.json";
        var args = line.Split(" ");

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        mockRecipeSerializer.Verify(x => x.DeserializeAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Test]
    public void RunAsyncInteractTest()
    {
        var mockSnippet = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        mockRecipeSerializer.Setup(x => x.DeserializeAsync(It.IsAny<string[]>()))
            .Returns(Array.Empty<Recipe>().ToAsyncEnumerable);

        var sut = new Runner(new[] { mockSnippet.Object }, mockRecipeSerializer.Object);

        var args = Array.Empty<string>();
        const string input = @"./sample1.cs ./sample2.cs
./sample3.cs




";
        using var inputReader = new StringReader(input);
        Console.SetIn(inputReader);

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void RunAsyncWithMultipleSnippetTest()
    {
        var recipes = new[]
        {
            new Recipe { Input = new[] { "sample1.cs" } }, new Recipe { Input = new[] { "sample2.cs" } }
        };
        var mockSnippet1 = new Mock<ISnippet>();
        var mockSnippet2 = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        mockRecipeSerializer.Setup(x => x.DeserializeAsync(It.IsAny<string[]>()))
            .Returns(recipes.ToAsyncEnumerable());

        var sut = new Runner(new[] { mockSnippet1.Object, mockSnippet2.Object }, mockRecipeSerializer.Object);

        const string line = "--recipes sample1.json sample2.json";
        var args = line.Split(" ");

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet1.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        mockSnippet2.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public void RunAsyncWithInvalidArgsTest()
    {
        var mockSnippet = new Mock<ISnippet>();
        var mockRecipeSerializer = new Mock<IRecipeSerializer>();
        var sut = new Runner(new[] { mockSnippet.Object }, mockRecipeSerializer.Object);

        const string line = "--foo bar";
        var args = line.Split(" ");

        Assert.DoesNotThrowAsync(() => sut.RunAsync(args));
        mockSnippet.Verify(x => x.BuildAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}