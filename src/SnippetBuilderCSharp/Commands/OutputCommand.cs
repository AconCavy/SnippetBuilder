namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "output", ShortName = "o", Description = "Output directory")]
    public class OutputCommand : CommandBase, IRecipeApplier
    {
        public override bool Validate()
        {
            return true;
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Output = Parameter;
        }
    }
}