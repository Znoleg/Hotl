using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform _checkpoint;

    public Transform Checkpoint => _checkpoint;

}
