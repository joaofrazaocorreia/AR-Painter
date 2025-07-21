using System.Collections.Generic;
using UnityEngine;

public class PaintableObject : MonoBehaviour
{
    [SerializeField] private int numOfColors = 6;

    private List<List<GameObject>> colorableParts;

    private void Start()
    {
        AssignColorableParts();
    }

    public void AssignColorableParts()
    {
        colorableParts = new List<List<GameObject>>();

        int assignmentLoops = (int)Mathf.Ceil(transform.childCount / numOfColors);
        int childIndex = 0;

        for (int i = 0; i < assignmentLoops; i++)
        {
            for (int j = 0; j < numOfColors && childIndex < transform.childCount; j++)
            {
                while (colorableParts.Count <= j)
                    colorableParts.Add(new List<GameObject>());

                Transform partToAdd = transform.GetChild(childIndex++);

                if (partToAdd != null)
                    colorableParts[i].Add(partToAdd.gameObject);
            }
        }
    }

    public void PaintParts(int partsIndex, Color color)
    {
        partsIndex = Mathf.Clamp(partsIndex, 0, numOfColors-1);

        foreach (GameObject part in colorableParts[partsIndex])
        {
            part.GetComponent<Renderer>().material.color = color;
        }
    }
}
