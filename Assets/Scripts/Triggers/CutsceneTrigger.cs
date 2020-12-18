using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public Texture2D face;

    [TextArea(3, 10)]
    public string text;
}

public class CutsceneTrigger : Trigger
{
    [SerializeField] private Dialogue[] _frames;

    public override void DoAction()
    {
        CutsceneManager.Current.StartCutscene(_frames);
        Destroy(gameObject);
    }
}
