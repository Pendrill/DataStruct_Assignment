using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a card class. It holds all the info relevant to the card object
/// </summary>
public class Card {

    //name of the card, for example Q_hearts, which represents the Queen of Hearts
    private string p_name;
    //value of the card ragind from 1 to 11
    private int p_value;
    //wether the card is visible in the scene (never used)
    private bool p_visible;
    //whether the card is still in the deck of cards
    private bool p_inDeck;
    //materials for the front and back of the specific card (only front is used)
    private Material p_frontMat, p_backMat;
    //reference to the card game object in the scene
    private GameObject p_physicalCard;

    /// <summary>
    /// card default constructor. Sets everything to its defaul, except for card back (which we don't end up using)
    /// </summary>
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
    /// <summary>
    /// more precise constructor which inherits from the one above. Takes a value, suit (Hearts, Spade, etc) and a counter which 
    /// will serve as the index for the array holding the card material
    /// </summary>
    /// <param name="value"></param>
    /// <param name="suit"></param>
    /// <param name="counter"></param>
    public Card(int value, int suit, int counter): this(){
        
        //if the value of the card is below 10
        if(value <= 10)
        {
            //then it is not a Jack King or Queen
            //Debug.Log(suit);
            //we set the name of the card, example Q_hearts, which represents the Queen of Hearts
            p_name = (value) + "_" + GameManager.instance.cardSuits[suit];
            //we keep a reference to the value of the card
            p_value = value;
        }else if(value > 10){ //if the card is a Jack Queen or King
            //Debug.Log(value);
            //Set the name accordingly
            p_name = GameManager.instance.cardFace[value]+ "_" + GameManager.instance.cardSuits[suit];
            //And keep track of the value
            p_value = 10;
        }
        //keep track of the material for the front of the card as well.
        p_frontMat = GameManager.instance.cardMaterial[counter];
        //p_physicalCard = GameObject.Instantiate(GameManager.instance.physicalCard, new Vector3(0, 0, 0), GameManager.instance.physicalCard.transform.rotation);
        //p_physicalCard.GetComponent<MeshRenderer>().material = p_frontMat;
    }

    /// <summary>
    /// getter for the name of the card
    /// </summary>
    public string name
    {
        get
        {
            return p_name;
        }
    }

    /// <summary>
    /// getter and setter for the value of the card
    /// </summary>
    public int value
    {
        get
        {
            return p_value;
        }
        set
        {
            this.p_value = value;
        }
    }

    /// <summary>
    /// getter and setter for the bool that checks whether the card is in the deck
    /// </summary>
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
    /// <summary>
    /// getter and setter for the gameobject representing the card in the scene
    /// </summary>
    public GameObject physicalCard
    {
        get
        {
            return p_physicalCard;
        }
        set
        {
            this.p_physicalCard = value;
            //we also need to apply the front material to the card
            p_physicalCard.GetComponent<MeshRenderer>().material = p_frontMat;
        }
    }
    /// <summary>
    /// getter and setter to the front material of the card
    /// </summary>
    public Material frontMat
    {
        get
        {
            return p_frontMat;
        }
        set
        {
            this.p_frontMat = value;
            
        }
    }
	



}
