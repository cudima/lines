using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lines
{

  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public class View : Window
  {
    int m_Rows, m_Cols;

    Border[,] m_Borders;
    Rectangle[,] m_Rectangles;

    Border m_MouseDownBorder;

    const double m_CellWidth = 60;
    const double m_CellHeight = 60;

    SolidColorBrush m_brush;
    ColorAnimation m_ca;

    Point[] m_movePath;
    Point[] m_disappearPath;
    ushort m_moveColor;

    private SolidColorBrush[] m_ColorBrushes = {
                                               Brushes.Snow,

                                               // Brushes.Orchid,
                                               Brushes.MediumPurple,
                                               Brushes.CornflowerBlue,
                                               Brushes.MediumTurquoise,
                                               Brushes.MediumSeaGreen,
                                               // Brushes.LightSkyBlue,
                                               Brushes.Orange,
                                               Brushes.IndianRed,
                                               Brushes.LightPink
                                         };

    private Style m_ColdBorderStyle;
    private Style m_HotBorderStyle;

    private Grid setupMainGrid()
    {
      m_Borders = new Border[m_Rows, m_Cols];
      m_Rectangles = new Rectangle[m_Rows, m_Cols];

      Grid mainGrid = new Grid();

      mainGrid.Width = m_Rows * m_CellWidth + m_Rows * 2;
      mainGrid.Height = m_Cols * m_CellHeight + m_Cols * 2;

      mainGrid.Background = m_ColorBrushes[0];
      // this.AddChild(mainGrid);

      // mainGrid.ShowGridLines = true;
      for (int i = 0; i < m_Rows; i++)
        mainGrid.RowDefinitions.Add(new RowDefinition());

      for (int i = 0; i < m_Cols; i++)
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

      initStyles();

      for (int i = 0; i < m_Rows; i++)
      {
        for (int j = 0; j < m_Cols; j++)
        {
          Border b = new Border();
          b.Style = m_ColdBorderStyle;
          b.MouseUp += mainGrid_MouseUp;
          b.MouseDown += mainGrid_MouseDown;

          Rectangle r = new Rectangle();
          // r.Fill = Brushes.Chocolate;
          // r.Opacity = .5;
          r.Fill = m_ColorBrushes[0];

          b.Child = r;

          Grid.SetRow(b, i);
          Grid.SetColumn(b, j);
          Grid.SetZIndex(b, 5);

          mainGrid.Children.Add(b);
          m_Borders[i, j] = b;
          m_Rectangles[i, j] = r;
        }
      }

      return mainGrid;
    }

    private Menu setupMainMenu()
    {
      Menu menu = new Menu();
      MenuItem mi = new MenuItem();
      mi.Header = "_Debug";
      menu.Items.Add(mi);
      return menu;
    }

    public View(int numRows, int numCols, int numColors)
    {
      m_Rows = numRows;
      m_Cols = numCols;

      Title = "Square Lines";
      Width = Height = 650;

      DockPanel panel = new DockPanel();

      Menu mainMenu = setupMainMenu();
      DockPanel.SetDock(mainMenu, Dock.Top);
      panel.Children.Add(mainMenu);

      Grid mainGrid = setupMainGrid();
      panel.Children.Add(mainGrid);

      this.AddChild(panel);

      // game = Game.Instance;

      // this.Show();
    }

    public void Place(Point point, ushort color)
    {
      Rectangle r = m_Rectangles[point.Row, point.Col];
      r.Fill = m_ColorBrushes[color];
    }

    public void Select(Point point, bool on)
    {
      Border b = m_Borders[point.Row, point.Col];
      if (on)
      {
        // b.Style = m_HotBorderStyle;
        b.Background = m_brush;
        // b.BorderBrush = Brushes.LightPink;
        m_brush.BeginAnimation(SolidColorBrush.ColorProperty, m_ca);
      }
      else
      {
        m_brush.BeginAnimation(SolidColorBrush.ColorProperty, null);
        // b.BorderBrush = Brushes.LightSteelBlue;
        // b.Style = m_ColdBorderStyle;
        b.Background = m_ColorBrushes[0];
      }
    }

    public delegate void MoveCompletedHandler(Point start, Point end, ushort c);
    public event MoveCompletedHandler OnMoveCompleted;

    private void moveAnimation_OnCompleted(object sender, EventArgs e)
    {
      foreach (Point p in m_movePath)
      {
        if (m_movePath.First() == p)
          m_Rectangles[p.Row, p.Col].Fill = m_ColorBrushes[0];
        else if (m_movePath.Last() == p)
          m_Rectangles[p.Row, p.Col].Fill = m_ColorBrushes[m_moveColor];
        else
        {
          m_Borders[p.Row, p.Col].Padding = new Thickness(10);
          m_Rectangles[p.Row, p.Col].Fill = m_ColorBrushes[0];
        }
      }

      Place(m_movePath.First(), 0);
      Place(m_movePath.Last(), m_moveColor);

      OnMoveCompleted(m_movePath.First(), m_movePath.Last(), m_moveColor);
      m_movePath = null;
    }

    public void Move(Point[] path, ushort color)
    {
      m_movePath = path;
      m_moveColor = color;

      m_brush.BeginAnimation(SolidColorBrush.ColorProperty, null);

      // intermediate animation
      // starts immediately. fades in and out
      ColorAnimation ca1 = new ColorAnimation();
      ca1.From = m_ColorBrushes[0].Color;
      // ca1.From = Brushes.LightSteelBlue.Color;
      ca1.To = m_ColorBrushes[m_moveColor].Color;
      // ca1.To = Brushes.Red.Color;
      ca1.Duration = new Duration(TimeSpan.FromMilliseconds(500));
      ca1.BeginTime = TimeSpan.FromMilliseconds(0);
      ca1.AutoReverse = true;

      ColorAnimation ca2 = new ColorAnimation();
      ca2.From = m_ColorBrushes[m_moveColor].Color;
      ca2.To = m_ColorBrushes[0].Color;
      ca2.Duration = new Duration(TimeSpan.FromMilliseconds(500));
      ca2.BeginTime = ca1.BeginTime + TimeSpan.FromMilliseconds(500);

      ColorAnimation ca3 = new ColorAnimation();
      ca3.From = m_ColorBrushes[0].Color;
      ca3.To = m_ColorBrushes[m_moveColor].Color;
      ca3.Duration = new Duration(TimeSpan.FromMilliseconds(250));
      ca1.BeginTime = ca1.BeginTime;


      SolidColorBrush br1 = new SolidColorBrush();
      SolidColorBrush br2 = new SolidColorBrush();
      br2.Color = m_ColorBrushes[m_moveColor].Color;
      SolidColorBrush br3 = new SolidColorBrush();

      foreach(Point p in m_movePath) {
        if (p == m_movePath.First())
          m_Rectangles[p.Row, p.Col].Fill = br2;
        else if (p == m_movePath.Last())
          m_Rectangles[p.Row, p.Col].Fill = br3;
        else
        {
          // Border b = m_Borders[p.Row, p.Col];
          // b.BorderBrush = br1;
          // b.BorderThickness = new Thickness(2);
          m_Borders[p.Row, p.Col].Padding = new Thickness(20);
          m_Rectangles[p.Row, p.Col].Fill = br1;
        }
      }

      /*
      Point last = m_movePath.Last();
      m_Rectangles[last.Row, last.Col].Fill = br2;
      */

      // deselect the starting point
      Select(m_movePath.First(), false);

      ca1.Completed += moveAnimation_OnCompleted;

      br1.BeginAnimation(SolidColorBrush.ColorProperty, ca1);
      br2.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
      br3.BeginAnimation(SolidColorBrush.ColorProperty, ca3);
    }

    private void disappearAnimation_OnCompleted(object sender, EventArgs e)
    {
      foreach (Point p in m_disappearPath)
        Place(p, 0);
    }

    public void Disappear(Point[] lines)
    {
      m_disappearPath = lines;

      var linesByBrush = new List<Point>[m_ColorBrushes.Length];
      foreach (Point p in lines)
      {
        SolidColorBrush b = m_Rectangles[p.Row, p.Col].Fill as SolidColorBrush;
        for (int i = 1; i < m_ColorBrushes.Length; i++)
        {
          if (b == m_ColorBrushes[i])
          {
            if (linesByBrush[i] == null)
              linesByBrush[i] = new List<Point>();
            linesByBrush[i].Add(p);
            break;
          }
        }
      }

      bool first = true;
      for (int i = 1; i < linesByBrush.Length; i++)
      {
        if (linesByBrush[i] == null)
          continue;

        ColorAnimation ca = new ColorAnimation();
        ca.From = m_ColorBrushes[i].Color;
        ca.To = m_ColorBrushes[0].Color;
        ca.Duration = new Duration(TimeSpan.FromMilliseconds(500));
        // ca1.From = Brushes.LightSteelBlue.Color;
        // ca1.To = Brushes.Red.Color;

        SolidColorBrush br = new SolidColorBrush();
        br.Color = m_ColorBrushes[i].Color;

        foreach (Point p in linesByBrush[i])
          m_Rectangles[p.Row, p.Col].Fill = br;

        if (first)
        {
          ca.Completed += disappearAnimation_OnCompleted;
          first = false;
        }

        br.BeginAnimation(SolidColorBrush.ColorProperty, ca);
      }
    }

    public delegate void ViewEventHandler(object sender, Point p);
    public event ViewEventHandler OnClick;

    private void mainGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
      // MessageBox.Show(e.OriginalSource.GetType().ToString());
      try
      {
        Border b = sender as Border;
        if (b == m_MouseDownBorder)
        {
          // Border b = LogicalTreeHelper.GetParent(r) as Border;
          // b.Padding = new Thickness(20);
          // r.Fill = Brushes.Fuchsia;

          Point p = new Point((ushort)Grid.GetRow(b), (ushort)Grid.GetColumn(b));
          OnClick(this, p);
        }

        m_MouseDownBorder = null;
      }
      catch (System.InvalidCastException)
      {
      }
    }

    private void mainGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
      m_MouseDownBorder = sender as Border;
    }

    private void initStyles()
    {

      /*
      b.Background = Brushes.Azure;
      b.Background = Brushes.GhostWhite;
      b.BorderBrush = Brushes.Gray;
      b.BorderThickness = new Thickness(1);
      b.Padding = new Thickness(10);
      b.Width = mCellWidth;
      b.Height = mCellHeight;
      b.SnapsToDevicePixels = true;
      b.Margin = new Thickness(1);
      */

      m_ColdBorderStyle = new Style();

      m_ColdBorderStyle.Setters.Add(new Setter(Border.BackgroundProperty, m_ColorBrushes[0]));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.BorderBrushProperty, Brushes.LightSteelBlue));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1)));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.SnapsToDevicePixelsProperty, true));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.WidthProperty, m_CellWidth));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.HeightProperty, m_CellHeight));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10)));
      m_ColdBorderStyle.Setters.Add(new Setter(Border.MarginProperty, new Thickness(1)));

      m_HotBorderStyle = new Style(typeof(Border), m_ColdBorderStyle);
      m_HotBorderStyle.Setters.Add(new Setter(BackgroundProperty, Brushes.LavenderBlush));
      m_HotBorderStyle.Setters.Add(new Setter(BorderBrushProperty, Brushes.LightPink));

      m_ca = new ColorAnimation();
      m_ca.From = Colors.GhostWhite;
      m_ca.To = Colors.LightGray;
      m_ca.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
      m_ca.AutoReverse = true;
      m_ca.RepeatBehavior = RepeatBehavior.Forever;

      m_brush = new SolidColorBrush();
      m_brush.Color = Colors.LavenderBlush;

      Trigger trigger = new Trigger();
      trigger.Property = Border.IsMouseOverProperty;
      trigger.Value = true;
      trigger.Setters.Add(new Setter(BorderBrushProperty, Brushes.Red));

      m_ColdBorderStyle.Triggers.Add(trigger);
    }

  }
}
