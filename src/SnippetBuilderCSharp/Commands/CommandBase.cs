using System.Collections.Generic;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    public abstract class CommandBase : ICommand
    {
        public string Parameter => Params.First();
        public IEnumerable<string> Parameters => Params;
        protected readonly List<string> Params;

        protected CommandBase()
        {
            Params = new List<string>();
        }

        public void Add(string parameter)
        {
            Params.Add(parameter);
        }

        public abstract bool Validate();
    }
}