namespace Game
{
    public enum RelationshipType{Friend,BFF,Ignored,Stranger}
    [System.Serializable]
    public class RelationshipData
    {
        public string userName;
        public bool isOnline;
        public RelationshipType type;
    }
}