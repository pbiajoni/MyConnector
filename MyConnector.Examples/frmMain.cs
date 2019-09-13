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
            txtlog.AppendText("open connection" + Environment.NewLine);
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

                Table Table = new Table("tmycon");
                Table.AddVarCharField("test", 10);
                Table.Fields.Add(new Field()
                {
                    Name = "date_and_time",
                    Default = "",
                    Type = "datetime",
                    AllowNull = true
                });

                database.Tables.Add(Table);

                database.ValidateAllTables();

                myCon.Commit();
            }
            catch (Exception er)
            {

                txtlog.AppendText(er.Message + Environment.NewLine);
            }
        }
    }
}
