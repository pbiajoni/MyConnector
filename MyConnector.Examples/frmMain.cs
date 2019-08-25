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
        MyCon myCon = new MyCon("SERVER=localhost;PORT=3306;UID=proton;PASSWORD=wMZ3gE6L;DATABASE=proton;");
        public frmMain()
        {
            InitializeComponent();
            myCon.OnExecuteQuery += MyCon_OnExecuteQuery;
            myCon.OnCommited += MyCon_OnCommited;
            myCon.OnOpenConnection += MyCon_OnOpenConnection;
            myCon.OnCloseConnection += MyCon_OnCloseConnection;
            myCon.OnRollBack += MyCon_OnRollBack;
        }

        private void MyCon_OnRollBack()
        {
            txtlog.AppendText("execute rollback" + Environment.NewLine);
        }

        private void MyCon_OnCloseConnection()
        {
            txtlog.AppendText("was closed" + Environment.NewLine);
        }

        private void MyCon_OnOpenConnection()
        {
            txtlog.AppendText("open connection"+ Environment.NewLine);
        }

        private void MyCon_OnCommited()
        {
            txtlog.AppendText("was commited" + Environment.NewLine);
        }

        private void MyCon_OnExecuteQuery(string query)
        {
            txtlog.AppendText(query + Environment.NewLine);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
            
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            try
            {
                Database database = new Database(myCon.Database, myCon);

                Table Table = new Table("users");
                Table.Engine = "InnoDB";

                //Table.Fields.Add(new Field()
                //{
                //    Name = "id",
                //    AllowNull = false,
                //    Type = "int(11)",
                //    Key = "PRI",
                //    Extra = "auto_increment"
                //});

                //Table.Fields.Add(new Field()
                //{
                //    Name = "username",
                //    Type = "varchar(20)",
                //    AllowNull = false,
                //});

                //Table.Fields.Add(new Field()
                //{
                //    Name = "password",
                //    Type = "varchar(50)",
                //    AllowNull = false,
                //});

                //Table.Fields.Add(new Field()
                //{
                //    Name = "fullname",
                //    Type = "varchar(100)",
                //    AllowNull = true
                //});

                //Table.Fields.Add(new Field()
                //{
                //    Name = "email",
                //    Type = "varchar(100)",
                //    AllowNull = true
                //});

                //Table.Fields.Add(new Field()
                //{
                //    Name = "status",
                //    Type = "int(1)",
                //    AllowNull = false,
                //    Default = "0"
                //});

                Table.AddVarCharField("teste_campo", 54);
                Table.AddVarCharField("teste_campo2", 52);

                Table.Indexes.Add(new Index()
                {
                    Unique = true,
                    KeyName = "unique_username",
                    ColumnName = "username"
                });

                database.Tables.Add(Table);
                database.ValidateTable("users");
                myCon.Commit();
            }
            catch (Exception er)
            {

                txtlog.AppendText(er.Message + Environment.NewLine);
            }
        }
    }
}
