using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchnmunchLogic : MonoBehaviour
{

    public GameObject[] FoodObjects;
    public GameObject BoxBackground;
    public GameObject Box;
    public GameObject BoxFound;
    public Camera cam;
    public GameObject CursorObj;
    public Text FoodGoalText;
    public float BoxLiftSpeed = 3f;
    public GameObject HoverObj;
    public Sprite HoverGoodSprite;
    public Sprite HoverBadSprite;
    public Text FeedbackText;

    public int GridWidth = 7;
    public int GridHeight = 7;
    public float FoodMargin = 0.1f;
    public float FoodLeftStart = 0.3f;
    private bool GameStarted = false;
    private float aspect;
    private float worldHeight;
    private float worldWidth;
    private float LeftStart;

    private float IconWidth;
    private float IconHeight;
    private int selectedFood;
    private int winningFood;

    public GameObject LiveImage;
    public int Lives = 7;
    private float LiveImgMargin = 0.5f;
    private GameObject[] LiveObjects;
    private int FoodGoal = 0;
    private int FoodFound = 0;
    private GameObject[,] Grid;
    private GameObject[,] GridBox;
    private GameObject HoverObjGame;
    private float[,] BoxHeightGoal;
    private float StartCountDown = 3;
    private float winningFoodTimer;
    // Start is called before the first frame update
    void Start()
    {
      Grid = new GameObject[GridWidth,GridHeight];
      GridBox = new GameObject[GridWidth,GridHeight];
      BoxHeightGoal = new float[GridWidth,GridHeight];

      aspect = (float)Screen.width / Screen.height;
      worldHeight = cam.orthographicSize * 2;
      worldWidth = worldHeight * aspect;
      LeftStart = worldWidth * FoodLeftStart - worldWidth/2;

      LiveObjects = new GameObject[Lives];
      HoverObjGame = Instantiate(HoverObj, new Vector3(10,10,10), Quaternion.identity);

      Cursor.visible = false;

      IconWidth = ((worldWidth - FoodMargin - worldWidth * FoodLeftStart) / (GridWidth + 1));
      IconHeight = ((worldHeight - FoodMargin ) / (GridHeight + 1));


        for(int x = 0; x < GridWidth; x++)
          {
            for(int y = 0; y < GridHeight; y++)
              {
                  Instantiate(BoxBackground, new Vector3(LeftStart + (x+1) * IconWidth,(y+1) * IconHeight - worldHeight / 2 - FoodMargin,0), Quaternion.identity);
                  int CurrentItem = (int)Random.Range(0,FoodObjects.Length);
                  GameObject Clone = Instantiate(FoodObjects[CurrentItem], new Vector3(LeftStart + (x+1) * IconWidth,(y+1) * IconHeight - worldHeight / 2 - FoodMargin,0), Quaternion.identity);
                  Grid[x,y] = Clone;
              }
          }

          for(int i = 0; i < Lives; i++)
          {
            GameObject Clone = Instantiate(LiveImage,new Vector3(-(Lives*LiveImgMargin)/2+(i+0.5f)*LiveImgMargin,(worldHeight/2)-0.5f,0), Quaternion.identity);
            LiveObjects[i] = Clone;
          }

          for(int x = 0; x < GridWidth; x++)
            {
              for(int y = 0; y < GridHeight; y++)
                {
                    BoxHeightGoal[x,y] = 0;
                    GameObject Clone = Instantiate(Box, new Vector3(LeftStart + (x+1) * IconWidth,(y+1) * IconHeight - worldHeight / 2 - FoodMargin + BoxHeightGoal[x,y],0), Quaternion.identity);
                    GridBox[x,y] = Clone;
              }

            }
    }

    // Update is called once per frame
    void Update()
    {
      Vector2 MousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
      CursorObj.transform.position = MousePosition;
      if (StartCountDown > 0)
      {
        StartCountDown -= Time.deltaTime;
      }
      if (winningFoodTimer > 0)
      {
        winningFoodTimer -= Time.deltaTime;
        if (winningFoodTimer <= 0)
        {
          FeedbackText.enabled = false;
        }
      }
      for(int x = 0; x < GridWidth; x++)
        {
          for(int y = 0; y < GridHeight; y++)
            {
                if (GridBox[x,y])
                {
                  GridBox[x,y].transform.position = Vector3.Lerp(GridBox[x,y].transform.position, new Vector3(LeftStart + (x+1) * IconWidth,(y+1) * IconHeight - worldHeight / 2 - FoodMargin + BoxHeightGoal[x,y],0), Time.deltaTime * BoxLiftSpeed);
                  if (GridBox[x,y].transform.position.y > (y+1) * IconHeight - worldHeight / 2 - FoodMargin + 5.5f)
                  {
                    Destroy(GridBox[x,y]);
                  }
                  if (StartCountDown <= 0 && !GameStarted)
                  {
                    BoxHeightGoal[x,y] = 6;
                  }
              }
            }
          }
      if (StartCountDown <= 0)
      {
      if(!GameStarted && Input.GetMouseButtonDown(0))
      {
      GameStarted = true;

      for(int x = 0; x < GridWidth; x++)
        {
          for(int y = 0; y < GridHeight; y++)
            {
                BoxHeightGoal[x,y] = 5;
                GameObject Clone = Instantiate(Box, new Vector3(LeftStart + (x+1) * IconWidth,(y+1) * IconHeight - worldHeight / 2 - FoodMargin + BoxHeightGoal[x,y],0), Quaternion.identity);
                BoxHeightGoal[x,y] = 0;

                GridBox[x,y] = Clone;
                if (winningFood == Grid[x,y].GetComponent<MunchItem>().ItemNumber)
                {
                  FoodGoal += 1;
                }
            }
          }

        }
        if (GameStarted)
        {
          FoodGoalText.text = "Found " + FoodFound + " of " + FoodGoal;
        }

        int HoverX = (int)((MousePosition.x + worldWidth * FoodLeftStart - FoodMargin) / IconWidth -1.5f);
        int HoverY = (int)((MousePosition.y + worldHeight/2 - FoodMargin) / IconHeight -0.4f);

        if (HoverX >= 0 && HoverX < GridWidth && HoverY >= 0 && HoverY < GridHeight){
          SpriteRenderer sprite = HoverObjGame.GetComponent<SpriteRenderer>();
          HoverObjGame.transform.position = new Vector3(LeftStart + (HoverX+1) * IconWidth, (HoverY+1) * IconHeight - worldHeight / 2 - FoodMargin,0);
          sprite.sprite = HoverGoodSprite;
          if (winningFood > 0 && winningFood != Grid[HoverX,HoverY].GetComponent<MunchItem>().ItemNumber){
            sprite.sprite = HoverBadSprite;
          }
        } else {
          HoverObjGame.transform.position = new Vector3(10,10,0);
        }
      if(Input.GetMouseButtonDown(0))
      {
        int SelectedX = HoverX;
        int SelectedY = HoverY;
        if (Grid[SelectedX,SelectedY])
        {
          selectedFood = Grid[SelectedX,SelectedY].GetComponent<MunchItem>().ItemNumber;
          if (winningFood  == 0)
          {
            winningFood = selectedFood;
            for(int x = 0; x < GridWidth; x++)
              {
                for(int y = 0; y < GridHeight; y++)
                  {
                      if (winningFood == Grid[x,y].GetComponent<MunchItem>().ItemNumber)
                      {
                        FoodGoal += 1;
                      }
                  }
                }

              }
          if (winningFood == selectedFood){
            Instantiate(BoxFound, new Vector3(LeftStart + (SelectedX+1) * IconWidth, (SelectedY+1) * IconHeight - worldHeight / 2 - FoodMargin,0), Quaternion.identity);
            FoodFound += 1;
            if (GridBox[SelectedX,SelectedY]) {
              FeedbackText.transform.position = cam.WorldToScreenPoint(new Vector3(LeftStart + (SelectedX+1) * IconWidth, (SelectedY+1) * IconHeight - worldHeight / 2 - FoodMargin,0));
              FeedbackText.enabled = true;
              winningFoodTimer = 1f;
            }
            if (FoodFound == FoodGoal){
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            for (int x = -1; x <= 1; x++)
              {
              for (int y = -1; y <= 1; y++)
                {
                  if (SelectedX + x >= 0 && SelectedX + x < GridWidth)
                  {
                    if (SelectedY + y >= 0 && SelectedY + y < GridHeight)
                    {
                      BoxHeightGoal[SelectedX + x,SelectedY + y] = 6;
                      if (GridBox[SelectedX + x,SelectedY + y]){
                      SpriteRenderer sprite = GridBox[SelectedX + x,SelectedY + y].GetComponent<SpriteRenderer>();
                      if (sprite)
                      {
                        sprite.sortingOrder = 10;
                      }
                    }
                    }
                  }
                }
              }
          } else {
            if (Lives == 1)
            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } else {
              Destroy(LiveObjects[Lives-1]);
              Lives -= 1;
            }
          }
          Debug.Log("SelectedFood = " + selectedFood);
          Debug.Log("WinningFood = " + winningFood);
        }
      }
    }
}
}
