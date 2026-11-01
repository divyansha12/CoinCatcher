using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class FixCoinCatcher
{
    [MenuItem("Tools/Fix Coin Catcher")]
    public static void Fix()
    {
        // 1. Grab the bucket sprite, then delete stray copies
        Sprite bucketSprite = null;
        foreach (SpriteRenderer s in Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
        {
            if (s.gameObject.name.StartsWith("Glossy Golden Coin Bucket"))
            {
                if (bucketSprite == null) bucketSprite = s.sprite;
                Undo.DestroyObjectImmediate(s.gameObject);
            }
        }
        if (bucketSprite == null)
        {
            foreach (string guid in AssetDatabase.FindAssets("Glossy t:Sprite"))
            {
                bucketSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
                if (bucketSprite != null) break;
            }
        }

        // 2. Fix the Bucket object
        GameObject bucket = GameObject.Find("Bucket");
        if (bucket == null) { Debug.LogError("No object named Bucket in the scene."); return; }

        BucketController[] ctrls = bucket.GetComponents<BucketController>();
        for (int i = 1; i < ctrls.Length; i++) Undo.DestroyObjectImmediate(ctrls[i]);
        if (ctrls.Length == 0) bucket.AddComponent<BucketController>();

        SpriteRenderer sr = bucket.GetComponent<SpriteRenderer>();
        if (sr == null) sr = bucket.AddComponent<SpriteRenderer>();
        if (bucketSprite != null) sr.sprite = bucketSprite;
        sr.sortingOrder = 5;

        bucket.transform.position = new Vector3(0, -3.5f, 0);
        bucket.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Rigidbody2D rb = bucket.GetComponent<Rigidbody2D>();
        if (rb == null) rb = bucket.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        BoxCollider2D col = bucket.GetComponent<BoxCollider2D>();
        if (col == null) col = bucket.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        if (sr.sprite != null)
        {
            Bounds b = sr.sprite.bounds;
            col.size = new Vector2(b.size.x * 0.8f, b.size.y * 0.25f);
            col.offset = new Vector2(0, b.extents.y * 0.5f);
        }

        // 3. Fix the Coin prefab
        GameObject coinPrefab = null;
        foreach (string guid in AssetDatabase.FindAssets("Coin t:Prefab"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            if (root.GetComponent<SpriteRenderer>() == null) root.AddComponent<SpriteRenderer>();
            Rigidbody2D crb = root.GetComponent<Rigidbody2D>();
            if (crb == null) crb = root.AddComponent<Rigidbody2D>();
            crb.bodyType = RigidbodyType2D.Dynamic;
            crb.gravityScale = 0.5f;
            crb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            if (root.GetComponent<Collider2D>() == null) root.AddComponent<CircleCollider2D>();
            if (root.GetComponent<Coin>() == null) root.AddComponent<Coin>();
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            break;
        }

        // 4. Make sure the spawner has the coin prefab
        CoinSpawner spawner = Object.FindFirstObjectByType<CoinSpawner>();
        if (spawner != null && coinPrefab != null)
        {
            spawner.coinPrefab = coinPrefab;
            EditorUtility.SetDirty(spawner);
        }

        // 5. Enable old Input (arrow keys / A D / mouse)
        bool inputChanged = false;
        Object[] ps = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
        if (ps.Length > 0)
        {
            SerializedObject so = new SerializedObject(ps[0]);
            SerializedProperty p = so.FindProperty("activeInputHandler");
            if (p != null && p.intValue != 2)
            {
                p.intValue = 2;
                so.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.SaveAssets();
                inputChanged = true;
            }
        }

        EditorSceneManager.MarkSceneDirty(bucket.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();

        if (inputChanged)
            EditorUtility.DisplayDialog("Done", "Fixed! Input settings changed, so please quit and reopen Unity once, then press Play.", "OK");
        else
            EditorUtility.DisplayDialog("Done", "Fixed! Press Play.", "OK");
    }
}
