using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PPB_Installer.Controller
{
    public class InstallerController
    {
        InstallerForm f = new InstallerForm();

        public InstallerController()
        {
            LoadData();
            InitListeners();
            Application.Run(f);
        }

        void LoadData()
        {

        }

        void InitListeners()
        {
            f.install.Click += Install;
        }

        private void Install(object sender, EventArgs e)
        {
            f.install.Enabled = false;

            CreateDatabase();
            CreateTables();
            CreateRegisters();

            f.messageText.Text = "La instalació s'ha completat exitosament.";

            f.install.Text = "Finalitzar";
            f.install.Click -= Install;
            f.install.Click += End;
            f.install.Enabled = true;
        }

        private void End(object sender, EventArgs e)
        {
            f.Close();
        }

        private void CreateDatabase()
        {
            string conStr = "Server=.\\sqlexpress;Integrated security=SSPI;database=master";

            SqlConnection con = new SqlConnection(conStr);

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();

                    f.messageText.Text = "Eliminant base de dades anterior...";
                    SqlCommand command = new SqlCommand("DROP DATABASE IF EXISTS [PPB_Storage]", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    f.messageText.Text = "Creant base de dades...";
                    command = new SqlCommand("CREATE DATABASE [PPB_Storage]", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void CreateTables()
        {
            string conStr = "Server=.\\sqlexpress;Integrated security=SSPI;database=PPB_Storage";

            SqlConnection con = new SqlConnection(conStr);

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();

                    f.messageText.Text = "Creant taules...";
                    SqlCommand command = new SqlCommand("CREATE TABLE \"products\" (" +
                        "\"id\" int IDENTITY (1, 1) NOT NULL PRIMARY KEY," +
                        "\"barcode\" nvarchar(13) NOT NULL UNIQUE," +
                        "\"image\" nvarchar(255) NOT NULL," +
                        "\"name\" nvarchar(255) NOT NULL," +
                        "\"price\" real NOT NULL," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("CREATE TABLE \"commands\" (" +
                        "\"id\" int IDENTITY (1, 1) NOT NULL PRIMARY KEY," +
                        "\"number\" int NOT NULL," +
                        "\"date\" date NOT NULL," +
                        "\"ready\" bit NOT NULL," +
                        "\"delivered\" bit NOT NULL," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("CREATE TABLE \"products_commands\"(" +
                        "\"id\" int IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                        "\"commandId\" int NOT NULL FOREIGN KEY REFERENCES \"dbo\".\"commands\"(\"id\")," +
                        "\"productId\" int NOT NULL FOREIGN KEY REFERENCES \"dbo\".\"products\"(\"id\")," +
                        "\"quantity\" int NOT NULL," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("CREATE TABLE \"roles\" (" +
                        "\"id\" int IDENTITY (1, 1) NOT NULL PRIMARY KEY," +
                        "\"name\" nvarchar(255) NOT NULL," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("CREATE TABLE \"users\" (" +
                        "\"id\" int IDENTITY (1, 1) NOT NULL PRIMARY KEY," +
                        "\"first_name\" nvarchar(255) NOT NULL," +
                        "\"last_name\" nvarchar(255) NOT NULL," +
                        "\"username\" nvarchar(255) NOT NULL UNIQUE," +
                        "\"password\" nvarchar(255) NOT NULL," +
                        "\"role_id\" int NOT NULL FOREIGN KEY REFERENCES \"dbo\".\"roles\"(\"id\")," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("CREATE TABLE \"permissions\" (" +
                        "\"id\" int IDENTITY (1, 1) NOT NULL PRIMARY KEY," +
                        "\"name\" nvarchar(255) NOT NULL," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("CREATE TABLE \"permissions_roles\" (" +
                        "\"id\" int IDENTITY (1, 1) NOT NULL PRIMARY KEY," +
                        "\"roleId\" int NOT NULL FOREIGN KEY REFERENCES \"dbo\".\"roles\"(\"id\")," +
                        "\"permissionId\" int NOT NULL FOREIGN KEY REFERENCES \"dbo\".\"permissions\"(\"id\")," +
                        ")", con);

                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void CreateRegisters()
        {
            string conStr = "Server=.\\sqlexpress;Integrated security=SSPI;database=PPB_Storage";

            SqlConnection con = new SqlConnection(conStr);

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();

                    f.messageText.Text = "Creant registres...";
                    SqlCommand command = new SqlCommand("SET IDENTITY_INSERT [dbo].[permissions] ON", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("INSERT INTO[dbo].[permissions] ([id], [name]) VALUES " +
                        "(1, 'READ USERS')," +
                        "(2, 'WRITE USERS')," +
                        "(3, 'READ ROLES')," +
                        "(4, 'WRITE ROLES')," +
                        "(5, 'READ PRODUCTS')," +
                        "(6, 'WRITE PRODUCTS')," +
                        "(7, 'READ COMMANDS')," +
                        "(8, 'WRITE COMMAND')", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SET IDENTITY_INSERT [dbo].[permissions] OFF", con);
                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("SET IDENTITY_INSERT [dbo].[roles] ON", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("INSERT INTO [dbo].[roles] ([id], [name]) VALUES " +
                        "(1, 'Administrator')", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SET IDENTITY_INSERT [dbo].[roles] OFF", con);
                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("SET IDENTITY_INSERT [dbo].[users] ON", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("INSERT INTO [dbo].[users] ([id], [first_name], [last_name], [username], [password], [role_id]) VALUES " +
                        "(1, 'John', 'Doe', 'admin', '$2y$10$7U/oWytzZkWSnB7CRbTSM.6hETZcEuqWNDRvhS1LDbBnOJALpE7Ee', 1)", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SET IDENTITY_INSERT [dbo].[users] OFF", con);
                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;

                    command = new SqlCommand("INSERT INTO [dbo].[permissions_roles] ([roleId], [permissionId]) VALUES " +
                        "(1, 1)," +
                        "(1, 2)," +
                        "(1, 3)," +
                        "(1, 4)," +
                        "(1, 5)," +
                        "(1, 6)," +
                        "(1, 7)," +
                        "(1, 8)", con);
                    command.ExecuteNonQuery();

                    f.loadingBar.Value++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
