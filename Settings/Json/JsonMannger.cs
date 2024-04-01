using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum JsonType
{
    LitJson,
    JsonUtlity
}
public class JsonMannger
{
    #region 单例模式
    private JsonMannger() { }
    private static JsonMannger instance = new JsonMannger();
    public static JsonMannger Instance => instance;
    #endregion

    /// <summary>
    /// 使用Json数据序列化对象到本地，保存在默认位置
    /// </summary>
    /// <param name="data">要保存的对象</param>
    /// <param name="name">文件名称（无需后缀）</param>
    /// <param name="jsontype">选择保存的模式，若不填写，默认为LitJson</param>
    public void SaveJsonData_DefaultPath(object data, string name, JsonType jsontype = JsonType.LitJson)
    {
        string value = null;

        switch (jsontype)
        {
            case JsonType.LitJson:
                value = JsonMapper.ToJson(data);
                break;
            case JsonType.JsonUtlity:
                value = JsonUtility.ToJson(data);
                break;
        }

        File.WriteAllText(Application.persistentDataPath + "\\" + name + ".json", value);
    }

    /// <summary>
    /// 使用Json数据序列化对象到本地，通过指定路径保存
    /// </summary>
    /// <param name="data">要保存的对象</param>
    /// <param name="path">要保存文件的路径（不需要"Application.persistentDataPath\"前缀）</param>
    /// <param name="name">文件名称（无需后缀）</param>
    /// <param name="jsontype">选择保存的模式，若不填写，默认为LitJson</param>
    public void SaveJsonData_ToPath(object data, string path, string name, JsonType jsontype = JsonType.LitJson)
    {
        string value = null;

        string savePath = Application.persistentDataPath + "\\" + path;
        Directory.CreateDirectory(savePath);

        switch (jsontype)
        {
            case JsonType.LitJson:
                value = JsonMapper.ToJson(data);
                break;
            case JsonType.JsonUtlity:
                value = JsonUtility.ToJson(data);
                break;
        }

        File.WriteAllText(savePath + name + ".json", value);
    }



    /// <summary>
    /// 传入已读取好的文本内容来读取Json数据，反序列化返回对象
    /// </summary>
    /// <typeparam name="T">类对象，请使用相对应的类进行接收</typeparam>
    /// <param name="text">已读取好的文本内容</param>
    /// <param name="jsontype">选择读取的模式，若不填写，默认为LitJson</param>
    /// <returns>类对象，请使用相对应的类进行接收</returns>
    public T LoadJsonData<T>(string text, JsonType jsontype = JsonType.LitJson)
    {
        switch (jsontype)
        {
            case JsonType.LitJson:
                return JsonMapper.ToObject<T>(text);
            case JsonType.JsonUtlity:
                return JsonUtility.FromJson<T>(text);
            default:
                return default(T);
        }
    }

    /// <summary>
    /// 从默认路径读取Json数据，反序列化返回对象
    /// </summary>
    /// <typeparam name="T">类对象，请使用相对应的类进行接收</typeparam>
    /// <param name="name">文件名称（无需后缀）</param>
    /// <param name="jsontype">选择读取的模式，若不填写，默认为LitJson</param>
    /// <returns>类对象，请使用相对应的类进行接收</returns>
    public T LoadJsonData_DefaultPath<T>(string name, JsonType jsontype = JsonType.LitJson)
    {
        string path = Application.persistentDataPath + "\\" + name + ".json";

        string temps = File.ReadAllText(path);

        switch (jsontype)
        {
            case JsonType.LitJson:
                return JsonMapper.ToObject<T>(temps);
            case JsonType.JsonUtlity:
                return JsonUtility.FromJson<T>(temps);
            default:
                return default(T);
        }
    }

    /// <summary>
    /// 从指定路径读取Json数据，反序列化返回对象
    /// </summary>
    /// <typeparam name="T">类对象，请使用相对应的类进行接收</typeparam>
    /// <param name="path">文件路径（不需要"Application.persistentDataPath\"前缀）</param>
    /// <param name="name">文件名称（无需后缀）</param>
    /// <param name="jsontype">选择读取的模式，若不填写，默认为LitJson</param>
    /// <returns>类对象，请使用相对应的类进行接收</returns>
    public T LoadJsonData_ToPath<T>(string path, string name, JsonType jsontype = JsonType.LitJson)
    {
        // 设置文件指定路径，若缺失文件夹将创建
        Directory.CreateDirectory(Application.persistentDataPath + "\\" + path);
        string savePath = Application.persistentDataPath + "\\" + path + name + ".json";

        string data = File.ReadAllText(savePath);

        switch (jsontype)
        {
            case JsonType.LitJson:
                return JsonMapper.ToObject<T>(data);
            case JsonType.JsonUtlity:
                return JsonUtility.FromJson<T>(data);
            default:
                return default;
        }
    }
}