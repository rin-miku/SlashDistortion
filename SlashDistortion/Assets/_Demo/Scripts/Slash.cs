using DG.Tweening;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private Material distortionMat;
    private Material slashMat;

    private void Start()
    {
        distortionMat = transform.GetChild(0).GetComponentInChildren<MeshRenderer>().materials[0];
        slashMat = transform.GetChild(1).GetComponentInChildren<MeshRenderer>().materials[0];

        transform.DOLocalRotate(new Vector3(0f, -60f, 0f), 0.3f, RotateMode.LocalAxisAdd)
            .OnComplete(() => 
            { 
                Destroy(gameObject); 
            });

        float distortionStrength = 0.1f;
        DOVirtual.Float(distortionStrength, 0f, 0.15f, (value) => { distortionMat.SetFloat("_DistortionStrength", value); }).SetDelay(0.15f);

        float slashAlpah = 1f;
        DOVirtual.Float(slashAlpah, 0f, 0.15f, (value) => { slashMat.SetFloat("_Alpha", value); }).SetDelay(0.15f);
    }

    public void EnableSlash(bool value)
    {
        transform.GetChild(1).gameObject.SetActive(value);
    }
}
