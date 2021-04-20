using Game.UI;

namespace Game
{
    public class GameEvent
    {
        public EventType type;
        public string title;
        public string text;
        public AnnounceBoxIcon icon;
    }

    public enum EventType{FriendRequest,NewFriend,Discovery,Mail};
    public class GameEventDictionary : SerializableDictionary<string,GameEvent>{}
}