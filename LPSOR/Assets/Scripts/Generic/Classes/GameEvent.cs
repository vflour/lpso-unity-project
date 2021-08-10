using Game.UI;

namespace Game
{
    public class GameEvent
    {
        public AnnounceBoxType type;
        public string title;
        public string text;
        public AnnounceBoxIcon icon;
    }
    
    public class GameEventDictionary : SerializableDictionary<string,GameEvent>{}
}