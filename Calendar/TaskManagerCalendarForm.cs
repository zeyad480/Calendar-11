using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace TaskManagerApp
{
    /// <summary>
    /// Task Manager - Calendar screen.
    /// All controls live in TaskManagerCalendarForm.Designer.cs (visible/editable in the VS designer).
    /// This file only adds runtime polish: rounded corners, double buffering and nav selection.
    /// </summary>
    public partial class TaskManagerCalendarForm : Form
    {
        private static readonly Color SidebarColor = Color.FromArgb(17, 24, 39);
        private static readonly Color SidebarHover = Color.FromArgb(31, 41, 55);
        private static readonly Color MutedText = Color.FromArgb(156, 163, 175);
        private static readonly Color ActiveBlue = Color.FromArgb(37, 99, 235);
        private static readonly Color ActiveBlueDn = Color.FromArgb(29, 78, 216);

        private Button[] _navButtons;

        public TaskManagerCalendarForm()
        {
            InitializeComponent();

            EnableDoubleBuffer(this);
            EnableDoubleBuffer(pnlMain);
            EnableDoubleBuffer(pnlCalendarBorder);
            EnableDoubleBuffer(tblCalendar);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;   // designer keeps the plain square preview

            ApplyRoundedShapes();
            HookNavigation();
        }

        // ---------------------------------------------------------------- rounded shapes

        private void ApplyRoundedShapes()
        {
            foreach (Button b in _navButtons ?? BuildNavArray()) MakeRounded(b, 10);
            MakeRounded(pnlBadgeNotifications, 10);
            MakeRounded(pnlStatusDot, 10);
            MakeRounded(btnNewTask, 8);
            MakeRounded(lblDay05, 26);          // blue "today" circle

            foreach (Control c in tblCalendar.Controls)
                foreach (Control child in c.Controls)
                    if (child is Panel && child.Name.StartsWith("pnlChipDay", StringComparison.Ordinal))
                        MakeRounded(child, 6);
        }

        private static void MakeRounded(Control c, int radius)
        {
            Action apply = delegate
            {
                if (c.Width <= 0 || c.Height <= 0) return;
                using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, c.Width, c.Height), radius))
                {
                    Region old = c.Region;
                    c.Region = new Region(path);
                    if (old != null) old.Dispose();
                }
            };
            apply();
            c.SizeChanged += delegate { apply(); };
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = Math.Max(1, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ---------------------------------------------------------------- sidebar navigation

        private Button[] BuildNavArray()
        {
            _navButtons = new[]
            {
                btnNavDashboard, btnNavProjects, btnNavKanbanBoard,
                btnNavReports, btnNavNotifications, btnNavProfile
            };
            return _navButtons;
        }

        private void HookNavigation()
        {
            foreach (Button b in _navButtons ?? BuildNavArray())
                b.Click += NavButton_Click;
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            Button active = (Button)sender;
            foreach (Button b in _navButtons)
            {
                bool on = ReferenceEquals(b, active);
                b.BackColor = on ? ActiveBlue : SidebarColor;
                b.ForeColor = on ? Color.White : MutedText;
                b.Font = new Font(b.Font, on ? FontStyle.Bold : FontStyle.Regular);
                b.FlatAppearance.MouseOverBackColor = on ? ActiveBlue : SidebarHover;
                b.FlatAppearance.MouseDownBackColor = on ? ActiveBlueDn : SidebarHover;
            }
        }

        // ---------------------------------------------------------------- helpers

        private static void EnableDoubleBuffer(Control c)
        {
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(c, true, null);
        }
    }
}
