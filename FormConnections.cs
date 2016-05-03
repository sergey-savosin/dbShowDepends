using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using dbShowDepends.Data;

namespace dbShowDepends
{
    public partial class FormConnections : Form
    {

        XmlSerializer xmlser = new XmlSerializer(typeof(SetupConnectionCollection));
        //const string strFilter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
        string settingsFileName = "connections.xml";

        public FormConnections(string _settingsFilename)
        {
            InitializeComponent();
            settingsFileName = _settingsFilename;

            bsConnections.DataSource = typeof(SetupConnectionCollection);
            bsConnections.DataMember = "Connections";
            bsConnections.AddNew();

            lbConnections.DataSource = bsConnections;
            lbConnections.DisplayMember = "ConnectionName";

            tbConnectionName.DataBindings.Add("Text", bsConnections, "ConnectionName");
            tbServerName.DataBindings.Add("Text", bsConnections, "ServerName");
            cbDbName.DataBindings.Add("Text", bsConnections, "DbName");
            cbIsAnalyzeBadObjects.DataBindings.Add("Checked", bsConnections, "IsAnalyzeBadObjects");
            tbSortOrder.DataBindings.Add("Text", bsConnections, "SortOrder");
        }

        /// <summary>
        /// Загрузить настройки из файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void LoadSettings(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            bsConnections.DataSource = xmlser.Deserialize(sr);
            sr.Close();
        }

        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void SaveSettings(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = settingsFileName;

            //prepare array
            SetupConnectionCollection cc = new SetupConnectionCollection();
            cc.Connections = new List<SetupConnection>();
            foreach (SetupConnection c in bsConnections.List)
            {
                cc.Connections.Add(c);
            }

            //save array
            StreamWriter sw = new StreamWriter(fileName);
            xmlser.Serialize(sw, cc);
            sw.Close();
        }

        /// <summary>
        /// Загрузить список соединений и вернуть в виде коллекции
        /// </summary>
        /// <returns>Коллекция соединений</returns>
        public SetupConnectionCollection ConnectionCollectionGet()
        {
            if (string.IsNullOrEmpty(settingsFileName))
                return null;

            LoadSettings(settingsFileName);

            //prepare array
            SetupConnectionCollection cc = new SetupConnectionCollection();
            cc.Connections = new List<SetupConnection>();
            foreach (SetupConnection c in bsConnections.List)
            {
                cc.Connections.Add(c);
            }

            return cc;
        }

        //private void btLoad_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Filter = strFilter;

        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        LoadSettings(dlg.FileName);
        //    }
        //}

        //private void btSave_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog dlg = new SaveFileDialog();
        //    dlg.Filter = strFilter;

        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        SaveSettings(dlg.FileName);
        //    }
        //}

        private void FormConnections_FormClosing(object sender, FormClosingEventArgs e)
        {
            bsConnections.EndEdit();
            if (!string.IsNullOrEmpty(settingsFileName))
            {
                //if (MessageBox.Show("Save changes?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (1==1)
                {
                    string setupFile;
                    setupFile = Application.StartupPath + "\\" + settingsFileName;
                    SaveSettings(setupFile);
                    DialogResult = DialogResult.Yes;
                }
                else
                {
                    DialogResult = DialogResult.No;
                }
            }
            else
            {
                this.DialogResult = DialogResult.No;
            }

        }

        private void FormConnections_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settingsFileName))
            {
                LoadSettings(settingsFileName);
            }
        }

        
    }
}
