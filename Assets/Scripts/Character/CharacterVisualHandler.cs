using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualHandler : MonoBehaviour
{
    [SerializeField] private Sprite knockedDown, stabbed, bulletWound;
    [SerializeField] private GameObject bloodPool, bloodSpurt, bloodSpray;
    private SpriteRenderer spriteRenderer;
    private Target target;

    private void Start()
    {
        target = GetComponent<Target>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Target._OnDamage += 
    }

}
