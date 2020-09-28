using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Start_Buttons : MonoBehaviour
{
    public GameObject canvas;
    public GameObject scenes;
    private Save_T savedata;
    public Load loadscript;
    private PadSlots[] padSlots;
    private SpeciesTypes[] speciesTypes;
    public int CharacterLimit;

    public bool remembername = false;

    public void Fr1Play() {
        string screen_name = "";
        InputField text_comp = canvas.transform.Find("Fr_1").Find("screen_name").GetComponent<InputField>();
        screen_name = text_comp.text;
        string verdict = CheckString(screen_name);
        
        if (verdict == "rdy")
        {
            setfr1vis(false);
            setfr2vis(true,true);
            loadscript.screen_name = screen_name;
        }
        else
        {
            text_comp.text = "";
            StartCoroutine(DisplaySCexception(verdict, text_comp));
        }
    }

    public void Fr2Create()
    {
        if (loadscript.remain_chars > 0)
        {
            canvas.transform.Find("Fr_2").gameObject.SetActive(false);
            loadscript.load_char_sel();
            scenes.transform.Find("create_a_pet").Find("no_pet").gameObject.SetActive(false);
            scenes.transform.Find("create_a_pet").Find("models").gameObject.SetActive(false);
            loadscript.fr3vis(true);
            RenderPetChoose(0);
        }
    }

    public void RenderPetChoose(int slot)
    {

        Transform patmodels = scenes.transform.Find("create_a_pet").Find("displaypets");
        scenes.transform.Find("create_a_pet").gameObject.SetActive(true);
        speciesTypes = loadscript.speciesTypes;
        padSlots = loadscript.padSlots;
        InvisibleOtherPets(patmodels);
        loadscript.loadDisplayChars();

        Transform pet1 = patmodels.GetChild(slot);
        Transform pet2 = null;
        Transform pet0 = null;

        if (slot + 1 < speciesTypes.Length)
        {
            pet2 = patmodels.GetChild(slot + 1);
            RenderSpecificCAPpet(2, pet2);
        }
        if (slot - 1 >= 0)
        {
            pet0 = patmodels.GetChild(slot - 1);
            RenderSpecificCAPpet(0, pet0);
        }
        RenderSpecificCAPpet(1, pet1);
        canvas.transform.Find("Fr_3").Find("char_name").GetComponentInChildren<UnityEngine.UI.Text>().text = speciesTypes[slot].name;

    }

    public void Fr3Continue()
    {
        Transform patmodels = scenes.transform.Find("create_a_pet").Find("displaypets");

        fr4vis(true);
        loadscript.fr3vis(false);

        InvisibleOtherPets(patmodels);
        patmodels.GetChild(loadscript.cslot).gameObject.SetActive(true);

        loadscript.setfr3display(false);

    }

    public void Fr4Create()
    {
        string charname = canvas.transform.Find("Fr_4").Find("char_name").Find("Text").GetComponent<Text>().text;
        string verdict = CheckString(charname);

        if (verdict == "rdy")
        {
            NewCharacterSlot(charname);

            loadscript.remain_chars = 2;
            loadscript.cslot = 0;
            loadscript.aslot = loadscript.savedata.characterslots.Count - 1;

            foreach (CharacterSlot v in loadscript.savedata.characterslots) loadscript.loadSavedChars(v);
            setfr2vis(true, false);
            fr4vis(false);
        }
        else
        {
            InputField char_name = canvas.transform.Find("Fr_4").Find("char_name").GetComponent<InputField>();
            char_name.text = "";
            StartCoroutine(DisplaySCexception(verdict, char_name));
        }
    }


    public void loadgamescene()
    {
        loadscript.setloadvis(true);
        loadscript.writesavefile();
        StartCoroutine(loadscript.ld_scene());
        setfr2vis(false, true);
    }
    
    //

    void NewCharacterSlot(string name)
    {
        speciesTypes = loadscript.speciesTypes;
        CharacterSlot newchar = new CharacterSlot(name, speciesTypes[loadscript.cslot].name);

        loadscript.savedata.characterslots.Add(newchar);
        loadscript.writesavefile();
        
    }

    IEnumerator DisplaySCexception(string verdict, InputField textcomp)
    {
        textcomp.text = "";
        textcomp.text = verdict;

        yield return new WaitForSeconds(2);
        textcomp.text = "Screen Name";
    }

    public void SetCharacterLimit()
    {
        InputField inputfield = transform.GetComponent<InputField>();
        if (inputfield.text.Length > CharacterLimit)
        {
           inputfield.text = inputfield.text.Substring(0, CharacterLimit);
        }
    }

    public void rmbrnameButton()
    {
        if (remembername == false) {
            remembername = true;
            transform.Find("en").gameObject.SetActive(true);
            loadscript.remembername = true;
        }
        else {
            remembername = false;
            transform.Find("en").gameObject.SetActive(false);
            loadscript.remembername = false;
        }
    }

    string CheckString(string s_c)
    {
        string verdict = "rdy";
        if (s_c.Length < 2)
        {
            verdict = "Too short!";
        }
        else if (s_c.Length > 20)
        {
            verdict = "Too long!";
        }

        return verdict;
    }

    /// 

    public void nxCAP()
    {
        
        speciesTypes = loadscript.speciesTypes;
        loadscript.cslot++;
        setarrowcapvis(true);

        if (loadscript.cslot >= speciesTypes.Length)
        {
            loadscript.cslot = 0;
            canvas.transform.Find("Fr_3").Find("bk").gameObject.SetActive(false);
        }
        else if (loadscript.cslot == speciesTypes.Length-1) canvas.transform.Find("Fr_3").Find("nx").gameObject.SetActive(false);
  
        RenderPetChoose(loadscript.cslot);
    }

    public void bkCAP()
    {
        speciesTypes = loadscript.speciesTypes;
        loadscript.cslot--;

        setarrowcapvis(true);
        if (loadscript.cslot < 0 )
        {
            loadscript.cslot = speciesTypes.Length-1;
            canvas.transform.Find("Fr_3").Find("nx").gameObject.SetActive(false);
        }
        else if (loadscript.cslot == 0 ) canvas.transform.Find("Fr_3").Find("bk").gameObject.SetActive(false);

        RenderPetChoose(loadscript.cslot);
    }

    void setarrowcapvis(bool val)
    {
        canvas.transform.Find("Fr_3").Find("bk").gameObject.SetActive(val);
        canvas.transform.Find("Fr_3").Find("nx").gameObject.SetActive(val);
    }


    //

    public void nxPLR()
    {
        Transform modelcontain = scenes.transform.Find("create_a_pet").Find("models");

        speciesTypes = loadscript.speciesTypes;
        loadscript.aslot++;
        if (loadscript.aslot >= modelcontain.childCount)
        {
            loadscript.aslot = 0;
        }

        InvisibleOtherPets(modelcontain);
        scenes.transform.Find("create_a_pet").Find("models").GetChild(loadscript.aslot).gameObject.SetActive(true);

        setCAPnametag(loadscript.savedata.characterslots[loadscript.aslot]);
    }

    public void bkPLR()
    {
        Transform modelcontain = scenes.transform.Find("create_a_pet").Find("models");

        speciesTypes = loadscript.speciesTypes;
        loadscript.aslot--;
        if (loadscript.aslot < 0)
        {
            loadscript.aslot = modelcontain.childCount - 1;
        }
        InvisibleOtherPets(modelcontain);
        scenes.transform.Find("create_a_pet").Find("models").GetChild(loadscript.aslot).gameObject.SetActive(true);

        setCAPnametag(loadscript.savedata.characterslots[loadscript.aslot]);
    }

    void setCAPnametag(CharacterSlot charslot)
    {
        Transform char_name = canvas.transform.Find("Fr_2").Find("char_name");
        char_name.Find("name").GetComponent<Text>().text = charslot.name;
        char_name.Find("type").GetComponent<Text>().text = charslot.species;
    }

    //

    public void InvisibleOtherPets(Transform patmodels)
    {
        for ( int i = 0; i < patmodels.childCount; i++)
        {
            patmodels.GetChild(i).gameObject.SetActive(false);
        }
    }

    int FindCharAngle(string val)
    {
        if (val == "f") return 0;
        else return 1;        
    }

    void RenderSpecificCAPpet(int slot, Transform pet)
    {
        pet.position = padSlots[slot].petposition;
        pet.localScale = padSlots[slot].charsize;
        pet.GetComponent<Animator>().SetInteger("Idle_Pose", FindCharAngle(padSlots[slot].charangle));
        pet.gameObject.SetActive(true);
    }

    public void DestroyCAPpets(Transform patmodels)
    {
        for (int i = 0; i < patmodels.childCount; i++)
        {
            Destroy(patmodels.GetChild(i).gameObject);
        }
    }

    ///


    void setfr1vis(bool val)
    {
        canvas.transform.Find("Fr_1").gameObject.SetActive(val);
        scenes.transform.Find("bg_0").gameObject.SetActive(val);
        scenes.transform.Find("create_a_pet").gameObject.SetActive(!val);
    }


    void setfr2vis(bool val, bool ex)
    {
        canvas.transform.Find("Fr_2").gameObject.SetActive(val);
        if (ex) scenes.transform.Find("create_a_pet").gameObject.SetActive(val);
        else
        {
            DestroyCAPpets(scenes.transform.Find("create_a_pet").Find("models"));
            loadfr2remains();
        }
        DestroyCAPpets(scenes.transform.Find("create_a_pet").Find("displaypets"));

    }
    void loadfr2remains()
    {
        Transform fr_2 = canvas.transform.Find("Fr_2");
        Transform createapet = scenes.transform.Find("create_a_pet");

        if (loadscript.remain_chars == 0) fr_2.Find("create").gameObject.SetActive(false);
        createapet.Find("models").gameObject.SetActive(true);
        createapet.Find("displaypets").gameObject.SetActive(false);
        if (loadscript.aslot > 0)
        {
            fr_2.Find("bk").gameObject.SetActive(true);
            fr_2.Find("nx").gameObject.SetActive(true);
        }
        fr_2.Find("play").gameObject.SetActive(true);
    }



    void fr4vis(bool val)
    {
        canvas.transform.Find("Fr_4").gameObject.SetActive(val);
    }





}
