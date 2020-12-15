using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "name", ShortName = "n", Description = "Output file name")]
    public class NameCommand : CommandBase
    {
        public override bool Validate()
        {
            return Parameters.Any();
        }
    }
}