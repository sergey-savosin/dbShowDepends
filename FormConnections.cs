using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using dbShowDepends.Data;
using dbShowDepends.Settings;

namespace dbShowDepends
{
    public partial class FormConnections : Form
    {

        XmlSerializer xmlser = new XmlSerializer(typeof(SetupConnectionCollection));
        //const string strFilter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

        public FormConnections()
        {
            InitializeComponent();

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
        /// Загрузить настройки соединений
        /// </summary>
        public void LoadSettings()
        {
            string path = Application.StartupPath;
            SetupConnectionCollection col = SettingLayer.LoadSetupConnectionCollection(path);
            bsConnections.DataSource = col;
        }

        /// <summary>
        /// Сохранить настройки соединений
        /// </summary>
        public void SaveSettings()
        {
            //prepare array
            SetupConnectionCollection cc = new SetupConnectionCollection();
            cc.Connections = new List<SetupConnection>();
            foreach (SetupConnection c in bsConnections.List)
            {
                cc.Connections.Add(c);
            }

            //save array
            string path = Application.StartupPath;
            SettingLayer.SaveSetupConnectionCollection(path, cc);
        }

        /// <summary>
        /// Загрузить список соединений и вернуть в виде коллекции
        /// </summary>
        /// <returns>Коллекция соединений</returns>
        public SetupConnectionCollection ConnectionCollectionGet()
        {
            LoadSettings();

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

            //string setupFile;
            //setupFile = Application.StartupPath + "\\" + settingsFileName;
            SaveSettings();
            DialogResult = DialogResult.Yes;

        }

        private void FormConnections_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }
    }
}
