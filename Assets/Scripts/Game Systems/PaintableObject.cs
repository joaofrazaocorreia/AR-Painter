using System.Collections.Generic;
using UnityEngine;

public class PaintableObject : MonoBehaviour
{
    [SerializeField] private Transform partsParent;
    [SerializeField] private ParticleSystem victoryParticles;
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

        int assignmentLoops = (int)Mathf.Ceil(partsParent.childCount / gameManager.NumOfColors);

        for (int i = 0; i < assignmentLoops; i++)
        {
            for (int j = 0; j < gameManager.NumOfColors && j < partsParent.childCount; j++)
            {
                while (colorableParts.Count <= j)
                    colorableParts.Add(new List<GameObject>());

                Transform currentPartsParent = partsParent.GetChild(j);

                for (int k = 0; k < currentPartsParent.childCount; k++)
                {
                    Transform partToAdd = currentPartsParent.GetChild(k);

                    if (partToAdd != null)
                        colorableParts[j].Add(partToAdd.gameObject);
                }
            }
        }
    }

    public void PaintParts(int partsIndex, Color color)
    {
        partsIndex = Mathf.Clamp(partsIndex, 0, gameManager.NumOfColors - 1);

        foreach (GameObject part in colorableParts[partsIndex])
        {
            part.GetComponent<Renderer>().material.color = color;
        }
    }

    public void VictoryParticles()
    {
        victoryParticles.gameObject.SetActive(true);
    }
}
