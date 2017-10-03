using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    //set the gamemanager as a singleton, as to be accessed from the card class
    public static GameManager instance = new GameManager();
    //string array that contains all the suits
    string[] suit = { "clover", "diamond", "heart", "spade" };
    //string array that contains the name of face cards
    string[] face = { "Jack", "Queen", "King" };
    //int array that keeps track of specific value identifiers for cards
    int[] number = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14 };
    //Card array that keeps track of all the cards
    Card[] cardReference = new Card[52];
    //gameObject which is a reference to the template card
    public GameObject physicalCard;
    //Material array that holds all front card materials
    public Material[] cardMaterial = new Material[52];
    //Material that holds back of card material
    public Material cardBack;
    //Dictionary that assigns the Suit of a card based on Key value. used for the name of the card in card class
    public Dictionary<int, string> cardSuits = new Dictionary<int, string>();
    //Dictionary that assigns the number value to a card based on key. used for the name of the card in card class
    public Dictionary<int, string> cardFace = new Dictionary<int, string>();
    //A queue which will represent the deck of cards
    public Queue Deck = new Queue();
    //A list which represents the cards the player holds
    public List<Card> playerHand = new List<Card>();
    //a list which represents the cards the AI holds
    public List<Card> AIHand = new List<Card>();
    //The various gamestates the game can be in
    enum GameState { wait, start, PlayerAction, AIAction, GameOver, Restart }
    //keeps track of the current state
    GameState currentState;
    //keeps track of time tsince last state change
    float lastStateChange = 0.0f;
    //vector 3 keeping track of the starting card postion for the player and AI, but also both their current positions in the scene
    Vector3 playerCardStartingPosition = new Vector3(10, -3, 4), AICardStartingPosition = new Vector3(-10f, 5.5f, 4f),
        currentPlayerPosition, currentAIPosition;
    //The value of the hand for both the player and the AI
    public int playerValue = 0, AIValue = 0;
    //bools to see if the player or AI has an ace. Also if the player won
    bool playerHasAce = false, AIHasAce = false, playerWins = false;
    //references to the buttons in the scene
    public Button HitMe,Stay, Exit, Restart;
    //reference to the UI game over text
    public Text gameOverText;


    // Use this for initialization
    void Start () {
        //Initialize the singleton
        instance = this;
        //intialize the curent position of the player and AI's cards
        currentPlayerPosition = playerCardStartingPosition;
        currentAIPosition = AICardStartingPosition;
        //set counter to 0
        int counter = 0;
        //We add the card suits to the dictionary based on a key value 0-3
        for (int i = 0; i < 4; i++)
        {
            cardSuits.Add(i, suit[i]);
        }
        //We do the same for the face card 12-14;
        for (int i = 12; i< 15; i++)
        {
            //Debug.Log(counter); 
            cardFace.Add(i, face[counter]);
            counter += 1;
            
        }
        //we create all the cards
        createAllCards();
        //we shuffle the cards, and add them to the deck
        shuffleCards();
        //we set the gamestate to start
        setCurrentState(GameState.start);
        //debugCard();
        //debugDeck();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(currentState);
        //Use a switch statement to access the various states.
        switch (currentState)
        {
            //wait, start, PlayerAction, AIAction, GameOver, Restart
            //wait is used while waiting for the player to do something and thus access another state
            case GameState.wait:

                break;
            //The starting game state
            case GameState.start:
                //remove the Hit me and Stay buttons
                HitMe.gameObject.SetActive(false);
                Stay.gameObject.SetActive(false);
                //Get a reference for the first two cards of the player and AI.
                //These are the first 4 cards from the deck
                //we also remove those cards from deck
                Card tempPlayerCard1 = (Card)Deck.Dequeue();
                Card tempAICard1 = (Card)Deck.Dequeue();
                Card tempPlayerCard2 = (Card)Deck.Dequeue();
                Card tempAICard2 = (Card)Deck.Dequeue();
                //We add those cards to the Hands of the player and AI
                playerHand.Add(tempPlayerCard1);
                playerHand.Add(tempPlayerCard2);
                AIHand.Add(tempAICard1);
                AIHand.Add(tempAICard2);
                //we then deal the cards in the scene
                DealCards();
                //it is now the player turn to act
                setCurrentState(GameState.PlayerAction);
                break;

            //This is when the player can act
            case GameState.PlayerAction:
                //Enable the hit me and stay buttons
                HitMe.gameObject.SetActive(true);
                Stay.gameObject.SetActive(true);
                //move to the wait state
                setCurrentState(GameState.wait);                
                break;

            //This is when it is the Ai that must act
            case GameState.AIAction:
                //we remove the player's action buttons
                HitMe.gameObject.SetActive(false);
                Stay.gameObject.SetActive(false);
                //The AI will act every half second if the value of their hand is lower than that of its oponent
                if (getStateElapsed() > 0.5f && AIValue < playerValue)
                {
                    //gets a new card
                    AIHit();
                    //reset the timer
                    lastStateChange = Time.time;

                }
                //we update the value of the Ai and players hand
                updateValue();
                //if the AI has a better hand than the player 
                if(AIValue >= playerValue && currentState != GameState.GameOver)
                {
                    //then the player loses
                    playerWins = false;
                    //and we move to the game over state
                    setCurrentState(GameState.GameOver);
                }
                break;

            //This is when the game is over
            case GameState.GameOver:
                //player action buttons are disabled
                HitMe.gameObject.SetActive(false);
                Stay.gameObject.SetActive(false);
                //we check to see whether the player won or lost
                if (!playerWins)
                {
                    //Lose text
                    gameOverText.text = "YOU LOSE: " + playerValue + " VS. " + AIValue;
                }else
                {
                    //win text
                    gameOverText.text = "YOU WIN: " + playerValue + " VS. " + AIValue;
                }
                //Exit and restart button appear
                Exit.gameObject.SetActive(true);
                Restart.gameObject.SetActive(true);
                break;

            //This is the state where the game need to restarts
            case GameState.Restart:
                //goes to the restart function
                restart();
                break;
            
        }
        //will update the value of the player and AI
        updateValue();
	}

    /// <summary>
    /// the player asks to receive an other card. Accessed through a button in the scene
    /// </summary>
    public void hitMe()
    {
        //keep a temp reference to the card
        Card tempPlayerCard1 = (Card)Deck.Dequeue();
        //add the card to the hand
        playerHand.Add(tempPlayerCard1);
        //instantiate and display the card in the scene
        playerHand[playerHand.Count-1].physicalCard = Instantiate(physicalCard, currentPlayerPosition, physicalCard.transform.rotation);
        //check the card if it is an ace
        if (playerHand[playerHand.Count-1].value == 1)
        {
            //set the value to 11
            playerHand[playerHand.Count - 1].value = 11;
            //add it the the player's value
            playerValue += playerHand[playerHand.Count - 1].value;
            //the player now has an ace in their hand
            playerHasAce = true;
        }
        //if it is not an ace
        else
        {
            //just add the value of the card to the player
            playerValue += playerHand[playerHand.Count - 1].value;
        }
        //update the new position for the next card
        currentPlayerPosition.x -= 5;
    }

    /// <summary>
    /// the player decides to keep their hand and let the AI play.
    /// Accessed from a button in the scene
    /// </summary>
    public void stay()
    {
        //set the state to the AI Action
        setCurrentState(GameState.AIAction);
    }

    /// <summary>
    /// the Ai wants a new card
    /// </summary>
    public void AIHit()
    {
        //keep a temp reference to the card
        Card tempAICard1 = (Card)Deck.Dequeue();
        //add it to the hand 
        AIHand.Add(tempAICard1);
        //add it to the scene
        AIHand[AIHand.Count - 1].physicalCard = Instantiate(physicalCard, currentAIPosition, physicalCard.transform.rotation);
        //if Ace
        if (AIHand[AIHand.Count - 1].value == 1)
        {
            //change value to 11
            AIHand[AIHand.Count - 1].value = 11;
            //add to AI value
            AIValue += AIHand[AIHand.Count - 1].value;
            //Ai now has ace
            AIHasAce = true;
        }
        else//if not
        {
            //simply add value to AI
            AIValue += AIHand[AIHand.Count - 1].value;
        }
        //update position for next card
        currentAIPosition.x += 5;
    }

    /// <summary>
    /// function used at the start, will instantiate all the necessary card objects.
    /// </summary>
    void createAllCards()
    {
        //keep temp counter
        int counter = 0;
        //go through the value of the cards
        for (int i = 0; i < 13; i++)
        {
            //nested for loop with the suit of the card
            for(int j = 0; j < 4; j++)
            {
                //cards are ordered based on suit and then value
                //create all the 52 cards and keep reference
                cardReference[i + (j * 13)] = new Card(number[i], j, counter);
                //increase counter
                counter++;
            }
        }
    }

    /// <summary>
    /// We randomly pick a card from the reference and add to a queue which is out deck
    /// </summary>
    void shuffleCards()
    {
        //keep ref to temp counter
        int counter = 0;
        //while counter is below 52, which means until deck has 52 cards
        while(counter < 52)
        {
            //get a random number which will serve as index
            int randInt = Random.Range(0, 52);
            //check if card at that index is in the deck
            if (!cardReference[randInt].inDeck)//if no
            {
                //add it to the deck
                Deck.Enqueue(cardReference[randInt]);
                //set its indeck bool to true
                cardReference[randInt].inDeck = true;
                //and increase counter
                counter++;
            }else//if it is already in the deck
            {
                //set temp bool to true
                bool temp = true;
                //while temp is true
                while (temp)
                {
                    //we check if index is a the last spot in array
                    if(randInt == 51)
                    {
                        //we loop back to start
                        randInt = 0;
                    }else//if it is not
                    {
                        //increase the rand index by 1
                        randInt += 1;
                    }
                    //we are checking if the next card is in the deck
                    if (!cardReference[randInt].inDeck)//if it isn't
                    {
                        //then we add it to the deck
                        Deck.Enqueue(cardReference[randInt]);
                        //set in deck to true
                        cardReference[randInt].inDeck = true;
                        //increase counter
                        counter++;
                        //and stop nested while loop
                        temp = false;
                    }//if it is while loop will keep going and index will increase
                }
            }
        }
    }

    /// <summary>
    /// check to see what the values of the player and AI are based on the cards in their hands
    /// </summary>
    void updateValue()
    {
        //check player first
        //reset the player value
        playerValue = 0;
        
        //go through their cards
        for(int i = 0; i< playerHand.Count; i++)
        {
            //add up all values
            playerValue += playerHand[i].value;
        }
        //check if player is over 21 (maybe lost)
        if(playerValue > 21)
        {
            //check if they have an ace
            if (playerHasAce)
            {
                //if they do, replace the value of the ace from 11 to 1
                //the player thus can no longer reuse the ace
                playerHasAce = false;
                //go through the hand of the player
                for(int i = 0; i < playerHand.Count; i++)
                {
                    //check for ace
                    if(playerHand[i].value == 11)
                    {
                        //reset the values of ace and hand
                        playerHand[i].value = 1;
                        playerValue -= 10;
                    }
                }
                //check again to see if still above 21
                if(playerValue > 21)
                {
                    //if yes that its game over, the plaeyr lost
                    setCurrentState(GameState.GameOver);
                    playerWins = false;
                    return;
                }
            }else // if they don't have an ace and are above 21
            {
                //then it is game over and the player lost
                setCurrentState(GameState.GameOver);
                playerWins = false;
                return;
            }
        }
        //we then reset the AI value
        AIValue = 0;
        //we go through its cards
        for (int i = 0; i < AIHand.Count; i++)
        {
            //and add the value up
            AIValue += AIHand[i].value;
        }
        //we check if it is over 21
        if (AIValue > 21)
        {
            //we check if it has an ace
            if (AIHasAce)//if yes
            {
                // we replace the value of the ace from 11 to 1
                //the player thus can no longer reuse the ace
                AIHasAce = false;
                //go through the hand again
                for (int i = 0; i < AIHand.Count; i++)
                {
                    //check for aces
                    if (AIHand[i].value == 11)
                    {
                        //reset ace and value
                        AIHand[i].value = 1;
                        AIValue -= 10;
                    }
                }
                //check again to see if above 21
                if (AIValue > 21)
                {
                    //if yes then the player wins
                    setCurrentState(GameState.GameOver);
                    playerWins = true;
                    return;
                }
            }
            else//if hte Ai is above 21 and does not have an ace
            {
                //then the player wins
                setCurrentState(GameState.GameOver);
                playerWins = true;
                return;
            }
        }


    }

    /// <summary>
    /// This is the function that will initially deal the first two cards to the player and AI
    /// </summary>
    void DealCards()
    {
        //Goes through 2
        for( int i =0; i<2;i++)
        {
            //Instantiates and adds a card to the players hand
            playerHand[i].physicalCard = Instantiate(physicalCard, currentPlayerPosition, physicalCard.transform.rotation);
            //checks to see if the player has an ace
            if(playerHand[i].value == 1)
            {
                //if yes then we change its value from 1 to 11
                playerHand[i].value = 11;
                //add the value to the player
                playerValue += playerHand[i].value;
                //and indicate that the player has an ace
                playerHasAce = true;
            }
            //if it is not an ace
            else
            {
                //then we add the value of the card to the player
                playerValue += playerHand[i].value;
            }

            //We do the same process for the AI
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
            //We update the positions for where the next cards for the player and AI should be displayed in the scene
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
        //udapte the state and the time since the state has been changes
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
        //goes through the card refernces and prints their name and value
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
        //goes through the deck and prints the cards one by one
        for (int i = 0; i < cardReference.Length; i++)
        {
            Card tempCard = (Card) Deck.Dequeue();
            Debug.Log(tempCard.name);
        }
    }

    /// <summary>
    /// exits game. Accessed from a button in the scene
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
    /// <summary>
    /// restart the game by setting the game state to restart.Accessed from a button in the scene
    /// </summary>
    public void restartButton()
    {
        setCurrentState(GameState.Restart);
    }
    /// <summary>
    /// resets all the important values of the game as to restart it
    /// </summary>
    void restart()
    {
        //clear the deck
        Deck.Clear();

        //we reset all aces to 1 in values in the player's hand
        for(int i = 0; i < playerHand.Count; i++)
        {
            if(playerHand[i].value == 11)
            {
                playerHand[i].value = 1;
            }
            //and destroy the game object cards
            Destroy(playerHand[i].physicalCard);
        }
        //and clear the hand
        playerHand.Clear();
        //we do the same for the AI's hand
        for (int i = 0; i < AIHand.Count; i++)
        {
            if (AIHand[i].value == 11)
            {
                AIHand[i].value = 1;
            }
            Destroy(AIHand[i].physicalCard);
        }
        AIHand.Clear();

        //remove the restart and exit button, and game over text
        Restart.gameObject.SetActive(false);
        Exit.gameObject.SetActive(false);
        gameOverText.text = "";

        //player and AI value are back to 0
        playerValue = 0;
        AIValue = 0;
        //player has nto yet won
        playerWins = false;
        //player and AI no longer hold aces
        playerHasAce = false;
        AIHasAce = false;
        //current card positions are reset
        currentPlayerPosition = playerCardStartingPosition;
        currentAIPosition = AICardStartingPosition;
        //none of the cards are in the deck
        for(int i = 0; i < cardReference.Length; i++)
        {
            cardReference[i].inDeck = false;
        }
        //we reshuffle the deck
        shuffleCards();
        //and we set the game to start.
        setCurrentState(GameState.start);
    }
}
