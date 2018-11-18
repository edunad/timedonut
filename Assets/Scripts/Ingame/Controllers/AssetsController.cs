using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetsController {
    
    public static Dictionary<string, Sprite> storedSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, AudioClip> storedAudioClip = new Dictionary<string, AudioClip>();

    // Asset loading and caching
    public static T GetResource<T>(string resourceID) {
        if (typeof(T) == typeof(Sprite)) {
            if (storedSprites.ContainsKey(resourceID)) return (T)Convert.ChangeType(storedSprites[resourceID], typeof(T));
            object spr = Convert.ChangeType(Resources.Load(resourceID, typeof(T)), typeof(T));

            AssetsController.storedSprites.Add(resourceID, (Sprite)spr);
            Debug.Log("[AssetsController] <color='red'>LOADED - Sprite</color> {<color='blue'>" + resourceID + "</color>}");

            return (T)spr;
        }else if (typeof(T) == typeof(AudioClip)) {
            if (storedAudioClip.ContainsKey(resourceID)) return (T)Convert.ChangeType(storedAudioClip[resourceID], typeof(T));
            object snd = Convert.ChangeType(Resources.Load(resourceID, typeof(T)), typeof(T));

            AssetsController.storedAudioClip.Add(resourceID, (AudioClip)snd);
            Debug.Log("[AssetsController] <color='red'>LOADED - AudioClip</color> {<color='blue'>" + resourceID + "</color>}");

            return (T)snd;
        }

        return default(T);
    }
}
