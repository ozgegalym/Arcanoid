using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Добавьте этот импорт для работы с UI

public class PlayerScript : MonoBehaviour
{
    public float playerSpeed = 5.0f; // Скорость перемещения игрока
    private Vector3 playerPosition;
    public float boundary = 9.0f; // Границы движения игрока
    private int playerLives;
    private int playerPoints;
    public AudioClip pointSound;
    public AudioClip lifeSound;
    private AudioSource audioSource;

    public Text levelCompleteText; // Ссылка на текст для отображения завершения уровня
    public Text gameOverText; // Ссылка на текст для отображения окончания игры

    // Start is called before the first frame update
    void Start()
    {
        // get the initial position of the game object
        playerPosition = gameObject.transform.position;

        playerLives = 3;
        playerPoints = 0;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from the player object.");
        }

        // Убедитесь, что текст уровня не отображается при старте
        if (levelCompleteText != null)
        {
            levelCompleteText.text = "";
        }
        else
        {
            Debug.LogError("Level Complete Text is not assigned in the inspector.");
        }

        // Убедитесь, что текст окончания игры не отображается при старте
        if (gameOverText != null)
        {
            gameOverText.text = "";
        }
        else
        {
            Debug.LogError("Game Over Text is not assigned in the inspector.");
        }
    }

    void addPoints(int points)
    {
        playerPoints += points;
        if (audioSource != null && pointSound != null)
        {
            audioSource.PlayOneShot(pointSound);
        }
        else
        {
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned.");
            }
            if (pointSound == null)
            {
                Debug.LogError("PointSound AudioClip is not assigned.");
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5.0f, 3.0f, 200.0f, 200.0f), "Lives: " + playerLives + "    Score: " + playerPoints);
    }

    void TakeLife()
    {
        playerLives--;
        if (audioSource != null && lifeSound != null)
        {
            audioSource.PlayOneShot(lifeSound);
        }
        else
        {
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned.");
            }
            if (lifeSound == null)
            {
                Debug.LogError("LifeSound AudioClip is not assigned.");
            }
        }
        CheckGameOver();
    }

    void WinLose()
    {
        // blocks destroyed
        if ((GameObject.FindGameObjectsWithTag("Block")).Length == 0)
        {
            // check the current level
            if (Application.loadedLevelName == "Level1")
            {
                // Показать сообщение о завершении уровня
                if (levelCompleteText != null)
                {
                    levelCompleteText.text = "Вы прошли уровень!";
                    StartCoroutine(LoadNextLevelAfterDelay(2f)); // Загрузка следующего уровня после задержки
                }
            }
            else
            {
                Application.Quit();
            }
        }
    }

    void CheckGameOver()
    {
        // restart the game
        if (playerLives == 0)
        {
            // Показать сообщение об окончании игры
            if (gameOverText != null)
            {
                gameOverText.text = "Игра окончена!";
                StartCoroutine(RestartGameAfterDelay(2f)); // Перезапуск игры после задержки
            }
        }
    }

    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.LoadLevel("Level2");
    }

    private IEnumerator RestartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.LoadLevel("Level1");
    }

    // Update is called once per frame
    void Update()
    {
        // horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        playerPosition.x += horizontalInput * playerSpeed * Time.deltaTime;

        // leave the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // boundaries
        playerPosition.x = Mathf.Clamp(playerPosition.x, -boundary, boundary);

        // update the game object transform
        transform.position = playerPosition;

        // Check game state
        WinLose();
    }
}
