using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CivilianManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> _civilians;
    [SerializeField] private GameObject _pointPopUp;
    [SerializeField] private GameObject _pointEnd;
    [SerializeField] private GameObject _timerPopUp;
    [SerializeField] private GameObject _timeEnd;
    [SerializeField] private Canvas canvas;
    private void Start()
    {
        _civilians = new List<GameObject>();
    }
    public void RemoveCivilian(object sender, GameObject civilian)
    {
        if (!_civilians.Contains(civilian))
        {
            return;
        }
        _civilians.Remove(civilian);
        Vector3 randomVector = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(5f, 10f), UnityEngine.Random.Range(-5f, 5f));
        GameObject point = Instantiate(_pointPopUp, Camera.main.WorldToScreenPoint(civilian.transform.position + randomVector), _pointPopUp.transform.rotation, canvas.transform);
        Tween.Scale(point.transform, Vector3.zero, duration: 1, ease: Ease.InOutSine);
        Tween.Position(point.transform, _pointEnd.transform.position, duration: 1, ease: Ease.OutSine);
        civilian.GetComponent<CivilianDeath>().OnKilled -= RemoveCivilian;
    }
    public void CaptureCivilian(object sender, GameObject civilian)
    {
        Vector3 randomVector = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(5f, 10f), UnityEngine.Random.Range(-5f, 5f));
        GameObject point = Instantiate(_timerPopUp, Camera.main.WorldToScreenPoint(civilian.transform.position + randomVector), _timerPopUp.transform.rotation, canvas.transform);
        Tween.Scale(point.transform, Vector3.zero, duration: 1, ease: Ease.InOutSine);
        Tween.Position(point.transform, _timeEnd.transform.position, duration: 1, ease: Ease.OutSine);
        civilian.GetComponent<CivilianDeath>().OnKilled -= RemoveCivilian;
    }

    public void AddToCivilianList(GameObject civilian)
    {
        civilian.GetComponent<CivilianDeath>().OnKilled += RemoveCivilian;
        civilian.GetComponent<CivilianDeath>().OnCaptured += CaptureCivilian;
        _civilians.Add(civilian);
    }
}
