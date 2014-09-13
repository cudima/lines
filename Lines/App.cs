using System;
using System.Windows.Threading;

namespace Lines
{
  public class App : System.Windows.Application
  {
    [System.STAThreadAttribute()]
    public static void Main()
    {
      Lines.App app = new Lines.App();

      new Game(9, 9, 7);

      app.Run();

    }

    private static void dt_Tick(object sender, EventArgs e)
    {
      // mGame.NextTripple();
    }

  }
}
