using NRules.Collections;
using Xunit;

namespace NRules.Tests.Collections
{
    public class OrderedPriorityQueueTest
    {
        [Fact]
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
            Assert.Equal("a", queue.Dequeue());
            Assert.Equal("b", queue.Dequeue());
            Assert.Equal("c", queue.Dequeue());
            Assert.Equal("d", queue.Dequeue());
            Assert.Equal("e", queue.Dequeue());
        }

        [Fact]
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
            Assert.Equal("a", queue.Dequeue());
            Assert.Equal("b", queue.Dequeue());
            Assert.Equal("c", queue.Dequeue());
            Assert.Equal("d", queue.Dequeue());
            Assert.Equal("e", queue.Dequeue());
            Assert.Equal("f", queue.Dequeue());
            Assert.Equal("g", queue.Dequeue());
            Assert.Equal("h", queue.Dequeue());
        }
    }
}