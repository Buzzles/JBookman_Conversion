using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace JBookman_Mapper
{
    class CreateMapInputForm : Form
    {
        private ushort m_iMapColumnCount = 0;
        private ushort m_iMapRowCount = 0;
        private ushort m_iMapID = 0;
        private string m_sMapFileName = null;

        Label lblCols;
        Label lblRows;
        Label lblMapID;
        Label lblMapFileName;
        TextBox textCols;
        TextBox textRows;
        TextBox textMapID;
        TextBox textMapFileName;

        Button okButton;
        Button cancelButton;
        

        public CreateMapInputForm()
        {
            //empty construct
        }

        protected override void OnLoad(EventArgs e)
        {
           // MessageBox.Show("Dialog loaded");
            this.Size = new Size(200, 250);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Text = "Map Setiings";
            this.Location = new Point(540,312);
            
            //components:
            lblCols = new Label();
            lblRows = new Label();
            lblMapID = new Label();
            lblMapFileName = new Label();
            textCols = new TextBox();
            textRows = new TextBox();
            textMapID = new TextBox();
            textMapFileName = new TextBox();
            okButton = new Button();
            cancelButton = new Button();



            okButton.Size = new Size(50, 25);
            //okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Text = "OK";
            okButton.Location = new Point(ClientRectangle.Left+25, 175);
            okButton.MouseClick += new MouseEventHandler(this.okClickEvent);

            cancelButton.Size = new Size(50, 25);
            cancelButton.Location = new Point(((ClientRectangle.Right-cancelButton.Width)-25), 175);
            cancelButton.Text = "Cancel";
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            lblMapID.Text = "Map ID:";
            lblMapID.Location = new Point(30, 30);
            lblMapID.Size = new Size(90, 30);
                        
            lblCols.Text = "Map Columns:";
            lblCols.Location = new Point(30, 60);
            lblCols.Size = new Size(90, 30);

            lblRows.Text = "Map Rows:";
            lblRows.Location = new Point(30,90);
            lblRows.Size = new Size(90, 30);

            lblMapFileName.Text = "Map Name:";
            lblMapFileName.Location = new Point(30, 130);
            lblMapFileName.Size = new Size(70, 30);

            textMapID.Text = "0";
            textMapID.Location = new Point(120, 30);
            textMapID.Size = new Size(30, 30);

            textCols.Text = "0";
            textCols.Location = new Point(120, 60);
            textCols.Size = new Size(30, 30);
            
            textRows.Text = "0";
            textRows.Location = new Point(120, 90);
            textRows.Size = new Size(30, 30);

            textMapFileName.Text = "";
            textMapFileName.Location = new Point(100, 130);
            textMapFileName.Size = new Size(70, 30);

            this.Controls.AddRange(new Control[]{ okButton, cancelButton,lblMapID,lblCols,lblRows,textMapID,textCols,textRows, lblMapFileName,textMapFileName });
        }

        private void okClickEvent(object sender, EventArgs e)
        {
            bool bSucceed = false;
            //grab values, try and convert.
          if(!textMapFileName.Text.Equals("") && !textRows.Text.Equals("0") && !textCols.Text.Equals("0"))
          {
            
            try
            {
                m_iMapID = ushort.Parse(textMapID.Text);
                m_iMapColumnCount = ushort.Parse(textCols.Text);
                m_iMapRowCount = ushort.Parse(textRows.Text);
                m_sMapFileName = textMapFileName.Text;
                bSucceed = true;
            }
            catch (FormatException exception)
            {
                MessageBox.Show("Incorrect format, please enter a number between 0 and 65,535"+exception);
            }
            
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

            if (bSucceed)
            { this.DialogResult = System.Windows.Forms.DialogResult.OK; }

        }
        }

        private void onCancelEvent(object sender, EventArgs e)
        {

        }

        public ushort MapID
        {
            get{return m_iMapID;}
            //set { }
        }
        public ushort MapCols
        {
            get { return m_iMapColumnCount; }
        }
        public ushort MapRows
        {
            get { return m_iMapRowCount; }
        }
        public string MapName
        {
            get { return m_sMapFileName; }
        }
    }
}
