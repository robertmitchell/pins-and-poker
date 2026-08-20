using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsShufflerPopup : MonoBehaviour
{
    public Image cardImage;           // Image component for displaying the card
    public Sprite cardBackSprite;     // Sprite for the back of the card
    public List<Sprite> deckOfCards;  // List of all 52 card sprites

    public float flipDuration = 1.0f; // Total time to complete one flip (in seconds)
    public float flipFrequency = 2.0f; // Frequency of flip (in seconds between each flip)

    private int currentCardIndex = 0; // Tracks the current card in the deck
    private bool isFlipping = false;

    void Start()
    {
        if (deckOfCards.Count == 0)
        {
            Debug.LogError("No card sprites assigned to deckOfCards!");
            return;
        }

        // Start the flip loop
        InvokeRepeating(nameof(StartFlip), flipFrequency, flipFrequency);
    }

    private void StartFlip()
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipAnimation());
        }
    }

    private IEnumerator FlipAnimation()
    {
        isFlipping = true;

        float halfDuration = flipDuration / 2;
        float time = 0f;

        // First half of the flip (scale down to 0 on the X-axis)
        while (time < halfDuration)
        {
            cardImage.transform.localScale = new Vector3(Mathf.Lerp(1, 0, time / halfDuration), 1, 1);
            time += Time.deltaTime;
            yield return null;
        }

        // Swap the card sprite to a random card at the halfway point
        cardImage.sprite = GetRandomCardSprite();

        // Second half of the flip (scale up from 0 to 1 on the X-axis)
        time = 0f;
        while (time < halfDuration)
        {
            cardImage.transform.localScale = new Vector3(Mathf.Lerp(0, 1, time / halfDuration), 1, 1);
            time += Time.deltaTime;
            yield return null;
        }

        cardImage.transform.localScale = Vector3.one; // Reset scale
        isFlipping = false;
    }

    private Sprite GetRandomCardSprite()
    {
        // Choose a random card sprite from the deck
        currentCardIndex = Random.Range(0, deckOfCards.Count);
        return deckOfCards[currentCardIndex];
    }
}
