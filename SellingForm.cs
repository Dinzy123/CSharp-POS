using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Shop
{
    public partial class SellingForm : Form
    {
        public SellingForm()
        {
            InitializeComponent();
        }

        //private void panel1_Paint(object sender, PaintEventArgs e)
        //{
        //    sellername.Text = Globals.Get();  // This will get the seller's name from the Globals class
        //    date.Text = DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
        //}

        private void label5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        SqlConnection Con = new SqlConnection(@"Data Source=Dinzy;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;");

        private void prodList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            prodname.Text = prodList.SelectedRows[0].Cells[0].Value.ToString();
            Qcheck.Text = prodList.SelectedRows[0].Cells[1].Value.ToString();
            price.Text = prodList.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void fetchData()
        {
            Con.Open();
            string query = "select ProdName, Quantity, Price, CostPrice from ProdTable";
            SqlDataAdapter sda = new SqlDataAdapter(query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var data = new DataSet();
            sda.Fill(data);
            prodList.DataSource = data.Tables[0];
            Con.Close();
        }

        private void fetchDataSpecific()
        {
            Con.Open();
            string query = "select ProdName, Quantity, Price, CostPrice from ProdTable where category='" + categoryS.SelectedValue.ToString() + "'";
            SqlDataAdapter sda = new SqlDataAdapter(query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var data = new DataSet();
            sda.Fill(data);
            prodList.DataSource = data.Tables[0];
            Con.Close();
        }

        private void fetchDataSpecificText()
        {
            Con.Open();
            string query = "select ProdName, Quantity, Price from ProdTable where ProdName like '" + "%" + search.Text + "%" + "'";
            SqlDataAdapter sda = new SqlDataAdapter(query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var data = new DataSet();
            sda.Fill(data);
            prodList.DataSource = data.Tables[0];
            Con.Close();
        }

        private void FetchCat()
        {
            Con.Open();
            String query = "select CatName from CatTable";
            SqlCommand command = new SqlCommand(query, Con);
            SqlDataReader read;
            read = command.ExecuteReader();
            DataTable data = new DataTable();
            data.Columns.Add("CatName", typeof(string));
            data.Load(read);
            categoryS.ValueMember = "catName";
            categoryS.DataSource = data;
            Console.WriteLine(data.GetType());
            Con.Close();
        }

        private void SellingForm_Load(object sender, EventArgs e)
        {
            fetchData();
            FetchCat();
        }

        decimal Gtotal = 0;
        int n = 0;

        private void prodaddbtn_Click(object sender, EventArgs e)
        {
            decimal priceValue;
            int quantityValue;
            decimal costPriceValue;

            // Try parsing price as a decimal and quantity as an integer
            bool isPriceValid = decimal.TryParse(price.Text, out priceValue);
            bool isQuantityValid = int.TryParse(quantity.Text, out quantityValue);
            bool isCostPriceValid = decimal.TryParse(prodList.SelectedRows[0].Cells[3].Value.ToString(), out costPriceValue);

            // Check if any required fields are empty
            if (string.IsNullOrWhiteSpace(prodname.Text) || string.IsNullOrWhiteSpace(quantity.Text) || string.IsNullOrWhiteSpace(price.Text))
            {
                MessageBox.Show("Can't Add! Missing Info");
                return; // Exit the method if any fields are empty
            }

            // Check if quantity exceeds available stock
            if (!isQuantityValid || quantityValue > Convert.ToInt32(Qcheck.Text))
            {
                MessageBox.Show("Can't Add! Product quantity is less than required.");
                return; // Exit the method if quantity is invalid or exceeds stock
            }

            // If price is not valid, show an error message
            if (!isPriceValid)
            {
                MessageBox.Show("Invalid price value. Please enter a valid decimal number.");
                return; // Exit the method if price is invalid
            }

            if (!isCostPriceValid)
            {
                MessageBox.Show("Invalid cost price value. Please enter a valid decimal number.");
                return;
            }

            // Calculate the total (now using decimal for price)
            decimal total = priceValue * quantityValue;

            // Increment the item number (n) and add the product to the DataGridView
            n++;
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(activesale);
            row.Cells[0].Value = n;
            row.Cells[1].Value = prodname.Text;
            row.Cells[2].Value = priceValue.ToString("F2"); // Format price to 2 decimal places
            row.Cells[3].Value = quantityValue.ToString(); // Store quantity as a number
            row.Cells[4].Value = total.ToString("F2"); // Format total to 2 decimal places
            row.Cells[5].Value = costPriceValue.ToString("F2"); // FIXED COST PRICE DISPLAY

            activesale.Rows.Add(row);

            // Update the grand total (now using decimal for total)
            Gtotal += total;
            totallb.Text = "R " + Gtotal.ToString("F2");

            // Clear input fields after adding the product
            prodname.Text = "";
            price.Text = "";
            quantity.Text = "";
        }


        private void activesale_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (activesale.Rows.Count > 0)
            {
                prodidedit.Text = activesale.SelectedRows[0].Cells[0].Value.ToString();
                prodnameedit.Text = activesale.SelectedRows[0].Cells[1].Value.ToString();
                qtyedit.Text = activesale.SelectedRows[0].Cells[3].Value.ToString();
            }
        }

        private void prodeditbtn_Click(object sender, EventArgs e)
        {
            if (prodidedit.Text == "")
            {
                MessageBox.Show("Product Not Selected \nPlease select the product to edit");
            }
            else
            {
                // Check if the value in Cells[4] is null or empty
                var currentValue = activesale.SelectedRows[0].Cells[4].Value?.ToString().Trim();
                if (string.IsNullOrEmpty(currentValue) || !decimal.TryParse(currentValue, out decimal validValue))
                {
                    MessageBox.Show($"Invalid value in total column: '{currentValue}'. Please check the data.");
                    return; // Exit the method if the value is not valid
                }

                // If valid, proceed with your logic
                Gtotal = Gtotal - validValue;

                // Check if qtyedit is a valid number
                if (decimal.TryParse(qtyedit.Text, out decimal newQty))
                {
                    decimal newTotal = newQty * Convert.ToDecimal(activesale.SelectedRows[0].Cells[2].Value.ToString());
                    activesale.SelectedRows[0].Cells[4].Value = newTotal;
                    Gtotal = Gtotal + newTotal;
                    totallb.Text = "R " + Gtotal;
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantity.");
                    return;
                }

                prodidedit.Text = "";
                prodnameedit.Text = "";
                qtyedit.Text = "";
            }
        }




        private void proddelbtn_Click(object sender, EventArgs e)
        {
            if (prodidedit.Text == "")
            {
                MessageBox.Show("Product Not Selected \nPlease select the product to delete");
            }
            else
            {
                // Ensure the value in the "Total" column is a valid number before subtracting it
                string totalValue = activesale.SelectedRows[0].Cells[4].Value.ToString();
                decimal totalDecimal;
                if (decimal.TryParse(totalValue, out totalDecimal))
                {
                    Gtotal -= totalDecimal;
                    totallb.Text = "R " + Gtotal.ToString("F2");
                }
                else
                {
                    MessageBox.Show("Invalid total value. Cannot delete product.");
                    return;
                }

                // Clear the edit fields and remove the selected row
                prodidedit.Text = "";
                prodnameedit.Text = "";
                qtyedit.Text = "";
                activesale.Rows.Remove(activesale.SelectedRows[0]);
            }
        }


        private void calculate_Click(object sender, EventArgs e)
        {
            if (paid.Text == "")
            {
                MessageBox.Show("Enter Paid Amount");
            }
            else
            {
                if (Gtotal > Convert.ToInt32(paid.Text))
                {
                    MessageBox.Show("Paid Amount Not Enough To Complete Purchase");
                }
                else
                {
                    change.Text = "R " + (decimal.Parse(paid.Text) - Gtotal).ToString("F2");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LOGIN lg = new LOGIN();
            lg.Show();
            this.Hide();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("PureWave Detergents", new Font("Ariel", 30, FontStyle.Bold), Brushes.Maroon, new Point(250));

            string showdate = "Date:\t" + date.Text + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string top = "********************************************************\n";
            string top1 = "\t\tPurchace Receipt \t\n";
            string top2 = "********************************************************\n\n";
            string topunder = "--------------------------------------------------------\n\n";
            string top3 = "Item\tPrice(R)\tQty\tTotal(R)\n";
            string down = "***********************************************************\n";
            string downG = "GrandTotal:\t";
            string downP = "Paid:\t\t";
            string downC = "Change:\t";
            string down1 = "***********************************************************\n";
            string down2 = "\n\n\nThank You For Shopping\n\n\n";

            e.Graphics.DrawString(showdate, new Font("Ariel", 15, FontStyle.Bold), Brushes.Black, new Point(130, 100));

            e.Graphics.DrawString(top + top1 + top2, new Font("Ariel", 22, FontStyle.Bold), Brushes.Maroon, new Point(75, 150));
            e.Graphics.DrawString(top3 + topunder, new Font("Ariel", 22, FontStyle.Bold), Brushes.Maroon, new Point(130, 300));

            int n = 0, pt = 360;
            while (n < activesale.Rows.Count)
            {
                String output = activesale.Rows[n].Cells[1].Value.ToString() + "\t\t" + activesale.Rows[n].Cells[2].Value.ToString() + "\t     " + activesale.Rows[n].Cells[3].Value.ToString() + "\t      " + activesale.Rows[n].Cells[4].Value.ToString() + "\t      " + activesale.Rows[n].Cells[5].Value.ToString();
                e.Graphics.DrawString(output, new Font("Ariel", 20, FontStyle.Bold), Brushes.Black, new Point(130, pt));
                string NameU = activesale.Rows[n].Cells[1].Value.ToString();
                string PriceU = activesale.Rows[n].Cells[2].Value.ToString();
                string QtyU = activesale.Rows[n].Cells[3].Value.ToString();
                string TotalU = activesale.Rows[n].Cells[4].Value.ToString();
                string CostU = activesale.Rows[n].Cells[5].Value.ToString();
                AddSales("insert into AllSalesTable (Name, Price, Qty, Total, Date, CostPrice) values ('" + NameU + "','" + PriceU + "','" + QtyU + "','" + TotalU + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + CostU + "')");
                UpdateProdQty("update ProdTable set Quantity=(Quantity -" + Convert.ToInt32(QtyU) + ") where ProdName='" + NameU + "'" + " and Price=" + PriceU + ";");

                n++;
                pt += 50;
            }
            String Paid = "R " + paid.Text;
            e.Graphics.DrawString(down, new Font("Ariel", 20, FontStyle.Bold), Brushes.Maroon, new Point(75, pt + 50));
            e.Graphics.DrawString(downG + totallb.Text + "\n", new Font("Ariel", 20, FontStyle.Bold), Brushes.Maroon, new Point(130, pt + 100));
            e.Graphics.DrawString(downP + Paid + "\n", new Font("Ariel", 20, FontStyle.Bold), Brushes.Maroon, new Point(130, pt + 150));
            e.Graphics.DrawString(downC + change.Text + "\n", new Font("Ariel", 20, FontStyle.Bold), Brushes.Maroon, new Point(130, pt + 200));
            e.Graphics.DrawString(down1, new Font("Ariel", 20, FontStyle.Bold), Brushes.Maroon, new Point(75, pt + 250));
            e.Graphics.DrawString(down2, new Font("Ariel", 20, FontStyle.Bold), Brushes.Maroon, new Point(250, pt + 300));
        }

        private void AddSales(String sale)
        {
            Con.Open();
            String query = sale;
            SqlCommand command = new SqlCommand(query, Con);
            command.ExecuteNonQuery();
            Con.Close();
        }

        private void UpdateProdQty(String q)
        {
            Con.Open();
            String query = q;
            SqlCommand command = new SqlCommand(query, Con);
            command.ExecuteNonQuery();
            Con.Close();
        }

        private void AddHist()
        {
            Con.Open();
            String query = "insert into HistoryTable (AttName, Date, Amount) values ('" + sellername.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Gtotal.ToString() + "')";
            SqlCommand command = new SqlCommand(query, Con);
            command.ExecuteNonQuery();
            Con.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (activesale.Rows.Count < 0 || totallb.Text == "" || change.Text == "")
            {
                MessageBox.Show("Information to print not complete.\nCheck Items and Paid Amount");
            }
            else
            {
                if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();
                }
                AddHist();
                activesale.Rows.Clear();
                totallb.Text = "";
                change.Text = "";
                paid.Text = "";
                Gtotal = 0;
                n = 0;
                fetchData();
            }

        }

        private void categoryS_SelectionChangeCommitted(object sender, EventArgs e)
        {
            fetchDataSpecific();
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            fetchDataSpecificText();
        }

       private void done_Click(object sender, EventArgs e)
{
    if (activesale.Rows.Count <= 0 || string.IsNullOrWhiteSpace(totallb.Text) || string.IsNullOrWhiteSpace(change.Text))
    {
        MessageBox.Show("Information to print not complete.\nCheck Items and Paid Amount");
        return;
    }

    using (SqlConnection con = new SqlConnection(@"Data Source=Dinzy;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;"))
    {
        con.Open();

        for (int m = 0; m < activesale.Rows.Count; m++)
        {
            string nameU = activesale.Rows[m].Cells[1].Value.ToString();
            decimal priceU = Convert.ToDecimal(activesale.Rows[m].Cells[2].Value);
            int qtyU = Convert.ToInt32(activesale.Rows[m].Cells[3].Value);
            decimal totalU = Convert.ToDecimal(activesale.Rows[m].Cells[4].Value);
            DateTime dateNow = DateTime.Now;
            decimal costU = Convert.ToDecimal(activesale.Rows[m].Cells[5].Value);

                    // Insert into AllSalesTable
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO AllSalesTable (Name, Price, CostPrice, Qty, Total, Date) VALUES (@Name, @Price, @CostPrice, @Qty, @Total, @Date)", con))
            {
                cmd.Parameters.AddWithValue("@Name", nameU);
                cmd.Parameters.AddWithValue("@Price", priceU);
                cmd.Parameters.AddWithValue("@CostPrice", costU);
                cmd.Parameters.AddWithValue("@Qty", qtyU);
                cmd.Parameters.AddWithValue("@Total", totalU);
                cmd.Parameters.AddWithValue("@Date", dateNow);
                cmd.ExecuteNonQuery();
            }

            // Update ProdTable
            using (SqlCommand cmdUpdate = new SqlCommand("UPDATE ProdTable SET Quantity = Quantity - @Qty WHERE ProdName = @ProdName AND Price = @Price", con))
            {
                cmdUpdate.Parameters.AddWithValue("@Qty", qtyU);
                cmdUpdate.Parameters.AddWithValue("@ProdName", nameU);
                cmdUpdate.Parameters.AddWithValue("@Price", priceU);
                cmdUpdate.ExecuteNonQuery();
            }
        }

        con.Close();
    }

    AddHist();
    activesale.Rows.Clear();
    totallb.Text = "";
    change.Text = "";
    paid.Text = "";
    Gtotal = 0;
    n = 0;
    fetchData();
}


        private void refresh_Click(object sender, EventArgs e)
        {
            search.Clear();
            fetchData();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void SellingForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
