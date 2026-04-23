using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using APITool.Models;
using APITool.Services;

namespace APITool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Получаем URL
            string url = UrlTextBox.Text;

            // 2. Получаем выбранный метод (GET / POST)
            ComboBoxItem selectedItem = (ComboBoxItem)MethodComboBox.SelectedItem;
            string selectedMethod = selectedItem.Content.ToString();

            HttpMethodType method = HttpMethodType.GET;

            if (selectedMethod == "POST")
            {
                method = HttpMethodType.POST;
            }

            // 3. Получаем body
            string body = RequestBodyTextBox.Text;

            // 4. Валидация для POST
            if (method == HttpMethodType.POST)
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    StatusTextBox.Text = "Error";
                    ResponseTextBox.Text = "Request body is empty.";
                    return;
                }

                if (!IsValidJson(body))
                {
                    StatusTextBox.Text = "Error";
                    ResponseTextBox.Text = "Request body is not valid JSON.";
                    return;
                }
            }

            // 5. UI: показываем, что отправка началась
            StatusTextBox.Text = "Sending...";
            ResponseTextBox.Text = "";

            // 6. Создаём request
            var request = new ApiRequest
            {
                Url = url,
                Method = method,
                Body = body
            };

            // 7. Отправляем через сервис
            var apiService = new ApiService();
            ApiResponse response = await apiService.SendAsync(request);

            // 8. Обработка ошибки
            if (!response.IsSuccess)
            {
                StatusTextBox.Text = "Error";
                ResponseTextBox.Text = response.ErrorMessage;
                return;
            }

            // 9. Успешный ответ
            StatusTextBox.Text = response.StatusCode.ToString();
            ResponseTextBox.Text = FormatJson(response.Body);
        }
        private void MethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MethodComboBox.SelectedItem == null || RequestBodyTextBox == null)
                return;

            ComboBoxItem selectedItem = (ComboBoxItem)MethodComboBox.SelectedItem;
            string selectedMethod = selectedItem.Content.ToString();

            if (selectedMethod == "GET")
            {
                RequestBodyTextBox.Text = "";
                RequestBodyTextBox.IsEnabled = false;
            }
            else
            {
                RequestBodyTextBox.IsEnabled = true;
            }
        }
        private bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                JToken.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private string FormatJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            try
            {
                var parsedJson = JToken.Parse(json);
                return parsedJson.ToString(Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }
    }
}
