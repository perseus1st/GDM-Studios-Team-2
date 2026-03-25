using UnityEngine;
using TMPro;
using System.Linq;

public class PackingList : MonoBehaviour
{
    [SerializeField] private TMP_Text dodgeballText;
    [SerializeField] private TMP_Text racketsText;
    [SerializeField] private TMP_Text danceMatText;
    [SerializeField] private TMP_Text headoutText;

    private Color32 green = new Color32(0,255,15,255);
    private GameManager gameManager = GameManager.Instance;
    private string[] requiredGames = {"dodgeball", "badminton", "ddr"}; //add "kite" when implemented

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // badmintonText = transform.Find("Dodgeball Text").gameObject.GetComponent<TextMeshPro>();
        // racketsText = transform.Find("Rackets Text").gameObject.GetComponent<TextMeshPro>();
        // danceMatText = transform.Find("Dance Mat Text").gameObject.GetComponent<TextMeshPro>();
        // kiteText = transform.Find("Kite Text").gameObject.GetComponent<TextMeshPro>();

        if (gameManager.IsMinigameCompleted("dodgeball"))
        {
            dodgeballText.text = "<s>• Dodgeball </s>";
            dodgeballText.color = green;
        }
        if (gameManager.IsMinigameCompleted("badminton"))
        {
            racketsText.text = "<s>• Rackets </s>";
            racketsText.color = green;
        }
        if (gameManager.IsMinigameCompleted("ddr"))
        {
            danceMatText.text = "<s>• Dance Mat </s>";
            danceMatText.color = green;
        }

        if (requiredGames.All(game => gameManager.IsMinigameCompleted(game)))
        {
            dodgeballText.gameObject.SetActive(false);
            racketsText.gameObject.SetActive(false);
            danceMatText.gameObject.SetActive(false);
            //kiteMatText.gameObject.SetActive(false);

            headoutText.gameObject.SetActive(true);
        }
    }

}