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
                        var json = File.ReadAllText(dialog.FileName);
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
            var root = new TreeNode("Exhibition") { Tag = "Exhibition" };
            var exhibitsNode = new TreeNode("Exhibits") { Tag = "Exhibits" };
            if (exhibition.Exhibits != null)
            {
                foreach (var ex in exhibition.Exhibits)
                {
                    var exNode = new TreeNode($"{ex.Title} ({ex.Year})")
                    {
                        Tag = $"Exhibit_{ex.Id}"
                    };
                    exhibitsNode.Nodes.Add(exNode);
                }
            }
            var eventsNode = new TreeNode("Events") { Tag = "Events" };
            if (exhibition.Events != null)
            {
                foreach (var ev in exhibition.Events)
                {
                    var evNode = new TreeNode($"{ev.Name} ({ev.Date:yyyy-MM-dd})")
                    {
                        Tag = $"Event_{ev.Id}"
                    };
                    eventsNode.Nodes.Add(evNode);
                }
            }
            var artistsNode = new TreeNode("Artists") { Tag = "Artists" };
            if (exhibition.Artists != null)
            {
                foreach (var a in exhibition.Artists)
                {
                    var artistNode = new TreeNode($"{a.Name} ({a.BirthYear}-{a.DeathYear})")
                    {
                        Tag = $"Artist_{a.Id}"
                    };
                    if (a.Artworks != null)
                    {
                        foreach (var art in a.Artworks)
                        {
                            var artNode = new TreeNode($"{art.Title} ({art.Year})")
                            {
                                Tag = $"Artwork_{art.Id}"
                            };
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
            if (e.Node.Tag == null) return;
            var tag = e.Node.Tag.ToString();
            currentEntityType = tag;
            if (tag.StartsWith("Exhibit_") || //начинается ли строка с указанного текста
                tag.StartsWith("Event_") ||
                tag.StartsWith("Artist_") ||
                tag.StartsWith("Artwork_"))
            {
                ShowSingleEntity(tag);
            }
            else
            {
                LoadDataToGrid();
            }
        }
        private void ShowSingleEntity(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            string[] parts = tag.Split('_'); // Разделяем Tag на тип и GUID
            if (parts.Length != 2) return;
            var type = parts[0];
            if (!Guid.TryParse(parts[1], out Guid id))
                return;
            object entity = null;
            switch (type)
            {
                case "Exhibit":
                    if (exhibition.Exhibits != null)
                    {
                        foreach (var ex in exhibition.Exhibits)
                        {
                            if (ex.Id == id)
                            {
                                entity = ex;
                                break;
                            }
                        }
                    }
                    break;
                case "Event":
                    if (exhibition.Events != null)
                    {
                        foreach (var ev in exhibition.Events)
                        {
                            if (ev.Id == id)
                            {
                                entity = ev;
                                break;
                            }
                        }
                    }
                    break;
                case "Artist":
                    if (exhibition.Artists != null)
                    {
                        foreach (var a in exhibition.Artists)
                        {
                            if (a.Id == id)
                            {
                                entity = a;
                                break;
                            }
                        }
                    }
                    break;
                case "Artwork":
                    if (exhibition.Artists != null)
                    {
                        foreach (var artist in exhibition.Artists)
                        {
                            if (artist.Artworks != null)
                            {
                                foreach (var artwork in artist.Artworks)
                                {
                                    if (artwork.Id == id)
                                    {
                                        entity = artwork;
                                        break; 
                                    }
                                }
                            }
                            if (entity != null) 
                                break; 
                        }
                    }
                    break;
            }
            if (entity != null)  // Если объект найден, показываем форму с деталями
            {
                FormDetails detailsForm = new FormDetails(entity, type);
                detailsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Объект не найден");
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
                    if (exhibition.Exhibits != null)
                    {
                        var exhibitList = exhibition.Exhibits
                            .Select(ex => new
                            {
                                ex.Id,
                                ex.Title,
                                ex.Type,
                                ex.Year,
                                ex.Material,
                                Hall = ex.ExhibitLocation != null ? ex.ExhibitLocation.Hall : "—",
                                Stand = ex.ExhibitLocation != null ? ex.ExhibitLocation.Stand.ToString() : "—"
                            })
                            .ToList();
                        dataGridViewInfo.DataSource = exhibitList;
                    }
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
                    MessageBox.Show("Выберите конкретный элемент");
                    return;  
            }
        }
        private void LoadInitialImage()
        {
            string[] paths = {"Data/Выставка.jpg","../../Data/Выставка.jpg","../../../Data/Выставка.jpg","../Data/Выставка.jpg","Выставка.jpg"};
            var found = false;
            foreach (var path in paths)
            {
                var fullPath = Path.GetFullPath(path);

                if (File.Exists(fullPath))
                {
                    try
                    {
                        if (pictureBox.Image != null)
                            pictureBox.Image.Dispose();

                        pictureBox.Image = Image.FromFile(fullPath);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        found = true;
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private void BtnShow_Click(object sender, EventArgs e)
        {
            if (dataGridViewInfo.CurrentRow == null) return;

            var cellValue = dataGridViewInfo.CurrentRow.Cells["Id"].Value;

            if (cellValue == null) return;

            if (!Guid.TryParse(cellValue.ToString(), out Guid id)) return;

            object entity = null;

            switch (currentEntityType)
            {
                case "Exhibits":
                    entity = exhibition.Exhibits?.FirstOrDefault(x => x.Id == id);
                    break;

                case "Events":
                    entity = exhibition.Events?.FirstOrDefault(x => x.Id == id);
                    break;

                case "Artists":
                    entity = exhibition.Artists?.FirstOrDefault(x => x.Id == id);
                    break;
            }

            if (entity != null)
            {
                FormDetails detailsForm = new FormDetails(entity, currentEntityType);
                detailsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Объект не найден");
            }
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExhibition));
            this.btnLoadXML = new System.Windows.Forms.Button();
            this.btnLoadJSON = new System.Windows.Forms.Button();
            this.btnShow = new System.Windows.Forms.Button();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.splitter = new System.Windows.Forms.Splitter();
            this.btnExit = new System.Windows.Forms.Button();
            this.treeViewExhibition = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadXML
            // 
            this.btnLoadXML.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnLoadXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadXML.Location = new System.Drawing.Point(336, 1);
            this.btnLoadXML.Name = "btnLoadXML";
            this.btnLoadXML.Size = new System.Drawing.Size(179, 42);
            this.btnLoadXML.TabIndex = 4;
            this.btnLoadXML.Text = "Загрузить XML";
            this.btnLoadXML.UseVisualStyleBackColor = false;
            // 
            // btnLoadJSON
            // 
            this.btnLoadJSON.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnLoadJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadJSON.Location = new System.Drawing.Point(336, 49);
            this.btnLoadJSON.Name = "btnLoadJSON";
            this.btnLoadJSON.Size = new System.Drawing.Size(181, 45);
            this.btnLoadJSON.TabIndex = 5;
            this.btnLoadJSON.Text = "Загрузить JSON";
            this.btnLoadJSON.UseVisualStyleBackColor = false;
            // 
            // btnShow
            // 
            this.btnShow.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnShow.Location = new System.Drawing.Point(521, 0);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(155, 41);
            this.btnShow.TabIndex = 6;
            this.btnShow.Text = "Показать ";
            this.btnShow.UseVisualStyleBackColor = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(310, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 510);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // pictureBox
            // 
            this.pictureBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox.InitialImage")));
            this.pictureBox.Location = new System.Drawing.Point(682, 1);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(344, 93);
            this.pictureBox.TabIndex = 9;
            this.pictureBox.TabStop = false;
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(310, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(23, 510);
            this.splitter.TabIndex = 10;
            this.splitter.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnExit.Location = new System.Drawing.Point(521, 49);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(155, 45);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Закрыть";
            this.btnExit.UseVisualStyleBackColor = false;
            // 
            // treeViewExhibition
            // 
            this.treeViewExhibition.Cursor = System.Windows.Forms.Cursors.No;
            this.treeViewExhibition.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeViewExhibition.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeViewExhibition.Location = new System.Drawing.Point(0, 0);
            this.treeViewExhibition.Name = "treeViewExhibition";
            this.treeViewExhibition.Size = new System.Drawing.Size(310, 510);
            this.treeViewExhibition.TabIndex = 1;
            // 
            // FormExhibition
            // 
            this.ClientSize = new System.Drawing.Size(1038, 510);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.btnLoadJSON);
            this.Controls.Add(this.btnLoadXML);
            this.Controls.Add(this.treeViewExhibition);
            this.Name = "FormExhibition";
            this.Text = "Выставка";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }
    }
}  
