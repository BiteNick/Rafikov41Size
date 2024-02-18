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
        public ProductPage()
        {
            InitializeComponent();
            ProductPageUpdate();
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
    }
}
