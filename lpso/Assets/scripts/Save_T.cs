using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save_T
{
   public string savedscreenname;
   public List<CharacterSlot> characterslots;
   public bool remembername;
   public int characterchosen;

   public CharacterSlot GetCharacterSlot(int key)
    {
        CharacterSlot charslot = characterslots[key];
        return charslot;
    }
    public Save_T(string ssn, List<CharacterSlot> l_cs, bool rmn, int aslot)
    {
        savedscreenname = ssn;
        characterslots = l_cs;
        remembername = rmn;
        characterchosen = aslot;
    }



}

////////
[System.Serializable]
public class CharacterSlot
{
    public string name;
    public string species;


    public CharacterSlot(string nam,string spec)
    {
        name = nam;
        species = spec;

    }

}



////////
