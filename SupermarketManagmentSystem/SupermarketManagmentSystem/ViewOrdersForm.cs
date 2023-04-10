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
    public partial class ViewOrdersForm : Form
    {
        private SupermarketManagementSystemContext context = new SupermarketManagementSystemContext();
        public ViewOrdersForm()
        {
            InitializeComponent();
        }

        private void ReLoadData()
        {
            var orders = (from o in context.Orders
                          select o).ToList();
            foreach (var item in orders)
            {
                var entry = context.Entry(item);
                entry.Reference(o => o.Account).Load();
            }
            dataGridView1.DataSource = (from o in orders
                                        select new
                                        {
                                            o.Id,
                                            o.Account.FullName,
                                            o.CreatedDate,
                                            o.TotalPrice
                                        }).ToList();
        }
        private void LoadDataToOrderDetail(int orderid)
        {
            var orderDetail = (from o in context.OrderDetails
                               where o.OrderId == orderid
                               select new
                               {
                                   o.Id,
                                   o.ProductName,
                                   o.ProductPrice,
                                   o.Quantity,
                                   o.Total
                               });
            dataGridView2.DataSource = orderDetail.ToList();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ViewOrdersForm_Load(object sender, EventArgs e)
        {
            ReLoadData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int orderId = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                LoadDataToOrderDetail(orderId);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            CategoryForm form = new CategoryForm();
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

        }

        private void lblSeller_Click(object sender, EventArgs e)
        {
            UserManagmentForm form = new UserManagmentForm();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView2.SelectedRows[0].Cells[0].Value;
                var orderDettail = (from o in context.OrderDetails
                                    where o.Id == id
                                    select o).FirstOrDefault();
                if (orderDettail != null)
                {
                    txtID.Text = orderDettail.Id.ToString();
                    txtName.Text = orderDettail.ProductName.ToString();
                    txtPrice.Text = orderDettail.ProductPrice.ToString();
                    txtQuantity.Text = orderDettail.Quantity.ToString();
                }
            }
        }

        public bool ValidData()
        {
            bool valid = true;
            double price = 0;
            int quantity = 0;
            if (txtName.Text.Trim().Length == 0)
            {
                valid = false;
                errorProvider1.SetError(txtName, "Invalid Format!");
            }
            if (double.TryParse(txtPrice.Text, out price) == false)
            {
                valid = false;
                errorProvider2.SetError(txtPrice, "Invalid Format!");
            }
            if (price < 0)
            {
                valid = false;
                errorProvider2.SetError(txtPrice, "Invalid Format!");
            }
            if (int.TryParse(txtQuantity.Text, out quantity) == false)
            {
                valid = false;
                errorProvider2.SetError(txtPrice, "Invalid Format!");
            }
            if (quantity < 1)
            {
                valid = false;
                errorProvider2.SetError(txtPrice, "Invalid Format!");
            }
            return valid;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView2.SelectedRows[0].Cells[0].Value;
                var orderDetail = (from o in context.OrderDetails
                                   where o.Id == id
                                   select o).FirstOrDefault();
                if (orderDetail != null)
                {
                    if (ValidData())
                    {
                        orderDetail.ProductName = txtName.Text.Trim();
                        orderDetail.ProductPrice = double.Parse(txtPrice.Text.Trim());
                        orderDetail.Quantity = int.Parse(txtQuantity.Text.Trim());
                        orderDetail.Total = orderDetail.Quantity * orderDetail.ProductPrice;
                        context.SaveChanges();
                        var order = (from o in context.Orders
                                     where o.Id == orderDetail.OrderId
                                     select o).FirstOrDefault();
                        if (order != null)
                        {
                            var e1 = context.Entry(order);
                            e1.Collection(c => c.OrderDetails).Load();
                            double Total = 0;
                            foreach (var item in order.OrderDetails)
                            {
                                Total += item.Total;
                            }
                            order.TotalPrice = Total;
                            context.SaveChanges();
                        }
                        MessageBox.Show("Edit sucessfuly");
                        ReLoadData();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView2.SelectedRows[0].Cells[0].Value;
                var orderDetail = (from o in context.OrderDetails
                                   where o.Id == id
                                   select o).FirstOrDefault();
                if (orderDetail != null)
                {
                    var order = (from o in context.Orders
                                 where o.Id == orderDetail.OrderId
                                 select o).FirstOrDefault();
                    context.OrderDetails.Remove(orderDetail);
                    context.SaveChanges();
                    if (order != null)
                    {
                        var e1 = context.Entry(order);
                        e1.Collection(c => c.OrderDetails).Load();
                        double Total = 0;
                        foreach (var item in order.OrderDetails)
                        {
                            Total += item.Total;
                        }
                        order.TotalPrice = Total;
                        context.SaveChanges();
                    }
                    MessageBox.Show("Delete sucessfuly");
                    ReLoadData();

                }
            }

        }
    }
}
