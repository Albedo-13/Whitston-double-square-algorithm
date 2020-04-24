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
using System.Windows.Shapes;

namespace Whitson
{
    /// <summary>
    /// Логика взаимодействия для Output.xaml
    /// </summary>
    public partial class Output : Window
    {
        public Output()
        {
            InitializeComponent();
            ReturnButton.Click += ReturnButton_Click;

            LeftMatrix.Text = CharArrToString(MainWindow.LeftMat);
            LeftMatrix.IsReadOnly = true;

            RightMatrix.Text = CharArrToString(MainWindow.RightMat);
            RightMatrix.IsReadOnly = true;

            Message.Text = MainWindow.UserMessage;
            Message.IsReadOnly = true;

            Cipher.Text = MainWindow.EncryptedMessage;
            Message.IsReadOnly = true;
        }

        private string CharArrToString(char[,] arr)
        {
            char[,] charArr = arr;
            string result = "";
            for (int i = 0; i < charArr.GetLength(0); i++)
            {
                for (int j = 0; j < charArr.GetLength(1); j++)
                {
                    result += $"{charArr[i,j]} ";
                }
                result += "\n";
            }
            return result;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
