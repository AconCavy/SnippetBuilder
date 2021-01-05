using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SnippetBuilder.Commands;
using SnippetBuilder.IO;
using SnippetBuilder.Models;

namespace SnippetBuilder.Test.Commands
{
    public class RecipeCommandTest
    {
        [Test]
        public void ValidateTest1()
        {
            var mockFilesStreamBroker = new Mock<IFileStreamBroker>();
            var mockFilesBroker = new Mock<IFileBroker>();
            mockFilesBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(false);
            var sut = new RecipeCommand(mockFilesStreamBroker.Object, mockFilesBroker.Object);
            var actual = sut.Validate();
            Assert.That(actual, Is.False);
        }

        [Test]
        public void ValidateTest2()
        {
            var mockFilesStreamBroker = new Mock<IFileStreamBroker>();
            var mockFilesBroker = new Mock<IFileBroker>();
            mockFilesBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
            var sut = new RecipeCommand(mockFilesStreamBroker.Object, mockFilesBroker.Object);
            sut.Append("recipe.json");
            var actual = sut.Validate();
            Assert.That(actual, Is.True);
        }

        [Test]
        public async Task GetRecipesAsyncTest()
        {
            const string json =
                @"[{""name"": ""sample"", ""output"": ""./sample"", ""paths"": [""file1.cs"",""directory/file2.cs"",""directory/file3.cs""]}]";

            async IAsyncEnumerable<string> GetJson()
            {
                yield return json;
            }

            var expected = new Recipe
            {
                Name = "sample",
                Output = "./sample",
                Paths = new[]
                {
                    "file1.cs",
                    "directory/file2.cs",
                    "directory/file3.cs"
                }
            };


            var mockFilesStreamBroker = new Mock<IFileStreamBroker>();
            mockFilesStreamBroker.Setup(x => x.ReadLinesAsync(It.IsAny<string>())).Returns(GetJson);
            var mockFilesBroker = new Mock<IFileBroker>();
            mockFilesBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
            var sut = new RecipeCommand(mockFilesStreamBroker.Object, mockFilesBroker.Object);
            sut.Append("recipe.json");
            await foreach (var actual in sut.GetRecipesAsync())
            {
                Assert.That(actual.Name, Is.EqualTo(expected.Name));
                Assert.That(actual.Output, Is.EqualTo(expected.Output));
                Assert.That(actual.Paths, Is.EqualTo(expected.Paths));
            }
        }
    }
}