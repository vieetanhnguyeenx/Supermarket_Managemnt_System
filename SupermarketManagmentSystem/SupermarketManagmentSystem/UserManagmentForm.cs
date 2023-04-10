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
using System.Xml.Linq;

namespace SupermarketManagmentSystem
{
    public partial class UserManagmentForm : Form
    {
        private SupermarketManagementSystemContext context = new SupermarketManagementSystemContext();
        public UserManagmentForm()
        {
            InitializeComponent();
        }

        private void ReloadData()
        {
            var account = (from u in context.Accounts
                           where u.Id != 1
                           select u).ToList();
            foreach (var item in account)
            {
                var entry = context.Entry(item);
                entry.Reference(a => a.Role).Load();
            }
            dataGridView1.DataSource = (from u in account
                                        select new
                                        {
                                            u.Id,
                                            u.Username,
                                            u.Password,
                                            u.Phone,
                                            u.FullName,
                                            u.Role.RoleName
                                        }).ToList();
            List<string> roleList = (from r in context.Roles
                                     select r.RoleName).ToList();
            cbbRole.DataSource = roleList.ToList();
            if (account.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var acc = (from a in context.Accounts
                           where a.Id == id
                           select a).FirstOrDefault();
                if (acc != null)
                {
                    var e1 = context.Entry(acc);
                    e1.Reference(p => p.Role).Load();
                    txtID.Text = acc.Id.ToString();
                    txtPassword.Text = acc.Password;
                    txtFullName.Text = acc.FullName;
                    txtPhone.Text = acc.Phone;
                    txtUserName.Text = acc.Username;
                    cbbRole.SelectedIndex = cbbRole.FindStringExact(acc.Role.RoleName);
                    cbbRole.SelectedIndex = cbbRole.Items.IndexOf(acc.Role.RoleName);
                }
            }



        }
        private void UserManagmentForm_Load(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var acc = (from a in context.Accounts
                           where a.Id == id
                           select a).FirstOrDefault();
                if (acc != null)
                {
                    var e1 = context.Entry(acc);
                    e1.Reference(p => p.Role).Load();
                    txtID.Text = acc.Id.ToString();
                    txtPassword.Text = acc.Password;
                    txtFullName.Text = acc.FullName;
                    txtPhone.Text = acc.Phone;
                    txtUserName.Text = acc.Username;
                    cbbRole.SelectedIndex = cbbRole.FindStringExact(acc.Role.RoleName);
                    cbbRole.SelectedIndex = cbbRole.Items.IndexOf(acc.Role.RoleName);
                }
            }
        }

        public bool ValidData()
        {
            bool valid = true;
            if (txtUserName.Text.Trim().Length == 0)
            {
                valid = false;
                errorProvider1.SetError(txtUserName, "Invalid Format!");
            }
            if (txtPassword.Text.Trim().Length == 0)
            {
                valid = false;
                errorProvider2.SetError(txtPassword, "Invalid Format!");
            }
            if (txtPhone.Text.Trim().Length == 0)
            {
                valid = false;
                errorProvider3.SetError(txtPhone, "Invalid Format!");
            }
            if (txtFullName.Text.Trim().Length == 0)
            {
                valid = false;
                errorProvider4.SetError(txtFullName, "Invalid Format!");
            }
            return valid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();
            if (ValidData())
            {
                Account acc = new Account()
                {
                    Username = txtUserName.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    RoleId = cbbRole.SelectedIndex + 1
                };
                context.Accounts.Add(acc);
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
                    var acc = (from a in context.Accounts
                               where a.Id == id
                               select a).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.FullName = txtFullName.Text.Trim();
                        acc.Phone = txtPhone.Text.Trim();
                        acc.Username = txtUserName.Text.Trim();
                        acc.Password = txtPassword.Text.Trim();
                        acc.RoleId = cbbRole.SelectedIndex + 1;
                        context.SaveChanges();
                        MessageBox.Show("Edit Successfully", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ReloadData();
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFullName.Text = "";
            txtID.Text = "";
            txtPassword.Text = "";
            txtPhone.Text = "";
            txtUserName.Text = "";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var acc = (from a in context.Accounts
                           where a.Id == id
                           select a).FirstOrDefault();
                if (acc != null)
                {
                    var e1 = context.Entry(acc);
                    e1.Collection(a => a.Orders).Load();
                    foreach (var item in acc.Orders)
                    {
                        var e12 = context.Entry(item);
                        e12.Collection(o => o.OrderDetails).Load();
                        context.OrderDetails.RemoveRange(item.OrderDetails);
                        context.SaveChanges();
                    }
                    context.Orders.RemoveRange(acc.Orders);
                    context.SaveChanges();
                    context.Remove(acc);
                    context.SaveChanges();
                    MessageBox.Show("Delete Successfully", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReloadData();
                }
            }

        }

        private void lblSeller_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {
            CategoryForm form = new CategoryForm();
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
    }
}
