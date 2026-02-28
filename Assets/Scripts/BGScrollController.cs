using UnityEngine;
using UnityEngine.UI;

public class BGScrollController : MonoBehaviour
{
    public RectTransform imagePrefab;
    public int imageCount = 5;
    public float speed = 50f;
    public float spacing = 200f;
    public bool moveRight = true;

    private RectTransform[] images;

    void Start()
    {
        images = new RectTransform[imageCount];

        for (int i = 0; i < imageCount; i++)
        {
            RectTransform img = Instantiate(imagePrefab, transform);
            img.anchoredPosition =
                new Vector2(i * spacing * (moveRight ? -1 : 1), 0);

            images[i] = img;
        }

        //imagePrefab.gameObject.SetActive(false);
    }

    void Update()
    {
        float dir = moveRight ? 1 : -1;

        foreach (RectTransform img in images)
        {
            img.anchoredPosition +=
                Vector2.right * dir * speed * Time.deltaTime;

            if (moveRight && img.anchoredPosition.x > 1000)
                img.anchoredPosition =
                    new Vector2(-1000, img.anchoredPosition.y);

            if (!moveRight && img.anchoredPosition.x < -1000)
                img.anchoredPosition =
                    new Vector2(1000, img.anchoredPosition.y);
        }
    }
}
