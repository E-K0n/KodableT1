using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class MazeParser : MonoBehaviour
{


    [SerializeField]
    private Tilemap mazeMap;

    [Header("Tiles")]
    [SerializeField]
    private TileBase pathTile, wallTile, stairTile;

    [SerializeField]
    private GameObject mazeOrigin;

    [SerializeField]
    private GameObject navigateButton;

    public GameObject playerSprite;
    public Vector3Int entranceTile;
    public Vector3Int exitTile;


    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GameObject.Find("inputField").GetComponent<TMP_InputField>();
    }

    public string[] ParseInput()
    {

        //Parses input ASCII Maze and splits it into lines based on /n

        string inputtext = inputField.text;
        return inputtext.Split('\n');
        
    }

    public void PopulateMazeMap()
    {
        string[] splitinput = ParseInput();


        //finds the starting position and initializes variables 

        Vector3Int originposition = Vector3Int.FloorToInt(mazeOrigin.transform.position);

        //add an offset to avoid having to create a condition for i = 0 in the loop
        Vector3Int offset = new Vector3Int(-1, -1, 0);
        originposition += offset;

        Vector3Int tileposition = originposition;

        //Debug.Log(tileposition);

        //run through each line, building a tilemap based on the character found inside.
        //Use an arraylist to record for any entrances or exits 
        ArrayList holelist = new ArrayList();


        for (int i = 0; i < splitinput.Length; i++)
        {

            tileposition.y -= 1;
            tileposition.x = originposition.x;


            //remove \r and \n from each string 
            splitinput[i] = splitinput[i].Replace("\r", "");
            splitinput[i] = splitinput[i].Replace("\n", "");

            for (int j = 0; j < splitinput[i].Length; j++)
            {
                tileposition.x += 1;
                string currentrow = splitinput[i];
                
                switch (currentrow[j])

                {

                    case ' ':
                        mazeMap.SetTile(tileposition, pathTile);

                        //if a path is found at a wall, it is an entrance or exit. Add it to the holelist. 
                        if (i == 0 || i == splitinput.Length - 1 || j == 0 || j == splitinput[i].Length - 1)
                        {
                            holelist.Add(tileposition);
                            //Debug.Log(tileposition);
                            mazeMap.SetTile(tileposition, stairTile);
                        }

                        break;

                    case '+':
                        mazeMap.SetTile(tileposition, wallTile);
                        break;

                    case '-':
                        mazeMap.SetTile(tileposition, wallTile);
                        break;

                    case '|':
                        mazeMap.SetTile(tileposition, wallTile);
                        break;

                    default:
                        mazeMap.SetTile(tileposition, wallTile);
                        break;
                }



                }
            }

        //Compress Bounds 
        mazeMap.CompressBounds();


        //check for maze validity - there must be exactly 2 entrances/exits
        if (holelist.Count != 2)
            Debug.Log("INVALID MAZE");

        entranceTile = (Vector3Int)holelist[0];
        exitTile = (Vector3Int)holelist[1];

        //activate the navigate Button
        navigateButton.SetActive(true);

        //move playersprite to entrancetile and enable it
        playerSprite.transform.position = entranceTile;
        playerSprite.SetActive(true);

    }


}
