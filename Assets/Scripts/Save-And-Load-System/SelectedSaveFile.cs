using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedSaveFile : MonoBehaviour
{
    private int fileNumber;
    private SceneController sceneController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    sceneController = FindFirstObjectByType<SceneController>();
    if (sceneController == null)
        Debug.LogError("No SceneController found in the scene!");
    }
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
        sceneController.StartAnimation("MC_Room");
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