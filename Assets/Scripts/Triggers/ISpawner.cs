using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Triggers
{
    interface ISpawner
    {
        GameObject Prefab { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }
        void Spawn();
    }
}
