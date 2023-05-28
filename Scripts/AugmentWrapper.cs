using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentWrapper : MonoBehaviour
{
    //assigned in inspector
    public Sprite[] sprites;

    public int numAugments { get { return augmentsList.Count; } }

    //variable contianing an augment instance with their ID as key
    Dictionary<int, Augment> augmentsList = new Dictionary<int, Augment>
    {
        {0, new Fortitude()},
        {1, new Magnitude()},
        {2, new RapidFire()},
        {3, new Velocity()},
        {4, new Quickness()},
        {5, new BlastShot()},
        {6, new MeteoricImpact()},
        {7, new HighExplosives()},
        {8, new Supersonic()},
        {9, new ReflectiveStrike()},
        {10, new PlatedArmor()},
        {11, new SpikedHull()},
        {12, new PreemptiveFire()},
        {13, new WeaknessExposure()},
        {14, new Adrenaline()},
        {15, new StormsEye()},
        {16, new Eruption()},
        {17, new Defiance()},
        {18, new Corrosion()},
        {19, new Absorption()},

    };

    //get the augment instance with given id
    public Augment GetAugment(int id)
    {
        if (id >= augmentsList.Count || id < 0) return null;
        return augmentsList[id];
    }

    //get the augment sprite with given id
    public Sprite GetAugmentSprite(int id)
    {
        if (id >= sprites.Length || id < 0) return null;
        return sprites[id];
    }
}
