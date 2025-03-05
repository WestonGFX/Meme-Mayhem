using UnityEngine;

public class FloorExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Show transition effect
            StartCoroutine(FloorTransition());
        }
    }
    
    private System.Collections.IEnumerator FloorTransition()
    {
        // Fade out
        // (In full implementation, this would use a UI fade effect)
        
        yield return new WaitForSeconds(1f);
        
        // Proceed to next floor
        GameManager.Instance.GetComponent<DungeonGenerator>().AdvanceToNextFloor();
    }
}