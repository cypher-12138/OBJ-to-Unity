using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // !!!【重点】改成你电脑上obj文件的绝对路径
    public string objFilePath = @"D:\testmodel\cube.obj";
    private ObjImporter importer;

    void Start()
    {
        importer = GetComponent<ObjImporter>();
        GameObject model = importer.ImportObj(objFilePath);
        model.transform.position = Vector3.zero;

        // 读取顶点示例
        Vector3[] vertices = importer.GetAllVertices();
        Debug.Log($"模型顶点总数：{vertices.Length}");
        for (int i = 0; i < Mathf.Min(5, vertices.Length); i++)
        {
            Debug.Log($"顶点{i}：{vertices[i]}");
        }
    }
}