using NUnit.Framework;
using SnippetBuilderCSharp.Commands;

namespace SnippetBuilderCSharp.Test.Commands
{
    public class CommandAttributeTests
    {
        [Test]
        public void EqualsAnyTest()
        {
            var sut = new CommandAttribute {ShortName = "a", LongName = "abc"};

            var actual = sut.EqualsAny("-a");
            Assert.That(actual, Is.True);

            actual = sut.EqualsAny("--abc");
            Assert.That(actual, Is.True);

            actual = sut.EqualsAny("-A");
            Assert.That(actual, Is.True);

            actual = sut.EqualsAny("--AbC");
            Assert.That(actual, Is.True);

            actual = sut.EqualsAny("-abc");
            Assert.That(actual, Is.False);

            actual = sut.EqualsAny("-b");
            Assert.That(actual, Is.False);

            actual = sut.EqualsAny("--abcd");
            Assert.That(actual, Is.False);
        }
    }
}