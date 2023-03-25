using SupermarketManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupermarketManagmentSystem
{

    public partial class CategoryForm : Form
    {
        private SupermarketManagementSystemContext context = new SupermarketManagementSystemContext();
        public CategoryForm()
        {
            InitializeComponent();
        }

        private void ReloadData()
        {

            List<Category> categories = (from c in context.Categories
                                         select c).ToList();

            dataGridView1.DataSource = (from c in categories
                                        select new
                                        {
                                            c.Id,
                                            c.Name,
                                            c.Description
                                        }).ToList();
        }

        private void lblTile_Click(object sender, EventArgs e)
        {

        }

        private void CategoryForm_Load(object sender, EventArgs e)
        {
            ReloadData();
        }

        private bool ValidData()
        {
            bool check = true;
            if (txtName.Text.Trim().Length == 0)
            {
                check = false;
                errorProvider1.SetError(txtName, "Invalid Format!");
            }
            if (txtDesc.Text.Trim().Length == 0)
            {
                check = false;
                errorProvider2.SetError(txtDesc, "Invalid Format!");
            }

            return check;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var category = (from c in context.Categories
                                where c.Id == id
                                select c).FirstOrDefault();
                if (category != null)
                {
                    txtID.Text = category.Id.ToString();
                    txtName.Text = category.Name.ToString();
                    txtDesc.Text = category?.Description?.ToString();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            if (ValidData())
            {
                Category c = new Category()
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDesc.Text.Trim(),
                };
                context.Categories.Add(c);
                context.SaveChanges();
                MessageBox.Show("Add Successfully", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadData();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (ValidData())
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                    var category = (from c in context.Categories
                                    where c.Id == id
                                    select c).FirstOrDefault();
                    if (category != null)
                    {
                        category.Name = txtName.Text.Trim();
                        category.Description = txtDesc.Text.Trim();
                        context.SaveChanges();
                        MessageBox.Show("Edit Successfully", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ReloadData();
                    }

                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtID.Text = string.Empty;
            txtName.Text = string.Empty;
            txtDesc.Text = string.Empty;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var category = (from c in context.Categories
                                where c.Id == id
                                select c).FirstOrDefault();
                if (category != null)
                {
                    var e1 = context.Entry(category);
                    e1.Collection(c => c.Products).Load();
                    foreach (var item in category.Products)
                    {
                        var e12 = context.Entry(item);
                        e12.Collection(p => p.OrderDetails).Load();
                        context.OrderDetails.RemoveRange(item.OrderDetails);
                    }
                    context.Products.RemoveRange(category.Products);
                    context.SaveChanges();
                    context.Categories.Remove(category);
                    context.SaveChanges();
                    MessageBox.Show("Delete Successfully", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReloadData();
                }
            }
        }

        private void lblSeller_Click(object sender, EventArgs e)
        {
            UserManagmentForm form = new UserManagmentForm();
            this.Hide();
            form.ShowDialog();
            this.Close();

        }

        private void label8_Click(object sender, EventArgs e)
        {
            SallerManagmentForm form = new SallerManagmentForm();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            ViewOrdersForm form = new ViewOrdersForm();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}
