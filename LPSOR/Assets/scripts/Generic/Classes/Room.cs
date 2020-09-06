using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct Room
    {
        public string name;
        public int population;
        public int buddiesInRoom;

        public int maxPopulation;

        public Room(string name, int population, int buddies, int maxPopulation)
        {
            this.name = name;
            this.population = population;
            this.buddiesInRoom = buddies;
            this.maxPopulation = maxPopulation;
        }
    }
}
