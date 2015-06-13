using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BarleyBreak
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Глубина поиска
        /// </summary>
        private int _deepness;

        private int _minPrevIter;

        /// <summary>
        /// Поле игры. Хранится цифра-кнопка, которая сейчас находится на i-той позиции поля
        /// </summary>
        private List<byte> _map;

        /// <summary>
        /// Решение задачи методом IDA*
        /// Массив ходов-передвижений пустой клетки (0-вверх, 1-вправо, 2-вниз, 3-влево)
        /// </summary>
        private List<byte> _solution;

        /// <summary>
        /// Пустая позиция
        /// </summary>
        private byte _zeroField;

        /// <summary>
        /// Количество ходов в текущей игре
        /// </summary>
        private int _turnsCount;

        /// <summary>
        /// Проверка завершения игры, раскраска формы
        /// </summary>
        private void CheckGoal()
        {
            var goal = true;
            for (var i = 0; i < 15; i++)
                if (_map[i] != i + 1)
                {
                    goal = false;
                    break;
                }

            BackColor = goal ? Color.LawnGreen : SystemColors.Control;
        }

        private Control _buttonToMove;
        private int _buttonMoveStep;
        private Point _startLoc;
        private Point _finishLoc;

        /// <summary>
        /// Игровой ход
        /// </summary>
        /// <param name="buttonNumber">Цифра на пятнашке</param>
        /// <param name="sender"> Объект - кнопка для перемещения</param>
        private void MakeTurn(byte buttonNumber, Control sender)
        {
            // Неверный клик, подвинуть нельзя
            var buttonPosition = (byte)_map.IndexOf(buttonNumber);
            if (buttonPosition != _zeroField - 1 && buttonPosition != _zeroField + 1 &&
                    buttonPosition != _zeroField - 4 && buttonPosition != _zeroField + 4) return;

            // Свапаем с пустой позицией
            _map[_zeroField] = buttonNumber;
            _zeroField = buttonPosition;
            _map[buttonPosition] = 0;

            // Анимация движения кнопки
            _startLoc = sender.Location;
            _finishLoc = button0.Location;
            _buttonToMove = sender;
            _buttonMoveStep = 1;
            timer1.Start();
            button0.Location = _startLoc;

            // Счетчик ходов
            _turnsCount++;
            toolStripStatusLabelTurnsCount.Text = @"Ходов: " + _turnsCount;

            CheckGoal();
        }

        /// <summary>
        /// Начало новой игры
        /// </summary>
        private void StartNewGame()
        {
            _map = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 };
            toolStripStatusLabelTurnsCount.Text = @"Ходов: 0";
            _zeroField = 15;
            _turnsCount = 0;

            int evenCount;
            do
            {
                // Перемешивание цифр
                var rng = new Random();
                var n = _map.Count - 1;
                while (n > 3)
                {
                    n--;
                    var k = rng.Next(n + 1);
                    var value = _map[k];
                    _map[k] = _map[n];
                    _map[n] = value;
                }

                // Проверка решаемости
                evenCount = 0;
                for (var i = 0; i < 14; i++)
                    for (var j = i + 1; j < 15; j++)
                        if (_map[i] > _map[j]) evenCount++;
            } while (evenCount % 2 != 0);


            // Расстановка кнопок
            button0.Location = new Point(12 + _map.IndexOf(0) % 4 * 81, 27 + _map.IndexOf(0) / 4 * 81);
            button1.Location = new Point(12 + _map.IndexOf(1) % 4 * 81, 27 + _map.IndexOf(1) / 4 * 81);
            button2.Location = new Point(12 + _map.IndexOf(2) % 4 * 81, 27 + _map.IndexOf(2) / 4 * 81);
            button3.Location = new Point(12 + _map.IndexOf(3) % 4 * 81, 27 + _map.IndexOf(3) / 4 * 81);
            button4.Location = new Point(12 + _map.IndexOf(4) % 4 * 81, 27 + _map.IndexOf(4) / 4 * 81);
            button5.Location = new Point(12 + _map.IndexOf(5) % 4 * 81, 27 + _map.IndexOf(5) / 4 * 81);
            button6.Location = new Point(12 + _map.IndexOf(6) % 4 * 81, 27 + _map.IndexOf(6) / 4 * 81);
            button7.Location = new Point(12 + _map.IndexOf(7) % 4 * 81, 27 + _map.IndexOf(7) / 4 * 81);
            button8.Location = new Point(12 + _map.IndexOf(8) % 4 * 81, 27 + _map.IndexOf(8) / 4 * 81);
            button9.Location = new Point(12 + _map.IndexOf(9) % 4 * 81, 27 + _map.IndexOf(9) / 4 * 81);
            button10.Location = new Point(12 + _map.IndexOf(10) % 4 * 81, 27 + _map.IndexOf(10) / 4 * 81);
            button11.Location = new Point(12 + _map.IndexOf(11) % 4 * 81, 27 + _map.IndexOf(11) / 4 * 81);
            button12.Location = new Point(12 + _map.IndexOf(12) % 4 * 81, 27 + _map.IndexOf(12) / 4 * 81);
            button13.Location = new Point(12 + _map.IndexOf(13) % 4 * 81, 27 + _map.IndexOf(13) / 4 * 81);
            button14.Location = new Point(12 + _map.IndexOf(14) % 4 * 81, 27 + _map.IndexOf(14) / 4 * 81);
            button15.Location = new Point(12 + _map.IndexOf(15) % 4 * 81, 27 + _map.IndexOf(15) / 4 * 81);

            CheckGoal();
        }

        /// <summary>
        /// Эвристическая функция для анализа количества оставшихся ходов (Манхеттеновское расстояние)
        /// </summary>
        /// <returns></returns>
        private byte EstimateTurns()
        {
            var result = 0;
            for (var i = 0; i < 16; i++)
            {
                var goalPosition = (i == 0) ? 15 : i - 1;
                var currPosition = _map.IndexOf((byte)i);
                var goalDist = Math.Abs(currPosition % 4 - goalPosition % 4) + Math.Abs(currPosition / 4 - goalPosition / 4);
                result += goalDist;
            }
            return (byte)result;
        }

        /// <summary>
        /// Поиск в глубину с обрезанием
        /// </summary>
        /// <param name="g">Сделано шагов</param>
        /// <param name="prevMove">Предыдущий ход (антизацикливание)</param>
        /// <param name="x0">Текущая позиция X пустого поля</param>
        /// <param name="y0">Текущая позиция Y пустого поля</param>
        /// <returns>true - решение найдено, false - в противном случае</returns>
        private bool RecSearch(int g, int prevMove, int x0, int y0)
        {
            var h = EstimateTurns();
            if (h == 0) return true;

            var f = g + h;
            if (f > _deepness)
            {
                if (_minPrevIter > f) _minPrevIter = f;
                return false;
            }

            // Смещения пустой клетки при всех четырех ходах
            var dx = new List<int> { 0, -1, 0, 1 };
            var dy = new List<int> { 1, 0, -1, 0 };
            // Противоположные ходы
            var oppMove = new List<int> { 2, 3, 0, 1 };

            for (var i = 0; i < 4; i++)
            {
                if (oppMove[i] == prevMove) continue;

                var newX = x0 + dx[i];
                var newY = y0 + dy[i];

                if (!(0 <= newX && newX <= 3 && 0 <= newY && newY <= 3)) continue;

                SwapFieldsOnTempMap(y0 * 4 + x0, newY * 4 + newX);
                var result = RecSearch(g + 1, i, newX, newY);
                SwapFieldsOnTempMap(y0 * 4 + x0, newY * 4 + newX);

                if (!result) continue;
                _solution.Insert(0, (byte)i);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Вспомогательная процедура, описание тривиально
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        private void SwapFieldsOnTempMap(int x1, int x2)
        {
            var temp = _map[x1];
            _map[x1] = _map[x2];
            _map[x2] = temp;
        }

        /// <summary>
        /// Итерация поиска в глубину и IDA*
        /// </summary>
        /// <returns>true - решение найдено, false - в противном случае</returns>
        private bool IdaStar()
        {
            var result = false;
            _solution = new List<byte>();

            _deepness = EstimateTurns();
            while (_deepness <= 60)
            {
                _minPrevIter = int.MaxValue;
                var x0 = _map.IndexOf(0) % 4;
                var y0 = _map.IndexOf(0) / 4;
                result = RecSearch(0, -1, x0, y0);
                _deepness = _minPrevIter;
                if (result) break;
            }
            return result;
        }

        public Form1()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakeTurn(1, (Control)sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MakeTurn(2, (Control)sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MakeTurn(3, (Control)sender);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MakeTurn(4, (Control)sender);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MakeTurn(5, (Control)sender);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MakeTurn(6, (Control)sender);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MakeTurn(7, (Control)sender);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MakeTurn(8, (Control)sender);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MakeTurn(9, (Control)sender);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MakeTurn(10, (Control)sender);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MakeTurn(11, (Control)sender);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MakeTurn(12, (Control)sender);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MakeTurn(13, (Control)sender);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            MakeTurn(14, (Control)sender);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            MakeTurn(15, (Control)sender);
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Автоматический поиск решения задачи
        /// Работает достатчно быстро при количестве ходов 40-50 (0-2 минуты)
        /// Значительно замедляется при количестве необходимых ходов > 50 (5 минут и больше)
        /// Рекомендуется вначале выставлять фишки 1-4 вручную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void автоиграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = @"Выполняется...";
            Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Программное нажатие клавиши, для воспроизведения решения, найденного алгоритмом
        /// </summary>
        /// <param name="n">Номер кнопки</param>
        private void PerformClick(int n)
        {
            switch (n)
            {
                case 1:
                    button1.PerformClick();
                    break;
                case 2:
                    button2.PerformClick();
                    break;
                case 3:
                    button3.PerformClick();
                    break;
                case 4:
                    button4.PerformClick();
                    break;
                case 5:
                    button5.PerformClick();
                    break;
                case 6:
                    button6.PerformClick();
                    break;
                case 7:
                    button7.PerformClick();
                    break;
                case 8:
                    button8.PerformClick();
                    break;
                case 9:
                    button9.PerformClick();
                    break;
                case 10:
                    button10.PerformClick();
                    break;
                case 11:
                    button11.PerformClick();
                    break;
                case 12:
                    button12.PerformClick();
                    break;
                case 13:
                    button13.PerformClick();
                    break;
                case 14:
                    button14.PerformClick();
                    break;
                case 15:
                    button15.PerformClick();
                    break;
            }
        }

        /// <summary>
        /// Расставляет фишки в специальном порядке, гарантированно решаемом достаточно быстро
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void эмуляторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _map = new List<byte> { 2, 5, 4, 6, 1, 13, 11, 3, 14, 12, 7, 10, 8, 15, 9, 0 };

            // Расстановка кнопок
            button0.Location = new Point(12 + _map.IndexOf(0) % 4 * 81, 27 + _map.IndexOf(0) / 4 * 81);
            button1.Location = new Point(12 + _map.IndexOf(1) % 4 * 81, 27 + _map.IndexOf(1) / 4 * 81);
            button2.Location = new Point(12 + _map.IndexOf(2) % 4 * 81, 27 + _map.IndexOf(2) / 4 * 81);
            button3.Location = new Point(12 + _map.IndexOf(3) % 4 * 81, 27 + _map.IndexOf(3) / 4 * 81);
            button4.Location = new Point(12 + _map.IndexOf(4) % 4 * 81, 27 + _map.IndexOf(4) / 4 * 81);
            button5.Location = new Point(12 + _map.IndexOf(5) % 4 * 81, 27 + _map.IndexOf(5) / 4 * 81);
            button6.Location = new Point(12 + _map.IndexOf(6) % 4 * 81, 27 + _map.IndexOf(6) / 4 * 81);
            button7.Location = new Point(12 + _map.IndexOf(7) % 4 * 81, 27 + _map.IndexOf(7) / 4 * 81);
            button8.Location = new Point(12 + _map.IndexOf(8) % 4 * 81, 27 + _map.IndexOf(8) / 4 * 81);
            button9.Location = new Point(12 + _map.IndexOf(9) % 4 * 81, 27 + _map.IndexOf(9) / 4 * 81);
            button10.Location = new Point(12 + _map.IndexOf(10) % 4 * 81, 27 + _map.IndexOf(10) / 4 * 81);
            button11.Location = new Point(12 + _map.IndexOf(11) % 4 * 81, 27 + _map.IndexOf(11) / 4 * 81);
            button12.Location = new Point(12 + _map.IndexOf(12) % 4 * 81, 27 + _map.IndexOf(12) / 4 * 81);
            button13.Location = new Point(12 + _map.IndexOf(13) % 4 * 81, 27 + _map.IndexOf(13) / 4 * 81);
            button14.Location = new Point(12 + _map.IndexOf(14) % 4 * 81, 27 + _map.IndexOf(14) / 4 * 81);
            button15.Location = new Point(12 + _map.IndexOf(15) % 4 * 81, 27 + _map.IndexOf(15) / 4 * 81);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_buttonMoveStep < 6)
            {
                _buttonToMove.Location = new Point(_startLoc.X + (_finishLoc.X - _startLoc.X) / 5 * _buttonMoveStep,
                    _startLoc.Y + (_finishLoc.Y - _startLoc.Y) / 5 * _buttonMoveStep);
                _buttonMoveStep++;
            }
            else
            {
                _buttonToMove.Location = _finishLoc;
                timer1.Stop();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = @"";
            Enabled = true;
            timer2.Start();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            IdaStar();
        }

        /// <summary>
        /// Воспроизведение ходов, построенных алгоритмом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            var turn = _solution[0];
            _solution.RemoveAt(0);

            switch (turn)
            {
                case 0:
                    PerformClick(_map[_zeroField + 4]);
                    break;
                case 1:
                    PerformClick(_map[_zeroField - 1]);
                    break;
                case 2:
                    PerformClick(_map[_zeroField - 4]);
                    break;
                case 3:
                    PerformClick(_map[_zeroField + 1]);
                    break;
            }

            if (_solution.Count == 0) timer2.Stop();
        }
    }
}
