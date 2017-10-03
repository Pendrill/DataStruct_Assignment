using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = new GameManager();
    string[] suit = { "clover", "diamond", "heart", "spade" };
    string[] face = { "Jack", "Queen", "King" };
    int[] number = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14 };
    Card[] cardReference = new Card[52];
    public GameObject physicalCard;
    public Material[] cardMaterial = new Material[52];
    public Material cardBack;
    public Dictionary<int, string> cardSuits = new Dictionary<int, string>();
    public Dictionary<int, string> cardFace = new Dictionary<int, string>();
    public Queue Deck = new Queue();
    // Use this for initialization
    void Start () {
        instance = this;
        int counter = 0;
        for (int i = 0; i < 4; i++)
        {
            cardSuits.Add(i, suit[i]);
        }
        for (int i = 12; i< 15; i++)
        {
            //Debug.Log(counter); 
            cardFace.Add(i, face[counter]);
            counter += 1;
            
        }
        createAllCards();
        shuffleCards();
        //debugCard();
        debugDeck();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// function used at the start, will instantiate all the necessary card objects.
    /// </summary>
    void createAllCards()
    {
        int counter = 0;
        for (int i = 0; i < 13; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                cardReference[i + (j * 13)] = new Card(number[i], j, counter);
                counter++;
            }
        }
    }
    void shuffleCards()
    {
        int counter = 0;
        while(counter < 52)
        {
            int randInt = Random.Range(0, 52);
            if (!cardReference[randInt].inDeck)
            {
                Deck.Enqueue(cardReference[randInt]);
                cardReference[randInt].inDeck = true;
                counter++;
            }else
            {
                bool temp = true;
                while (temp)
                {
                    if(randInt == 51)
                    {
                        randInt = 0;
                    }else
                    {
                        randInt += 1;
                    }
                    if (!cardReference[randInt].inDeck)
                    {
                        Deck.Enqueue(cardReference[randInt]);
                        cardReference[randInt].inDeck = true;
                        counter++;
                        temp = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Debug to check that the cards were created and added to the array properly
    /// </summary>
    void debugCard()
    {
        for(int i = 0; i<cardReference.Length; i++)
        {
            Debug.Log(cardReference[i].name + "\t" + cardReference[i].value);
        }
    }

    /// <summary>
    /// Debug to check that the cards were shuffled properly
    /// </summary>
    void debugDeck()
    {
        for (int i = 0; i < cardReference.Length; i++)
        {
            Card tempCard = (Card) Deck.Dequeue();
            Debug.Log(tempCard.name);
        }
    }
}
