using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject tutorialPanel;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = tutorialPanel.GetComponent<Animator>();
        tutorialPanel.SetActive(false); 
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f/ animator.speed)
        {
            animator.speed = 0;

            Debug.Log("Animacion detenida");
        }
        
    }

    public void OnTutorialButtonClick()
    {
        tutorialPanel.SetActive(false);
        
    }

     public void OnVerTutorialButtonClick()
    {
        tutorialPanel.SetActive(true);
        animator.speed = 1; // Asegúrate de que la animación se ejecute
        animator.Play("Tutorial", 0, 0.0f); // Reinicia la animación desde el principio
    }
};
