using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CutsceneElementBase : MonoBehaviour
{
    public float duration;
    private CutsceneHandler cutsceneHandler;

    public void Start()
    {
        cutsceneHandler = GetComponent<CutsceneHandler>();
    }

    public virtual void Execute()
    {
        
    }

    protected IEnumerator WaitAndAdvance()
    {
        yield return new WaitForSeconds(duration);
        cutsceneHandler.PlayNextElement();
    }
}
