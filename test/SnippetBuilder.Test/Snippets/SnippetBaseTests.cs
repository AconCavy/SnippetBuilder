using System;
using Moq;
using NUnit.Framework;
using SnippetBuilder.IO;
using SnippetBuilder.Models;
using SnippetBuilder.Snippets;
using SnippetBuilder.Test.Fakes;

namespace SnippetBuilder.Test.Snippets;

public class SnippetBaseTests
{
    [Test]
    public void BuildAsyncWithCreateDirectoryTest()
    {
        var recipe = CreateRecipe();
        var mockFileProvider = new Mock<IFileProvider>();
        var fakeFileStreamProvider = new FakeFileStreamProvider();
        mockFileProvider.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
        mockFileProvider.Setup(x => x.ExistsDirectory(It.IsAny<string>())).Returns(false);

        SnippetBase sut = new VisualStudioCodeSnippet((IFileStreamProvider)mockFileProvider.Object, (IFileProvider)fakeFileStreamProvider);

        Assert.DoesNotThrow(() => sut.BuildAsync(recipe));
        mockFileProvider.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void BuildAsyncByRecipeTest()
    {
        var recipe = CreateRecipe();
        recipe.Extensions = null;
        var fakeFileProvider = new FakeFileProvider();
        var fakeFileStreamProvider = new FakeFileStreamProvider();

        SnippetBase sut = new VisualStudioCodeSnippet((IFileStreamProvider)fakeFileProvider, (IFileProvider)fakeFileStreamProvider);
        Assert.DoesNotThrowAsync(async () => await sut.BuildAsync(recipe));
    }

    [Test]
    public void BuildAsyncByExtensionsTest()
    {
        var recipe = CreateRecipe();
        var mockFileProvider = new Mock<IFileProvider>();
        var fakeFileStreamProvider = new FakeFileStreamProvider();
        mockFileProvider.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(false);
        mockFileProvider.Setup(x => x.ExistsDirectory(It.IsAny<string>())).Returns(true);

        SnippetBase sut = new VisualStudioCodeSnippet((IFileStreamProvider)mockFileProvider.Object, (IFileProvider)fakeFileStreamProvider);

        Assert.DoesNotThrow(() => sut.BuildAsync(recipe));
    }

    [Test]
    public void BuildAsyncThrowsExceptionTest()
    {
        var recipe = CreateRecipe();
        var mockFileProvider = new Mock<IFileProvider>().Object;
        var mockFileStreamProvider = new Mock<IFileStreamProvider>().Object;
        SnippetBase sut = new VisualStudioCodeSnippet((IFileStreamProvider)mockFileProvider, (IFileProvider)mockFileStreamProvider);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await sut.BuildAsync(new Recipe { Output = recipe.Output, Input = recipe.Input }));
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await sut.BuildAsync(new Recipe { Name = recipe.Name, Input = recipe.Input }));
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await sut.BuildAsync(new Recipe { Name = recipe.Name, Output = recipe.Output }));
    }

    private static Recipe CreateRecipe() =>
        new Recipe
        {
            Name = "HelloSample",
            Output = "./output",
            Input = new[] { "HelloSample.cs", "directory" },
            Extensions = new[] { ".cs" }
        };
}