using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RhythmGameBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Object.FindObjectOfType<RhythmGame>() != null)
        {
            return;
        }

        GameObject root = new GameObject("RhythmGame");
        root.AddComponent<RhythmGame>();
    }
}

public class RhythmGame : MonoBehaviour
{
    public static RhythmGame Instance { get; private set; }
    [Header("Gameplay")]
    [SerializeField] private float spawnInterval = 0.35f;
    [SerializeField] private float fallSpeed = 7.5f;
    [SerializeField] private float spawnY = 5.5f;
    [SerializeField] private float despawnY = -6.0f;

    [Header("Layout")]
    [SerializeField] private float laneOffsetX = 1.0f;
    [SerializeField] private float arrowY = -3.8f;
    [SerializeField] private float arrowScale = 1.2f;
    [SerializeField] private float noteOffset = 1.9f;

    [Header("Vector Styling")]
    [SerializeField] private float arrowLineWidth = 0.18f;
    [SerializeField] private float beamWidth = 0.35f;
    [SerializeField] private float beamLength = 2.4f;
    [SerializeField] private float noteRadius = 0.75f;
    [SerializeField] private int roundedSegments = 12;
    [SerializeField] private int circleSegments = 36;
    [SerializeField] private float noteStroke = 0.18f;
    [SerializeField] private Color mainColor = new Color(0.4667f, 0f, 1f, 1f);

    [Header("Speed")]
    [SerializeField] private float minSpeedMultiplier = 1.0f;
    [SerializeField] private float maxSpeedMultiplier = 3.0f;

    private float spawnTimer;
    private float baseFallSpeed;
    private float baseSpawnInterval;
    private Image flashImage;
    private Text comboText;
    private int comboCount;
    private float speedMultiplier = 1f;

    private Mesh dropletMesh;
    private Material baseMaterial;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Application.targetFrameRate = 60;
        RhythmMeshFactory.Initialize();

        dropletMesh = RhythmMeshFactory.Droplet;
        baseMaterial = RhythmMeshFactory.CreateBaseMaterial();
    }

    private void Start()
    {
        baseFallSpeed = fallSpeed;
        baseSpawnInterval = spawnInterval;
        BuildScene();
        BuildFlashUI();
        BuildComboUI();
    }

    private void Update()
    {
        UpdateSpeedHotkeys();

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= CurrentSpawnInterval)
        {
            spawnTimer -= CurrentSpawnInterval;
            SpawnNote(Random.Range(0, 2));
        }
    }

    private void BuildScene()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = Color.white;
        }

        CreateArrow(
            "LeftArrow",
            new Vector3(-laneOffsetX, arrowY, 0f),
            Key.F,
            mainColor,
            Vector3.left,
            true
        );

        CreateArrow(
            "RightArrow",
            new Vector3(laneOffsetX, arrowY, 0f),
            Key.J,
            mainColor,
            Vector3.right,
            false
        );
    }

    private void BuildFlashUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("UI");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        GameObject flashGo = new GameObject("ScreenFlash");
        flashGo.transform.SetParent(canvas.transform, false);
        flashImage = flashGo.AddComponent<Image>();
        flashImage.color = new Color(mainColor.r, mainColor.g, mainColor.b, 0f);
        RectTransform flashRect = flashImage.GetComponent<RectTransform>();
        flashRect.anchorMin = Vector2.zero;
        flashRect.anchorMax = Vector2.one;
        flashRect.offsetMin = Vector2.zero;
        flashRect.offsetMax = Vector2.zero;
    }

    private void BuildComboUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        GameObject comboGo = new GameObject("ComboText");
        comboGo.transform.SetParent(canvas.transform, false);
        comboText = comboGo.AddComponent<Text>();
        comboText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        comboText.fontSize = 64;
        comboText.alignment = TextAnchor.MiddleCenter;
        comboText.color = mainColor;
        comboText.text = "0";
        comboText.fontStyle = FontStyle.Bold;

        RectTransform rect = comboText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300f, 90f);
        rect.anchoredPosition = Vector2.zero;
    }

    private void UpdateSpeedHotkeys()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) { ApplySpeedMultiplier(1); }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) { ApplySpeedMultiplier(2); }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) { ApplySpeedMultiplier(3); }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) { ApplySpeedMultiplier(4); }
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) { ApplySpeedMultiplier(5); }
        else if (Keyboard.current.digit6Key.wasPressedThisFrame) { ApplySpeedMultiplier(6); }
        else if (Keyboard.current.digit7Key.wasPressedThisFrame) { ApplySpeedMultiplier(7); }
        else if (Keyboard.current.digit8Key.wasPressedThisFrame) { ApplySpeedMultiplier(8); }
        else if (Keyboard.current.digit9Key.wasPressedThisFrame) { ApplySpeedMultiplier(9); }
    }

    private void ApplySpeedMultiplier(int digit)
    {
        float t = Mathf.InverseLerp(1f, 9f, digit);
        speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, t);
    }

    public float CurrentFallSpeed
    {
        get { return baseFallSpeed * speedMultiplier; }
    }

    public float CurrentSpawnInterval
    {
        get { return baseSpawnInterval / speedMultiplier; }
    }

    public void FlashRed()
    {
        if (flashImage == null)
        {
            return;
        }
        ResetCombo();
        StopCoroutine(nameof(FlashRoutine));
        StartCoroutine(FlashRoutine());
    }

    public void RegisterHit()
    {
        comboCount += 1;
        UpdateComboText();
    }

    private void ResetCombo()
    {
        comboCount = 0;
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = comboCount.ToString();
        }
    }

    private IEnumerator FlashRoutine()
    {
        float duration = 0.15f;
        float peak = 0.55f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float alpha = Mathf.Lerp(peak, 0f, t);
            flashImage.color = new Color(mainColor.r, mainColor.g, mainColor.b, alpha);
            yield return null;
        }
        flashImage.color = new Color(mainColor.r, mainColor.g, mainColor.b, 0f);
    }

    private void CreateArrow(string name, Vector3 position, Key key, Color color, Vector3 direction, bool left)
    {
        GameObject arrow = new GameObject(name);
        arrow.transform.SetParent(transform, false);
        arrow.transform.position = position;
        arrow.transform.localScale = Vector3.one * arrowScale;
        arrow.transform.rotation = Quaternion.identity;

        LineRenderer line = arrow.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = 3;
        line.widthMultiplier = arrowLineWidth;
        line.material = new Material(baseMaterial);
        line.startColor = mainColor;
        line.endColor = mainColor;
        line.numCornerVertices = roundedSegments;
        line.numCapVertices = roundedSegments;
        line.sortingOrder = 2;
        line.sortingLayerID = SortingLayer.NameToID("Default");

        Vector3 top = left ? new Vector3(0.5f, 0.5f, 0f) : new Vector3(-0.5f, 0.5f, 0f);
        Vector3 mid = Vector3.zero;
        Vector3 bot = left ? new Vector3(0.5f, -0.5f, 0f) : new Vector3(-0.5f, -0.5f, 0f);
        line.SetPosition(0, top);
        line.SetPosition(1, mid);
        line.SetPosition(2, bot);

        GameObject beam = new GameObject("Beam");
        beam.transform.SetParent(arrow.transform, false);
        beam.transform.localPosition = Vector3.zero;

        LineRenderer beamRenderer = beam.AddComponent<LineRenderer>();
        beamRenderer.useWorldSpace = false;
        beamRenderer.positionCount = 2;
        beamRenderer.SetPosition(0, Vector3.zero);
        beamRenderer.SetPosition(1, Vector3.up * beamLength);
        beamRenderer.widthMultiplier = beamWidth;
        beamRenderer.material = new Material(baseMaterial);
        beamRenderer.startColor = new Color(mainColor.r, mainColor.g, mainColor.b, 0.35f);
        beamRenderer.endColor = new Color(mainColor.r, mainColor.g, mainColor.b, 0.35f);
        beamRenderer.numCornerVertices = roundedSegments;
        beamRenderer.numCapVertices = roundedSegments;
        beamRenderer.sortingOrder = 1;
        beamRenderer.sortingLayerID = SortingLayer.NameToID("Default");

        BoxCollider2D box = beam.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(beamWidth, beamLength);
        box.offset = new Vector2(0f, beamLength * 0.5f);

        ArrowHit hit = beam.AddComponent<ArrowHit>();
        hit.SetActive(false);

        ArrowController controller = arrow.AddComponent<ArrowController>();
        controller.Configure(
            key,
            arrow.transform,
            beam.transform,
            beamRenderer,
            hit,
            mainColor,
            direction,
            beamWidth,
            noteRadius
        );
    }



    private void SpawnNote(int laneIndex)
    {
        float direction = laneIndex == 0 ? -1f : 1f;
        float x = (laneOffsetX * direction) + (noteOffset * direction);

        GameObject note = new GameObject("Note");
        note.transform.SetParent(transform, false);
        note.transform.position = new Vector3(x, spawnY, 0f);

        LineRenderer circleRenderer = note.AddComponent<LineRenderer>();
        circleRenderer.useWorldSpace = false;
        circleRenderer.loop = true;
        circleRenderer.positionCount = circleSegments;
        circleRenderer.widthMultiplier = noteStroke;
        circleRenderer.material = new Material(baseMaterial);
        circleRenderer.startColor = mainColor;
        circleRenderer.endColor = circleRenderer.startColor;
        circleRenderer.numCornerVertices = roundedSegments;
        circleRenderer.numCapVertices = roundedSegments;
        circleRenderer.sortingOrder = 3;
        circleRenderer.sortingLayerID = SortingLayer.NameToID("Default");

        RhythmMeshFactory.FillCirclePoints(circleRenderer, noteRadius, circleSegments);

        CircleCollider2D noteCollider = note.AddComponent<CircleCollider2D>();
        noteCollider.isTrigger = true;
        noteCollider.radius = noteRadius;
        Rigidbody2D rb = note.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0f;

        Note noteScript = note.AddComponent<Note>();
        noteScript.Configure(fallSpeed, despawnY, dropletMesh, baseMaterial, circleRenderer.startColor);
    }
}

public class ArrowController : MonoBehaviour
{
    [SerializeField] private float extendDistance = 1.2f;
    [SerializeField] private float extendDuration = 0.03f;
    [SerializeField] private float retractDuration = 0.04f;

    private Key key;
    private Transform arrowTransform;
    private Transform beamTransform;
    private LineRenderer beamRenderer;
    private ArrowHit hit;
    private Vector3 baseArrowPosition;
    private Vector3 extendDirection = Vector3.right;
    private bool animating;
    private Color baseColor;
    private float beamWidth = 0.35f;
    private float hitRadius = 0.4f;
    private bool hitThisPulse;

    public void Configure(
        Key keyCode,
        Transform arrow,
        Transform beam,
        LineRenderer beamLineRenderer,
        ArrowHit arrowHit,
        Color color,
        Vector3 direction,
        float beamBaseWidth,
        float noteHitRadius)
    {
        key = keyCode;
        arrowTransform = arrow;
        beamTransform = beam;
        beamRenderer = beamLineRenderer;
        hit = arrowHit;
        baseColor = color;
        extendDirection = direction.normalized;
        beamWidth = beamBaseWidth;
        hitRadius = noteHitRadius;
        baseArrowPosition = arrowTransform.position;

        float angle = Mathf.Atan2(extendDirection.y, extendDirection.x) * Mathf.Rad2Deg - 90f;
        beamTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        if (!animating && Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
        {
            StartCoroutine(ExtendRoutine());
        }
    }

    private IEnumerator ExtendRoutine()
    {
        animating = true;
        hitThisPulse = false;
        hit.SetActive(true);

        yield return Animate(0f, 1f, extendDuration);
        yield return Animate(1f, 0f, retractDuration);

        hit.SetActive(false);
        if (!hitThisPulse && RhythmGame.Instance != null)
        {
            RhythmGame.Instance.FlashRed();
        }
        animating = false;
    }

    public void RegisterHit()
    {
        hitThisPulse = true;
    }

    private IEnumerator Animate(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(from, to, t);

            arrowTransform.position = baseArrowPosition + extendDirection * extendDistance * eased;

            float beamScale = Mathf.Lerp(0.1f, 1f, eased);
            beamTransform.localScale = new Vector3(1f, beamScale, 1f);
            beamTransform.localPosition = Vector3.zero;

            Color c = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(0.1f, 0.5f, eased));
            beamRenderer.startColor = c;
            beamRenderer.endColor = c;

            if (!hitThisPulse)
            {
                TryHit();
            }

            yield return null;
        }
    }

    private void TryHit()
    {
        Vector2 center = (Vector2)(baseArrowPosition + extendDirection * extendDistance);
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            Note note = hits[i].GetComponent<Note>();
            if (note != null && note.PopExternal())
            {
                hitThisPulse = true;
                if (RhythmGame.Instance != null)
                {
                    RhythmGame.Instance.RegisterHit();
                }
                break;
            }
        }
    }
}

public class ArrowHit : MonoBehaviour
{
    private BoxCollider2D box;
    private LineRenderer lineRenderer;
    public bool Active { get; private set; }
    private ArrowController controller;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        controller = GetComponentInParent<ArrowController>();
    }

    public void SetActive(bool active)
    {
        Active = active;
        if (box != null)
        {
            box.enabled = active;
        }
        if (lineRenderer != null)
        {
            lineRenderer.enabled = active;
        }
    }

    public void RegisterHit()
    {
        if (controller != null)
        {
            controller.RegisterHit();
        }
    }
}

public class Note : MonoBehaviour
{
    private float despawnY;
    private Mesh dropletMesh;
    private Material baseMaterial;
    private Color color;
    private bool popped;

    public void Configure(float fallSpeed, float despawnLimit, Mesh droplet, Material material, Color noteColor)
    {
        despawnY = despawnLimit;
        dropletMesh = droplet;
        baseMaterial = material;
        color = noteColor;
    }

    private void Update()
    {
        float speed = RhythmGame.Instance != null ? RhythmGame.Instance.CurrentFallSpeed : 0f;
        transform.position += Vector3.down * speed * Time.deltaTime;
        if (transform.position.y < despawnY)
        {
            if (RhythmGame.Instance != null)
            {
                RhythmGame.Instance.FlashRed();
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ArrowHit hit = other.GetComponent<ArrowHit>();
        if (hit != null && hit.Active && PopExternal())
        {
            hit.RegisterHit();
            if (RhythmGame.Instance != null)
            {
                RhythmGame.Instance.RegisterHit();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ArrowHit hit = other.GetComponent<ArrowHit>();
        if (hit != null && hit.Active && PopExternal())
        {
            hit.RegisterHit();
            if (RhythmGame.Instance != null)
            {
                RhythmGame.Instance.RegisterHit();
            }
        }
    }

    public bool PopExternal()
    {
        if (popped)
        {
            return false;
        }
        popped = true;
        Pop();
        return true;
    }

    private void Pop()
    {
        if (popped == false)
        {
            popped = true;
        }
        GameObject splash = new GameObject("Splash");
        splash.transform.position = transform.position;
        Splash splashScript = splash.AddComponent<Splash>();
        splashScript.Configure(dropletMesh, baseMaterial, color);
        Destroy(gameObject);
    }
}

public class Splash : MonoBehaviour
{
    private readonly List<Droplet> droplets = new List<Droplet>();
    private float lifetime = 0.6f;
    private float timer;

    private struct Droplet
    {
        public Transform Transform;
        public Vector2 Velocity;
        public LineRenderer Renderer;
    }

    public void Configure(Mesh dropletMesh, Material baseMaterial, Color color)
    {
        int count = 8;
        for (int i = 0; i < count; i++)
        {
            GameObject d = new GameObject("Droplet");
            d.transform.SetParent(transform, false);
            d.transform.localPosition = Vector3.zero;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float speed = Random.Range(1.2f, 3.6f);

            LineRenderer renderer = d.AddComponent<LineRenderer>();
            renderer.useWorldSpace = false;
            renderer.loop = true;
            renderer.positionCount = 20;
            renderer.widthMultiplier = 0.12f;
            renderer.material = new Material(baseMaterial);
            renderer.startColor = new Color(color.r, color.g, color.b, 0.9f);
            renderer.endColor = renderer.startColor;
            renderer.numCornerVertices = 8;
            renderer.numCapVertices = 8;
            renderer.sortingOrder = 4;
            renderer.sortingLayerID = SortingLayer.NameToID("Default");
            RhythmMeshFactory.FillCirclePoints(renderer, 0.14f, 20);

            droplets.Add(new Droplet
            {
                Transform = d.transform,
                Velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed,
                Renderer = renderer
            });
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifetime);

        for (int i = 0; i < droplets.Count; i++)
        {
            Droplet droplet = droplets[i];
            droplet.Transform.position += (Vector3)(droplet.Velocity * Time.deltaTime);
            float alpha = Mathf.Lerp(0.9f, 0f, t);
            Color c = droplet.Renderer.startColor;
            Color next = new Color(c.r, c.g, c.b, alpha);
            droplet.Renderer.startColor = next;
            droplet.Renderer.endColor = next;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

public static class RhythmMeshFactory
{
    public static Mesh Circle { get; private set; }
    public static Mesh Droplet { get; private set; }

    public static void Initialize()
    {
        if (Circle != null)
        {
            return;
        }

        Circle = CreateCircleMesh(48);
        Droplet = CreateCircleMesh(16);
    }

    public static void FillCirclePoints(LineRenderer line, float radius, int segments)
    {
        if (segments < 3)
        {
            segments = 3;
        }

        Vector3[] points = new Vector3[segments];
        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * step;
            points[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }

        line.positionCount = segments;
        line.SetPositions(points);
    }

    public static Material CreateBaseMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        }
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        Material mat = new Material(shader);
        mat.color = Color.white;
        return mat;
    }

    private static Mesh CreateCircleMesh(int segments)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero);
        float angleStep = Mathf.PI * 2f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            vertices.Add(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f));
        }

        for (int i = 1; i <= segments; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

}
