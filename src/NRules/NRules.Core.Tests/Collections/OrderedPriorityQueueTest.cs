using NRules.Core.Collections;
using NUnit.Framework;

namespace NRules.Core.Tests.Collections
{
    [TestFixture]
    public class OrderedPriorityQueueTest
    {
        [Test]
        public void Queue_Enqueue_DequeueReturnsByPriority()
        {
            //Arrange
            var queue = new OrderedPriorityQueue<int, string>();

            //Act
            queue.Enqueue(1, "e");
            queue.Enqueue(2, "d");
            queue.Enqueue(3, "c");
            queue.Enqueue(5, "a");
            queue.Enqueue(4, "b");

            //Assert
            Assert.AreEqual("a", queue.Dequeue());
            Assert.AreEqual("b", queue.Dequeue());
            Assert.AreEqual("c", queue.Dequeue());
            Assert.AreEqual("d", queue.Dequeue());
            Assert.AreEqual("e", queue.Dequeue());
        }

        [Test]
        public void Queue_EnqueueWithDupPriorities_DequeueReturnsByPriorityThenByOrderOfInsertion()
        {
            //Arrange
            var queue = new OrderedPriorityQueue<int, string>();

            //Act
            queue.Enqueue(1, "h");
            queue.Enqueue(2, "e");
            queue.Enqueue(2, "f");
            queue.Enqueue(2, "g");
            queue.Enqueue(3, "d");
            queue.Enqueue(5, "a");
            queue.Enqueue(4, "b");
            queue.Enqueue(4, "c");

            //Assert
            Assert.AreEqual("a", queue.Dequeue());
            Assert.AreEqual("b", queue.Dequeue());
            Assert.AreEqual("c", queue.Dequeue());
            Assert.AreEqual("d", queue.Dequeue());
            Assert.AreEqual("e", queue.Dequeue());
            Assert.AreEqual("f", queue.Dequeue());
            Assert.AreEqual("g", queue.Dequeue());
            Assert.AreEqual("h", queue.Dequeue());
        }
    }
}