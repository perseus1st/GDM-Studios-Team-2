using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.InteropServices;

public class SaveFileSelection : MonoBehaviour
{

    [SerializeField] TMP_Text file1Text;
    [SerializeField] TMP_Text file2Text;
    [SerializeField] TMP_Text file3Text;
    [SerializeField] GameObject FileWithDataPanel;

    private SceneController sceneController;

    void Awake()
    {
    sceneController = FindFirstObjectByType<SceneController>();
    if (sceneController == null)
        Debug.LogError("No SceneController found in the scene!");
    }

    private void OnEnable(){
        if (SaveSystem.SaveExists(1))
        {
            file1Text.text = "<sprite name=floppydiskicon> File 1";
        } else
        {
            file1Text.text = "File 1";
        }

        if (SaveSystem.SaveExists(2))
        {
            file2Text.text = "<sprite name=floppydiskicon> File 2";
        } else
        {
            file2Text.text = "File 2";
        }

        if (SaveSystem.SaveExists(3))
        {
            file3Text.text = "<sprite name=floppydiskicon> File 3";
        } else
        {
            file3Text.text = "File 3";
        }
        
    }

    public void OnSelectSaveSlot(int slot)
    {
        if (SaveSystem.SaveExists(slot))
        {
           this.gameObject.SetActive(false);
           FileWithDataPanel.SetActive(true);
           FileWithDataPanel.GetComponent<SelectedSaveFile>().setFileNumber(slot);
        }
        else
        {
            GameManager.Instance.completedMinigames.Clear();
            GameManager.Instance.highScores.Clear();
            GameManager.Instance.currentSaveSlot = slot;
            sceneController.StartAnimation("Cutscene1");
        }
    }
}