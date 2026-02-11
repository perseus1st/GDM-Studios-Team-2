using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedSaveFile : MonoBehaviour
{
    private int fileNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Selected file " + fileNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setFileNumber(int number)
    {
        fileNumber = number;
    }

    public void onContinue()
    {
        SaveSystem.Load(fileNumber);
        Debug.Log("Loading save slot " + fileNumber);
        SceneManager.LoadScene("MC_Room");
    }

    public void onDelete()
    {
        SaveSystem.Delete(fileNumber);

        if (GameManager.Instance.currentSaveSlot == fileNumber)
        {
            GameManager.Instance.completedMinigames.Clear();
            GameManager.Instance.highScores.Clear();
            GameManager.Instance.currentSaveSlot = -1;
        }
    }
}