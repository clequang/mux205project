using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    private ItemCollector itemCollector;

    private void Start()
    {
        itemCollector = this.GetComponent<ItemCollector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.SendMessage("Collect", this.gameObject.tag);
            Destroy(this.gameObject);
        }
    }
}
