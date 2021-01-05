using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SnippetBuilder.Commands;
using SnippetBuilder.IO;

namespace SnippetBuilder.Test.Commands
{
    public class CommandProviderTests
    {
        [Test]
        public void RegisterCommandTest()
        {
            var sut = new CommandProvider();
            var mockCommand = new Mock<ICommand>().Object;
            Assert.DoesNotThrow(() => sut.RegisterCommand(mockCommand));
        }

        [Test]
        public void ResolveCommandTest()
        {
            var sut = new CommandProvider();
            var expected = new NameCommand();
            sut.RegisterCommand(expected);

            var actual = sut.ResolveCommand<NameCommand>();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ResolveCommandMultipleResultTest1()
        {
            var sut = new CommandProvider();
            var expected = new NameCommand();
            var command1 = new NameCommand();
            var command2 = new OutputCommand();
            sut.RegisterCommand(expected);
            sut.RegisterCommand(command1);
            sut.RegisterCommand(command2);

            var actual = sut.ResolveCommand<NameCommand>();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ResolveCommandNoResultTest1()
        {
            var sut = new CommandProvider();
            Assert.Throws<InvalidOperationException>(() => sut.ResolveCommand<NameCommand>());
        }

        [Test]
        public void ResolveCommandsTest()
        {
            var sut = new CommandProvider();
            var command1 = new NameCommand();
            var command2 = new NameCommand();
            var command3 = new OutputCommand();
            sut.RegisterCommand(command1);
            sut.RegisterCommand(command2);
            sut.RegisterCommand(command3);

            var actual = sut.ResolveCommands<NameCommand>();
            Assert.That(actual, Is.EqualTo(new[] {command1, command2}));
        }

        [Test]
        public void TryResolveCommandTest()
        {
            var sut = new CommandProvider();
            var expected = new NameCommand();
            sut.RegisterCommand(expected);

            var result = sut.TryResolveCommand<NameCommand>(out var actual);
            Assert.That(result, Is.True);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TryResolveCommandNoResultTest()
        {
            var sut = new CommandProvider();

            var result = sut.TryResolveCommand<NameCommand>(out var actual);
            Assert.That(result, Is.False);
            Assert.That(actual, Is.EqualTo(default));
        }

        [Test]
        public void TryResolveCommandNoResultWithRecipeCommandTest()
        {
            var sut = new CommandProvider();

            var result = sut.TryResolveCommand<RecipeCommand>(out var actual);
            Assert.That(result, Is.False);
            Assert.That(actual, Is.EqualTo(default));
        }

        [Test]
        public void BuildWithNoArgsTest()
        {
            var sut = new CommandProvider();
            Assert.DoesNotThrow(() => sut.Build(null));
            Assert.DoesNotThrow(() => sut.Build(Array.Empty<string>()));
        }

        [Test]
        public void BuildCombinationTest()
        {
            var options = new[]
            {
                "-n sample",
                "-o ./sample",
                "-p file1.cs directory/file2.cs directory/file3.cs",
                "-r recipe.json"
            };

            for (var i = 0; i < 1 << options.Length; i++)
            {
                var args = new List<string>();
                for (var j = 0; j < options.Length; j++)
                    if (((i >> j) & 1) == 1)
                        args.AddRange(options[j].Split(" "));

                var sut = CreateCommandProvider();
                sut.Build(args.ToArray());
                var commands = new ICommand[]
                {
                    sut.ResolveCommand<NameCommand>(),
                    sut.ResolveCommand<OutputCommand>(),
                    sut.ResolveCommand<PathsCommand>(),
                    sut.ResolveCommand<RecipeCommand>()
                };
                for (var j = 0; j < options.Length; j++)
                {
                    var expected = ((i >> j) & 1) == 1;
                    var actual = commands[j].Validate();
                    Assert.That(actual, Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void BuildWithHelpTest()
        {
            var sut = CreateCommandProvider();
            Assert.DoesNotThrow(() => sut.Build(new[] {"-h"}));
        }

        private static CommandProvider CreateCommandProvider()
        {
            var mokFileStreamBroker = new Mock<IFileStreamBroker>();
            var mockFilesBroker = new Mock<IFileBroker>();
            mockFilesBroker.Setup(x => x.ExistsFile(It.IsAny<string>())).Returns(true);
            var commandProvider = new CommandProvider();
            commandProvider.RegisterCommand(new NameCommand());
            commandProvider.RegisterCommand(new OutputCommand());
            commandProvider.RegisterCommand(new PathsCommand(mockFilesBroker.Object));
            commandProvider.RegisterCommand(new HelpCommand());
            commandProvider.RegisterCommand(new RecipeCommand(mokFileStreamBroker.Object, mockFilesBroker.Object));

            return commandProvider;
        }
    }
}