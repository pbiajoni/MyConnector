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
        MyCon myCon = new MyCon("SERVER=localhost;PORT=3306;UID=mycon;PASSWORD=1234;DATABASE=myconnector;");
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
                });

                Table.Fields.Add(new Field()
                {
                    Name = "password",
                    Type = "varchar(50)",
                    AllowNull = false,
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

                Table.AddVarCharField("teste_campo", 54);
                Table.AddVarCharField("teste_campo2", 52);

                Table.Indexes.Add(new Index()
                {
                    Unique = true,
                    KeyName = "unique_username",
                    ColumnName = "username"
                });

                database.Tables.Add(Table);

                Table = new Table("countries");
                Table.AddIdField();
                Table.AddVarCharField("name", 50);
                database.Tables.Add(Table);

                Table = new Table("states");
                Table.AddIdField();
                Table.AddIntField("country_id");
                Table.AddIntField("user_id");
                Table.AddVarCharField("name", 50);

                Table.References.Add(new References("countries", "id", "user_id", "fk_state_country"));
                database.Tables.Add(Table);

                Table = new Table("cities");
                Table.AddIdField();
                Table.AddIntField("state_id");
                Table.AddVarCharField("name", 100);

                Table.References.Add(new References("states", "id", "state_id", "fk_city_state"));
                database.Tables.Add(Table);

                database.ValidateTable("users");
                database.ValidateTable("countries");
                database.ValidateTable("states");
                database.ValidateTable("cities");

                myCon.Commit();
            }
            catch (Exception er)
            {

                txtlog.AppendText(er.Message + Environment.NewLine);
            }
        }
    }
}
