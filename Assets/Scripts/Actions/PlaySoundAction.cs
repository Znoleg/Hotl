using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAction : InteractTarget
{
    [SerializeField] private List<Sound> _soundsToPlay = new List<Sound>();
    public override void DoAction()
    {
        if (!AudioManager.Instance) return;

        foreach (Sound sound in _soundsToPlay)
            AudioManager.Instance.Play(sound);
    }

}
