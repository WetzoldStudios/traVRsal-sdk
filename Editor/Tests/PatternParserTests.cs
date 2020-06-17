using NUnit.Framework;

namespace traVRsal.SDK
{
    public class PatternParserTest
    {
        [Test]
        public void EmptyPattern()
        {
            PatternParser p = new PatternParser("");

            Assert.IsNull(p.GetNextExecution());
            Assert.IsNull(p.GetNextAction());

            p = new PatternParser(null);

            Assert.IsNull(p.GetNextExecution());
            Assert.IsNull(p.GetNextAction());
        }

        [Test]
        public void SimplePattern()
        {
            PatternParser p = new PatternParser("500 , x");

            Assert.True(p.GetNextExecution() > 0);
            Assert.AreEqual("x", p.GetNextAction());

            p.Next();

            Assert.IsNull(p.GetNextExecution());
            Assert.IsNull(p.GetNextAction());
        }

        [Test]
        public void GlobalLoopingPattern()
        {
            PatternParser p = new PatternParser("500,x", true);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(p.GetNextExecution() > 0);
                Assert.AreEqual("x", p.GetNextAction());

                p.Next();
            }
        }

        [Test]
        public void ComplexPattern()
        {
            PatternParser p = new PatternParser("500,x,20,y,loop 1,2,50,z");

            Assert.True(p.GetNextExecution() > 0);
            Assert.AreEqual("x", p.GetNextAction());

            p.Next();

            Assert.True(p.GetNextExecution() > 0);
            Assert.AreEqual("y", p.GetNextAction());

            p.Next();

            Assert.True(p.GetNextExecution() > 0);
            Assert.AreEqual("y", p.GetNextAction());

            p.Next();

            Assert.True(p.GetNextExecution() > 0);
            Assert.AreEqual("y", p.GetNextAction());

            p.Next();

            Assert.True(p.GetNextExecution() > 0);
            Assert.AreEqual("z", p.GetNextAction());

            p.Next();

            Assert.IsNull(p.GetNextExecution());
            Assert.IsNull(p.GetNextAction());
        }

        [Test]
        public void InfinitePattern()
        {
            PatternParser p = new PatternParser("50,x,20,y,10,z,loop 3,*");

            for (int i = 0; i < 100; i++)
            {
                Assert.True(p.GetNextExecution() > 0);
                Assert.AreEqual("x", p.GetNextAction());

                p.Next();

                Assert.True(p.GetNextExecution() > 0);
                Assert.AreEqual("y", p.GetNextAction());

                p.Next();

                Assert.True(p.GetNextExecution() > 0);
                Assert.AreEqual("z", p.GetNextAction());

                p.Next();
            }
        }
    }
}