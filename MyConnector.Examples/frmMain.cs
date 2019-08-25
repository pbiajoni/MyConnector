using MyConnector.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyConnector.Examples
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            MyCon myCon = new MyCon("convert zero datetime = True; allow zero datetime = false; allow user variables = true; Connect Timeout = 30; SERVER = 162.241.203.55; PORT = 3306; UID = sabend06_proton; PASSWORD = wR@ma4AH&*(; DATABASE = sabend06_proton;");
            Database database = new Database(myCon.Database, myCon);

            Table Table = new Table("users");
            Table.Engine = "InnoDB";

            Table.Fields.Add(new Field()
            {
                Name = "id",
                AllowNull = false,
                Type = "int(11)",
                Key = "PRI",
                Extra = "auto_increment"
            });

            Table.Fields.Add(new Field()
            {
                Name = "username",
                Type = "varchar(20)",
                AllowNull = false,
                Default = "''"
            });

            Table.Fields.Add(new Field()
            {
                Name = "password",
                Type = "varchar(50)",
                AllowNull = false,
                Default = "''"
            });

            Table.Fields.Add(new Field()
            {
                Name = "fullname",
                Type = "varchar(100)",
                AllowNull = true
            });

            Table.Fields.Add(new Field()
            {
                Name = "email",
                Type = "varchar(100)",
                AllowNull = true
            });

            Table.Fields.Add(new Field()
            {
                Name = "status",
                Type = "int(1)",
                AllowNull = false,
                Default = "0"
            });

            Table.Indexes.Add(new Index()
            {
                Unique = true,
                KeyName = "unique_username",
                ColumnName = "username"
            });

            database.Tables.Add(Table);
            database.ValidateTable("users");
        }
    }
}
