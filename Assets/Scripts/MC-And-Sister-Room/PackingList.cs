using UnityEngine;
using TMPro;

public class PackingList : MonoBehaviour
{
    [SerializeField] private TMP_Text dodgeballText;
    [SerializeField] private TMP_Text racketsText;
    [SerializeField] private TMP_Text danceMatText;
    [SerializeField] private TMP_Text kiteText;

    private Color32 green = new Color32(0,255,15,255);
    private GameManager gameManager = GameManager.Instance;


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
        if (gameManager.IsMinigameCompleted("DDR"))
        {
            danceMatText.text = "<s>• Dance Mat </s>";
            danceMatText.color = green;
        }
        if (gameManager.IsMinigameCompleted("kite"))
        {
            kiteText.text = "<s>• Kite </s>";
            kiteText.color = green;
        }
    }

}
