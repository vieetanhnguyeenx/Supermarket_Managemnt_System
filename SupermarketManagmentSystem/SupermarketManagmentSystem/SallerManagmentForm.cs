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
    public partial class SallerManagmentForm : Form
    {
        private SupermarketManagementSystemContext context = new SupermarketManagementSystemContext();
        private List<OrderDetail> orderDetails = new List<OrderDetail>();
        public SallerManagmentForm()
        {
            InitializeComponent();
        }

        private void SallerManagmentForm_Load(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            var products = (from p in context.Products
                            select new
                            {
                                p.Id,
                                p.Name,
                                p.Price,
                                p.Quantity
                            }).ToList();
            dtgProducts.DataSource = products.ToList();
            var orderDetailList = (from o in orderDetails
                                   select new
                                   {
                                       o.ProductName,
                                       o.ProductPrice,
                                       o.ProductId,
                                       o.Quantity,
                                       o.Total
                                   });
            dtgOrderDetail.DataSource = orderDetailList.ToList();
            for (int i = 0; i < dtgOrderDetail.RowCount; i++)
            {
                dtgOrderDetail[i, 0].Value = i + 1;
            }

            double total = 0;
            foreach (var item in orderDetails)
            {
                total += (double)item.Total;
            }
            lblTotal.Text = total.ToString();

        }

        private bool ValidData()
        {
            bool check = true;
            int quantity = 0;

            if (int.TryParse(txtQuantity.Text, out quantity) == false)
            {
                check = false;
                errorProvider1.SetError(txtQuantity, "Invalid Format!");
            }
            if (quantity < 1)
            {
                check = false;
                errorProvider1.SetError(txtQuantity, "Invalid Format!");
            }
            return check;
        }

        private bool ValidDataEdit()
        {
            bool check = true;
            int editQuantity = 0;
            if (int.TryParse(txtQuantityEdit.Text, out editQuantity) == false)
            {
                check = false;
                errorProvider2.SetError(txtQuantityEdit, "Invalid Format!");
            }
            if (editQuantity < 1)
            {
                check = false;
                errorProvider1.SetError(txtQuantityEdit, "Invalid Format!");
            }
            return check;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (ValidData())
            {
                int productid = int.Parse(txtProductID.Text);
                var orderDetail = (from o in orderDetails
                                   where o.ProductId == productid
                                   select o).FirstOrDefault();
                if (orderDetail != null)
                {
                    foreach (var item in orderDetails)
                    {
                        if (item.ProductId == orderDetail.ProductId)
                        {
                            int oldQuantity = item.Quantity;
                            item.Quantity = oldQuantity + int.Parse(txtQuantity.Text);
                            item.Total = item.Quantity * item.ProductPrice;
                            break;
                        }
                    }
                    ReloadData();
                }
                else
                {
                    OrderDetail orderDetail1 = new OrderDetail()
                    {
                        ProductName = txtName.Text,
                        ProductPrice = double.Parse(txtPrice.Text),
                        Quantity = int.Parse(txtQuantity.Text),
                        Total = double.Parse(txtPrice.Text) * int.Parse(txtQuantity.Text),
                        ProductId = productid,
                    };
                    orderDetails.Add(orderDetail1);
                    ReloadData();
                }

            }
        }

        private void dtgProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dtgProducts.SelectedRows.Count > 0)
            {
                int id = (int)dtgProducts.SelectedRows[0].Cells[0].Value;
                var product = (from p in context.Products
                               where p.Id == id
                               select p).FirstOrDefault();
                if (product != null)
                {
                    txtProductID.Text = product.Id.ToString();
                    txtID.Text = "";
                    txtName.Text = product.Name;
                    txtPrice.Text = product.Price.ToString();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dtgOrderDetail.SelectedRows.Count > 0)
            {
                if (ValidDataEdit())
                {
                    int id = (int)dtgOrderDetail.SelectedRows[0].Cells[2].Value;
                    var orderDetail = (from p in orderDetails
                                       where p.ProductId == id
                                       select p).FirstOrDefault();
                    if (orderDetail != null)
                    {
                        foreach (var item in orderDetails)
                        {
                            if (item.ProductId == orderDetail.ProductId)
                            {
                                item.Quantity = int.Parse(txtQuantityEdit.Text);
                                item.Total = item.Quantity * item.ProductPrice;
                                break;
                            }
                        }
                        ReloadData();
                    }
                }

            }

        }

        private void dtgOrderDetail_SelectionChanged(object sender, EventArgs e)
        {
            if (dtgOrderDetail.SelectedRows.Count > 0)
            {
                int id = (int)dtgOrderDetail.SelectedRows[0].Cells[2].Value;
                var orderDetail = (from p in orderDetails
                                   where p.ProductId == id
                                   select p).FirstOrDefault();
                if (orderDetail != null)
                {
                    txtQuantityEdit.Text = orderDetail.Quantity.ToString();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dtgOrderDetail.SelectedRows.Count > 0)
            {
                int id = (int)dtgOrderDetail.SelectedRows[0].Cells[2].Value;
                OrderDetail order = (from p in orderDetails
                                     where p.ProductId == id
                                     select p).FirstOrDefault();
                if (order != null)
                {
                    orderDetails.Remove(order);
                    ReloadData();
                }
            }
        }

        private void btnAddOrder_Click(object sender, EventArgs e)
        {
            var acc = (from a in context.Accounts
                       where a.Id == 1
                       select a).FirstOrDefault();
            if (acc != null)
            {
                Order order = new Order()
                {
                    Account = acc,
                    AccountId = acc.Id,
                    CreatedDate = DateTime.Now,
                    TotalPrice = double.Parse(lblTotal.Text)
                };
                context.Orders.Add(order);
                context.SaveChanges();
                int orderId = order.Id;
                foreach (var item in orderDetails)
                {
                    item.OrderId = orderId;
                    context.OrderDetails.Add(item);
                }
                context.SaveChanges();
                MessageBox.Show("Create order success");
            }


        }

        private void label6_Click(object sender, EventArgs e)
        {
            ViewOrdersForm form = new ViewOrdersForm();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            CategoryForm form = new CategoryForm();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();
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

        private void label10_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }
    }
}
