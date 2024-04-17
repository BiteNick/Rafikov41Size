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

namespace Rafikov41Size
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public static List<Product> currentProducts = new List<Product>();
        public static List<OrderProduct> currentOrderProducts = new List<OrderProduct>();
        private User user;

        public ProductPage(User user)
        {
            InitializeComponent();
            Authorize(user);
            ProductPageUpdate();
        }

        private void Authorize(User user)
        {
            if (user != null)
            {
                this.user = user;
                ClientName.Text = $"{user.UserSurname} {user.UserName} {user.UserPatronymic}";
                switch (user.UserRole)
                {
                    case 1:
                        RoleName.Text = "Клиент";
                        break;
                    case 2:
                        RoleName.Text = "Менеджер";
                        break;
                    case 3:
                        RoleName.Text = "Администратор";
                        break;
                }
            }
            else
            {
                user = null;
                ClientName.Text = "гость";
                RoleName.Text = "Гость";
            }
        }

        public void ProductPageUpdate()
        {
            var currentDBList = Rafikov41Entities.GetContext().Product.ToList();
            ProductsCount.Text = currentDBList.Count.ToString();

            if (TBoxSearch.Text.Length > 0)
            {
                currentDBList = currentDBList.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            }

            if (DiscountCB.SelectedIndex == 1)
            {
                currentDBList = currentDBList.Where(p => p.ProductDiscountAmount < 10).ToList();
            }
            else if (DiscountCB.SelectedIndex == 2)
            {
                currentDBList = currentDBList.Where(p => p.ProductDiscountAmount > 10 && p.ProductDiscountAmount < 15).ToList();
            }
            else if (DiscountCB.SelectedIndex == 3)
            {
                currentDBList = currentDBList.Where(p => p.ProductDiscountAmount > 15).ToList();
            }

            if (SortRB.IsChecked == true)
            {
                currentDBList = currentDBList.OrderBy(p => p.ProductCost).ToList();
            }
            else if (SortRBDesc.IsChecked == true)
            {
                currentDBList =currentDBList.OrderByDescending(p => p.ProductCost).ToList();
            }

            ProductsCountCurrent.Text = currentDBList.Count.ToString();
            ProductPageView.ItemsSource = currentDBList;

            if (currentOrderProducts.Count > 0)
            {
                ShowOrderBtn.Visibility = Visibility.Visible;
            }
            else
            {
                ShowOrderBtn.Visibility = Visibility.Hidden;
            }
        }

        private void AddOrder(Product product)
        {
            if (ProductPageView.SelectedIndex >= 0)
            {
                int orderProductID = Rafikov41Entities.GetContext().Order.ToList().Last().OrderID+1;

                if (!currentProducts.Contains(product))
                {
                    product.Amount = 1;
                    currentProducts.Add(product);
                    var currentOrderProduct = new OrderProduct();
                    currentOrderProduct.ProductArticleNumber = product.ProductArticleNumber;
                    currentOrderProduct.OrderID = orderProductID;
                    currentOrderProduct.Amount = 1;
                    currentOrderProducts.Add(currentOrderProduct);
                }
                else
                {
                    product.Amount++;
                    foreach (var item in currentOrderProducts)
                    {
                        if (item.ProductArticleNumber == product.ProductArticleNumber)
                        {
                            item.Amount++;
                            break;
                        }
                    }
                }

                ShowOrderBtn.Visibility = Visibility.Visible;
            }
        }

       

        private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductPageUpdate();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProductPageUpdate();
        }

        private void SortRBNo_Checked(object sender, RoutedEventArgs e)
        {
            ProductPageUpdate();
        }

        private void SortRB_Checked(object sender, RoutedEventArgs e)
        {
            ProductPageUpdate();
        }

        private void SortRBDesc_Checked(object sender, RoutedEventArgs e)
        {
            ProductPageUpdate();
        }

        private void DiscountCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductPageUpdate();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddOrder(ProductPageView.SelectedItem as Product);
        }

        private void ShowOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowOrderWindow orderWindow = new ShowOrderWindow(this, ClientName.Text, user);
            orderWindow.Show();
        }
    }
}
