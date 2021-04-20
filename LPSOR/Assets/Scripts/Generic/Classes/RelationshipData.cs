namespace Game
{
    public enum RelationshipType{Friend,BFF,Ignored}
    public class RelationshipData
    {
        public string userName;
        public bool isOnline;
        public RelationshipType type;
    }
}