using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//private enum AIState { Chasing,  }

public class AIPlayerController : PlayerController
{
    private Mage opponent;
    private Mage mage;

    public void Initialize(Mage mage, Mage opponent)
    {
        this.mage = mage;
        this.opponent = opponent;
        //this.match = match;
    }

    new private void Start()
    {
        StartCoroutine(UpdateMove());

        base.Start();
    }
    private IEnumerator UpdateMove()
    {
        while (true)
        {
            List<ManaSlot> g1 = mage.GetManaSlots();
            List<ManaSlot> g2 = opponent.GetManaSlots();

            List<Vector2> dirs = new List<Vector2>();
            List<float> scores = new List<float>();

            foreach (ManaSlot m1 in g1)
            {
                Projectile p1 = m1.GetProjectile();
                if (p1 == null) continue;

                foreach (ManaSlot m2 in g2)
                {
                    Projectile p2 = m2.GetProjectile();
                    if (p2 == null) continue;

                    Vector2 v = p2.transform.position - p1.transform.position;
                    dirs.Add(v.normalized);

                    float score = 0;
                    if (Projectile.Defeats(p1, p2))
                        score += 100;
                    score -= v.magnitude / 100f;

                    scores.Add(score);
                }
            }


            Vector2 chosen_dir = Vector2.zero;
            float best_score = -1;
            for (int i = 0; i < scores.Count; ++i)
            {
                if (scores[i] > best_score)
                {
                    best_score = scores[i];
                    chosen_dir = dirs[i];
                }
            }

            if (g1.Count > 0 && g2.Count > 0)
            {
                InputMove = chosen_dir;
            }

            
            //if (Random.value < 0.3f) InputScrollGroups(1);
            yield return null;
        }
    }

    private Vector2 RandomDirection()
    {
        float angle = Random.Range(0, Mathf.PI * 2f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
