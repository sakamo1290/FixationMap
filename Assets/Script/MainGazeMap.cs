using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainGazeMap : MonoBehaviour
{
    public Material iniMat;
    public Material paintMat;
    public Transform gazePoint;
    public Transform gazeOrigin;

    Material mainMaterial;
    RenderTexture mainTex;

    public void SaveMap(string mapPath)
    {
        SaveRenderTextureToJpg(mainTex, mapPath);
        //reset material
        Graphics.Blit(mainTex, mainTex, iniMat);
    }
    void Start()
    {
        mainTex = new RenderTexture(770, 1000, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        Graphics.Blit(mainTex, mainTex, iniMat);

        mainMaterial = GetComponent<Renderer>().material;
        mainMaterial.SetTexture("_MainTex", mainTex);
    }
    void FixedUpdate()
    {
        RenderTexture buf = RenderTexture.GetTemporary(770, 1000); ;
        MatUpdate();
        Graphics.Blit(mainTex, buf, paintMat);
        Graphics.Blit(buf, mainTex);
        RenderTexture.ReleaseTemporary(buf);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveMap(Application.dataPath + "/test.png");
            Debug.Log("reset and save");
        }
    }
    void MatUpdate()
    {
        paintMat.SetVector("_CubePos", new Vector4(gazePoint.position.x, gazePoint.position.y, gazePoint.position.z, 1));
        paintMat.SetVector("_GazeOri", new Vector4(gazeOrigin.position.x, gazeOrigin.position.y, gazeOrigin.position.z, 1));
        paintMat.SetTexture("_MainTex", mainTex);
        paintMat.SetMatrix("_TransformMatrix", transform.localToWorldMatrix);
    }

    void SaveRenderTextureToJpg(RenderTexture RenderTextureRef, string saveFilePath)
    {
        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.ARGB32, false);
        RenderTexture.active = RenderTextureRef;
        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        UnityEngine.Object.Destroy(tex);

        //Write to a file in the project folder
        File.WriteAllBytes(saveFilePath, bytes);

    }
}
