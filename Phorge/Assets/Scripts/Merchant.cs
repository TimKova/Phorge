using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Merchant : MonoBehaviour
{
    public readonly Vector3 MENU_TLC = new Vector3(-800, 256f, 9f);
    public const float buttonOffset = 176f;


    public GameObject player_manager;
    private static Player_Inventory playerInventory;
    public GameObject buttonPrefab;
    public List<GameObject> buttons = new List<GameObject>();

    int currentMaterial;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = player_manager.GetComponent<Player_Inventory>();
        for (int c = 0; c < Player_Inventory.numMaterials; c++)
        {
            string name = Player_Inventory.materialNames[c];

            GameObject templateButton = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
            templateButton.transform.SetParent(this.gameObject.transform.GetChild(0));
            templateButton.transform.localScale = new Vector3(1, 1, 1);
            templateButton.transform.localPosition = new Vector3(MENU_TLC.x + buttonOffset * (c % 10), MENU_TLC.y - buttonOffset * (c / 10), MENU_TLC.z);
            templateButton.transform.localRotation = Quaternion.identity;

            int localIndex = c;
            (templateButton.GetComponentAtIndex(3) as Button).onClick.AddListener(delegate { buyUp(localIndex); });

            var icon = Resources.Load<Texture2D>(name + "Icon");
            (templateButton.transform.GetChild(0).gameObject.GetComponentAtIndex(2) as Image).sprite = Sprite.Create(icon, new Rect(0.0f, 0.0f, icon.width, icon.height), new Vector2(0.5f, 0.5f), 100.0f);

            var title = (templateButton.transform.GetChild(1).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI);
            title.text = name;

            var currentAmount = (templateButton.transform.GetChild(2).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI);
            currentAmount.gameObject.tag = "materialQuantity";
            currentAmount.gameObject.name = "materialQuantity" + c;
            currentAmount.text = playerInventory.ingots[c].getQuantity()+"";
            
            buttons.Add(templateButton);
            //Material ingotMat = Resources.Load(ingotName) as Material;
            //templateIngot.GetComponent<Renderer>().material = ingotMat;
            //templateIngot.tag = "ingotPrefab";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buyUp(int index)
    {
        print(index);
        float price = playerInventory.ingots[index].getPrice();
        print("Buying " + playerInventory.ingots[index].getName() + " for " + price);
        Player_Inventory.money -= price;
        print("you now have $" + Player_Inventory.money);
        //return price;
        playerInventory.purchaseMaterial(index);
        setCount(index);
    }

    public void refreshQuantities()
    {
        for (int i = 0; i < Player_Inventory.numMaterials; i++)
        {
            setCount(i);
        }
    }

    
    public void setCount(int matIndex)
    {
        //print("BOOYAH");
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == ("materialQuantity"+matIndex))
            {
                textComp.text = (playerInventory.getMaterial(matIndex).getQuantity() + "");
            }
        }
        //return materials[matIndex].getQuantity();
    }
}
