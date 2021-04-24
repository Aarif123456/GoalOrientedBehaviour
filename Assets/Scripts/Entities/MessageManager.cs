using System;
using UnityEngine;
using System.Collections.Generic;
using Utility.DataStructures;

namespace Entities {
    [Serializable]
    public struct Message{
        public string Text { get; set;}
        public Color Color { get; set;}
        public override string ToString(){
            return Text;
        }
    }
    public class MessageManager : MonoBehaviour {
        private static MessageManager _instance;
        public static MessageManager Instance {
            get { return _instance ??= GameObject.Find("Game").GetComponent<MessageManager>(); }
        }
        
        /* Will map Entity to a list that manages the messages */
        private readonly Dictionary<int, List<Message>> messageDictionary = new Dictionary<int, List<Message>>();
        /* Number of max messages stored in a queue*/
        private const int MAX_MESSAGES = 10;

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
            if(!messageDictionary.ContainsKey(entityId)) messageDictionary.Add(entityId, new List<Message>());
        }
        public void AddMessage(Entity entity, Message message){
            AddMessage(entity.id, message);
        }

        public void AddMessage(int entityId, Message message){
            if(!messageDictionary.ContainsKey(entityId)) AddEntity(entityId);
            messageDictionary[entityId].Push(message);
            if(messageDictionary[entityId].Count > MAX_MESSAGES){
                messageDictionary[entityId].Dequeue();
            }
        }

        public List<Message> GetMessages(Entity entity, int numMessages = MAX_MESSAGES){
            return GetMessages(entity.id, numMessages);
        }

        public List<Message> GetMessages(int entityId, int numMessages = MAX_MESSAGES){
            messageDictionary.TryGetValue(entityId, out var messages);
            if (messages == null) return new List<Message>();
            numMessages = Mathf.Min(numMessages, messages.Count);
            if(numMessages<messages.Count) return messages;
            /*If we don't want all the messages then return a sub-array*/
            var startIndex = messages.Count-numMessages;
            return messages.GetRange(startIndex, numMessages);
        }

    }
}