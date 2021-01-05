using System.Collections.Generic;
using System.Linq;

namespace SnippetBuilder.Commands
{
    public abstract class CommandBase : ICommand
    {
        public string Parameter => Params.FirstOrDefault() ?? string.Empty;
        public IEnumerable<string> Parameters => Params;
        protected readonly List<string> Params;

        protected CommandBase()
        {
            Params = new List<string>();
        }

        public abstract bool Validate();

        public void Append(string parameter)
        {
            Params.Add(parameter);
        }
    }
}