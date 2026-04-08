using System.Drawing;
using System.Windows.Forms;
namespace Exhibition
{
    public partial class FormDetails : Form
    {
        public FormDetails(object entity, string entityType)
        {
            InitializeComponent();
            DisplayDetails(entity, entityType);
        }
        private void DisplayDetails(object entity, string entityType)
        {
            panelDetails.Controls.Clear();
            int y = 10;
            foreach (var prop in entity.GetType().GetProperties())
            {
                string value = prop.GetValue(entity)?.ToString() ?? "null";
                Label label = new Label
                {
                    Text = $"{prop.Name}: {value}",
                    AutoSize = true,
                    Location = new Point(10, y)
                };
                panelDetails.Controls.Add(label);
                y += 25;
            }
        }
    }
}
