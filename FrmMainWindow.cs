﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.Common;
using System.Diagnostics;
using FastColoredTextBoxNS;
using dbShowDepends.Data;
using dbShowDepends.Settings;

namespace dbShowDepends
{
    public partial class FrmMainWindow : Form
    {
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        private DbLayer _dbLayer;
        private SetupConnectionCollection connectionCollection;

        // БД выбирается из настройки
        private DbLayer getDbLayer()
        {
            return getDbLayer(tscbDatabaseName.Text);
        }

        // БД выбирается из названия объекта
        private DbLayer getDbLayer(string DbName)
        {
            SetupConnection cn = (SetupConnection)cbConnection.SelectedItem;
            string cnStr = cn.ServerName;
            if (string.IsNullOrWhiteSpace(DbName))
                DbName = cn.DbName;
            bool isAnalyzeBadObjects = cn.IsAnalyzeBadObjects;

            Console.WriteLine($"ServerName={cnStr}, DbName = {DbName}");

            prepareDbLayer(cnStr, DbName);
            testCurrentDbName(DbName, isAnalyzeBadObjects);
            return _dbLayer;
        }


        // Очистить существующее соединение
        private void resetDbLayer()
        {
            _dbLayer = null;
        }

        // Подготовка dblayer для использования
        private void prepareDbLayer(string serverName, string dbName)
        {
            // создать единственный экземпляр
            if (_dbLayer == null)
            {
                Console.WriteLine($"new DbLayer: Server={serverName}, dbName={dbName}");
                var dbParams = new DbParams(serverName, true, "", "", dbName);

                _dbLayer = new DbLayer(dbParams);
            }

            Debug.Assert(_dbLayer != null, "_dbLayer != null");
        }

        // при необходимости изменить текущую базу данных
        private void testCurrentDbName(string targetDbName, bool isPrepareTempTable)
        {
            if (targetDbName.Length == 0)
                return;

            Debug.Assert(_dbLayer != null, "_dbLayer != null");

            var curDbName = _dbLayer.GetCurrentDbName();
            if (0 != string.Compare(curDbName, targetDbName, true))
            {
                _dbLayer.ChangeDb(targetDbName);
            }


            // подготовить рабочую таблицу
            if (isPrepareTempTable)
            {
                _dbLayer.PrepareTempTable();
                toolStripStatusLabel1.Text = string.Format("Temp table for db {0} prepared", targetDbName);
            }
            else
            {
                toolStripStatusLabel1.Text = string.Format("Changed to db {0}", targetDbName);
            }
        }

        public FrmMainWindow()
        {
            InitializeComponent();
            bsConnections.DataSource = connectionCollection; //typeof(DbConnectionCollection);
            bsConnections.DataMember = "Connections";

            //cbConnection.ComboBox.DataSource = bsConnections;
            //cbConnection.ComboBox.DisplayMember = "ConnectionName";
            //cbConnection.ComboBox.Width *= 2;

            init_fctbSrcCode();
            init_imageList();
        }

        private void FrmMainWindow_Load(object sender, EventArgs e)
        {
            LoadBsConnections();

            var dbParams = SettingLayer.LoadDefaultParams();
            cbConnection.Text = dbParams.ServerName;
            tscbDatabaseName.Text = dbParams.DbName;

        }

        private void init_imageList()
        {
            //var img = objectTreeViewImageList.Images[0];
            //var imgSeleted = objectTreeViewImageList.Images[1];

            objectTreeViewImageList.Images.Clear();
            var colors = new List<Pen>();
            colors.Add(Pens.Black);
            colors.Add(Pens.Red);

            var objTypes = DbObjectTypes.GetObjectTypes();

            // по типам объектов
            foreach (var objType in objTypes)
            {
                //по цветам
                foreach(var penColor in colors)
                {
                    var b = new Bitmap(16, 16);
                    Graphics g = Graphics.FromImage(b);

                    Rectangle displayRectangle = new Rectangle(new Point(1, 1), new Size(14, 14));
                    Point p = new Point(8, 2);
                    StringFormat format1 = new StringFormat(StringFormatFlags.NoClip)
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Center
                    };
                    Font fnt1 = new Font("Terminal", 9, FontStyle.Regular, GraphicsUnit.Pixel);

                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    g.DrawRectangle(penColor, displayRectangle);

                    string s = string.Format("{0}", objType.ToString());
                    g.DrawString(s, fnt1, Brushes.Black, p, format1);

                    objectTreeViewImageList.Images.Add(b);
                }
            }
                
        }

        private void cbConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dbLayer != null)
            {
                _dbLayer.Disconnect();
                resetDbLayer();
            }

            tscbDatabaseName.Items.Clear();
            SetupConnection cn = (SetupConnection)cbConnection.SelectedItem;
            if (cn != null)
                tscbDatabaseName.Text = cn.DbName;
            else
                tscbDatabaseName.Text = "";

            int i = cbConnection.SelectedIndex;
            toolStripStatusLabel1.Text = $"Selected index changed: new index = {i} at {DateTime.Now}. DBName = {tscbDatabaseName.Text}";
        }

        // Обновить список доступных баз данных
        private void tscbDatabaseName_DropDown(object sender, EventArgs e)
        {
            var dbList = new List<string>();

            tscbDatabaseName.Items.Clear();

            try
            {
                // Для получения списка БД достаточно использовать master
                List<DbDataRecord> res = getDbLayer("master").GetDatabaseList();

                foreach (var r in res)
                {
                    tscbDatabaseName.Items.Add(r["name"].ToString());
                }
            }
            catch (Exception ex)
            {
                tscbDatabaseName.Items.Add("<error>");
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        // Начать поиск объектов
        private void tsbFindObject_Click(object sender, EventArgs e)
        {
            startSearch(tstbObjectName.Text);
        }

        // Поиск дочерних элементов текущего узла дерева
        private void treeObj_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                string fullObjName = e.Node.Text;
                string dbName = "", objName = "";
                parseObjectName(fullObjName, ref dbName, ref objName);

                if (string.IsNullOrWhiteSpace(dbName))
                    dbName = tscbDatabaseName.Text;

                var recs = getDbLayer(dbName).GetReferencedObjects(objName);
                e.Node.Nodes.Clear();
                foreach (var rc in recs)
                {
                    string dbNameChild = rc["RefDbName"].ToString();
                    string str = rc["RefName"].ToString();
                    if (!string.IsNullOrEmpty(dbNameChild))
                    {
                        str = dbNameChild + '.' + str;
                    }
                    var tn = e.Node.Nodes.Add(str);
                    tn.Nodes.Add("to do");
                    tn.ImageIndex = getTreeObjColorIndex(rc["Type"].ToString(), false);
                    tn.SelectedImageIndex = getTreeObjColorIndex(rc["Type"].ToString(), true);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "At AfterExpand");
            }
        }

        // Отобразить текст выделенного объекта
        private void treeObj_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                string fullObjName = e.Node.Text;
                string dbName = "", objName = "", objType = "";
                parseObjectName(fullObjName, ref dbName, ref objName);

                if (string.IsNullOrWhiteSpace(dbName))
                    dbName = tscbDatabaseName.Text;
                
                var src = getDbLayer(dbName).GetObjectSource(objName, ref objType);
                
                fctbSrcCode.Clear();
                fctbSrcCode.Text = src;

                // обновить иконку объекта после считывания дополнительной информации
                if (!string.IsNullOrEmpty(objType))
                {
                    e.Node.ImageIndex = getTreeObjColorIndex(objType, false);
                    e.Node.SelectedImageIndex = getTreeObjColorIndex(objType, true);
                }

                toolStripStatusLabel1.Text = "Db: " + dbName + ", object: " + objName + ", fullName: " + fullObjName;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "At AfterSelect");
            }
        }

        private void init_fctbSrcCode()
        {
            //set language
            fctbSrcCode.ClearStylesBuffer();
            fctbSrcCode.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            //

            fctbSrcCode.Language = Language.SQL;
            fctbSrcCode.OnSyntaxHighlight(new TextChangedEventArgs(fctbSrcCode.Range));
        }

        private void fctbSrcCode_SelectionChangedDelayed(object sender, EventArgs e)
        {
            fctbSrcCode.VisibleRange.ClearStyle(SameWordsStyle);
            if (!fctbSrcCode.Selection.IsEmpty)
                return;//user selected diapason

            //get fragment around caret
            var fragment = fctbSrcCode.Selection.GetFragment(@"[@\[\]\.\w]");
            string text = fragment.Text;
            text = text.Replace("[", @"\[");
            text = text.Replace("]", @"\]");
            text = text.Replace(".", @"\.");

            if (text.Length == 0)
                return;
            
            //highlight same words
            if (text[0] == '@')
                // Для переменной ищем переменные
                text = text + @"\b";
            else
                // Для обычного текста исключаем переменные
                text = @"(?<!@)\b" + text + @"\b";

            //toolStripStatusLabel1.Text = (text.Length == 0 ? "No selection" : "Selected [" + text + "]");

            var ranges = fctbSrcCode.VisibleRange.GetRanges(text).ToArray();
            if(ranges.Length>1)
            foreach(var r in ranges)
                r.SetStyle(SameWordsStyle);
        }

        private int getTreeObjColorIndex(string objType, bool isSelected)
        {
            int typeIndex;

            if (string.IsNullOrEmpty(objType))
            {
                // 0 - тип объекта не определён
                typeIndex = 0;
            }
            else
            {
                typeIndex = DbObjectTypes.GetObjectTypeIndex(objType.Trim());

                if (typeIndex < 0)
                    typeIndex = 0;
            }

            return typeIndex * 2 + (isSelected ? 1 : 0);
        }

        private void startSearch(string searchString)
        {
            var dbList = new List<string>();

            try
            {
                treeObj.Nodes.Clear();
                var objTypes = new List<DbObjectType>();
                objTypes.Add(DbObjectType.P);
                objTypes.Add(DbObjectType.V);
                objTypes.Add(DbObjectType.FN);
                objTypes.Add(DbObjectType.IF);
                objTypes.Add(DbObjectType.TF);

                var res = getDbLayer().GetObjectList(searchString, objTypes);

                foreach (var r in res)
                {
                    var tn = treeObj.Nodes.Add(r["FullName"].ToString());
                    tn.Nodes.Add("to do");
                    tn.ImageIndex = getTreeObjColorIndex(r["Type"].ToString(), false);
                    tn.SelectedImageIndex = getTreeObjColorIndex(r["Type"].ToString(), true);

                }

                // сохранить настройки соединения
                var dbParams = new DbParams(cbConnection.Text, true, "", "", tscbDatabaseName.Text);
                SettingLayer.SaveDefaultParams(dbParams);

                // подготовить рабочую таблицу
                //dbLayer.PrepareTempTable(); - перенесено в prepareCurrentDbName
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Search for objects");
            }
        }

        private void tstbObjectName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                startSearch(tstbObjectName.Text);
            }
        }

        private void tsbClearSearchText_Click(object sender, EventArgs e)
        {
            tstbObjectName.Clear();
        }

        /// <summary>
        /// Разбор имени объекта
        /// </summary>
        /// <param name="fullObjName">Полное имя в формате dbname.schema.objname</param>
        /// <param name="dbName">Имя БД или пустая строка</param>
        /// <param name="objName">Схема и имя объекта</param>
        private void parseObjectName(string fullObjName, ref string dbName, ref string objName)
        {
            string[] parts = fullObjName.Split('.');
            if (parts.Count() == 3)
            {
                dbName = parts[0];
                objName = parts[1] + "." + parts[2];
            }
            else
            {
                dbName = "";
                objName = fullObjName;
            }
        }

        private void setupConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res;

            var f = new FormConnections();
            res = f.ShowDialog(this);
            if (res == DialogResult.Yes)
            {
                LoadBsConnections();
                resetDbLayer();
                toolStripStatusLabel1.Text = "Setup file refreshed";
            }
        }

        private void LoadBsConnections()
        {
            var f = new FormConnections();
            connectionCollection = f.ConnectionCollectionGet();


            cbConnection.ComboBox.DataSource = null;
            cbConnection.ComboBox.DisplayMember = null;
            bsConnections.DataSource = null;
            bsConnections.DataSource = connectionCollection;
            bsConnections.DataMember = "connections";

            
            cbConnection.ComboBox.DataSource = bsConnections;
            cbConnection.ComboBox.DisplayMember = "ConnectionName";
        }

    }
}