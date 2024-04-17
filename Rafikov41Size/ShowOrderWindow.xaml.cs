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

namespace Rafikov41Size
{
    /// <summary>
    /// Логика взаимодействия для ShowOrderWindow.xaml
    /// </summary>
    public partial class ShowOrderWindow : Window
    {

        private ProductPage productPage;
        private List<PickupPoint> pickupPoints;
        private User user;
        public ShowOrderWindow(ProductPage productPage, string clientName, User user)
        {
            InitializeComponent();
            this.productPage = productPage;
            this.user = user;
            ClientNameText.Text = clientName;
            OrderNumberID.Text = ProductPage.currentOrderProducts.First().OrderID.ToString();

            OrdersListView.ItemsSource = ProductPage.currentProducts;
            pickupPoints = Rafikov41Entities.GetContext().PickupPoint.ToList();
            foreach (var pickupPoint in pickupPoints)
            {
                orderPlaceCB.Items.Add($"{pickupPoint.City} {pickupPoint.Street} {pickupPoint.House}, Артикул: {pickupPoint.ArticleNumber}");
            }
            orderPlaceCB.SelectedIndex = 0;

            UpdateWindow();
        }

        private void UpdateWindow()
        {
            SetDeliveryDate();
            saveUpdate();
            OrdersListView.Items.Refresh();
        }

        private void SubAmountBtn_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;

            int index = ProductPage.currentOrderProducts.IndexOf(ProductPage.currentOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == product.ProductArticleNumber));

            
            ProductPage.currentOrderProducts[index].Amount--;
            ProductPage.currentProducts[index].Amount--;

            if (ProductPage.currentProducts[index].Amount == 0)
            {
                ProductPage.currentOrderProducts.RemoveAt(index);
                ProductPage.currentProducts.RemoveAt(index);
            }


            productPage.ProductPageUpdate();
            UpdateWindow();
        }

        private void AddAmountBtn_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;

            int index = ProductPage.currentOrderProducts.IndexOf(ProductPage.currentOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == product.ProductArticleNumber));

                ProductPage.currentOrderProducts[index].Amount++;
                ProductPage.currentProducts[index].Amount++;

            UpdateWindow();

        }

        private void SetDeliveryDate()
        {
            bool isAvailable = true;
            foreach (var product in ProductPage.currentProducts)
            {
                if (product.Amount > product.ProductQuantityInStock || product.ProductQuantityInStock < 3)
                {
                    isAvailable = false;
                }
            }

            if (isAvailable)
                PrintOrderReady.DisplayDate = DateTime.Today.AddDays(3);
            else
                PrintOrderReady.DisplayDate = DateTime.Today.AddDays(6);
            PrintOrderReady.Text = PrintOrderReady.DisplayDate.ToString();


            ChooseOrderDate.DisplayDateStart = PrintOrderReady.DisplayDate;
            ChooseOrderDate.DisplayDateEnd = PrintOrderReady.DisplayDate.AddDays(14);

            if (ChooseOrderDate.DisplayDate <= PrintOrderReady.DisplayDate)
            {
                ChooseOrderDate.DisplayDate = PrintOrderReady.DisplayDate;
                ChooseOrderDate.Text = ChooseOrderDate.DisplayDate.ToString();
            }
        }

        private void saveUpdate()
        {
            bool saveEnable = true;
            if (OrdersListView.Items.Count > 0)
            {
                if (ChooseOrderDate != null)
                {
                    foreach (Product item in OrdersListView.Items)
                    {
                        if (item.Amount == 0)
                        {
                            saveEnable = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                saveEnable = false;
            }

            if (saveEnable)
                SaveOrder.Visibility = Visibility.Visible;
            else
                SaveOrder.Visibility = Visibility.Hidden;

        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            List<Order> orders = Rafikov41Entities.GetContext().Order.ToList();
            Order order = new Order();
            order.OrderStatus = "Новый";
            order.OrderDeliveryDate = ChooseOrderDate.DisplayDate;
            order.PickupPoint = pickupPoints.First(p => p.ArticleNumber == orderPlaceCB.SelectedItem.ToString().Split().Last());
            order.OrderDate = DateTime.Now;
            if (user != null)
                order.ClientID = user.UserID;
            else
                order.ClientID = null;
            order.OrderReceiveCode = (Convert.ToInt32(orders.Last().OrderReceiveCode) + 1).ToString();


            Rafikov41Entities.GetContext().Order.Add(order);

            Rafikov41Entities.GetContext().OrderProduct.AddRange(ProductPage.currentOrderProducts);

            Rafikov41Entities.GetContext().SaveChanges();

            MessageBox.Show("Запись добавлена!");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedIndex >= 0)
            {
               
            }
            ProductPage.currentOrderProducts.RemoveAt(OrdersListView.SelectedIndex);
            ProductPage.currentProducts.RemoveAt(OrdersListView.SelectedIndex);
            productPage.ProductPageUpdate();
            UpdateWindow();
        }

    }
}
