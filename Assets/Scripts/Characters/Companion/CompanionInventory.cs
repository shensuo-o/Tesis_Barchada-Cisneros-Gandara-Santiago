using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanionInventory : MonoBehaviour
{
    public static CompanionInventory Instance { get; private set; }

    public OximoronSlot[] Slots;
    public Queue<Element> TakenElements;
    [SerializeField] public int index;
    [SerializeField] private Image SelectIndicator;

    [SerializeField] private TakeElement detector;

    [SerializeField] private GameObject errorPrompt;

    [SerializeField] private GameObject[] particles;

    void Awake()
    {
        Instance = this;
        TakenElements = new Queue<Element>(2);
        particles = new GameObject[2];
    }

    private void Update()
    {
        SelectIndicator.transform.position = Slots[index].transform.position;

        var scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
 
        if (scrollInput > 0)
        {
            index++;
            if (index >= Slots.Length)
            {
                index = 0;
            }
        }

        if (scrollInput < 0)
        {
            index--;
            if (index < 0)
            {
                index = 3;
            }
        }

        if (!detector.elementFound && TakenElements.Count != 0) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EquipElement(TakenElements.Peek());
            }
        }
    }

    public void AddElement(Element element)
    {
        TakenElements.Enqueue(element);
        if (TakenElements.Count == 1)
        {
            particles[0] = element.particles;
        }
        else if (TakenElements.Count == 2)
        {
            particles[1] = element.particles;
        }
        Debug.Log(TakenElements + ",     " + TakenElements.Count);
    }

    public void EquipElement(Element element)
    {
        if (Slots[index].elements[0] == null)
        {
            Slots[index].elements[0] = element;
            Slots[index].ShowElement();
            TakenElements.Dequeue();
            Debug.Log(TakenElements + ",     " + TakenElements.Count);
            return;
        }
        else if (Slots[index].elements[1] == null)
        {
            for (int i = 0; i < OximoronInventory.Instance.allOximorons.Length; i++)
            {
                if ((OximoronInventory.Instance.allOximorons[i].neededElement1 == element.elementType ||
                    OximoronInventory.Instance.allOximorons[i].neededElement1 == Slots[index].elements[0].elementType) &&
                    (OximoronInventory.Instance.allOximorons[i].neededElement2 == element.elementType ||
                    OximoronInventory.Instance.allOximorons[i].neededElement2 == Slots[index].elements[0].elementType))
                {
                    Slots[index].elements[1] = element;
                    Slots[index].ShowElement();
                    Slots[index].EquipOximoron();
                    TakenElements.Dequeue();
                    return;
                }
            }
            return;
        }
        else
        {
            StartCoroutine("CantEquip");
            return;
        }
    }

    private IEnumerator CantEquip()
    {
        errorPrompt.SetActive(true);
        yield return new WaitForSeconds(1);
        errorPrompt.SetActive(false);
    }
}
