﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingController : MonoBehaviour {

    [SerializeField]
    public List<SpriteRenderer> spriteDonuts;

    public Sprite donutOK;
    public Sprite donutNOPE;

    // Sadly there was no time to put the ratings in good use
    public int calculateAndRenderRating(int movedItems, int totalGoldenMoves) {
        if (this.spriteDonuts == null || this.spriteDonuts.Count <= 0) return 0;
        int rating = Mathf.Clamp(this.spriteDonuts.Count - (movedItems - totalGoldenMoves), 0, 5);

        for (int i = 0; i < this.spriteDonuts.Count; i++) {
            if (this.spriteDonuts[i] == null) continue;
            if (i < rating) {
                this.spriteDonuts[i].sprite = donutOK;
                this.spriteDonuts[i].color = Color.white;
            } else {
                this.spriteDonuts[i].sprite = donutNOPE;
                this.spriteDonuts[i].color = new Color32(56, 56, 56, 255);
            }
        }

        return rating;
    }
}
