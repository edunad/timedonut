using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingController : MonoBehaviour {

    [SerializeField]
    public List<SpriteRenderer> spriteDonuts;

    public Sprite donutOK;
    public Sprite donutNOPE;

    public void calculateRating(int movedItems, int totalGoldenMoves) {
        if (this.spriteDonuts == null || this.spriteDonuts.Count <= 0) return;
        int rating = Mathf.Clamp(this.spriteDonuts.Count - (movedItems - totalGoldenMoves), 0, 5);

        for (int i = 0; i < this.spriteDonuts.Count; i++) {
            if (this.spriteDonuts[i] == null) continue;
            if (i < rating) this.spriteDonuts[i].sprite = donutOK;
            else this.spriteDonuts[i].sprite = donutNOPE;
        }
    }
}
