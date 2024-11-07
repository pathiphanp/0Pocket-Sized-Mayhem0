using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    [SerializeField] float delayRead;
    int charC = 0;
    public void StartReadText(string suptitle, TMP_Text _TextUi)
    {
        StartCoroutine(ReadText(suptitle, _TextUi));
    }
    public IEnumerator ReadText(string suptitle, TMP_Text _TextUi)
    {
        while (charC < suptitle.Length)
        {
            yield return new WaitForSeconds(delayRead);
            charC++;
            string text = suptitle.Substring(0, charC);
            _TextUi.text = text;
        }
    }
}
