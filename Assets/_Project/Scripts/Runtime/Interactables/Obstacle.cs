using System;
using Unity.Mathematics;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Obstacle : MonoBehaviour, IBreakable
    {
        public void Break()
        {
            // TODO: Lets properly handle this..
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}