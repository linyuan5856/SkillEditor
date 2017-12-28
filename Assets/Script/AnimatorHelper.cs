using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{

    private Animator mAnimator;
    [SerializeField]
    private string[] clips;
    [SerializeField]
    private int index;
    [SerializeField]
    private bool play;

    // Use this for initialization
    void Start()
    {
        this.mAnimator = this.GetComponent<Animator>();
        this.Init();
    }


    void Init()
    {
        var allClips = this.mAnimator.runtimeAnimatorController.animationClips;

        this.clips = new string[allClips.Length];
        for (int i = 0; i < allClips.Length; i++)
        {
            this.clips[i] = allClips[i].name;
        }
    }


    void Update()
    {
        if (play)
        {
            int num = Mathf.Clamp(index, 0, this.clips.Length);
            this.mAnimator.Play(this.clips[num]);
        }
    }
}
