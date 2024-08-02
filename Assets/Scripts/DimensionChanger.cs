using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DimensionChanger : MonoBehaviour
{
    [SerializeField] MeshRenderer[] meshes;
    [SerializeField] MeshRenderer[] meshesFuture;
    [SerializeField] Material mat;
    [SerializeField] List<Material> pastMaterials;
    [SerializeField] List<Material> futureMaterials;
    //[SerializeField] MeshRenderer mr;
    //[SerializeField] Material normalMat;
    //[SerializeField] Transform player;
    //[SerializeField] Transform playerFuturePos;
    //[SerializeField] Transform playerPastPos;
    [SerializeField] Transform pastEnvironment;
    [SerializeField] Transform futureEnvironment;

    bool isFuture = false;
    bool isReady = true;

    private void Start()
    {
        PrepareFuture();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isReady) return;
            isReady = false;

            AudioManager.Instance.PlaySFX("teleport");

            if (!isFuture)
            {
                isFuture = true;
                pastMaterials.Clear();
                pastMaterials = new(meshes.Count());
                //GameManager.Instance.FOVSetter();
                foreach (MeshRenderer item in meshes)
                {
                    if (item != null)
                    {
                        pastMaterials.Add(item.material);
                        item.material = mat;
                        item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), 0f);

                        
                        DOTween.To(() => item.material.GetFloat(Shader.PropertyToID("_DissolveStrength")),
                                   x => item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), x),
                                   1f, 5)
                               .SetEase(Ease.OutCirc);
                    }
                    else
                    {
                        pastMaterials.Add(mat);
                    }
                }
                int count = 0;
                foreach (MeshRenderer item in meshesFuture)
                {
                    if (item != null)
                    {
                        item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), 1f);

                        DOTween.To(() => item.material.GetFloat(Shader.PropertyToID("_DissolveStrength")),
                                   x => item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), x),
                                   0f, 5)
                               .SetEase(Ease.OutCirc)
                               .OnComplete(() =>
                               {
                                   isReady = true;

                               });
                    }
                    StartCoroutine(SetNewMaterial(item, futureMaterials[count]));
                    count++;

                }

                StartCoroutine(ChangeScene(true));
                return;
            }
            else
            {
                isFuture = false;
                int count = 0;
                //GameManager.Instance.FOVSetter();
                foreach (MeshRenderer item in meshes)
                {
                    if (item != null)
                    {
                        item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), 1f);

                        DOTween.To(() => item.material.GetFloat(Shader.PropertyToID("_DissolveStrength")),
                                   x => item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), x),
                                   0f, 5)
                               .SetEase(Ease.OutCirc)
                               .OnComplete(() =>
                               {
                                   isReady = true;

                               });
                    }
                    StartCoroutine(SetNewMaterial(item, pastMaterials[count]));
                    count++;

                }
                PrepareFuture();


                StartCoroutine(ChangeScene(false));
                return;
            }
        }
    }

    IEnumerator SetNewMaterial(MeshRenderer mr, Material mat)
    {
        yield return new WaitForSeconds(2.3f);
        mr.material = mat;
    }

    IEnumerator ChangeScene(bool goToFuture)
    {
        GameManager.Instance.FOVSetter();
        if (goToFuture)
        {
            futureEnvironment.gameObject.SetActive(true);
            yield return new WaitForSeconds(.7f);
            pastEnvironment.gameObject.SetActive(false);
        }
        else
        {
            pastEnvironment.gameObject.SetActive(true);
            yield return new WaitForSeconds(.7f);
            futureEnvironment.gameObject.SetActive(false);
        }
    }


    void PrepareFuture()
    {
        futureMaterials.Clear();
        futureMaterials = new(meshesFuture.Count());
        foreach (MeshRenderer item in meshesFuture)
        {
            if (item != null)
            {
                futureMaterials.Add(item.material);
                item.material = mat;
                item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), 0f);


                DOTween.To(() => item.material.GetFloat(Shader.PropertyToID("_DissolveStrength")),
                           x => item.material.SetFloat(Shader.PropertyToID("_DissolveStrength"), x),
                           1f, 5)
                       .SetEase(Ease.OutCirc);
            }
            else
            {
                futureMaterials.Add(mat);
            }
        }
    }
}
