using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public List<Card> playerHand = new List<Card>();
    public List<Card> AIHand = new List<Card>();
    enum GameState { wait, start, PlayerAction, AIAction, GameOver, Restart }
    GameState currentState;
    float lastStateChange = 0.0f;
    Vector3 playerCardStartingPosition = new Vector3(10, -3, 4), AICardStartingPosition = new Vector3(-10f, 5.5f, 4f),
        currentPlayerPosition, currentAIPosition;
    public int playerValue = 0, AIValue = 0;
    bool playerHasAce = false, AIHasAce = false, playerWins = false;

    // Use this for initialization
    void Start () {
        instance = this;
        currentPlayerPosition = playerCardStartingPosition;
        currentAIPosition = AICardStartingPosition;
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
        setCurrentState(GameState.start);
        //debugCard();
        //debugDeck();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(currentState);
        switch (currentState)
        {
            //wait, start, PlayerAction, AIAction, GameOver, Restart
            case GameState.wait:

                break;
            case GameState.start:
                Card tempPlayerCard1 = (Card)Deck.Dequeue();
                Card tempAICard1 = (Card)Deck.Dequeue();
                Card tempPlayerCard2 = (Card)Deck.Dequeue();
                Card tempAICard2 = (Card)Deck.Dequeue();
                playerHand.Add(tempPlayerCard1);
                playerHand.Add(tempPlayerCard2);
                AIHand.Add(tempAICard1);
                AIHand.Add(tempAICard2);
                DealCards();
                setCurrentState(GameState.PlayerAction);
                break;

            case GameState.PlayerAction:
                
                
                setCurrentState(GameState.wait);
                
                break;
            case GameState.AIAction:

                break;
            case GameState.GameOver:

                break;
            case GameState.Restart:

                break;
            
        }
        updateValue();
	}

    public void hitMe()
    {
        Card tempPlayerCard1 = (Card)Deck.Dequeue();
        playerHand.Add(tempPlayerCard1);
        playerHand[playerHand.Count-1].physicalCard = Instantiate(physicalCard, currentPlayerPosition, physicalCard.transform.rotation);
        if (playerHand[playerHand.Count-1].value == 1)
        {
            playerHand[playerHand.Count - 1].value = 11;
            playerValue += playerHand[playerHand.Count - 1].value;
        }
        else
        {
            playerValue += playerHand[playerHand.Count - 1].value;
        }
        currentPlayerPosition.x -= 5;
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

    void updateValue()
    {
        playerValue = 0;
        AIValue = 0;
        for(int i = 0; i< playerHand.Count; i++)
        {
            playerValue += playerHand[i].value;
        }
        if(playerValue > 21)
        {
            if (playerHasAce)
            {
                playerValue -= 10;
                playerHasAce = false;
                for(int i = 0; i < playerHand.Count; i++)
                {
                    if(playerHand[i].value == 11)
                    {
                        playerHand[i].value = 1;
                    }
                }
                if(playerValue > 21)
                {
                    setCurrentState(GameState.GameOver);
                    playerWins = false;
                    return;
                }
            }else
            {
                setCurrentState(GameState.GameOver);
                playerWins = false;
                return;
            }
        }
        for (int i = 0; i < AIHand.Count; i++)
        {
            AIValue += AIHand[i].value;
        }
        if (AIValue > 21)
        {
            if (AIHasAce)
            {
                AIValue -= 10;
                AIHasAce = false;
                for (int i = 0; i < AIHand.Count; i++)
                {
                    if (AIHand[i].value == 11)
                    {
                        AIHand[i].value = 1;
                    }
                }
                if (AIValue > 21)
                {
                    setCurrentState(GameState.GameOver);
                    playerWins = true;
                    return;
                }
            }
            else
            {
                setCurrentState(GameState.GameOver);
                playerWins = true;
                return;
            }
        }
    }

    void DealCards()
    {
        for( int i =0; i<2;i++)
        {
            playerHand[i].physicalCard = Instantiate(physicalCard, currentPlayerPosition, physicalCard.transform.rotation);
            if(playerHand[i].value == 1)
            {
                playerHand[i].value = 11;
                playerValue += playerHand[i].value;
                playerHasAce = true;
            }
            else
            {
                playerValue += playerHand[i].value;
            }
            AIHand[i].physicalCard = Instantiate(physicalCard, currentAIPosition, physicalCard.transform.rotation);
            if (AIHand[i].value == 1)
            {
                AIHand[i].value = 11;
                AIValue += AIHand[i].value;
                AIHasAce = true;
            }
            else
            {
                AIValue += AIHand[i].value;
            }
            currentPlayerPosition.x -= 5;
            currentAIPosition.x += 5;
        }
    }

    /// <summary>
    /// sets the current state of the game manager
    /// </summary>
    /// <param name="state"></param>
    void setCurrentState(GameState state)
    {
        currentState = state;
        lastStateChange = Time.time;
    }

    /// <summary>
    /// returns the amount of time that has passed since the last state change
    /// </summary>
    /// <returns></returns>
    float getStateElapsed()
    {
        return Time.time - lastStateChange;
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
