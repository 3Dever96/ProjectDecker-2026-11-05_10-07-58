using System.Collections.Generic;
using UnityEngine;
using Animancer;

// Central manager for playing character animations by name.
public class AnimationHub : MonoBehaviour
{
    // Animancer component attached to the character that performs the actual animation playback.
    [SerializeField] private AnimancerComponent animancer;

    // Dictionary of animation names to clips so states can trigger animations by string instead of hardcoding references.
    [SerializeField] public Dictionary<string, AnimationClip> animLibrary = new Dictionary<string, AnimationClip>();

    // Play the animation matching the provided clip name if it exists in the library.
    public void PlayAnimation(string clip, float fadeDuration = 0.1f)
    {
        if (animLibrary.TryGetValue(clip, out AnimationClip animationClip))
        {
            animancer.Play(animationClip, fadeDuration);
        }
    }
}
