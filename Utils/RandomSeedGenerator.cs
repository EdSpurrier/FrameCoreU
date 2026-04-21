using System.Collections.Generic;
using UnityEngine;

namespace FrameCoreU.Utils
{
    public class RandomSeedGenerator : MonoBehaviour
    {
        public int maximumRandomSeeds = 150;
        public List<int> randomSeeds = new List<int>();
        public static List<int> randomSeeds_static = new List<int>();

        public static void RandomizeSeed()
        {
            Random.InitState(GetRandomSeed());
        }

        public static int GetRandomSeed()
        {
            if (randomSeeds_static.Count == 0)
            {
                return Random.Range(int.MinValue, int.MaxValue);
            }
            else
            {
                int newSeed = randomSeeds_static[0];
                randomSeeds_static.Remove(randomSeeds_static[0]);
                return newSeed;
            }

            ;
        }

        private void Awake()
        {
            AddRandomSeed();
        }

        // Start is called before the first frame update
        void Start()
        {
            AddRandomSeed();
        }

        // Update is called once per frame
        void Update()
        {
            AddRandomSeed();
        }


        void AddRandomSeed()
        {
            randomSeeds.Add(Random.Range(int.MinValue, int.MaxValue));

            if (randomSeeds.Count > maximumRandomSeeds)
            {
                randomSeeds.Remove(randomSeeds[0]);
            }

            ;

            randomSeeds_static = randomSeeds;

        }
    }
}