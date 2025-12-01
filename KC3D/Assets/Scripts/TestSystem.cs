using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Question
{
    [TextArea(2, 4)]
    public string questionText = "Введите вопрос";

    [TextArea(1, 2)]
    public string answerA = "Вариант A";

    [TextArea(1, 2)]
    public string answerB = "Вариант B";

    [TextArea(1, 2)]
    public string answerC = "Вариант C";

    [TextArea(1, 2)]
    public string answerD = "Вариант D";

    [Tooltip("Индекс правильного ответа (0 = A, 1 = B, 2 = C, 3 = D)")]
    [Range(0, 3)]
    public int correctAnswerIndex = 0;
}

public class TestSystem : MonoBehaviour
{
    [Header("НАСТРОЙКА ТЕСТА")]
    [Space(10)]

    [Tooltip("Список вопросов теста")]
    public List<Question> questions = new List<Question>();

    [Header("UI ЭЛЕМЕНТЫ")]
    [Space(10)]

    [Tooltip("Основная панель теста")]
    public GameObject testPanel;

    [Tooltip("Текст для отображения вопроса")]
    public Text questionText;

    [Header("ТЕКСТЫ ДЛЯ КНОПОК ОТВЕТОВ")]
    [Tooltip("Тексты для кнопок ответов (перетащите Text компоненты)")]
    public Text[] answerTexts = new Text[4];

    [Tooltip("Кнопки для ответов (перетащите Button компоненты)")]
    public Button[] answerButtons = new Button[4];

    [Tooltip("Текст для отображения результата")]
    public Text resultText;

    [Tooltip("Кнопка закрытия теста")]
    public Button closeTestButton;

    [Tooltip("Кнопка запуска теста на главной сцене")]
    public Button startTestButton;

    [Header("НАСТРОЙКИ ВНЕШНЕГО ВИДА")]
    [Space(10)]

    [Tooltip("Цвет правильного ответа")]
    public Color correctColor = Color.green;

    [Tooltip("Цвет неправильного ответа")]
    public Color incorrectColor = Color.red;

    [Tooltip("Цвет обычной кнопки")]
    public Color normalColor = Color.white;

    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool testActive = false;

    void Start()
    {
        // Скрываем панель теста при старте
        if (testPanel != null)
            testPanel.SetActive(false);

        // Настраиваем кнопки ответов
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null)
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        // Настраиваем кнопку начала теста
        if (startTestButton != null)
            startTestButton.onClick.AddListener(StartTest);

        // Настраиваем кнопку закрытия теста
        if (closeTestButton != null)
            closeTestButton.onClick.AddListener(CloseTest);
    }

    // Начать тест
    public void StartTest()
    {
        if (questions.Count == 0)
        {
            Debug.LogWarning("Нет вопросов для теста! Добавьте вопросы в инспекторе.");
            return;
        }

        testActive = true;
        currentQuestionIndex = 0;
        score = 0;

        if (testPanel != null)
            testPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "";

        // Сбрасываем цвета кнопок
        ResetButtonColors();

        ShowQuestion(currentQuestionIndex);
    }

    // Показать вопрос
    void ShowQuestion(int questionIndex)
    {
        if (questionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionIndex];

            // Обновляем текст вопроса
            if (questionText != null)
                questionText.text = $"Вопрос {questionIndex + 1}/{questions.Count}\n\n{currentQuestion.questionText}";

            // Получаем массив ответов
            string[] answers = GetAnswersArray(currentQuestion);

            // Обновляем тексты кнопок
            for (int i = 0; i < answerTexts.Length; i++)
            {
                if (answerTexts[i] != null && i < answers.Length)
                {
                    answerTexts[i].text = answers[i];

                    // Активируем/деактивируем кнопки в зависимости от наличия текста
                    if (answerButtons[i] != null)
                    {
                        answerButtons[i].gameObject.SetActive(!string.IsNullOrEmpty(answers[i]));
                    }
                }
            }

            Debug.Log($"Показан вопрос {questionIndex + 1}: {currentQuestion.questionText}");
        }
    }

    // Получить массив ответов из вопроса
    string[] GetAnswersArray(Question question)
    {
        return new string[]
        {
            question.answerA,
            question.answerB,
            question.answerC,
            question.answerD
        };
    }

    // Проверить ответ
    void CheckAnswer(int answerIndex)
    {
        if (!testActive) return;

        // Блокируем кнопки во время проверки
        SetButtonsInteractable(false);

        Question currentQuestion = questions[currentQuestionIndex];

        // Подсвечиваем правильный и неправильный ответы
        HighlightAnswers(currentQuestion.correctAnswerIndex, answerIndex);

        // Обновляем счет
        if (answerIndex == currentQuestion.correctAnswerIndex)
        {
            score++;
            Debug.Log("Правильный ответ!");
        }
        else
        {
            Debug.Log($"Неправильный ответ! Правильный: {currentQuestion.correctAnswerIndex}");
        }

        // Ждем немного перед переходом к следующему вопросу
        StartCoroutine(NextQuestionWithDelay());
    }

    // Подсветка ответов
    void HighlightAnswers(int correctIndex, int selectedIndex)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null && answerButtons[i].gameObject.activeInHierarchy)
            {
                Image buttonImage = answerButtons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    if (i == correctIndex)
                    {
                        buttonImage.color = correctColor;
                    }
                    else if (i == selectedIndex && i != correctIndex)
                    {
                        buttonImage.color = incorrectColor;
                    }
                }
            }
        }
    }

    // Задержка перед следующим вопросом
    private IEnumerator NextQuestionWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        NextQuestion();
    }

    // Следующий вопрос
    void NextQuestion()
    {
        // Разблокируем кнопки
        SetButtonsInteractable(true);

        // Сбрасываем цвета кнопок
        ResetButtonColors();

        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            FinishTest();
        }
    }

    // Установить взаимодействие кнопок
    void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in answerButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }

    // Сбросить цвета кнопок
    void ResetButtonColors()
    {
        foreach (Button button in answerButtons)
        {
            if (button != null)
            {
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = normalColor;
                }
            }
        }
    }

    // Завершить тест
    void FinishTest()
    {
        testActive = false;

        if (resultText != null)
        {
            float percentage = (float)score / questions.Count * 100;
            string grade = GetGrade(percentage);

            resultText.text = $"ТЕСТ ЗАВЕРШЕН!\n" +
                             $"Правильных ответов: {score}/{questions.Count}\n" +
                             $"Результат: {percentage:F1}%\n";
        }

        Debug.Log($"Тест завершен! Результат: {score}/{questions.Count}");
    }

    // Определить оценку
    string GetGrade(float percentage)
    {
        if (percentage >= 90) return "Отлично!";
        if (percentage >= 75) return "Хорошо";
        if (percentage >= 60) return "Удовлетворительно";
        return "Неудовлетворительно";
    }

    // Закрыть тест
    public void CloseTest()
    {
        testActive = false;

        if (testPanel != null)
            testPanel.SetActive(false);
    }

    // Метод для добавления нового вопроса через код (опционально)
    public void AddQuestion(string question, string a, string b, string c, string d, int correctIndex)
    {
        Question newQuestion = new Question
        {
            questionText = question,
            answerA = a,
            answerB = b,
            answerC = c,
            answerD = d,
            correctAnswerIndex = correctIndex
        };

        questions.Add(newQuestion);
    }

    // Очистить все вопросы
    public void ClearAllQuestions()
    {
        questions.Clear();
    }
}