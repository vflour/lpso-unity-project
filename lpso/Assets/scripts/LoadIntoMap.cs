using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class LoadIntoMap : MonoBehaviour
{
    public bool loaded = false;
    private Save_T savedata = new Save_T("",new List<CharacterSlot>(),false,0);
    SpeciesTypes[] spr;

    public Vector3 startpos = new Vector3(0,0,0);
    public GameObject characterfolder;
    public Transform map;
    public TileMap tilescr;

    public Canvas canvas;
    public Slider loadslide;
    public GameObject loadgui;
    public Camera camera;
    public GameObject character;

    public List<List<InventorySlot_s>> InventoryData;
    public InventoryScript inventoryscript;
    private player_move pm;

    void Start()
    {
        StartCoroutine(loading_gui());// loading screen as coroutine
        character = characterLoad();
        SetPlayerCharScript();
        inventoryscript.Summon(InventoryData, character.transform);
        tilescr.playerchar = character;
        tilescr.PlayerMove = pm;
        tilescr.StartMap();
        loaded = true;
        
    }

    void SetPlayerCharScript()
    {
        pm = character.AddComponent<player_move>();
        pm.playerchar = character;
        pm.camera = camera;
        pm.newpos = startpos;
    }

    // loading screen

    IEnumerator loading_gui()
    {
        for (float i = 0; i < 0.2; i += 0.01f)
        {
            loadslide.value = i;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitUntil(() => loaded);
        for (float i = 0.2f; i < 1; i += 0.01f)
        {
            loadslide.value = i;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        tilescr.rdy = true;
        camera.transform.position = character.transform.position - new Vector3(0,0,1);
        canvas.transform.Find("panel2").gameObject.SetActive(false);
        loadgui.SetActive(false);
        canvas.transform.Find("panel").gameObject.SetActive(true);
        canvas.transform.Find("panel3").gameObject.SetActive(true);
        pm.rdy = true;
    }
  

    // loading the character

    GameObject characterLoad()
    {
        loadSave();
        CharacterSlot char_s = savedata.characterslots[savedata.characterchosen];
        Json_Spec_Load();
        SpeciesTypes charspec = spr[FindSpeciesDict(char_s.species)];
        GameObject prefab = Resources.Load<GameObject>(charspec.model) as GameObject;

        GameObject charactermodel = Instantiate(prefab, startpos, Quaternion.identity, characterfolder.transform);
        charactermodel.name = savedata.savedscreenname;
        GlobalSort gb = charactermodel.AddComponent<GlobalSort>();
        gb.character = charactermodel.transform;
        gb.map = map.Find("Map").Find("map").Find("props");
        gb.SortPlr();

        CursorSet cursorset = charactermodel.AddComponent<CursorSet>();

        return charactermodel;
    }

    public int FindSpeciesDict(string sn)
    {
        Dictionary<string, int> speciesdict = new Dictionary<string, int>();
        speciesdict["Kitty"] = 0;
        speciesdict["Dog"] = 1;

        return speciesdict[sn];
    }

    void Json_Spec_Load()
    {
        TextAsset jsonfile = Resources.Load<TextAsset>("speciestypes");
        string jsonstring = jsonfile.text;
        spr = JsonHelper.FromJson<SpeciesTypes>(jsonstring);
    }

    void loadSave()
    {
        LoadInventorySave();
        

        string destination = Application.persistentDataPath + "/save.dat";
        FileStream savefile;

        if (File.Exists(destination)) savefile = File.OpenRead(destination);
        else
        {
            Debug.LogError("No file.");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Save_T data = (Save_T)bf.Deserialize(savefile);
        savefile.Close();

        savedata.savedscreenname = data.savedscreenname;
        savedata.characterslots = data.characterslots;
        savedata.remembername = data.remembername;
        savedata.characterchosen = data.characterchosen;
    }

    void LoadInventorySave()
    {
        string destination = Application.persistentDataPath + "/inventory.dat";
        FileStream inventorysave;
        if (File.Exists(destination)) inventorysave = File.OpenRead(destination);
        else
        {
            List<InventorySlot_s> page0 = new List<InventorySlot_s>()
            {
                {new InventorySlot_s("110001",1)},
                {new InventorySlot_s("100001",3)}
            };

            InventoryData = new List<List<InventorySlot_s>>()
            {
                { page0 }
            };
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        List<List<InventorySlot_s>> data = (List<List<InventorySlot_s>>)bf.Deserialize(inventorysave);
        inventorysave.Close();
        InventoryData = data;

    }
    

}

public class InventorySlot_s
{
    public string id;
    public int stack;

    public InventorySlot_s(string id, int stack)
    {
        this.id = id;
        this.stack = stack;
    }
}