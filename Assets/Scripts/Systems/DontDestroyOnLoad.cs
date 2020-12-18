using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private bool _dontDestroyChildren;
    private List<GameObject> _childrens;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (_dontDestroyChildren)
        {
            _childrens = GetComponentsInChildren<GameObject>().OfType<GameObject>().ToList();
            foreach (GameObject gm in _childrens)
                DontDestroyOnLoad(gm);
        }
    }
}
