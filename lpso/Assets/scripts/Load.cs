using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Load : MonoBehaviour
{
    public SpeciesTypes[] speciesTypes;
    public string screen_name = "";
    public Save_T savedata;
    public bool remembername = false;
    public GameObject c_a_pmodels;
    public GameObject plr_models;
    public Slider loadingslider;
    public PadSlots[] padSlots;
    public Start_Buttons startbuttons;


    public int aslot = 0;
    public int cslot = 0;

    public GameObject num_b;
    public int remain_chars = 2;

    public GameObject Scenes;
    public GameObject Canvas;

    private bool rdy = false;

    void Start()
    {
        Action load1 = delegate () { LoadStart(); };

        gameObject.AddComponent<CursorSet>();
        StartCoroutine(LoadingBar(load1));
        LoadData1InScene();
        rdy = true;
            
    }


    //
    
    public void load_char_sel()
    {
        rdy = false;
        Action loadrest = delegate () { Load2(); };
        setloadvis(true);
        StartCoroutine(LoadingBar(loadrest));

        Scenes.transform.Find("create_a_pet").gameObject.SetActive(false);
        Canvas.transform.Find("Fr_3").gameObject.SetActive(false);
        rdy = true; 

    }

    void LoadStart()
    {
        StartCoroutine(Transition());
        setfr1vis(true);
        setloadvis(false);
        loadingslider.value = 0;
    }

    public void Load2()
    {
        setloadvis(false);
        fr3vis(true);
        setfr3display(true);
        loadDisplayChars();
    }

    //


    public void writesavefile() // only fires when you make a new character
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream savefile;

        if (File.Exists(destination)) savefile = File.OpenWrite(destination);
        else savefile = File.Create(destination);

        Save_T data = new Save_T(screen_name, savedata.characterslots, remembername, aslot);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(savefile, data);
        savefile.Close();
    }

    public bool getsavefile() // fires during first loading screen
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream savefile;

        if (File.Exists(destination)) savefile = File.OpenRead(destination);
        else
        {
            savedata = new Save_T(screen_name, new List<CharacterSlot>(), remembername, 0);
            Debug.Log("No file.");
            return false;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Save_T data = (Save_T)bf.Deserialize(savefile);
        savefile.Close();

        savedata.savedscreenname = data.savedscreenname;
        savedata.characterslots = data.characterslots;
        savedata.remembername = data.remembername;
        savedata.characterchosen = aslot;

        return true;
    }

    public void LoadData1InScene()
    {
        bool has_save = getsavefile();
        setspeciestypes();
        if (has_save)
        {
            remembername = savedata.remembername;
            if (remembername == true) screen_name = savedata.savedscreenname;
            Canvas.transform.Find("Fr_1").Find("screen_name").GetComponent<InputField>().text = screen_name;
            Canvas.transform.Find("Fr_1").Find("rem").Find("en").gameObject.SetActive(remembername);

            foreach (CharacterSlot v in savedata.characterslots) loadSavedChars(v);
            
            aslot = savedata.characterslots.Count - 1;
            if (remain_chars == 0) Canvas.transform.Find("Fr_2").Find("create").gameObject.SetActive(false);
            else if (remain_chars == 1) ArrowVis(false);

        }
        else NoSaveSlot();
        
    }

    void NoSaveSlot()
    {
       ArrowVis(false);
       Scenes.transform.Find("create_a_pet").Find("no_pet").gameObject.SetActive(true);
       Canvas.transform.Find("Fr_2").Find("play").gameObject.SetActive(false); 
    }

    void ArrowVis(bool val)
    {
        Canvas.transform.Find("Fr_2").Find("bk").gameObject.SetActive(val);
        Canvas.transform.Find("Fr_2").Find("nx").gameObject.SetActive(val);
    }


    public Dictionary<string, int> speciesdict = new Dictionary<string, int>();
    public int FindSpeciesDict(string sn)
    {
        speciesdict["Kitty"] = 0;
        speciesdict["Dog"] = 1;

        return speciesdict[sn];
    }

    public void setspeciestypes()
    {
        TextAsset jsonfile = Resources.Load<TextAsset>("speciestypes");
        string jsonstring = jsonfile.text;
        speciesTypes = JsonHelper.FromJson<SpeciesTypes>(jsonstring);
    }

    public void loadDisplayChars()
    {
        foreach (SpeciesTypes v in speciesTypes)
        {
            GameObject prefab = Resources.Load<GameObject>(v.model) as GameObject;
            GameObject charmodel = Instantiate(prefab, new Vector3(-16.48f, -1.21f, 0), Quaternion.identity, c_a_pmodels.transform) as GameObject;
            charmodel.SetActive(false);
        }
    }

    public void loadSavedChars(CharacterSlot charslot)
    {
        SpeciesTypes charspec = speciesTypes[FindSpeciesDict(charslot.species)];
        GameObject prefab = Resources.Load<GameObject>(charspec.model) as GameObject;
        GameObject charmodel = Instantiate(prefab, new Vector3(-16.48f, -1.21f, 0), Quaternion.identity, plr_models.transform);
        Canvas.transform.Find("Fr_2").Find("char_name").Find("name").GetComponent<UnityEngine.UI.Text>().text = charslot.name;
        Canvas.transform.Find("Fr_2").Find("char_name").Find("type").GetComponent<UnityEngine.UI.Text>().text = charslot.species;
        Canvas.transform.Find("Fr_2").Find("char_name").gameObject.SetActive(true);

        startbuttons.InvisibleOtherPets(Scenes.transform.Find("create_a_pet").Find("models"));
        renderPLRpet(charmodel.transform);
        remain_chars--;
        num_b.GetComponent<UnityEngine.UI.Text>().text = remain_chars.ToString();
    }


    ///

    IEnumerator Transition()
    {
        GameObject transitionthing = Canvas.transform.Find("Transition").gameObject;
        transitionthing.SetActive(true);
        Animator animator = transitionthing.GetComponent<Animator>();
        animator.SetTrigger("OnTransition");
        yield return new WaitForSeconds(0.2f);
        transitionthing.SetActive(false);
    }


    IEnumerator LoadingBar(Action NewFunc)
    {
        for (float i = 0; i <= 0.2; i += 0.01f)
        {
            loadingslider.value = Mathf.Lerp(0, 0.2f, i);
            yield return Time.deltaTime;
        }

        yield return new WaitUntil(() => rdy);

        for (float i = 0.2f; i <= 1; i += 0.01f)
        {
            loadingslider.value = Mathf.Lerp(0.2f, 1, i);
            yield return Time.deltaTime;
        }

        NewFunc();
    }


    //

    public void setfr3display(bool set)
    {
        Scenes.transform.Find("create_a_pet").Find("pedestal0").gameObject.SetActive(set);
        Scenes.transform.Find("create_a_pet").Find("shadow0").gameObject.SetActive(set);
        Scenes.transform.Find("create_a_pet").Find("pedestal2").gameObject.SetActive(set);
        Scenes.transform.Find("create_a_pet").Find("shadow2").gameObject.SetActive(set);
    }

    void renderPLRpet(Transform pet)
    {
        pet.position = padSlots[1].petposition;
        pet.localScale = padSlots[1].charsize;
        pet.Find("f").gameObject.SetActive(false);
        pet.Find("fL").gameObject.SetActive(false);
        pet.Find(padSlots[1].charangle).gameObject.SetActive(true);
        pet.gameObject.SetActive(true);
    }


    public IEnumerator ld_scene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        loadingslider.value = 0;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingslider.value = progress;

            yield return null;
        }
    }

    public void setloadvis(bool val1)
    {
        Scenes.transform.Find("load").Find("image_bg").gameObject.SetActive(val1);
        Scenes.transform.Find("load").Find("bg_load").gameObject.SetActive(false);

        Scenes.transform.Find("load").gameObject.SetActive(val1);
        Canvas.transform.Find("loading").gameObject.SetActive(val1);
    }
    void setfr1vis(bool val)
    {
        Scenes.transform.Find("bg_0").gameObject.SetActive(val);
        Canvas.transform.Find("Fr_1").gameObject.SetActive(val);
    }

    public void fr3vis(bool val)
    {
        Scenes.transform.Find("create_a_pet").Find("displaypets").gameObject.SetActive(val);
        Canvas.transform.Find("Fr_3").gameObject.SetActive(val);

        Canvas.transform.Find("Fr_3").Find("bk").gameObject.SetActive(false);
        Canvas.transform.Find("Fr_3").Find("nx").gameObject.SetActive(true);
    }

}
