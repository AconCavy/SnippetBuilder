namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "name", ShortName = "n", Description = "Output file name")]
    public class NameCommand : CommandBase, IRecipeApplier
    {
        public override bool Validate()
        {
            return true;
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Name = Parameter;
        }
    }
}