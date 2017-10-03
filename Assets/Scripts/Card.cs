using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    private string p_name;
    private int p_value;
    private bool p_visible;
    private bool p_inDeck;

    public Card()
    {
        p_name = "";
        p_value = 0;
        p_visible = false;
        p_inDeck = false;
        
    }
    public Card(int value, int suit): this(){
        
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
	



}
