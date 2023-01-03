using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject Panel;
    public Text PanelText;
    public Vector2 fcp;
    public bool nUI = false;
    public Tile selectedtill;
    public Camera cam;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (nUI == false)
            {
                nUI = true;
                Panel.SetActive(true);
                PanelText.text = "Value: " + selectedtill.Value + "\nBuildings: " + selectedtill.BuildingNos + "\nZoning" + selectedtill.Zoning;
                fcp = Input.mousePosition;
                Panel.transform.position = new Vector3(fcp.x, fcp.y, 0);
            }
            else
            {
                //check if mouse pos is in panel, if it is then move the panel, if its not close the panel.
                Vector3[] PanelCorners = new Vector3[4];
                Panel.GetComponent<RectTransform>().GetWorldCorners(PanelCorners);

                if ((Input.mousePosition.x > PanelCorners[0].x && Input.mousePosition.x < PanelCorners[3].x) && (Input.mousePosition.y > PanelCorners[0].y && Input.mousePosition.y < PanelCorners[2].y))
                {
                }
                else
                {
                    nUI = false;
                    Panel.SetActive(false);
                    fcp = Input.mousePosition;
                }
            }
        }
    }
}
