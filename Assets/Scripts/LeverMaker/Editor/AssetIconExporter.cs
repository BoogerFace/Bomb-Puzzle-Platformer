using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;

public class AssetIconExporter : MonoBehaviour
{
    [Header("Batch Export Config")]
    [SerializeField] private string path = "Assets/Blocks/Platformer/yellow";
    [SerializeField] private Color32 backgroundColour = new Color32(82,82,82,255);

    [Header("Single Export Config")]
    [SerializeField] private Object singleObject;


    [ContextMenu("Batch Export Block Icons (.fbx)")]
    private void BatchExportBlockIcon()
    {
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();

        foreach (string s in Directory.GetFiles(path, "*.fbx").Select(Path.GetFileName).ToArray())
        {
            Object obj = AssetDatabase.LoadMainAssetAtPath(path+"/"+s);
            Texture2D tex = AssetPreview.GetAssetPreview(obj);
            while(tex == null && AssetPreview.IsLoadingAssetPreview(obj.GetInstanceID()))
            {
                tex = AssetPreview.GetAssetPreview(obj);
            }
            tex = AssetPreview.GetAssetPreview(obj);

            Color32[] pixels = tex.GetPixels32();
            for (var i = 0; i < pixels.Length; ++i)
            {
                if (pixels[i].ToString() == backgroundColour.ToString())
                {
                    pixels[i] = new Color32(0,0,0,0);
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();

            byte[] bytes = ImageConversion.EncodeToPNG(tex);
            File.WriteAllBytes(Application.dataPath + "/Resources/Textures/BlockIcons/" + obj.name + ".png", bytes);
        }

        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }

    
    [ContextMenu("Export Single Block Icon")]
    private void ExportSingleBlockIcon()
    {
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();

        Object obj = singleObject;
        Texture2D tex = AssetPreview.GetAssetPreview(obj);
        while(tex == null && AssetPreview.IsLoadingAssetPreview(obj.GetInstanceID()))
        {
            tex = AssetPreview.GetAssetPreview(obj);
        }
        tex = AssetPreview.GetAssetPreview(obj);

        Color32[] pixels = tex.GetPixels32();
        for (var i = 0; i < pixels.Length; ++i)
        {
            if (pixels[i].ToString() == backgroundColour.ToString())
            {
                pixels[i] = new Color32(0,0,0,0);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();

        byte[] bytes = ImageConversion.EncodeToPNG(tex);
        File.WriteAllBytes(Application.dataPath + "/Resources/Textures/BlockIcons/" + obj.name + ".png", bytes);

        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
}
