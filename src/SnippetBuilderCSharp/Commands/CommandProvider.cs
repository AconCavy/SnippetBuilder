using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    public class CommandProvider
    {
        private readonly List<CommandBase> _commands;

        public CommandProvider()
        {
            _commands = new List<CommandBase>();
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
                            command.Add(queue.Dequeue());
                }
            }
        }

        public void Register<T>(T command) where T : CommandBase
        {
            _commands.Add(command);
        }

        public T? Resolve<T>() where T : CommandBase
        {
            var commands = _commands.OfType<T>().ToArray();
            return commands.Any() ? commands[0] : null;
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