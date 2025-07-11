using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class TakeElement : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    [SerializeField] private CompanionInventory inventory;
    [SerializeField] private Element foundElement;
    [SerializeField] private bool elementFound;
    [SerializeField] private float timer;
    [SerializeField] private float absorbTime;
    public List<OximoronSlot> abailableSlots;

    [SerializeField] private GameObject clock;
    [SerializeField] private Image fill;
    [SerializeField] private GameObject redWarning;
    [SerializeField] private ParticleSystem absorbEffect;
    [SerializeField] private VisualEffect nearElement;
    [SerializeField] private float size;
    [ColorUsage(hdr: true, showAlpha: true)]
    [SerializedField] private Color nearColor;

    private void Update()
    {
        fill.fillAmount = timer;
        if (Input.GetKeyUp(KeyCode.E))
        {
            timer = 0;
            clock.SetActive(false);
            if(foundElement != null )
            {
                foundElement.SpeedDown();
            }
        }

        if (elementFound)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckSlots(foundElement);
                if (abailableSlots.Count != 0)
                {
                    absorbEffect.Play();
                }
            }

            if (Input.GetKey(KeyCode.E) && abailableSlots.Count >= 1)
            {
                clock.SetActive(true);
                timer += Time.deltaTime;
                foundElement.SpeedUp();
                if (timer >= absorbTime)
                {
                    timer = 0;
                    inventory.EquipElement(foundElement);
                    foundElement.StartCoroutine("TurnOffAndOn");
                    foundElement = null;
                    elementFound = false;
                    abailableSlots.Clear();
                    prompt.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.E) && abailableSlots.Count <= 0)
            {
                StartCoroutine("CantGrab");
            }
        }
        else if (!elementFound)
        {
            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                inventory.Slots[i].CanRecieveElement = true;
            }
            nearElement.SetFloat("TimeSize", size -= Time.deltaTime * 4);
            if (size < 0) { size = 0; }
        }
    }

    private IEnumerator CantGrab()
    {
        redWarning.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        redWarning.SetActive(false);
    }

    private void CheckSlots(Element element)
    {
        for (int i = 0; i < inventory.Slots.Length; i++)
        {
            if (inventory.Slots[i].elements[0] == null)
            {
                inventory.Slots[i].CanRecieveElement = true;
                abailableSlots.Add(inventory.Slots[i]);
                continue;
            }
            else if (inventory.Slots[i].elements[0].elementType == element.elementType)
            {
                inventory.Slots[i].CanRecieveElement = false;
                continue;
            }
            else if (inventory.Slots[i].elements[1] == null)
            {
                for (int n = 0; n < OximoronInventory.Instance.allOximorons.Length; n++)
                {
                    if ((OximoronInventory.Instance.allOximorons[n].neededElement1 == element.elementType &&
                        OximoronInventory.Instance.allOximorons[n].neededElement2 == inventory.Slots[i].elements[0].elementType) || 
                        (OximoronInventory.Instance.allOximorons[n].neededElement1 == inventory.Slots[i].elements[0].elementType && 
                        OximoronInventory.Instance.allOximorons[n].neededElement2 == element.elementType))
                    {
                        inventory.Slots[i].CanRecieveElement = true;
                        abailableSlots.Add(inventory.Slots[i]);
                        break;
                    }
                    else
                    {
                        inventory.Slots[i].CanRecieveElement = false;
                        abailableSlots.Remove(inventory.Slots[i]);
                    }
                }
            }
            else
            {
                inventory.Slots[i].CanRecieveElement = false;
                continue;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            foundElement = other.GetComponent<Element>();
            elementFound = true;
            prompt.SetActive(true);
            foundElement.LightUp();
            nearColor = foundElement.GetComponent<Element>().particleColor;
            nearColor *= 5;
            nearElement.SetVector4("Color", nearColor);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            nearElement.SetFloat("TimeSize", size += Time.deltaTime * 4);
            if (size > 5.1f) { size = 5.1f; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            if (foundElement != null) 
            {
                foundElement.SpeedDown();
                foundElement.LightDown();
            }
            prompt.SetActive(false);
            foundElement = null;
            elementFound = false;
            abailableSlots.Clear();
            timer = 0;
            clock.SetActive(false);
        }
    }
}
