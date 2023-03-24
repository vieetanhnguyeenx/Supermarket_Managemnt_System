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

        private bool ValidData()
        {
            bool check = true;
            int quantity = 0;
            double price = 0;
            if (txtName.Text.Trim().Length == 0)
            {
                check = false;
                errorProvider3.SetError(txtQuantity, "Invalid Format!");
            }
            if (int.TryParse(txtQuantity.Text, out quantity) == false)
            {
                check = false;
                errorProvider1.SetError(txtQuantity, "Invalid Format!");
            }
            if (double.TryParse(txtPrice.Text, out price) == false)
            {
                check = false;
                errorProvider2.SetError(txtPrice, "Invalid Format!");
            }
            return check;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            if (ValidData())
            {
                Product p = new Product()
                {
                    Name = txtName.Text.Trim(),
                    CategoryId = cbbCategory.SelectedIndex + 1,
                    Price = double.Parse(txtPrice.Text),
                    Quantity = int.Parse(txtQuantity.Text),
                };
                context.Products.Add(p);
                context.SaveChanges();
                MessageBox.Show("Add Successfully", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadData();
            }


        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtID.Text = "";
            txtName.Text = "";
            txtPrice.Text = "";
            txtQuantity.Text = "";
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            errorProvider2.Clear();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (ValidData())
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                    var product = (from p in context.Products
                                   where p.Id == id
                                   select p).FirstOrDefault();
                    if (product != null)
                    {
                        product.Name = txtName.Text;
                        product.Price = double.Parse(txtPrice.Text);
                        product.Quantity = int.Parse(txtQuantity.Text);
                        product.CategoryId = cbbCategory.SelectedIndex + 1;
                        context.SaveChanges();
                        MessageBox.Show("Edit Successfully", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ReloadData();
                    }
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var product = (from p in context.Products
                               where p.Id == id
                               select p).FirstOrDefault();
                if (product != null)
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                    MessageBox.Show("Delete Successfully", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReloadData();
                }
            }
        }

        private void cbbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbFilterCategory.SelectedIndex == -1)
            {
                return;
            }
            if (cbbFilterCategory.SelectedIndex == 0)
            {
                ReloadData();
            }
            else
            {
                int id = int.Parse(cbbFilterCategory?.SelectedIndex.ToString());
                Category cate = (from c in context.Categories
                                 where c.Id == id
                                 select c).FirstOrDefault();
                if (cate != null)
                {
                    var e1 = context.Entry(cate);
                    e1.Collection(c => c.Products).Load();
                    List<Product> products = cate.Products.ToList();

                    foreach (var item in products)
                    {
                        var e2 = context.Entry(item);
                        e2.Reference(p => p.Category).Load();
                    }
                    dataGridView1.DataBindings.Clear();
                    dataGridView1.DataSource = (from p in products
                                                select new
                                                {
                                                    p.Id,
                                                    p.Name,
                                                    p.Quantity,
                                                    p.Price,
                                                    Category = p.Category.Name
                                                }).ToList();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cbbFilterCategory.SelectedIndex = 0;
            ReloadData();
            Form1 form = new Form1();
            form.ShowDialog();
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            errorProvider3.Clear();
        }
    }
}