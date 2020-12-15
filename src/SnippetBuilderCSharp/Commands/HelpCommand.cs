namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "help", ShortName = "h", Description = "Help of the application")]
    public class HelpCommand : CommandBase
    {
        public override bool Validate()
        {
            return true;
        }
    }
}