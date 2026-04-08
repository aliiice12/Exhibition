using Exhibition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
namespace ProjectExhibition
{
    public partial class FormExhibition : Form
    {
        private DataGridView dataGridViewInfo;
        private Exhibition exhibition;
        private string currentEntityType;
        private ComponentResourceManager resources;
        public FormExhibition()
        {
            InitializeComponent();
            LoadInitialImage();
            dataGridViewInfo = new DataGridView
            {
                Location = new Point(320, 100),
                Size = new Size(650, 700),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(dataGridViewInfo);
            btnLoadXML.Click += BtnLoadXML_Click;
            btnLoadJSON.Click += BtnLoadJSON_Click;
            btnShow.Click += BtnShow_Click;
            btnExit.Click += BtnExit_Click;
            treeViewExhibition.AfterSelect += treeViewExhibition_AfterSelect;
            resources = new ComponentResourceManager(typeof(FormExhibition));
        }
        public FormExhibition(Exhibition exhibition) : this() 
        {
            this.exhibition = exhibition;
            FillTree();
        }
        private void BtnLoadXML_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "XML files (*.xml)|*.xml";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Exhibition));
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open))
                        {
                            exhibition = (Exhibition)serializer.Deserialize(fs);
                        }
                        MessageBox.Show("XML загружен успешно");
                        FillTree();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при загрузке XML");
                    }
                }
            }
        }
        private void BtnLoadJSON_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "JSON files (*.json)|*.json";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(dialog.FileName);
                        var root = JsonConvert.DeserializeObject<Root>(json); 
                        exhibition = root.Exhibition;                         
                        MessageBox.Show("JSON успешно загружен");
                        FillTree();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при загрузке JSON");
                    }
                }
            }
        }
        private void FillTree()
        {
            treeViewExhibition.Nodes.Clear();
            if (exhibition == null) return;
            TreeNode root = new TreeNode("Exhibition") { Tag = "Exhibition" };
            // Exhibits
            TreeNode exhibitsNode = new TreeNode("Exhibits") { Tag = "Exhibits" };
            if (exhibition.Exhibits != null)
            {
                foreach (var ex in exhibition.Exhibits)
                {
                    TreeNode exNode = new TreeNode($"{ex.Title} ({ex.Year})") { Tag = "Exhibit_" + ex.Id };
                    exhibitsNode.Nodes.Add(exNode);
                }
            }
            // Events
            TreeNode eventsNode = new TreeNode("Events") { Tag = "Events" };
            if (exhibition.Events != null)
            {
                foreach (var ev in exhibition.Events)
                {
                    TreeNode evNode = new TreeNode($"{ev.Name} ({ev.Date:yyyy-MM-dd})") { Tag = "Event_" + ev.Id };
                    eventsNode.Nodes.Add(evNode);
                }
            }
            // Artists
            TreeNode artistsNode = new TreeNode("Artists") { Tag = "Artists" };
            if (exhibition.Artists != null)
            {
                foreach (var a in exhibition.Artists)
                {
                    TreeNode artistNode = new TreeNode($"{a.Name} ({a.BirthYear}-{a.DeathYear})") { Tag = "Artist_" + a.Id };

                    // Artworks
                    if (a.Artworks != null && a.Artworks.Any())
                    {
                        foreach (var art in a.Artworks)
                        {
                            TreeNode artNode = new TreeNode($"{art.Title} ({art.Year})") { Tag = "Artwork_" + art.Id };
                            artistNode.Nodes.Add(artNode);
                        }
                    }
                    artistsNode.Nodes.Add(artistNode);
                }
            }
            root.Nodes.Add(exhibitsNode);
            root.Nodes.Add(eventsNode);
            root.Nodes.Add(artistsNode);
            treeViewExhibition.Nodes.Add(root);
            treeViewExhibition.ExpandAll();
        }
        private void treeViewExhibition_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                currentEntityType = e.Node.Tag.ToString();
                LoadDataToGrid();
            }
        }
        private void LoadDataToGrid()
        {
            if (exhibition == null || string.IsNullOrEmpty(currentEntityType))
                return;
            switch (currentEntityType)
            {
                case "Exhibition":
                    var allData = new List<object>();
                    if (exhibition.Exhibits != null)
                    {
                        allData.AddRange(exhibition.Exhibits.Select(ex => new
                        {
                            Type = "Exhibit",
                            ex.Id,
                            Name = ex.Title,
                            Info = $"{ex.Type}, {ex.Year}",
                            Extra = ex.ExhibitLocation != null
                                ? $"Hall: {ex.ExhibitLocation.Hall}, Stand: {ex.ExhibitLocation.Stand}"
                                : "—"
                        }));
                    }
                    if (exhibition.Events != null)
                    {
                        allData.AddRange(exhibition.Events.Select(ev => new
                        {
                            Type = "Event",
                            ev.Id,
                            Name = ev.Name,
                            Info = ev.Date.ToString("yyyy-MM-dd"),
                            Extra = ev.Organizer?.Organization ?? "—"
                        }));
                    }
                    if (exhibition.Artists != null)
                    {
                        allData.AddRange(exhibition.Artists.Select(a => new
                        {
                            Type = "Artist",
                            a.Id,
                            Name = a.Name,
                            Info = $"{a.BirthYear}-{a.DeathYear}",
                            Extra = a.Country
                        }));
                    }
                    dataGridViewInfo.DataSource = allData;
                    break;
                case "Exhibits":
                    var exhibitList = exhibition.Exhibits.Select(ex => new
                    {
                        ex.Id,
                        ex.Title,
                        ex.Type,
                        ex.Year,
                        ex.Material,
                        Hall = ex.ExhibitLocation != null ? ex.ExhibitLocation.Hall : "—",
                        Stand = ex.ExhibitLocation != null ? ex.ExhibitLocation.Stand.ToString() : "—"
                    }).ToList();
                    dataGridViewInfo.DataSource = exhibitList;
                    break;
                case "Events":
                    var eventList = exhibition.Events.Select(ev => new
                    {
                        ev.Id,
                        ev.Name,
                        Date = ev.Date.ToString("yyyy-MM-dd"),
                        ev.City,
                        Venue = ev.Location?.Venue ?? "",
                        Hall = ev.Location?.Hall ?? "",
                        Organizer = ev.Organizer?.Organization ?? "",
                        Visitors = ev.Visitors?.ExpectedNumber ?? 0
                    }).ToList();
                    dataGridViewInfo.DataSource = eventList;
                    break;

                case "Artists":
                    var artistList = exhibition.Artists.Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.Country,
                        a.BirthYear,
                        a.DeathYear,
                        Artworks = (a.Artworks != null && a.Artworks.Any())
                            ? string.Join(", ", a.Artworks.Select(x => x.Title))
                            : ""
                    }).ToList();
                    dataGridViewInfo.DataSource = artistList;
                    break;
                case null:
                default:
                    dataGridViewInfo.DataSource = null;
                    break;
            }
        }
        private void LoadInitialImage()
        {
            var imagePath = @"C:\Users\ПК\source\repos\Exhibition\Exhibition\Data\Выставка.jpg"; 
            if (File.Exists(imagePath))
            {
                if (pictureBox.Image != null)
                    pictureBox.Image.Dispose();
                pictureBox.Image = Image.FromFile(imagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                MessageBox.Show("Файл изображения не найден");
            }
        }
        private void BtnShow_Click(object sender, EventArgs e)
        {
            if (dataGridViewInfo.CurrentRow == null) return;
            object entity = null;
            string cellValue = dataGridViewInfo.CurrentRow.Cells["Id"].Value?.ToString();
            if (string.IsNullOrEmpty(cellValue)) return;
            if (!Guid.TryParse(cellValue, out Guid id)) return;
            switch (currentEntityType)
            {
                case "Exhibits":
                    entity = exhibition.Exhibits?.FirstOrDefault(x => Guid.Parse(x.Id) == id);
                    break;
                case "Events":
                    entity = exhibition.Events?.FirstOrDefault(x => Guid.Parse(x.Id) == id);
                    break;
                case "Artists":
                    entity = exhibition.Artists?.FirstOrDefault(x => Guid.Parse(x.Id) == id);
                    break;
            }
            if (entity != null)
            {
                FormDetails detailsForm = new FormDetails(entity, currentEntityType);
                detailsForm.ShowDialog();
            }
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FormExhibition));
            btnLoadXML = new Button();
            btnLoadJSON = new Button();
            btnShow = new Button();
            splitter2 = new Splitter();
            pictureBox = new PictureBox();
            splitter = new Splitter();
            btnExit = new Button();
            treeViewExhibition = new TreeView();
            ((ISupportInitialize)(this.pictureBox)).BeginInit();
            SuspendLayout();
            // 
            // btnLoadXML
            // 
            this.btnLoadXML.BackColor =SystemColors.ActiveCaption;
            this.btnLoadXML.Font = new Font("Microsoft Sans Serif", 12F,FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadXML.Location = new Point(336, 1);
            this.btnLoadXML.Name = "btnLoadXML";
            this.btnLoadXML.Size = new Size(179, 42);
            this.btnLoadXML.TabIndex = 4;
            this.btnLoadXML.Text = "Загрузить XML";
            this.btnLoadXML.UseVisualStyleBackColor = false;
            // 
            // btnLoadJSON
            // 
            this.btnLoadJSON.BackColor = SystemColors.ActiveCaption;
            this.btnLoadJSON.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular,GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadJSON.Location = new Point(336, 49);
            this.btnLoadJSON.Name = "btnLoadJSON";
            this.btnLoadJSON.Size = new Size(181, 45);
            this.btnLoadJSON.TabIndex = 5;
            this.btnLoadJSON.Text = "Загрузить JSON";
            this.btnLoadJSON.UseVisualStyleBackColor = false;
            // 
            // btnShow
            // 
            this.btnShow.Anchor = AnchorStyles.Top;
            this.btnShow.BackColor = SystemColors.ActiveCaption;
            this.btnShow.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.btnShow.Location = new Point(521, 0);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new Size(155, 41);
            this.btnShow.TabIndex = 6;
            this.btnShow.Text = "Показать ";
            this.btnShow.UseVisualStyleBackColor = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new Point(310, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new Size(3, 510);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = AnchorStyles.Right;
            this.pictureBox.InitialImage = ((Image)(resources.GetObject("pictureBox.InitialImage")));
            this.pictureBox.Location = new Point(682, 1);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new Size(344, 93);
            this.pictureBox.TabIndex = 9;
            this.pictureBox.TabStop = false;
            // 
            // splitter
            // 
            this.splitter.Location = new Point(313, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new Size(23, 510);
            this.splitter.TabIndex = 10;
            this.splitter.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = SystemColors.ActiveCaption;
            this.btnExit.Font = new Font("Microsoft Sans Serif", 12F,FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.ForeColor = SystemColors.ControlText;
            this.btnExit.Location = new Point(521, 49);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(155, 45);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Закрыть";
            this.btnExit.UseVisualStyleBackColor = false;
            // 
            // treeViewExhibition
            // 
            this.treeViewExhibition.Cursor = Cursors.No;
            this.treeViewExhibition.Dock = DockStyle.Left;
            this.treeViewExhibition.Font = new Font("Microsoft Sans Serif", 16.2F, FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeViewExhibition.Location = new Point(0, 0);
            this.treeViewExhibition.Name = "treeViewExhibition";
            this.treeViewExhibition.Size = new Size(310, 510);
            this.treeViewExhibition.TabIndex = 1;
            // 
            // FormExhibition
            // 
            ClientSize = new Size(1038, 510);
            Controls.Add(btnExit);
            Controls.Add(pictureBox);
            Controls.Add(splitter);
            Controls.Add(btnShow);
            Controls.Add(btnLoadJSON);
            Controls.Add(btnLoadXML);
            Controls.Add(treeViewExhibition);
            Name = "FormExhibition";
            Text = "Выставка";
            ((ISupportInitialize)(this.pictureBox)).EndInit();
            ResumeLayout(false);
        }
    }
}  
