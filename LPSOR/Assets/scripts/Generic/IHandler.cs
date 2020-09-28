using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IHandler
    {
        GameSystem system {get; set;}
        void Activate();
    }
}