using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width, length;
    public float waterPerc;
    public float HousePerc;
    public int SBRad;
    public int SBFreq;    
    public int FFRad;
    public int FFFreq;
    public int PLRad;
    public int PLFreq;    
    public int RRad;
    public int RFreq;
    public Vector2 TownCenter = new Vector2(-10000, -10000);
    bool TCChosen = false;
    int landtiles;
    int People = 0;
    public Tile tilePrefab;

    public Camera cam;

    public Dictionary<Vector2, Tile> Tiles;

    private void Start()
    {
        GenerateGrid();
        
    }

    void GenerateGrid()
    {
        Tiles = new Dictionary<Vector2, Tile>();

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < length; x++)
            {

                var SpawnedTile = Instantiate(tilePrefab, new Vector3(x,y), Quaternion.identity);
                SpawnedTile.name = "Tile" + x + " " + y;

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                SpawnedTile.Init(this, new Vector2(x,y));

                Tiles[new Vector2(x, y)] = SpawnedTile;
            }
        }

        GenerateWater();
        DetermineTC();
        cam.transform.position = new Vector3((float)width/2 - .5f, (float)length / 2 - .5f, -10);
    }

    void GenerateWater()
    {
        Vector2 WaterStart = new Vector2(Random.Range(0, width), Random.Range(0, width));
        Vector2 WaterCursorPos = WaterStart;

        var till = GetTileAtPos(WaterStart);
        int waterTiles = Mathf.FloorToInt((width * length) * (waterPerc/100));
        landtiles = width * length;

        if (till != null)
        {
            while (((WaterCursorPos.x <= width && WaterCursorPos.x >= 0) || (WaterCursorPos.y <= length  && WaterCursorPos.y >= 0)) && waterTiles > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int NextWaterTile = Random.Range(0, 4);

                    if (i == 0 && i == NextWaterTile)
                    {
                        Tiles.TryGetValue(new Vector2(WaterCursorPos.x - 1, WaterCursorPos.y), out var tilt);
                        WaterCursorPos = new Vector2(WaterCursorPos.x - 1, WaterCursorPos.y);
                        if (tilt != null)
                        {
                            tilt.isWater = true;
                            tilt.BuildingNos = 19;
                            tilt.isDirty = true;
                            waterTiles--;
                            landtiles--;
                        }
                    }
                    else if (i == 1 && i == NextWaterTile)
                    {
                        Tiles.TryGetValue(new Vector2(WaterCursorPos.x, WaterCursorPos.y + 1), out var tilt);
                        WaterCursorPos = new Vector2(WaterCursorPos.x, WaterCursorPos.y + 1);
                        if (tilt != null)
                        {
                            tilt.isWater = true;
                            tilt.BuildingNos = 19;
                            tilt.isDirty = true;
                            waterTiles--;
                            landtiles--;
                        }
                    }
                    else if (i == 2 && i == NextWaterTile)
                    {
                        Tiles.TryGetValue(new Vector2(WaterCursorPos.x + 1, WaterCursorPos.y), out var tilt);
                        WaterCursorPos = new Vector2(WaterCursorPos.x + 1, WaterCursorPos.y);
                        if (tilt != null)
                        {
                            tilt.isWater = true;
                            tilt.BuildingNos = 19;
                            tilt.isDirty = true;
                            waterTiles--;
                            landtiles--;
                        }
                    }
                    else if (i == 3 && i == NextWaterTile)
                    {
                        Tiles.TryGetValue(new Vector2(WaterCursorPos.x, WaterCursorPos.y - 1), out var tilt);
                        WaterCursorPos = new Vector2(WaterCursorPos.x, WaterCursorPos.y - 1);
                        if (tilt != null)
                        {
                            tilt.isWater = true;
                            tilt.BuildingNos = 19;
                            tilt.isDirty = true;
                            waterTiles--;
                            landtiles--;
                        }
                    }
                }

                if (!((WaterCursorPos.x <= width && WaterCursorPos.x >= 0) || (WaterCursorPos.y <= length && WaterCursorPos.y >= 0)))
                {
                    WaterCursorPos = new Vector2(Random.Range(0, width), Random.Range(0, width));
                }
            }
        }

    }

    void GenerateLayer1()
    {
        foreach (var tile in Tiles) 
        {
            #region houses/Base Layer
            int temp = Random.Range(0, 10);

            if (temp < ((HousePerc / 10) - (tile.Value.getD(TownCenter) / (width * .1))) + tile.Value.randomize()) //if it passes the house filter
            {
                if (tile.Value.getD(TownCenter) > width * .69) //if outside of 69% of distance from TC, make farmhouse
                    temp = 16;
                else if (tile.Value.getD(TownCenter) > width / 5) //if outside of TC radius, make house
                {
                    temp = 7;
                }
            }
            else //for whatever doesnt become a house
            {
                temp = Random.Range(8, 10); //random from forest to grasss

                if (checkRadiusforspriteAmt(tile.Value.pos, 1, 16) > 0 && temp == 8) //if there is a farmhouse in a 3x3 radius
                {
                    temp = 17; //make the tile a farm
                }
            }

            if (tile.Value.BuildingNos < 10)
                tile.Value.BuildingNos = temp;

            #endregion
            if (tile.Value.getD(TownCenter) == 0)
            {
                tile.Value.BuildingNos = 26;
            } //LAYER 1
            else if (tile.Value.getD(TownCenter) < width / 4)
            {
                //int scvar = Mathf.FloorToInt((tile.Value.getD(TownCenter) / (width / 5)) + (Random.Range(0.1f, 1) * Random.Range(1, 4)));
                ////tries to randomize the skyscraper generation inside of the town center radius.
                //if (scvar == 1)//if scvar gets floor'd to 0 then make the tile a skyscraper
                //{
                //    tile.Value.BuildingNos = 0;
                //    foreach (Transform child in tile.Value.transform)
                //    {
                //        GameObject.Destroy(child.gameObject);
                //    }
                //    tile.Value.isDirty = true;
                //}

                if (temp == 9 || temp == 8)
                {
                    int qic = Random.Range(8, 15);
                    if (qic > 12)
                    {
                        int dec = Random.Range(0, 1);
                        if (dec == 0)
                            temp = 28;
                        else
                            temp = 29;
                        tile.Value.BuildingNos = temp;
                    }

                    else if (qic > 11)
                        tile.Value.BuildingNos = 27;
                }


            } //if inside of twoncenter radius, division number decides the "buffer zone" bigger == smaller zone.
            else if (tile.Value.BuildingNos == 8 || tile.Value.BuildingNos == 9) //if it got this far and is a forest/grass
            {
                if (checkRadiusforspriteAmt(tile.Value.pos, PLRad, 7) > PLFreq && checkRadiusforspriteAmt(tile.Value.pos, PLRad, 10) < 1)
                {
                    int plazavar = 10;
                    Vector2 anchor = tile.Value.pos;
                    Vector2 usedpos;
                    for (int y = 0; y < 2; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            usedpos = anchor + new Vector2(x, -y);
                            if (GetTileAtPos(usedpos) != null)
                            {
                                GetTileAtPos(usedpos).BuildingNos = plazavar;
                                foreach (Transform child in GetTileAtPos(usedpos).transform)
                                {
                                    GameObject.Destroy(child.gameObject);
                                }
                                GetTileAtPos(usedpos).isDirty = true;
                            }
                            plazavar++;
                        }
                    }
                } // Plazas
                else if (checkRadiusforspriteAmt(tile.Value.pos, SBRad, 7) > SBFreq && checkRadiusforspriteAmt(tile.Value.pos, SBRad, 1) < 1)
                {
                    tile.Value.BuildingNos = 1;
                    foreach (Transform child in tile.Value.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    tile.Value.isDirty = true;
                } // small businesses
                else if (checkRadiusforspriteAmt(tile.Value.pos, SBRad, 7) > SBFreq && checkRadiusforspriteAmt(tile.Value.pos, SBRad, 27) < 1)
                {
                    tile.Value.BuildingNos = 27;
                    foreach (Transform child in tile.Value.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    tile.Value.isDirty = true;
                } // city services
                else if (checkRadiusforspriteAmt(tile.Value.pos, RRad, 7) > RFreq && checkRadiusforspriteAmt(tile.Value.pos, RRad, 5) < 1)
                {
                    tile.Value.BuildingNos = 5;
                    foreach (Transform child in tile.Value.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    tile.Value.isDirty = true;
                } // restaurants

                else if (checkRadiusforspriteAmt(tile.Value.pos, FFRad, 7) > FFFreq && checkRadiusforspriteAmt(tile.Value.pos, FFRad, 3) < 3)
                {
                    tile.Value.BuildingNos = 3;
                    foreach (Transform child in tile.Value.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    tile.Value.isDirty = true;
                } // Fast Food
            }


            tile.Value.LayerPass();
        }
    }

    void DetermineTC()
    {
        //choose random tile
        //check if chosen tile is water
        //check 10 tiles around to see for water
        //repeat until found

        while (!TCChosen)
        {
            //find random x and y values between width and length
            //use those values as tryget argument
            int Vx = Random.Range(0, width);
            int Vy = Random.Range(0, length);
            bool toBreak = false;
            Vector2 temp = new Vector2(Vx, Vy);
            Debug.Log(temp);
            //two for loops starting the vars at -3 and going to +3 creates squares for pos
            toBreak = checkRadiusForwater(temp, 3);

            if (!toBreak)
            {
                TCChosen = true;
                TownCenter = temp;
                //make spiraling placement from TC.
                //foreach (var tile in Tiles)
                //{
                //    tile.Value.ChangeSprite();
                //}
            }

            GenerateLayer1();
        }
    }

    bool checkRadiusForwater(Vector2 paraV2, int paraRadius)
    {
        bool toreturn = false;

        for (int x = -(paraRadius); x <= paraRadius; x++)
        {
            for (int y = -(paraRadius); y <= paraRadius; y++)
            {
                Vector2 tempcheck = paraV2;

                if (GetTileAtPos(tempcheck) != null)
                {
                    Tile CurrTill = GetTileAtPos(tempcheck);
                    if (CurrTill.isWater)
                    {
                        toreturn = true;
                    }

                    if (tempcheck.x >= length || tempcheck.x <= 0 || tempcheck.y >= width || tempcheck.y <= 0)
                    {
                        toreturn = true;
                    }
                }
            }
        }

        return toreturn;
    }

    public int checkRadiusforspriteAmt(Vector2 paraV2, int paraRadius, int sprite)
    {
        int toreturn = 0;

        for (int x = -(paraRadius); x <= paraRadius; x++)
        {
            for (int y = -(paraRadius); y <= paraRadius; y++)
            {
                Vector2 tempcheck = new Vector2(paraV2.x + x, paraV2.y + y);

                if (GetTileAtPos(tempcheck) != null)
                {
                    Tile CurrTill = GetTileAtPos(tempcheck);
                    if (CurrTill.BuildingNos == sprite)
                    {
                        toreturn++;
                    }

                    //if (tempcheck.x >= length || tempcheck.x <= 0 || tempcheck.y >= width || tempcheck.y <= 0)
                    //{
                    //    toreturn++;
                    //}
                }
            }
        }

        return toreturn;
    }

    public Tile GetTileAtPos(Vector2 pos)
    {
        if (Tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }
}
