////using Bot.Worker.Messages;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Bot.Worker
//{
//    public class Actor
//    {
//        //private IList<ProcessNewRequestsMessage> _messages = new List<ProcessNewRequestsMessage>();

//        public void AddMessage(ProcessNewRequestsMessage message)
//        {
//            _messages.Add(message);
//        }

//        public ProcessNewRequestsMessage GetTopMessage()
//        {
//            if (_messages.Count == 0)
//                return null;
//            return _messages.First();
//        }

//        public bool AreThereNewMessageToProcess()
//        {
//            return _messages.Count == 0;
//        }
//    }
//}
