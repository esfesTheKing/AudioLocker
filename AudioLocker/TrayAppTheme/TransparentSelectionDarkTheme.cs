namespace AudioLocker.TrayAppTheme;


// All definitions besides `MenuItemBorder` were taken directly from: `System.Windows.Forms/System/Windows/Forms/DarkProfessionalColors.cs`
public class TransparentSelectionDarkTheme : ProfessionalColorTable
{
    public override Color MenuItemBorder => Color.Transparent;

    public override Color MenuItemPressedGradientBegin
        => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

    public override Color MenuItemPressedGradientMiddle
        => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

    public override Color MenuItemPressedGradientEnd
        => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

    public override Color MenuItemSelected
        => SystemColors.ControlText;

    public override Color MenuItemSelectedGradientBegin
        => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);

    public override Color MenuItemSelectedGradientEnd
        => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);

    public override Color MenuStripGradientBegin
        => SystemColors.Control;

    public override Color MenuStripGradientEnd
        => SystemColors.Control;

    public override Color StatusStripGradientBegin
        => SystemColors.Control;

    public override Color StatusStripGradientEnd
        => SystemColors.Control;

    public override Color ToolStripDropDownBackground
        => SystemColors.Control;

    public override Color ImageMarginGradientBegin
        => SystemColors.Control;

    public override Color ImageMarginGradientMiddle
        => SystemColors.Control;

    public override Color ImageMarginGradientEnd
        => SystemColors.Control;
}
