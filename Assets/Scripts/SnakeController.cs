using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.WSA;
using Cursor = UnityEngine.Cursor;

public class SnakeController : MonoBehaviour
{
    public List<Transform> BodyParts = new List<Transform>();

    public float minDistance = 0.25f;

    public int beginSize = 0;
    public int maxScore = 0;
    public int maybeMaxScore = 0;
    
    public float speed = 5f;
    public float rotationSpeed = 100f;

    public float TimeFromLastRetry;

    public GameObject bodyprefab;

    public TextMeshProUGUI CurrentScore;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI maxScoreText;

    public GameObject DeadScreen;
    
    private float dis;
    private Transform curBodyPart;
    private Transform prevBodyPart;

    public bool IsAlive;
    
    public AudioSource src;
    public AudioClip dieSound;
    public AudioClip dieMusic;
    public AudioClip crawlSound;
    
    public Gradient gold;
    
    // Start is called before the first frame update
    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        TimeFromLastRetry = Time.time;
        
        DeadScreen.SetActive(false);

        for (int i = BodyParts.Count - 1; i >= 1; i--)
        {
            Destroy(BodyParts[i].gameObject);

            BodyParts.Remove(BodyParts[i]);
        }
        
        BodyParts[0].position = new Vector3(-10f, 1.078716f, 4.2f);

        BodyParts[0].rotation = Quaternion.Euler(0f, 90f, 0f);
        
        CurrentScore.gameObject.SetActive(true);

        CurrentScore.text = "Score: 0";

        IsAlive = true;
        
        if(beginSize != 0)
            for (int i = 0; i < beginSize; i++)
            {
                AddBodyPart();
            }
        
        src.clip = crawlSound;
        src.loop = true;
        src.volume = 0.8f;
        src.Play();
        
        //MAX SKOR
        if (maxScore == 0)
        {
            maxScoreText.gameObject.SetActive(false);
        }
        else
        {
            maxScoreText.gameObject.SetActive(true);
            maxScoreText.text = "Record: " + (maxScore).ToString();
        }
        
        //İmleç gizle
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsAlive)
        {
            Move();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (maybeMaxScore > maxScore)
            {
                maxScoreText.text = "Record: " + (maybeMaxScore).ToString();
            }
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        
        //O basınca kuyruk/skor ekler
        if(Input.GetKeyDown(KeyCode.O))
            AddBodyPart();
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            src.pitch += 0.5f;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            src.pitch -= 0.5f;
        }
        
        //R basınca oyunu yeniden başlatma
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartLevel();
        }
    }

    public void Move()
    {
        float curSpeed = speed;
        
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            curSpeed *= 1.5f;
        }
        
        BodyParts[0].Translate(BodyParts[0].forward * curSpeed * Time.smoothDeltaTime, Space.World);
        
        if(Input.GetAxis("Horizontal") != 0)
            BodyParts[0].Rotate(Vector3.up * rotationSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));

        for (int i = 1; i < BodyParts.Count; i++)
        {
            curBodyPart = BodyParts[i];
            prevBodyPart = BodyParts[i - 1];

            dis = Vector3.Distance(prevBodyPart.position, curBodyPart.position);

            Vector3 newpos = prevBodyPart.position;

            newpos.y = BodyParts[0].position.y;

            float T = Time.deltaTime * dis / minDistance * curSpeed;

            if (T > 0.5f)
                T = 0.5f;
            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
            curBodyPart.rotation = Quaternion.Slerp(curBodyPart.rotation, prevBodyPart.rotation, T);
        }
    }
    
    public void AddBodyPart()
    {
        Transform newpart = (Instantiate(bodyprefab,BodyParts[BodyParts.Count - 1].position, BodyParts[BodyParts.Count - 1].rotation) as GameObject).transform;
        
        newpart.SetParent(transform);
        
        BodyParts.Add(newpart);

        CurrentScore.text = "Score: " + (BodyParts.Count - beginSize - 1).ToString();
        maybeMaxScore = BodyParts.Count - beginSize - 1;
    }

    public void Die()
    {
        IsAlive = false;

        if (maybeMaxScore > maxScore)
        {
            maxScore = maybeMaxScore;
            ScoreText.text = "Your NEW RECORD is " + (BodyParts.Count - beginSize - 1).ToString();
            ScoreText.color = gold.Evaluate(1f);
        }
        else
        {
            ScoreText.text = "Your score was " + (BodyParts.Count - beginSize - 1).ToString();
            ScoreText.color = Color.white;
        }
        
        CurrentScore.gameObject.SetActive(false);
        maxScoreText.gameObject.SetActive(false);
        
        DeadScreen.SetActive(true);
        
        src.clip = dieSound;
        src.loop = false;
        src.volume = 0.6f;
        src.Play();

        StartCoroutine(PlayDieSoundThenContinue());
        
        IEnumerator PlayDieSoundThenContinue()
        {
            yield return new WaitForSeconds(1f);
            src.clip = dieMusic;
            src.loop = true;
            src.volume = 0.2f;
            src.Play();
        }
    }
}
