using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Map
{
    public class ChatHandler : MonoBehaviour, IHandler
    {
        [Header("Chat Canvas")]
        public Dictionary<Character, ChatCanvas> activeCanvases = new Dictionary<Character, ChatCanvas>();
        public GameObject canvasPrefab;
        [Header("Chat Bubble")] 
        public GameObject bubblePrefab;
        
        #region Initialization
        // IHandler methods
        public GameSystem system {get; set;}
        public bool Display { get; set; }

        public void Activate()
        {
            GetFilter();
        }
        
        // update the canvas position
        public void Update()
        {
            foreach (Character character in activeCanvases.Keys)
                activeCanvases[character].transform.position = character.transform.position;
            
        }
        #endregion
        #region Chatting Handler

        public void Chatted(Character character, string message)
        {
            ChatCanvas canvas = activeCanvases.ContainsKey(character)? activeCanvases[character] : CreateCanvas(character);
            CreateBubble(canvas, message);
        }
        

        private ChatBubble CreateBubble(ChatCanvas canvas, string message)
        {
            GameObject bubbleObject = Instantiate(bubblePrefab, canvas.transform);
            ChatBubble bubble = bubbleObject.GetComponent<ChatBubble>();
            bubble.Message = message;
            canvas.AddBubble(bubble);
            return bubble;
        }

        private ChatCanvas CreateCanvas(Character character)
        {
            GameObject canvasObject = Instantiate(canvasPrefab, transform);
            ChatCanvas canvas = canvasObject.GetComponent<ChatCanvas>();
            activeCanvases.Add(character, canvas);
            return canvas;
        }

        public bool CheckLocalQueue()
        {
            bool isNotFull = true;
            Character localCharacter = system.GetHandler<UserHandler>().localPlayer.session;
            if (activeCanvases.ContainsKey(localCharacter))
                isNotFull = activeCanvases[localCharacter].bubbles.Count < 3;
            return isNotFull;
        }
        #endregion
        #region Filter requests
        public ServerFilter filter;
        public void FilterString(string message, Action<FilteredMessage> callback)
        {
            FilteredMessage filteredMessage = new FilteredMessage(){message = message};
            switch (filter.filterType)
            {
                case FilterType.Blacklist:
                    filteredMessage.invalidWords = BlacklistFilter(message);
                    break;
                case FilterType.Whitelist:
                    filteredMessage.invalidWords = WhitelistFilter(message);
                    break;
                default:
                    filteredMessage.invalidWords = new string[0];
                    break;
            }
            callback(filteredMessage);
        }

        public void GetFilter()
        {
            system.ServerDataRequest("filterData", data => filter = data.ToObject<ServerFilter>());
        }
        private string[] BlacklistFilter(string message)
        {
            List<string> filtered = new List<string>();
            //string[] blacklist = new[] {"cheese", "bad", "dominator","blacklist"};
            string[] blacklist = filter.filterWords;
            
            foreach (string word in blacklist)
            {
                Regex regex = new Regex(word);
                MatchCollection matches = regex.Matches(message.ToLower());
                foreach (Match match in matches)
                    filtered.Add(match.Value);    
            }
            // Remove duplicate entries
            filtered = RemoveDuplicates(filtered);
            
            return filtered.ToArray();
        }
        private string[] WhitelistFilter(string message)
        {
            List<string> filtered = message.Split(new[] {' ',',','.',';','!','\''}, StringSplitOptions.RemoveEmptyEntries).ToList();
            string[] whitelist = filter.filterWords;
            string[] ogFiltered = filtered.ToArray();
            
            foreach(string word in ogFiltered)
                if (whitelist.Contains(word.ToLower()))
                    filtered.Remove(word);

            // Remove duplicate entries
            filtered = RemoveDuplicates(filtered);
            return filtered.ToArray();
        }

        public List<string> RemoveDuplicates(List<string> filteredWords)
        {
            List<string> sorted = new List<string>();
            foreach(string word in filteredWords)
                if(!sorted.Contains(word))
                    sorted.Add(word);
            return sorted;
        }
        
        #endregion
    }
}