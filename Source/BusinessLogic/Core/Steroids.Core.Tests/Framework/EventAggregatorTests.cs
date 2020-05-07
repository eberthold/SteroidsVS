using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Core.Framework;

namespace Steroids.Core.Tests.Framework
{
    [TestClass]
    public class EventAggregatorTests
    {
        private int _receivedCount = 0;

        [TestMethod]
        public void Send_ActiveSubscription_ReceivedMessage()
        {
            var sut = CreateSut();
            Register(sut);
            CheckSubscribers(sut, 1);
            SendCorrectMessage(sut);
            CheckReceived(1);
        }

        [TestMethod]
        public void Send_NotSubscribed_NotReceived()
        {
            var sut = CreateSut();
            SendCorrectMessage(sut);
            CheckReceived(0);
        }

        [TestMethod]
        public void Send_AfterUnsubscribe_NotReceived()
        {
            var sut = CreateSut();
            Register(sut);
            Unregister(sut);
            CheckSubscribers(sut, 0);
            SendCorrectMessage(sut);
            CheckReceived(0);
        }

        [TestMethod]
        public void Send_WrongMessage_NotReceived()
        {
            var sut = CreateSut();
            Register(sut);
            SendWrongMessage(sut);
            CheckReceived(0);
        }

        [TestMethod]
        public void GC_DeadObject_NoSubscribers()
        {
            var sut = CreateSut();
            _ = new Subscriber(sut);
            CheckSubscribers(sut, 1);
            GC.Collect();
            CheckSubscribers(sut, 0);
        }

        private void Register(EventAggregator sut)
        {
            sut.Subscribe<MessageToReceive>(Receive);
        }

        private void SendCorrectMessage(EventAggregator sut)
        {
            sut.Publish(new MessageToReceive { Count = 1 });
        }

        private void SendWrongMessage(EventAggregator sut)
        {
            sut.Publish(new MessageToIgnore());
        }

        private void Receive(MessageToReceive message)
        {
            _receivedCount += message.Count;
        }

        private void Unregister(EventAggregator sut)
        {
            sut.Unsubscribe<MessageToReceive>(Receive);
        }

        private void CheckReceived(int count)
        {
            Assert.AreEqual(_receivedCount, count);
        }

        private void CheckSubscribers(EventAggregator sut, int expected)
        {
            Assert.AreEqual(expected, sut.GetSubscriberCountFor<MessageToReceive>());
        }

        private EventAggregator CreateSut()
        {
            return new EventAggregator();
        }

        private class MessageToReceive
        {
            public int Count { get; set; } = 1;
        }

        private class MessageToIgnore
        {
        }

        private class Subscriber
        {
            public Subscriber(EventAggregator sut)
            {
                sut.Subscribe<MessageToReceive>(Callback);
            }

            private void Callback(MessageToReceive obj)
            {
            }
        }
    }
}
