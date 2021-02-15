using System;
using Moq;
using NUnit.Framework;
using SnippetBuilder.IO;
using SnippetBuilder.Models;
using SnippetBuilder.Snippets;
using SnippetBuilder.Test.Fakes;

namespace SnippetBuilder.Test.Snippets
{
    public class SnippetBaseTests
    {
        [Test]
        public void BuildAsyncWithCreateDirectoryTest()
        {
            var recipe = CreateRecipe();
            var mockFileBroker = new Mock<IFileBroker>();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            mockFileBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
            mockFileBroker.Setup(x => x.ExistsDirectory(It.IsAny<string>())).Returns(false);

            SnippetBase sut = new VisualStudioCodeSnippet(mockFileBroker.Object, fakeFileStreamBroker);

            Assert.DoesNotThrow(() => sut.BuildAsync(recipe));
            mockFileBroker.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BuildAsyncByRecipeTest()
        {
            var recipe = CreateRecipe();
            recipe.Extensions = null;
            var fakeFileBroker = new FakeFileBroker();
            var fakeFileStreamBroker = new FakeFileStreamBroker();

            SnippetBase sut = new VisualStudioCodeSnippet(fakeFileBroker, fakeFileStreamBroker);
            Assert.DoesNotThrowAsync(async () => await sut.BuildAsync(recipe));
        }

        [Test]
        public void BuildAsyncByExtensionsTest()
        {
            var recipe = CreateRecipe();
            var mockFileBroker = new Mock<IFileBroker>();
            var fakeFileStreamBroker = new FakeFileStreamBroker();
            mockFileBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(false);
            mockFileBroker.Setup(x => x.ExistsDirectory(It.IsAny<string>())).Returns(true);

            SnippetBase sut = new VisualStudioCodeSnippet(mockFileBroker.Object, fakeFileStreamBroker);

            Assert.DoesNotThrow(() => sut.BuildAsync(recipe));
        }

        [Test]
        public void BuildAsyncThrowsExceptionTest()
        {
            var recipe = CreateRecipe();
            var mockFileBroker = new Mock<IFileBroker>().Object;
            var mockFileStreamBroker = new Mock<IFileStreamBroker>().Object;
            SnippetBase sut = new VisualStudioCodeSnippet(mockFileBroker, mockFileStreamBroker);

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
}