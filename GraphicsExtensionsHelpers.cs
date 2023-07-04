internal static class GraphicsExtensionsHelpers
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, float x, float y, float width, float height, float radius)
    {
        // Create a graphics path to draw the rounded rectangle
        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
        gp.AddLine(x + radius, y, x + width - (radius * 2), y);
        gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);
        gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2));
        gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
        gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height);
        gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
        gp.AddLine(x, y + height - (radius * 2), x, y + radius);
        gp.AddArc(x, y, radius * 2, radius * 2, 180, 90);
        gp.CloseFigure();
        // Fill the graphics path with the brush
        g.FillPath(brush, gp);
    }
}