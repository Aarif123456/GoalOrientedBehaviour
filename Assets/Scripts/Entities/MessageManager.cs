using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.DataStructures;

namespace Entities {
    [Serializable]
    public struct Message {
        public string Text { get; set; }
        public Color Color { get; set; }

        public override string ToString(){
            return Text;
        }
    }

    public class MessageManager : MonoBehaviour {
        /* Number of max messages stored in a queue*/
        private const int _MAX_MESSAGES = 10;
        private static MessageManager _instance;

        /* Will map Entity to a list that manages the messages */
        private readonly Dictionary<int, List<Message>> _messageDictionary = new Dictionary<int, List<Message>>();

        public static MessageManager Instance {
            get { return _instance ??= GameObject.Find("Game").GetComponent<MessageManager>(); }
        }

        public void Start(){
            foreach (var agent in EntityManager.FindAll<Agent>()){
                AddEntity(agent);
            }
        }

        public void LateUpdate(){
            foreach (var agent in EntityManager.FindAll<Agent>()){
                agent.Brain.StoreThoughtProcess(this);
            }
        }

        public void AddEntity(Entity entity){
            AddEntity(entity.id);
        }

        public void AddEntity(int entityId){
            if (!_messageDictionary.ContainsKey(entityId)) _messageDictionary.Add(entityId, new List<Message>());
        }

        public void AddMessage(Entity entity, Message message){
            AddMessage(entity.id, message);
        }

        public void AddMessage(int entityId, Message message){
            if (!_messageDictionary.ContainsKey(entityId)) AddEntity(entityId);
            _messageDictionary[entityId].Push(message);
            if (_messageDictionary[entityId].Count > _MAX_MESSAGES) _messageDictionary[entityId].Dequeue();
        }

        public IEnumerable<Message> GetMessages(Entity entity, int numMessages = _MAX_MESSAGES){
            return GetMessages(entity.id, numMessages);
        }

        public IEnumerable<Message> GetMessages(int entityId, int numMessages = _MAX_MESSAGES){
            _messageDictionary.TryGetValue(entityId, out var messages);
            if (messages == null) return new List<Message>();
            numMessages = Mathf.Min(numMessages, messages.Count);
            if (numMessages < messages.Count) return messages;
            /*If we don't want all the messages then return a sub-array*/
            var startIndex = messages.Count - numMessages;
            return messages.GetRange(startIndex, numMessages);
        }
    }
}