using System.Collections.Generic;
using UnityEngine;
public enum ArrowDirection { Up, Down, Left, Right }

public class ArrowSequenceGenerator
{
    public static List<ArrowDirection> GenerateSequence(int length)
    {
        List<ArrowDirection> sequence = new List<ArrowDirection>();
        for (int i = 0; i < length; i++)
        {
            ArrowDirection randomArrow = (ArrowDirection)Random.Range(0, 4);
            sequence.Add(randomArrow);
        }
        return sequence;
    }
}
