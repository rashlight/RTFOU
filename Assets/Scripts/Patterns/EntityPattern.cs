using UnityEngine;
using System.Collections;

/// <summary>
/// Defines coroutine(s) for enemy or player's behavior.
/// </summary>
public class EntityPattern : MonoBehaviour {
    public float sequenceDelay;

    /// <summary>
    /// Starts the overridden entity's sequence.
    /// </summary>
    public virtual IEnumerator StartSequence()
    {
        yield return null;
    }
}