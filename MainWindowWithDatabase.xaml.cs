using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
// Класс для базы данных.
using System.Data;
using System.Data.SqlClient;

namespace Tetris2 {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  /// 

  public partial class MainWindow : Window {
    // Чтобы отрисовывать через временные интервалы.
    DispatcherTimer timer;
    // Чтобы генерить форму фигуры.
    Random figureRandom;
    // Счетчики ряда и столбцов для отрисовки новых фигур.
    private int rowCount = 0;
    private int columnCount = 0;
    // Положение фигуры.
    private int leftPos = 0;
    private int downPos = 0;
    // Размеры фигуры.
    private int FigureWidth;
    private int FigureHeigth;
    // Текущая и следующая фигура.
    private int currentFigureNumber;
    private int nextFigureNumber;
    // Переменные для хранения количества столбцов и рядов.
    private int tetrisGridColumn;
    private int tetrisGridRow;
    // Угол поворота фигуры.
    private int rotation = 0;
    // Пауза.
    private bool isGamePaused = false;
    // Флаг нарисована ли следующая фигура.
    private bool isNextFigureDrawed = false;
    // Флаг поворота.
    private bool isRotated = false;
    // Флаги столкновения.
    private bool isBottomCollided = false;
    private bool isLeftCollided = false;
    private bool isRightCollided = false;
    // Флаг проигрыша.
    private bool isGameOver = false;
    // Скорость игры. TODO: Увеличивать ее.
    private int gameSpeed = 250;
    // Очки.
    private int gameScore = 0;
    // Массив с текущей фигурой.
    private int[,] currentFigure = null;
    // 
    List<int> currentFigureRow = null;
    List<int> currentFigureColumn = null;

    // Массив с названиями фигур.
    string[] arrayFigures = {"",           "O_Figure",
                                 "I_Figure_0", "T_Figure_0",
                                 "S_Figure_0", "Z_Figure_0",
                                 "J_Figure_0", "L_Figure_0"};
    // Массивы с фигурами.
    // Квадрат.
    public int[,] O_Figure = new int[2, 2] {{1, 1},
                                                {1, 1}};
    // I-фигура.
    public int[,] I_Figure_0 = new int[2, 4] {{1, 1, 1, 1},
                                                  {0, 0, 0, 0}};
    public int[,] I_Figure_90 = new int[4, 2] {{1, 0},
                                                   {1, 0},
                                                   {1, 0},
                                                   {1, 0}};
    // T-фигура
    public int[,] T_Figure_0 = new int[2, 3] {{0, 1, 0},
                                                  {1, 1, 1}};
    public int[,] T_Figure_90 = new int[3, 2] {{1, 0},
                                                   {1, 1},
                                                   {1, 0}};
    public int[,] T_Figure_180 = new int[2, 3] {{1, 1, 1},
                                                    {0, 1, 0}};
    public int[,] T_Figure_270 = new int[3, 2] {{0, 1},
                                                    {1, 1},
                                                    {0, 1}};
    // S-фигура.
    public int[,] S_Figure_0 = new int[2, 3] {{0, 1, 1},
                                                  {1, 1, 0}};
    public int[,] S_Figure_90 = new int[3, 2] {{1, 0},
                                                   {1, 1},
                                                   {0, 1}};
    // Z-фигура.
    public int[,] Z_Figure_0 = new int[2, 3] {{1, 1, 0},
                                                  {0, 1, 1}};
    public int[,] Z_Figure_90 = new int[3, 2] {{0, 1},
                                                   {1, 1},
                                                   {1, 0}};
    // J-фигура.
    public int[,] J_Figure_0 = new int[2, 3] {{1,0,0},
                                                  {1,1,1}};
    public int[,] J_Figure_90 = new int[3, 2] {{1, 1},
                                                   {1, 0},
                                                   {1, 0}};
    public int[,] J_Figure_180 = new int[2, 3] {{1, 1, 1},
                                                    {0, 0, 1}};
    public int[,] J_Figure_270 = new int[3, 2] {{0, 1},
                                                    {0, 1},
                                                    {1, 1}};
    // L-фигура.
    public int[,] L_Figure_0 = new int[2, 3] {{0, 0, 1},
                                                  {1, 1, 1}};
    public int[,] L_Figure_90 = new int[3, 2] {{1, 0},
                                                   {1, 0},
                                                   {1, 1}};
    public int[,] L_Figure_180 = new int[2, 3] {{1, 1, 1},
                                                    {1, 0, 0}};
    public int[,] L_Figure_270 = new int[3, 2] {{1, 1},
                                                    {0, 1},
                                                    {0, 1}};

    public MainWindow() {
      InitializeComponent();
      // Считывание нажатия клавиши.
      KeyDown += MainWindow_KeyDown;
      // Создание интервала, в течение которого будет исполняться "ход" игры.
      timer = new DispatcherTimer {
        // Время между циклами 0, приоритет не важен (0), ?, длительность цикла 0.3 секунды.
        Interval = new TimeSpan(0, 0, 0, 0, gameSpeed)
      };

      timer.Tick += TimerTick;
      tetrisGridColumn = tetrisGrid.ColumnDefinitions.Count;
      tetrisGridRow = tetrisGrid.RowDefinitions.Count;
      figureRandom = new Random();
      // Присваивание индекса текущей и следующей фигур.
      currentFigureNumber = figureRandom.Next(1, 8);
      nextFigureNumber = figureRandom.Next(1, 8);
      GameOverTxt.Visibility = Visibility.Collapsed;
    }

    // Считывания клавиши поворота, движения вниз, вправо и влево.
    private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
      if (!timer.IsEnabled) {
        return;
      }
      switch (e.Key.ToString()) {
        case "Up":
          rotation += 90;
          if (rotation > 270) {
            rotation = 0;
          }
          FigureRotation(rotation);
          break;
        case "Down":
          downPos++;
          break;
        case "Right":
          // Проверка на столкновения.
          FigureCollided();
          if (!isRightCollided) {
            leftPos++;
          }
          isRightCollided = false;
          break;
        case "Left":
          // Проверка на столкновения.
          FigureCollided();
          if (!isLeftCollided) {
            leftPos--;
          }
          isLeftCollided = false;
          break;
      }
      MoveFigure();
    }

    // Поворот фигуры.
    private void FigureRotation(int _rotation) {
      // Проверка на столкновения.
      if (RotationCollided(rotation)) {
        rotation -= 90;
        return;
      }
      if (arrayFigures[currentFigureNumber].IndexOf("I_") == 0) {
        if (_rotation > 90) {
          _rotation = rotation = 0;
        }
        currentFigure = GetVariableByString("I_Figure_" + _rotation);
      } else if (arrayFigures[currentFigureNumber].IndexOf("T_") == 0) {
        currentFigure = GetVariableByString("T_Figure_" + _rotation);
      } else if (arrayFigures[currentFigureNumber].IndexOf("S_") == 0) {
        if (_rotation > 90) {
          _rotation = rotation = 0;
        }
        currentFigure = GetVariableByString("S_Figure_" + _rotation);
      } else if (arrayFigures[currentFigureNumber].IndexOf("Z_") == 0) {
        if (_rotation > 90) {
          _rotation = rotation = 0;
        }
        currentFigure = GetVariableByString("Z_Figure_" + _rotation);
      } else if (arrayFigures[currentFigureNumber].IndexOf("J_") == 0) {
        currentFigure = GetVariableByString("J_Figure_" + _rotation);
      } else if (arrayFigures[currentFigureNumber].IndexOf("L_") == 0) {
        currentFigure = GetVariableByString("L_Figure_" + _rotation);
      } else if (arrayFigures[currentFigureNumber].IndexOf("O_") == 0) {
        return;
      }
      isRotated = true;
      AddFigure(currentFigureNumber, leftPos, downPos);
    }


    // Движение фигуры вниз во время одного цикла игры.
    private void TimerTick(object sender, EventArgs e) {
      downPos++;
      MoveFigure();
    }


    // Сохранение данных в бд.
    private static void SaveConditionToDatabase(string ScoreText,
                                                int    CurrentFigure = 1,
                                                int    NextFigure = 1,
                                                string GridElements = "101001")
    {
      // Подключение.
      // Если бд изменять/подключать другую.
      string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Tetris;Integrated Security=True";
      using (SqlConnection connection = new SqlConnection(connectionString)) {
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        // insert change to update
        command.CommandText = @"INSERT INTO Condition VALUES (@Grid, @CurrentFigure, @NextFigure, @Score)";
        command.Parameters.Add("@Grid", SqlDbType.NVarChar).Value = GridElements;
        command.Parameters.Add("@CurrentFigure", SqlDbType.Int).Value = CurrentFigure;
        command.Parameters.Add("@NextFigure", SqlDbType.Int).Value = NextFigure;
        int Score = Int32.Parse(ScoreText);
        command.Parameters.Add("@Score", SqlDbType.Int).Value = Score;
        command.ExecuteNonQuery();
      }
    }


    #region вариант с Tuples
    // Получить состояние из бд.
    // Tuple?
    private static void GetConditionFromDatabase()
    {
      // Обращение к бд
      string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Tetris;Integrated Security=True";
      SqlConnection connection = new SqlConnection(connectionString);
      connection.Open();
      string sql = "SELECT * FROM Condition";
      SqlCommand command = new SqlCommand(sql, connection);
      SqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        int Id = reader.GetInt32(0);
        string GridElements = reader.GetString(1);
        int CurrentFigure = reader.GetInt32(2);
        int NextFigure = reader.GetInt32(3);
        int ScoreText = reader.GetInt32(4);
      }
      #region
      //connection.Open();
        //SqlCommand command = new SqlCommand();
        //command.Connection = connection;
        //command.CommandText = @"SELECT Score FROM Condition ORDER BY Id DESC LIMIT 1";
        //command.ExecuteReader().GetInt32;
        //string ScoreText = "";
        //int CurrentFigure = 1;
        //int NextFigure = 1;
        //string GridElements = "";
        ////SELECT * FROM TABLE order by id DESC LIMIT 1

      //command.ExecuteNonQuery();
      #endregion

      // Выдача данных.
      //return Tuple.Create(ScoreText, CurrentFigure, NextFigure, GridElements);
    }
    #endregion

    private static int GetScoreFromDatabase()
    {
      string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Tetris;Integrated Security=True";
      SqlConnection connection = new SqlConnection(connectionString);
      connection.Open();
      string sql = "SELECT * FROM Condition";
      SqlCommand command = new SqlCommand(sql, connection);
      SqlDataReader reader = command.ExecuteReader();
      int ScoreText = 0;
      while (reader.Read()) {
        ScoreText = reader.GetInt32(4);
      }
      // Выдача данных.
      return ScoreText;
    }


    #region not working (for nchar(1) currentFigure and nextFigure)
    private static int GetCurrentFigureFromDatabase() {
      string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Tetris;Integrated Security=True";
      SqlConnection connection = new SqlConnection(connectionString);
      connection.Open();
      string sql = "SELECT * FROM Condition";
      SqlCommand command = new SqlCommand(sql, connection);
      SqlDataReader reader = command.ExecuteReader();
      int CurrentFigure = 0;
      while (reader.Read()) {
        CurrentFigure = Int32.Parse(reader.GetString(2));
      }
      // Выдача данных.
      return CurrentFigure;
    }
    #endregion


    #region not working (for int currentFigure and nextFigure)
    //private static int GetCurrentFigureFromDatabase() {
    //  string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Tetris;Integrated Security=True";
    //  SqlConnection connection = new SqlConnection(connectionString);
    //  connection.Open();
    //  string sql = "SELECT * FROM Condition";
    //  SqlCommand command = new SqlCommand(sql, connection);
    //  SqlDataReader reader = command.ExecuteReader();
    //  int CurrentFigure = 0;
    //  while (reader.Read()) {
    //    CurrentFigure = reader.GetInt32(2);
    //  }
    //  // Выдача данных.
    //  return CurrentFigure;
    //}
    #endregion


    // Кнопка запуска/паузы.
    private void Button_Click_1(object sender, RoutedEventArgs e) {
      // Если проигрыш.
      if (isGameOver) {
        tetrisGrid.Children.Clear();
        nextFigureCanvas.Children.Clear();
        GameOverTxt.Visibility = Visibility.Collapsed;
        isGameOver = false;
      }
      // Если игра в процессе (идет или остановлена).
      if (!timer.IsEnabled) {
        // Если игра будет запущена,
        // то устанавливается счет и добавляется фигура.
        if (!isGamePaused) {
          scoreTxt.Text = "0";
          leftPos = 3;
          AddFigure(currentFigureNumber, leftPos);
          // Загрузка данных.
        }
        // Забираем очки из бд.
        scoreTxt.Text = GetScoreFromDatabase().ToString();
        #region Бросает NullException в GetVariableByString. Причина не ясна.
        // currentFigureNumber = GetCurrentFigureFromDatabase();
        #endregion
        timer.Start();
        startStopBtn.Content = "ПАУЗА";
        isGamePaused = true;
      }
      else {
        timer.Stop();
        startStopBtn.Content = "ПРОДОЛЖИТЬ";
        // Сохранение состояния игры.
        SaveConditionToDatabase(scoreTxt.Text, currentFigureNumber, nextFigureNumber);
        // TODO: сохранение поля.
        // SaveConditionToDatabase(scoreTxt.Text, currentFigureNumber, nextFigureNumber, tetrisGrid);
      }
    }

    // Отрисовка фигуры.
    private void AddFigure(int figureNumber, int _left = 0, int _down = 0) {
      // Удаление предыдущего положения фигуры.
      RemoveFigure();
      currentFigureRow = new List<int>();
      currentFigureColumn = new List<int>();
      Rectangle square = null;
      if (!isRotated) {
        currentFigure = null;
        currentFigure = GetVariableByString(arrayFigures[figureNumber].ToString());
      }
      int firstDim = currentFigure.GetLength(0);
      int secondDim = currentFigure.GetLength(1);
      FigureWidth = secondDim;
      FigureHeigth = firstDim;
      // Если I-фигура.
      if (currentFigure == I_Figure_90) {
        FigureWidth = 1;
      } else if (currentFigure == I_Figure_0) {
        FigureHeigth = 1;
      }
      // Отрисовка.
      for (int row = 0; row < firstDim; row++) {
        for (int column = 0; column < secondDim; column++) {
          int bit = currentFigure[row, column];
          if (bit == 1) {
            square = GetBasicSquare(Colors.Lime);
            tetrisGrid.Children.Add(square);
            square.Name = "moving_" + Grid.GetRow(square) + "_" + Grid.GetColumn(square);
            if (_down >= tetrisGrid.RowDefinitions.Count - FigureHeigth) {
              _down = tetrisGrid.RowDefinitions.Count - FigureHeigth;
            }
            Grid.SetRow(square, rowCount + _down);
            Grid.SetColumn(square, columnCount + _left);
            currentFigureRow.Add(rowCount + _down);
            currentFigureColumn.Add(columnCount + _left);
          }
          columnCount++;
        }
        columnCount = 0;
        rowCount++;
      }
      columnCount = 0;
      rowCount = 0;
      // Отрисовка подсказки следующей фигуры.
      if (!isNextFigureDrawed) {
        DrawNextFigure(nextFigureNumber);
      }
    }

    // Перемещение фигуры в новое положение.
    private void MoveFigure() {
      isLeftCollided = false;
      isRightCollided = false;
      // Проверка столкновения.
      FigureCollided();
      if (leftPos > (tetrisGridColumn - FigureWidth)) {
        leftPos = (tetrisGridColumn - FigureWidth);
      } else if (leftPos < 0) {
        leftPos = 0;
      }
      if (isBottomCollided) {
        FigureStoped();
        return;
      }
      AddFigure(currentFigureNumber, leftPos, downPos);
    }

    // Проверка на столкновения при повороте.
    private bool RotationCollided(int _rotation) {
      // Проверка под фигурой.
      if (CheckCollided(0, FigureWidth - 1)) {
        return true;
      }
        // Проверка над фигурой.
      else if (CheckCollided(0, -(FigureWidth - 1))) {
        return true;
      }
        // Проверка над фигурой.
      else if (CheckCollided(0, -1)) {
        return true;
      }
        // Проверка слева от фигуры.
      else if (CheckCollided(-1, FigureWidth - 1)) {
        return true;
      }
        // Проверка справа от фигуры.
      else if (CheckCollided(1, FigureWidth - 1)) {
        return true;
      }
      return false;
    }

    // Проверка на столкновения под фигурой, слева и справа.
    private void FigureCollided() {
      isBottomCollided = CheckCollided(0, 1);
      isLeftCollided = CheckCollided(-1, 0);
      isRightCollided = CheckCollided(1, 0);
    }

    // Проверка столкновения.
    private bool CheckCollided(int _leftRightOffset, int _bottomOffset) {
      Rectangle movingSquare;
      int squareRow = 0;
      int squareColumn = 0;
      for (int i = 0; i <= 3; i++) {
        squareRow = currentFigureRow[i];
        squareColumn = currentFigureColumn[i];
        try {
          movingSquare = (Rectangle)tetrisGrid.Children
          .Cast<UIElement>()
          .FirstOrDefault(e => Grid.GetRow(e) == squareRow + _bottomOffset && Grid.GetColumn(e) == squareColumn + _leftRightOffset);
          if (movingSquare != null) {
            if (movingSquare.Name.IndexOf("arrived") == 0) {
              return true;
            }
          }
        } catch {
        }
      }
      if (downPos > (tetrisGridRow - FigureHeigth)) {
        return true;
      }
      return false;
    }

    // Отрисовка фигуры, которая будет следующей.
    private void DrawNextFigure(int figureNumber) {
      nextFigureCanvas.Children.Clear();
      int[,] nextFigureFigure = null;
      nextFigureFigure = GetVariableByString(arrayFigures[figureNumber]);
      int firstDim = nextFigureFigure.GetLength(0);
      int secondDim = nextFigureFigure.GetLength(1);
      int x = 0;
      int y = 0;
      Rectangle square;
      for (int row = 0; row < firstDim; row++) {
        for (int column = 0; column < secondDim; column++) {
          int bit = nextFigureFigure[row, column];
          if (bit == 1) {
            square = GetBasicSquare(Colors.Lime);
            nextFigureCanvas.Children.Add(square);
            Canvas.SetLeft(square, x);
            Canvas.SetTop(square, y);
          }
          x += 20;
        }
        x = 0;
        y += 20;
      }
      isNextFigureDrawed = true;
    }

    // Если фигура столкнулась с дном или другой фигурой (остановилась).
    private void FigureStoped()
        {
            timer.Stop();
            // Условия проигрыша.
            if (downPos <= 2)
            {
                GameOver();
                return;
            }
            int index = 0;
            while (index < tetrisGrid.Children.Count)
            {
                UIElement element = tetrisGrid.Children[index];
                Rectangle square = (Rectangle)element;
                if (element is Rectangle)
                {
                    if (square.Name.IndexOf("moving_") == 0)
                    {
                        // Перевод состояния квадратов фигуры путем переименования.
                        string newName = square.Name.Replace("moving_", "arrived_");
                        square.Name = newName;
                    }
                }
                index++;
            }
            // Проверка завершена ли линия, сдвиг квадратов вниз.
            CheckComplete();
            Reset();
            timer.Start();
        }

    // Проверка завершена ли линия.
    private void CheckComplete() {
      int gridRow = tetrisGrid.RowDefinitions.Count;
      int gridColumn = tetrisGrid.ColumnDefinitions.Count;
      int squareCount = 0;
      for (int row = gridRow; row >= 0; row--) {
        squareCount = 0;
        for (int column = gridColumn; column >= 0; column--) {
          Rectangle square;
          square = (Rectangle)tetrisGrid.Children
         .Cast<UIElement>()
         .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);
          if (square != null) {
            if (square.Name.IndexOf("arrived") == 0) {
              squareCount++;
            }
          }
        }
        // Удаление линии, если она полная.
        if (squareCount == gridColumn) {
          DeleteLine(row);
          scoreTxt.Text = GetScore().ToString();
          CheckComplete();
        }
      }
    }

    // Удаление завершенной линии.
    private void DeleteLine(int row) {
      // Удаление линии.
      for (int i = 0; i < tetrisGrid.ColumnDefinitions.Count; i++) {
        Rectangle square;
        try {
          square = (Rectangle)tetrisGrid.Children
         .Cast<UIElement>()
         .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == i);
          tetrisGrid.Children.Remove(square);
        } catch {
        }
      }
      // Сдвиг остальных линий.
      foreach (UIElement element in tetrisGrid.Children) {
        Rectangle square = (Rectangle)element;
        if (square.Name.IndexOf("arrived") == 0 && Grid.GetRow(square) <= row) {
          Grid.SetRow(square, Grid.GetRow(square) + 1);
        }
      }
    }

    // Подсчет очков.
    private int GetScore() {
      // TODO: счетчик в зависимости от удаленных линий.
      gameScore += 1;
      return gameScore;
    }

    // Сброс переменных после падения фигуры.
    private void Reset() {
      downPos = 0;
      leftPos = 3;
      isRotated = false;
      rotation = 0;
      currentFigureNumber = nextFigureNumber;
      if (!isGameOver) {
        AddFigure(currentFigureNumber, leftPos);
      }
      isNextFigureDrawed = false;
      figureRandom = new Random();
      nextFigureNumber = figureRandom.Next(1, 8);
      isBottomCollided = false;
      isLeftCollided = false;
      isRightCollided = false;
    }


    private void clearDatabase()
    {
      string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Tetris;Integrated Security=True";
      using (SqlConnection connection = new SqlConnection(connectionString)) {
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        // insert to update
        command.CommandText = @"DELETE FROM Condition";
        command.ExecuteNonQuery();
      }
    }


    // Сброс переменных после проигрыша.
    private void GameOver() {
      isGameOver = true;
      // Очистка базы данных.
      clearDatabase();
      // Обнуление переменных.
      Reset();
      startStopBtn.Content = "НАЧАТЬ ИГРУ";
      GameOverTxt.Visibility = Visibility.Visible;
      rowCount = 0;
      columnCount = 0;
      leftPos = 0;
      isGamePaused = false;
      gameScore = 0;
      isNextFigureDrawed = false;
      currentFigure = null;
      currentFigureNumber = figureRandom.Next(1, 8);
      nextFigureNumber = figureRandom.Next(1, 8);
      timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed);
    }

    // Удаление фигуры.
    private void RemoveFigure()
        {
            int index = 0;
            while (index < tetrisGrid.Children.Count)
            {
                UIElement element = tetrisGrid.Children[index];
                Rectangle square = (Rectangle)element;
                if (element is Rectangle)
                {
                    if (square.Name.IndexOf("moving_") == 0)
                    {
                        tetrisGrid.Children.Remove(element);
                        index = -1;
                    }
                }
                index++;
            }
        }

    // Создание одного квадрата фигуры.
    private Rectangle GetBasicSquare(Color rectColor) {
      Rectangle rectangle = new Rectangle {
        // 21 px, чтобы было перекрытие границ и
        // чтобы расстояние между квадратами было в 1 px.
        Width = 21,
        Height = 21,
        StrokeThickness = 1,
        Stroke = Brushes.Black,
        Fill = Brushes.Lime
      };
      return rectangle;
    }

    // Обращение к переменной по названию в виде строки.
    private int[,] GetVariableByString(string variable) {
      return (int[,])this.GetType().GetField(variable).GetValue(this);
    }
  }
}
