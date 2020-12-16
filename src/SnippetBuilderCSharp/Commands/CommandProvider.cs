using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    public class CommandProvider
    {
        private readonly List<ICommand> _commands;

        public CommandProvider()
        {
            _commands = new List<ICommand>();
        }

        public void Build(string[]? args)
        {
            var queue = new Queue<string>(args ?? Array.Empty<string>());
            while (queue.Any())
            {
                var arg = queue.Dequeue();
                if (!arg.StartsWith("-")) continue;
                foreach (var command in _commands)
                foreach (var attribute in Attribute.GetCustomAttributes(command.GetType()))
                {
                    if (!(attribute is CommandAttribute commandAttribute)) continue;
                    if (!commandAttribute.EqualsAny(arg)) continue;
                    if (command is HelpCommand)
                        ShowHelp();
                    else
                        while (queue.TryPeek(out var top) && !top.StartsWith("-"))
                            command.Append(queue.Dequeue());
                }
            }
        }

        public void RegisterCommand<T>(T command) where T : ICommand
        {
            _commands.Add(command);
        }

        public T ResolveCommand<T>() where T : ICommand
        {
            return _commands.OfType<T>().First();
        }

        public IEnumerable<T> ResolveCommands<T>() where T : ICommand
        {
            return _commands.OfType<T>();
        }

        public bool TryResolveCommand<T>(out T result) where T : ICommand
        {
            var commands = _commands.OfType<T>().ToArray();
            if (commands.Any())
            {
                result = commands.First();
                return true;
            }

            result = default!;
            return false;
        }

        private void ShowHelp()
        {
            foreach (var attribute in _commands.SelectMany(command => Attribute.GetCustomAttributes(command.GetType())))
            {
                if (!(attribute is CommandAttribute commandAttribute)) continue;
                Console.WriteLine(
                    $"-{commandAttribute.ShortName}|--{commandAttribute.LongName}: {commandAttribute.Description}");
            }
        }
    }
}