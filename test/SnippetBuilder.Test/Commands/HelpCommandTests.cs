using NUnit.Framework;
using SnippetBuilder.Commands;

namespace SnippetBuilder.Test.Commands
{
    public class HelpCommandTests
    {
        [Test]
        public void ValidateTest()
        {
            var sut = new HelpCommand();
            var actual = sut.Validate();
            Assert.That(actual, Is.True);

            sut.Append("Sample");
            actual = sut.Validate();
            Assert.That(actual, Is.True);
        }
    }
}