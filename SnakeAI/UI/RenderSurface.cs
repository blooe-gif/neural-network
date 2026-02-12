using System.Windows.Forms;

namespace SnakeAI.UI;

public class RenderSurface : Panel
{
    public RenderSurface()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}
