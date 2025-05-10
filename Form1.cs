using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shop
{
    public partial class LOGIN : Form
    {
        public LOGIN()
        {
            InitializeComponent();
        }

       SqlConnection Con = new SqlConnection(@"Data Source=Dinzy;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;");

        private async void loginbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the username and password are correct
                if (username.Text != "" && password.Text != "")
                {
                    using (SqlConnection Con = new SqlConnection(@"Data Source=Dinzy;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;"))
                    {
                        Con.Open();
                        string query = "SELECT Role FROM AttTable WHERE AttName = @AttName AND Password = @Password";
                        SqlCommand command = new SqlCommand(query, Con);
                        command.Parameters.AddWithValue("@AttName", username.Text);
                        command.Parameters.AddWithValue("@Password", password.Text);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            string role = reader["Role"].ToString();

                            if (role == "ADMIN")
                            {
                                // Admin login successful
                                MessageBox.Show("Admin login successful!");
                                Forms products = new Forms(); // Admin form
                                products.Show();
                                await Task.Delay(500);
                                this.Hide();
                            }
                            else if (role == "NULL" || role == "User" ||string.IsNullOrEmpty(role))
                            {
                                // Role is NULL or empty, navigate to the SellerForm
                                MessageBox.Show("Login successful! Redirecting to Seller Form.");
                                SellingForm sellerForm = new SellingForm();
                                sellerForm.Show();
                                await Task.Delay(500);
                                this.Hide();
                            }
                            else
                            {
                                // Handle other roles here if needed
                                MessageBox.Show("Welcome, " + role + "!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Password!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter both Username and Password!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void formside_Paint(object sender, PaintEventArgs e)
        {
            // You can implement custom painting logic here if necessary
        }

        private async void no_account_Click(object sender, EventArgs e)
        {
            Register reg = new Register();
            reg.Show();
            await Task.Delay(500);
            this.Hide();
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            username.Text = "";
            password.Text = "";
        }
    }
}
