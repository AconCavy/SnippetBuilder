using System;

namespace SnippetBuilder.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string? LongName { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }

        public bool EqualsAny(string command)
        {
            if (command.StartsWith("--") &&
                string.Compare(command[2..], LongName, StringComparison.OrdinalIgnoreCase) == 0) return true;
            if (command.StartsWith("-") &&
                string.Compare(command[1..], ShortName, StringComparison.OrdinalIgnoreCase) == 0) return true;
            return false;
        }
    }
}