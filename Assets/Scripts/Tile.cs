using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Color BaseColor;
    public Color WaterColor;
    public GridManager Reference;
    public GameObject LayerPrefab;
    public Sprite[] Imgs;
    public int dir; //0 means up/down with road on left, 1 is left/right with road on bottom, 2 is up/down with road on right, 3 is left/right with road on top
    public bool changed;
    public Vector2 pos;
    public Camera mCam;
    public bool isWater;
    public bool isDirty = true;
    bool valSet = false;
    int storedValue;
    public int Value 
    {
        get 
        {
            if (valSet == false)
            {
                if (BuildingNos == 0)
                {
                    int toReturn = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 0)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x - 1, pos.y - 1));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 1)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x - 1, pos.y));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 2)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x - 1, pos.y + 1));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 3)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x, pos.y + 1));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 4)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x + 1, pos.y + 1));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 5)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x + 1, pos.y));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 6)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x + 1, pos.y - 1));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }
                        else if (i == 7)
                        {
                            Tile temp = GetTileAtPos(new Vector2(pos.x, pos.y - 1));
                            if (temp.BuildingNos == 0)
                                toReturn += 0;
                            else
                                toReturn += temp.Value;
                        }

                    }
                    storedValue = toReturn / 16;
                }
                else
                {
                    storedValue = BuildingNos * Zoning * 10000;
                }

                valSet = true;
            }

            return storedValue; 
        }
    }
    public int Zoning
    {
        get
        {
            if (isWater)
                return 2;

            return GetZoning();
        }
    }

    public int GetZoning()
    {
        return 1;
    }
    public int BuildingNos;

    public SpriteRenderer renderer;
    public GameObject Highlight;
    private Dictionary<Vector2, Tile> Tiles;

    public void Init(GridManager paraRef, Vector2 paraPos)
    {
        Tiles = paraRef.Tiles;
        Reference = paraRef;
        pos = paraPos;
        mCam = paraRef.cam;

        dir = Random.Range(0, 4);
    }

    public void LayerPass()
    {
        if (BuildingNos == 7)
        {
            HouseRando();
        }

        if (BuildingNos == 28)
        {
            AptRando();
        }

        if (BuildingNos == 29)
        {
            ThRando();
        }

        if (BuildingNos == 1 || BuildingNos == 3 || BuildingNos == 5 || BuildingNos == 10 || BuildingNos == 11 || BuildingNos == 12 || BuildingNos == 13 || BuildingNos == 14 || BuildingNos == 15)
            Asphalt();
    }

    public void HouseRando()
    {
        int gar = Random.Range(0,3); //gets a number to decide for garages, 0 for none, 1 for 1car, 2 for 2car
        if (gar != 0)
        {
            GameObject temp = Instantiate(LayerPrefab, this.gameObject.transform); // instantiates the layer.

            switch (gar)
            {
                case 1:
                    temp.GetComponent<SpriteRenderer>().sprite = Imgs[18]; // if gar == 1 then sets the layers sprite to 1car garage.
                    break;
                case 2:
                    temp.GetComponent<SpriteRenderer>().sprite = Imgs[19]; // if gar == 2 then sets the layers sprite to 2car garage.
                    break;
                default:
                    break;
            }
        }

        int Exp = Random.Range(0, 3); //gets a number to decide for Expansion, 0 for none, 1 for HouseExp, 2 for Deck
        if (Exp != 0)
        {
            GameObject TempExp = Instantiate(LayerPrefab, this.gameObject.transform); //instantiates the layer that hold the expansion

            switch (Exp)
            {
                case 1:
                    TempExp.GetComponent<SpriteRenderer>().sprite = Imgs[20]; //makes the layer sprite a deck exapnsion
                    if (Random.Range(0, 2) == 1)
                    {
                        GameObject TempTramp = Instantiate(LayerPrefab, this.gameObject.transform);
                        TempTramp.GetComponent<SpriteRenderer>().sprite = Imgs[24];
                    }
                    break;
                case 2:
                    TempExp.GetComponent<SpriteRenderer>().sprite = Imgs[23]; //makes the layer sprite a house expansion.
                    break;
                default:
                    if (Random.Range(0, 2) == 1)
                    {
                        GameObject TempTramp = Instantiate(LayerPrefab, this.gameObject.transform);
                        TempTramp.GetComponent<SpriteRenderer>().sprite = Imgs[24];
                    }
                    break;
            }
        }

        if (Exp != 2)
        {
            int pool = Random.Range(0, 3); //gets a number for pool, 0 = none, 1 = square, 2 = circle

            if (pool != 0)
            {
                GameObject TempPool = Instantiate(LayerPrefab, this.gameObject.transform);

                switch (pool)
                {
                    case 1:
                        TempPool.GetComponent<SpriteRenderer>().sprite = Imgs[21];
                        break;
                    case 2:
                        TempPool.GetComponent<SpriteRenderer>().sprite = Imgs[22];
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public void AptRando()
    {
        int gar = Random.Range(0, 2); //gets a number to decide for garages, 0 for none, 1 for lot
        if (gar != 0)
        {
            GameObject temp = Instantiate(LayerPrefab, this.gameObject.transform); // instantiates the layer.

            temp.GetComponent<SpriteRenderer>().sprite = Imgs[30]; // if gar == 1 then sets the layers sprite to 1car garage.
        }
    }

    public void ThRando()
    {
        int gar = Random.Range(0, 2); //gets a number to decide for garages, 0 for none, 1 for lot
        if (gar != 0)
        {
            GameObject temp = Instantiate(LayerPrefab, this.gameObject.transform); // instantiates the layer.

            temp.GetComponent<SpriteRenderer>().sprite = Imgs[31]; // if gar == 1 then sets the layers sprite to 1car garage.
        }
    }

    public void Asphalt()
    {

        GameObject temp = Instantiate(LayerPrefab, this.gameObject.transform); // instantiates the layer.

        temp.GetComponent<SpriteRenderer>().sprite = Imgs[32]; // if gar == 1 then sets the layers sprite to 1car garage.
        temp.GetComponent<SpriteRenderer>().sortingOrder = -1;
        
    }

    public void FixedUpdate()
    {
        if (isDirty)
        {
            isDirty = false;

            if (!isWater)
            {
                GetComponent<SpriteRenderer>().sprite = Imgs[BuildingNos];
            }

            if (isWater)
            {
                renderer.color = WaterColor;
            }
            else
                renderer.color = BaseColor;
        }

        if (BuildingNos < 9)
            switch (dir)
            {
                case 0:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 1:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case 2:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case 3:
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;
                default:
                    break;
            }
    }

    private void OnMouseEnter()
    {
        if (mCam.GetComponent<UI>().nUI == false)
        {
            Highlight.SetActive(true);
            mCam.GetComponent<UI>().selectedtill = this;
        }
    }

    private void OnMouseExit()
    {
        Highlight.SetActive(false);
    }

    public Tile GetTileAtPos(Vector2 pos)
    {
        if (Tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }

    public int randomize()
    {
        return Random.Range(-2, 3);
    }

    public int getD(Vector2 paraPos)
    {
        return Mathf.RoundToInt((pos - paraPos).magnitude) ;
    }

}
