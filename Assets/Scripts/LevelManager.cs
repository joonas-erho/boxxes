using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public string levelNumber;

    public float waitTimeBeforeLoadingNextLevel;

    public Text bigNumber;
    public Text cornerNumber;

    public Animator fadeAnimator;

    void Awake()
    {
        bigNumber.text = levelNumber;
        cornerNumber.text = levelNumber;
    }

    public IEnumerator EndLevel()
    {
        fadeAnimator.Play("Fader In");
        yield return new WaitForSeconds(waitTimeBeforeLoadingNextLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
