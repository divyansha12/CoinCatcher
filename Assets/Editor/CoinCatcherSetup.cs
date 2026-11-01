using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

public static class CoinCatcherSetup
{
    [MenuItem("Tools/Setup Coin Catcher")]
    static void Setup()
    {
        GameObject bucket = GameObject.Find("Bucket");
        if (bucket != null && bucket.GetComponent<BucketController>() == null)
            bucket.AddComponent<BucketController>();

        string path = "Assets/Coin.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        if (root.GetComponent<Coin>() == null)
            root.AddComponent<Coin>();
        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        GameObject spawnerGo = GameObject.Find("Spawner");
        if (spawnerGo == null) spawnerGo = new GameObject("Spawner");
        CoinSpawner spawner = spawnerGo.GetComponent<CoinSpawner>();
        if (spawner == null) spawner = spawnerGo.AddComponent<CoinSpawner>();
        spawner.coinPrefab = prefab;
        EditorUtility.SetDirty(spawner);

        GameObject gmGo = GameObject.Find("GameManager");
        if (gmGo == null) gmGo = new GameObject("GameManager");
        GameManager gm = gmGo.GetComponent<GameManager>();
        if (gm == null) gm = gmGo.AddComponent<GameManager>();
        GameObject st = GameObject.Find("ScoreText");
        if (st != null) gm.scoreText = st.GetComponent<TMP_Text>();
        EditorUtility.SetDirty(gm);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Coin Catcher setup done. Press Play.");
    }
}
