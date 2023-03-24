using Microsoft.EntityFrameworkCore;
using SupermarketManagmentSystem.Models;

namespace SupermarketManagmentSystem
{
    public partial class Form1 : Form
    {
        private SupermarketManagementSystemContext context = new SupermarketManagementSystemContext();
        public Form1()
        {
            InitializeComponent();
        }

        private void ReloadData()
        {
            List<Product> products = (from p in context.Products
                                      select p).ToList();


            List<String> categories = (from c in context.Categories
                                       select c.Name).ToList();

            foreach (var item in products)
            {
                var e = context.Entry(item);
                e.Reference(p => p.Category).Load();
            }

            dataGridView1.DataSource = (from p in products
                                        select new
                                        {
                                            p.Id,
                                            p.Name,
                                            p.Quantity,
                                            p.Price,
                                            Category = p.Category.Name
                                        }).ToList();
            cbbCategory.DataSource = (categories).ToList();
            categories.Insert(0, "All");
            cbbFilterCategory.DataSource = (categories).ToList();
            int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            var product = (from p in context.Products
                           where p.Id == id
                           select p).FirstOrDefault();
            if (product != null)
            {
                var e1 = context.Entry(product);
                e1.Reference(p => p.Category).Load();
                txtID.Text = product.Id.ToString();
                txtName.Text = product.Name.ToString();
                txtPrice.Text = product.Price.ToString();
                txtQuantity.Text = product.Quantity.ToString();
                cbbCategory.SelectedIndex = cbbCategory.FindStringExact(product.Category.Name);
                cbbCategory.SelectedIndex = cbbCategory.Items.IndexOf(product.Category.Name);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var product = (from p in context.Products
                               where p.Id == id
                               select p).FirstOrDefault();
                if (product != null)
                {
                    var e1 = context.Entry(product);
                    e1.Reference(p => p.Category).Load();
                    txtID.Text = product.Id.ToString();
                    txtName.Text = product.Name.ToString();
                    txtPrice.Text = product.Price.ToString();
                    txtQuantity.Text = product.Quantity.ToString();
                    cbbCategory.SelectedIndex = cbbCategory.FindStringExact(product.Category.Name);
                    cbbCategory.SelectedIndex = cbbCategory.Items.IndexOf(product.Category.Name);
                }
            }
        }

        private bool
        private void btnAdd_Click(object sender, EventArgs e)
        {

            Product p = new Product()
            {
                Name = txtName.Text,
                CategoryId = cbbCategory.SelectedIndex + 1,
                Price = double.Parse(txtPrice.Text),
                Quantity = int.Parse(txtQuantity.Text),
            };
            context.Products.Add(p);
            context.SaveChanges();
            ReloadData();
        }
    }
}