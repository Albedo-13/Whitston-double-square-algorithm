using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Whitson
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static char[,] LeftMat { get; set; }
        public static char[,] RightMat { get; set; }
        public static string UserMessage { get; set; }
        public static string EncryptedMessage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            EncryptButton.Click += EncryptButton_Click;
            ExitButton.Click += ExitButton_Click;
        }

        private void PrintTable(char[,] tableToEncrypt)
        {
            string result = "";
            for (int i = 0; i < tableToEncrypt.GetLength(0); i++)
            {
                for (int j = 0; j < tableToEncrypt.GetLength(1); j++)
                {
                    result += $"{tableToEncrypt[i, j]} ";
                }
                result += "\n";
            }
            MessageBox.Show(result);
        }

        private void IsFulledMenu()
        {
            if ((bool)!RussianButton.IsChecked && (bool)!EnglishButton.IsChecked)
            {
                MessageBox.Show("Пожалуйста, выберите язык.");
                return;
            }
            else if (LeftKey.Text == "" || RightKey.Text == "" || TextToEncrypt.Text == "")
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }
            else
            {
                LanguagePick();
                Output foo = new Output();
                foo.Owner = this;
                foo.Show();
            }
        }

        private bool NotIntAlphabet(char letter, char[] alphabet)
        {
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (letter == alphabet[i])
                {
                    return true;
                }
            }
            return false;
        }

        private string RemoveOtherSimbols(string text, char[] alphabet)
        {
            List<char> charList = new List<char>();
            for (int i = 0; i < text.Length; i++)
            {
                if (NotIntAlphabet(text[i], alphabet))
                {
                    charList.Add(text[i]);
                }
            }

            string result = "";
            for (int i = 0; i < charList.Count; i++)
            {
                result += charList[i];
            }
            return result;
        }

        // Если этой буквы (х) нет в таблице для шифра
        private bool CompareCheck(char x, char[,] arr)
        {
            foreach (var i in arr)
            {
                if (i == x)
                {
                    return false;
                }
            }
            return true;
        }

        // То добавляем в последнюю свободную (пустую) ячейку эту букву
        private void LetterInput(char x, ref char[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (!char.IsLetter(arr[i, j]))
                    {
                        arr[i, j] = x;
                        return;
                    }
                }
            }
        }

        // Заполнение таблицы ключом
        private void FillTableWithKey(ref char[,] tableToEncrypt, string key, char[] alphabet)
        {
            for (int i = 0; i < key.Length; i++)
            {
                if (CompareCheck(key[i], tableToEncrypt))
                {
                    LetterInput(key[i], ref tableToEncrypt);
                }
            }
        }

        // Заполнение таблицы остальными буквами алфавита
        private void FillTableWithAlphabet(ref char[,] tableToEncrypt, char[] alphabet)
        {
            foreach (var letter in alphabet)
            {
                if (CompareCheck(letter, tableToEncrypt))
                {
                    LetterInput(letter, ref tableToEncrypt);
                }
            }
        }

        static List<(char, char)> MainEncryptAlgorithm(char[,] LeftMatrix, char[,] RightMatrix, List<(char, char)> bigram)
        {
            for (int i = 0; i < bigram.Count; ++i)
            {
                ///! левая и правая таблицы
                var firstIndex = IndexOf(bigram[i].Item1, LeftMatrix);  // (i, j) пара первого элемента биграммы
                var secondIndex = IndexOf(bigram[i].Item2, RightMatrix); // (i, j) пара второго элемента биграммы

                // Разные строки и столбцы биграммной пары
                if (firstIndex.Item1 != secondIndex.Item1 && firstIndex.Item2 != secondIndex.Item2)
                {
                    if (firstIndex.Item1 > secondIndex.Item1 && firstIndex.Item2 < secondIndex.Item2)
                    {
                        // Вертикальный swap где первый элемент - ниже
                        var arg1 = RightMatrix[secondIndex.Item1, firstIndex.Item2];
                        var arg2 = LeftMatrix[firstIndex.Item1, secondIndex.Item2];
                        bigram[i] = (arg1, arg2);   
                    }

                    if (firstIndex.Item1 < secondIndex.Item1 && firstIndex.Item2 > secondIndex.Item2)
                    {
                        // Вертикальный swap где первый элемент - выше 
                        var arg1 = RightMatrix[secondIndex.Item1, firstIndex.Item2];
                        var arg2 = LeftMatrix[firstIndex.Item1, secondIndex.Item2];
                        bigram[i] = (arg1, arg2);
                    }

                    if (firstIndex.Item1 < secondIndex.Item1 && firstIndex.Item2 < secondIndex.Item2)
                    {
                        // Вертикальный swap где первый элемент слева
                        var arg1 = RightMatrix[firstIndex.Item1, secondIndex.Item2];
                        var arg2 = LeftMatrix[secondIndex.Item1, firstIndex.Item2];
                        bigram[i] = (arg1, arg2);
                    }

                    if (firstIndex.Item1 > secondIndex.Item1 && firstIndex.Item2 > secondIndex.Item2)
                    {
                        // Вертикальный swap где первый элемент справа
                        var arg1 = RightMatrix[firstIndex.Item1, secondIndex.Item2];
                        var arg2 = LeftMatrix[secondIndex.Item1, firstIndex.Item2];
                        bigram[i] = (arg1, arg2);
                    }
                }

                // Один ряд биграммной пары
                if (firstIndex.Item1 == secondIndex.Item1)
                {
                    // В условии ниже можно использовать размер любой из двух матриц, т.к. они равны
                    if (firstIndex.Item2 + 1 < LeftMatrix.GetLength(1) && secondIndex.Item2 + 1 < LeftMatrix.GetLength(1))
                    {
                        // Если не выходит за массив
                        var arg1 = RightMatrix[firstIndex.Item1, firstIndex.Item2 + 1];
                        var arg2 = LeftMatrix[secondIndex.Item1, secondIndex.Item2 + 1];
                        bigram[i] = (arg1, arg2);
                    }
                    
                    if (firstIndex.Item2 + 1 >= LeftMatrix.GetLength(1))
                    {
                        // Если первый элемент выходит за массив
                        var arg1 = RightMatrix[firstIndex.Item1, 0];
                        var arg2 = LeftMatrix[secondIndex.Item1, secondIndex.Item2 + 1];
                        bigram[i] = (arg1, arg2);
                    }

                    if (secondIndex.Item2 + 1 >= LeftMatrix.GetLength(1))
                    {
                        // Если второй элемент выходит за массив
                        var arg1 = RightMatrix[firstIndex.Item1, firstIndex.Item2 + 1];
                        var arg2 = LeftMatrix[secondIndex.Item1, 0];
                        bigram[i] = (arg1, arg2);
                    }
                }

                // Один столбец биграммной пары
                if (firstIndex.Item2 == secondIndex.Item2)
                {
                    if (firstIndex.Item1 + 1 < LeftMatrix.GetLength(0) && secondIndex.Item1 + 1 < LeftMatrix.GetLength(0))
                    {
                        // Если не выходит за массив
                        var arg1 = RightMatrix[firstIndex.Item1 + 1, firstIndex.Item2];
                        var arg2 = LeftMatrix[secondIndex.Item1 + 1, secondIndex.Item2];
                        bigram[i] = (arg1, arg2);
                    }

                    if (firstIndex.Item1 + 1 >= LeftMatrix.GetLength(0))
                    {
                        // Если первый элемент выходит за массив
                        var arg1 = RightMatrix[0, firstIndex.Item2];
                        var arg2 = LeftMatrix[secondIndex.Item1 + 1, firstIndex.Item2];
                        bigram[i] = (arg1, arg2);
                    }

                    if (secondIndex.Item1 + 1 >= LeftMatrix.GetLength(0))
                    {
                        // Если второй элемент выходит за массив
                        var arg1 = RightMatrix[firstIndex.Item1 + 1, secondIndex.Item2];
                        var arg2 = LeftMatrix[0, secondIndex.Item2];
                        bigram[i] = (arg1, arg2);
                    }
                }
            }

            return bigram;
        }

        static (int, int) IndexOf(char x, char[,] tableToEncrypt)
        {
            int indexI = 0;
            int indexJ = 0;
            for (int i = 0; i < tableToEncrypt.GetLength(0); i++)
            {
                for (int j = 0; j < tableToEncrypt.GetLength(1); j++)
                {
                    if (tableToEncrypt[i, j] == x)
                    {
                        indexI = i;
                        indexJ = j;
                    }
                    // Исключения
                    if (x == 'j')
                        return IndexOf('i', tableToEncrypt);
                    if (x == 'ё')
                        return IndexOf('е', tableToEncrypt);
                }
            }
            return (indexI, indexJ);
        }

        private string BigramView(List<(char, char)> bigram)
        {
            string result = "";
            foreach (var i in bigram)
            {
                result += $"({i.Item1},{i.Item2}) ";
            }
            return result;
        }

        private void ForAllLanguages(char[] alphabet, ref char[,] LeftMatrix, ref char[,] RightMatrix)
        {
            // Обработали ключи: перевели в нижний регистр и убрали лишние символы (не из алфавита)
            string leftK = LeftKey.Text;
            leftK = leftK.ToLower();
            leftK = RemoveOtherSimbols(leftK, alphabet);

            string rightK = RightKey.Text;
            rightK = rightK.ToLower();
            rightK = RemoveOtherSimbols(rightK, alphabet);

            // Обработали текст для шифрования
            string textToEncrypt = TextToEncrypt.Text;
            textToEncrypt = textToEncrypt.ToLower();
            textToEncrypt = RemoveOtherSimbols(textToEncrypt, alphabet);

            // Заполнение левой и правой таблицы ключом
            FillTableWithKey(ref LeftMatrix, leftK, alphabet);
            FillTableWithKey(ref RightMatrix, rightK, alphabet);

            // Дозаполнение таблиц остальной частью алфавита
            FillTableWithAlphabet(ref LeftMatrix, alphabet);
            FillTableWithAlphabet(ref RightMatrix, alphabet);

            //PrintTable(LeftMatrix);
            //PrintTable(RightMatrix);

            // Биграмма требует четного кол-ва символов
            if (textToEncrypt.Length % 2 == 1)
            {
                if ((bool)EnglishButton.IsChecked)
                {
                    textToEncrypt += alphabet[alphabet.Length - 4];
                }
                if ((bool)RussianButton.IsChecked)
                {
                    textToEncrypt += alphabet[alphabet.Length - 2];
                }
            }

            // Биграмма
            List<(char, char)> bigram = new List<(char, char)>();
            for (int i = 0; i < textToEncrypt.Length; i += 2)
            {
                bigram.Add((textToEncrypt[i], textToEncrypt[i + 1]));
            }
            
            MainWindow.UserMessage = BigramView(bigram);

            MainEncryptAlgorithm(LeftMatrix, RightMatrix, bigram);

            MainWindow.EncryptedMessage = BigramView(bigram);
        }

        private void LanguagePick()
        {
            char[,] LeftMatrix;
            char[,] RightMatrix;
            char[] RussianAlphabet = new char[] { 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п',
                                                          'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', 'ё'};
            char[] EnglishAlphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'k', 'l', 'm', 'n', 'o', 'p',
                                                          'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'j'};

            if ((bool)RussianButton.IsChecked)
            {
                LeftMatrix = new char[8, 4];
                RightMatrix = new char[8, 4];
                ForAllLanguages(RussianAlphabet, ref LeftMatrix, ref RightMatrix);

                MainWindow.LeftMat = LeftMatrix;
                MainWindow.RightMat = RightMatrix;
            }

            if ((bool)EnglishButton.IsChecked)
            {
                LeftMatrix = new char[5, 5];
                RightMatrix = new char[5, 5];
                ForAllLanguages(EnglishAlphabet, ref LeftMatrix, ref RightMatrix);
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            IsFulledMenu();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
