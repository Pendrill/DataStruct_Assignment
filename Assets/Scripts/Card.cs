using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    private string p_name;
    private int p_value;
    private bool p_visible;
    private bool p_inDeck;
    private Material p_frontMat, p_backMat;
    private GameObject p_physicalCard;

    public Card()
    {
        p_name = "";
        p_value = 0;
        p_visible = false;
        p_inDeck = false;
        p_physicalCard = null;
        p_frontMat = null;
        p_backMat = GameManager.instance.cardBack;
        
    }
    public Card(int value, int suit, int counter): this(){
        
        if(value <= 10)
        {
            //Debug.Log(suit);
            p_name = (value) + "_" + GameManager.instance.cardSuits[suit];
            p_value = value;
        }else if(value > 10){
            //Debug.Log(value);
            p_name = GameManager.instance.cardFace[value]+ "_" + GameManager.instance.cardSuits[suit];
            p_value = 10;
        }
        p_frontMat = GameManager.instance.cardMaterial[counter];
        p_physicalCard = GameObject.Instantiate(GameManager.instance.physicalCard, new Vector3(0, 0, 0), Quaternion.identity);
        p_physicalCard.GetComponent<MeshRenderer>().material = p_frontMat;
    }

    public string name
    {
        get
        {
            return p_name;
        }
    }

    public int value
    {
        get
        {
            return p_value;
        }
    }
    public bool inDeck
    {
        get
        {
            return p_inDeck;
        }
        set
        {
            this.p_inDeck = value;
        }
    }
	



}
