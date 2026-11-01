using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text scoreText;

    // Background: 0 = sunset, 1 = ocean, 2 = forest
    public int theme = 0;

    int score, lives = 3, combo;
    float timeLeft = 60f, elapsed;
    public bool over;
    TMP_Text timeText, livesText, stageText, comboText, overText;

    public int Stage { get { return Mathf.Clamp(1 + (int)(elapsed / 20f), 1, 3); } }
    public float SpawnInterval() { return 1.1f - 0.27f * (Stage - 1); }
    public float CoinGravity() { return 0.5f + 0.3f * (Stage - 1); }
    int Mult() { return Mathf.Min(4, 1 + combo / 5); }

    void Awake() { Instance = this; Time.timeScale = 1f; }

    void Start()
    {
        BuildBackground();
        BuildHud();
        Refresh();
    }

    void Update()
    {
        if (over)
        {
            if (Input.GetKeyDown(KeyCode.R)) ResetGame();
            return;
        }
        elapsed += Time.deltaTime;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) { timeLeft = 0f; End("Time's up!"); }
        Refresh();
    }

    public void Catch(int value)
    {
        if (over) return;
        combo++;
        score += value * Mult();
        Refresh();
    }

    // A coin hit the ground: no life lost, but the combo resets
    public void MissCoin()
    {
        if (over) return;
        combo = 0;
        Refresh();
    }

    // A bomb was caught: lose a life
    public void HitBomb()
    {
        if (over) return;
        lives--;
        combo = 0;
        Refresh();
        if (lives <= 0) End("You caught 3 bombs!");
    }

    void End(string reason)
    {
        over = true;
        int best = PlayerPrefs.GetInt("best", 0);
        if (score > best) { best = score; PlayerPrefs.SetInt("best", best); }
        overText.text = "GAME OVER\n" + reason + "\nScore: " + score + "   Best: " + best + "\nPress R to play again";
        overText.gameObject.SetActive(true);
        Refresh();
    }

    void ResetGame()
    {
        score = 0; lives = 3; combo = 0; timeLeft = 60f; elapsed = 0f; over = false;
        overText.gameObject.SetActive(false);
        foreach (Coin c in FindObjectsByType<Coin>(FindObjectsSortMode.None))
            Destroy(c.gameObject);
        Refresh();
    }

    void Refresh()
    {
        scoreText.text = "Score: " + score;
        timeText.text = Mathf.CeilToInt(timeLeft) + "s";
        stageText.text = "Stage " + Stage;
        livesText.text = "Lives: " + Mathf.Max(0, lives);
        comboText.text = "Combo x" + Mult();
    }

    TMP_Text MakeText(string name, Transform parent, Vector2 anchor, Vector2 pos, int size, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        RectTransform rt = t.rectTransform;
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(600, 80);
        t.fontSize = size;
        t.alignment = align;
        t.color = Color.white;
        return t;
    }

    void BuildHud()
    {
        Transform canvas = scoreText.transform.parent;

        RectTransform r = scoreText.rectTransform;
        r.anchorMin = new Vector2(0, 1); r.anchorMax = new Vector2(0, 1); r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(30, -20);
        r.sizeDelta = new Vector2(600, 80);
        scoreText.fontSize = 48;
        scoreText.alignment = TextAlignmentOptions.TopLeft;

        comboText = MakeText("ComboText", canvas, new Vector2(0, 1), new Vector2(30, -80), 34, TextAlignmentOptions.TopLeft);
        timeText = MakeText("TimeText", canvas, new Vector2(0.5f, 1), new Vector2(0, -20), 56, TextAlignmentOptions.Top);
        timeText.color = new Color(1f, 0.9f, 0.3f);
        stageText = MakeText("StageText", canvas, new Vector2(0.5f, 1), new Vector2(0, -85), 32, TextAlignmentOptions.Top);
        livesText = MakeText("LivesText", canvas, new Vector2(1, 1), new Vector2(-30, -20), 48, TextAlignmentOptions.TopRight);

        overText = MakeText("GameOverText", canvas, new Vector2(0.5f, 0.5f), Vector2.zero, 64, TextAlignmentOptions.Center);
        overText.rectTransform.sizeDelta = new Vector2(1200, 400);
        overText.gameObject.SetActive(false);
    }

    Sprite WhiteSprite()
    {
        Texture2D t = new Texture2D(4, 4);
        Color[] px = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = Color.white;
        t.SetPixels(px);
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    void BuildBackground()
    {
        Color top, bottom;
        int stars;
        switch (theme)
        {
            case 1:  top = new Color(0.04f, 0.18f, 0.35f); bottom = new Color(0.25f, 0.8f, 0.85f); stars = 0; break;
            case 2:  top = new Color(0.05f, 0.22f, 0.12f); bottom = new Color(0.65f, 0.88f, 0.4f); stars = 0; break;
            default: top = new Color(0.17f, 0.06f, 0.33f); bottom = new Color(1f, 0.5f, 0.37f); stars = 70; break;
        }

        int w = 4, h = 256;
        Texture2D tex = new Texture2D(w, h);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < h; y++)
        {
            Color c = Color.Lerp(bottom, top, (float)y / (h - 1));
            for (int x = 0; x < w; x++) tex.SetPixel(x, y, c);
        }
        tex.Apply();

        GameObject bg = new GameObject("Background");
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 1f);
        sr.sortingOrder = -100;
        bg.transform.localScale = new Vector3(30f / w, 12f / h, 1f);

        Sprite white = WhiteSprite();
        for (int i = 0; i < stars; i++)
        {
            GameObject s = new GameObject("Star");
            s.transform.SetParent(bg.transform, true);
            SpriteRenderer ssr = s.AddComponent<SpriteRenderer>();
            ssr.sprite = white;
            ssr.sortingOrder = -99;
            ssr.color = new Color(1f, 1f, 1f, Random.Range(0.4f, 1f));
            s.transform.position = new Vector3(Random.Range(-9f, 9f), Random.Range(-1f, 5.5f), 0);
            float sz = Random.Range(0.04f, 0.1f);
            s.transform.localScale = new Vector3(sz / (30f / w), sz / (12f / h), 1f);
        }
    }
}
