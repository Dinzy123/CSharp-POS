using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shop
{
    public partial class Register : Form
    {
        // Connection string
        string connectionString = @"Data Source=Dinzy;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;";

        public Register()
        {
            InitializeComponent();
        }

        private async void registerbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                MessageBox.Show("Username and Password are required.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection Con = new SqlConnection(connectionString))
            {
                try
                {
                    Con.Open();

                    string query = "INSERT INTO AttTable (AttName, Password, Role, Age, Number) VALUES (@AttName, @Password, @Role, @Age, @Number)";

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@AttName", username.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", password.Text.Trim());
                        cmd.Parameters.AddWithValue("@Role", "User"); // You can change this or make it selectable in UI
                        cmd.Parameters.AddWithValue("@Age", int.TryParse(age.Text.Trim(), out int parsedAge) ? parsedAge : 0);
                        cmd.Parameters.AddWithValue("@Number", string.IsNullOrWhiteSpace(number.Text.Trim()) ? "Unknown" : number.Text.Trim());

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Registration Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();

                        LOGIN login= new LOGIN();
                        login.Show();
                        await Task.Delay(500);
                        this.Hide(); // Hide the current register form
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            username.Text = "";
            password.Text = "";
            age.Text = "";
            number.Text = "";
        }

        private async void has_account_Click(object sender, EventArgs e)
        {
            LOGIN backlog = new LOGIN();
            backlog.Show();
            await Task.Delay(500);
            this.Hide();
        }

        // Unused event handlers (optional to remove)
        private void label5_Click(object sender, EventArgs e) { }
        private void number_TextChanged(object sender, EventArgs e) {
            string number_input = new string(number.Text.Where(char.IsDigit).ToArray());

            //Now si limit ku 10 characters
            if (number_input.Length > 10)
            {
                number_input = number_input.Substring(0, 10);
            }

            if (number.Text != number_input)
            {
                int selectionStart = age.SelectionStart - (age.Text.Length - number_input.Length);
                number.Text = number_input;
                number.SelectionStart = Math.Max(selectionStart, 0);
            }
        }
        private void age_TextChanged(object sender, EventArgs e)
        {
            // Remove any non-digit characters
            string input = new string(age.Text.Where(char.IsDigit).ToArray());

            // Limit to max 2 characters
            if (input.Length > 2)
            {
                input = input.Substring(0, 2);
            }

            // Only update if the text has changed (avoid recursive call)
            if (age.Text != input)
            {
                int selectionStart = age.SelectionStart - (age.Text.Length - input.Length);
                age.Text = input;
                age.SelectionStart = Math.Max(selectionStart, 0);
            }
        }

        private void password_TextChanged(object sender, EventArgs e) { }
        private void username_TextChanged(object sender, EventArgs e) { }
    }
}
