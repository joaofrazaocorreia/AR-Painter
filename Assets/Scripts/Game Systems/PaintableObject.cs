using System.Collections.Generic;
using UnityEngine;

public class PaintableObject : MonoBehaviour
{
    private GameManager gameManager;
    private List<List<GameObject>> colorableParts;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        AssignColorableParts();
    }

    public void AssignColorableParts()
    {
        colorableParts = new List<List<GameObject>>();

        int assignmentLoops = (int)Mathf.Ceil(transform.childCount / gameManager.NumOfColors);

        for (int i = 0; i < assignmentLoops; i++)
        {
            for (int j = 0; j < gameManager.NumOfColors && j < transform.childCount; j++)
            {
                while (colorableParts.Count <= j)
                    colorableParts.Add(new List<GameObject>());

                Transform partsParent = transform.GetChild(j);

                for (int k = 0; k < partsParent.childCount; k++)
                {
                    Transform partToAdd = partsParent.GetChild(k);

                    if (partToAdd != null)
                        colorableParts[j].Add(partToAdd.gameObject);
                }
            }
        }
    }

    public void PaintParts(int partsIndex, Color color)
    {
        partsIndex = Mathf.Clamp(partsIndex, 0, gameManager.NumOfColors-1);

        foreach (GameObject part in colorableParts[partsIndex])
        {
            part.GetComponent<Renderer>().material.color = color;
        }
    }
}
