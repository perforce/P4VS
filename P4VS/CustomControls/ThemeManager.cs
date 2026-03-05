using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Common;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Media.Imaging;

namespace Perforce.P4VS
{
    public partial class ThemeManager :IDisposable
    {
        Control.ControlCollection Controls = null;

        string ImageCacheKey = string.Empty;

        Dictionary<string, Image> ImageCache = null;
        Dictionary<string, List<Image>> ImageListCache = null;

        IList<Image> Images;
        IList<ImageList> ImageLists;

        public ThemeManager(Control.ControlCollection controls, string imageCacheKey)
        {
            ColorsChangeDelegate = new P4VsProviderService.SetControlColorsDelegate(SetControlColors);
            P4VsProvider.Instance.SccService.SetControlColors += ColorsChangeDelegate;

            Controls = controls;

            ImageCache  = new Dictionary<string, Image>();
            ImageListCache = new Dictionary<string, List<Image>>();

            ImageCacheKey = imageCacheKey;

            Images = null;
        }
        ~ThemeManager()
        {
            Dispose(false);
        }

        P4VsProviderService.SetControlColorsDelegate ColorsChangeDelegate = null;

        // Register context menu to apply VS theme colors
        public void RegisterContextMenu(ContextMenuStrip menu)
        {
            if (menu != null)
            {
                menu.RenderMode = ToolStripRenderMode.Professional;
                menu.Renderer = new ThemeManagerRenderer(new ThemeManagerColorTable(this));
            }
        }

        // Custom ColorTable to define the colors for the ContextMenuStrip based on VS theme
        public class ThemeManagerColorTable : ProfessionalColorTable
        {
            ThemeManager _owner;
            public ThemeManagerColorTable(ThemeManager owner)
            {
                _owner = owner;
            }
            public override Color MenuBorder { get { return _owner.ForeColor; } }
            public override Color MenuItemSelected { get { return VSColorTheme.GetThemedColor(EnvironmentColors.CommandBarMouseOverBackgroundBeginColorKey); } }
            public override Color MenuItemBorder { get { return VSColorTheme.GetThemedColor(EnvironmentColors.CommandBarSelectedBorderColorKey); } }
            public override Color MenuStripGradientBegin { get { return _owner.BackColor; } }
            public override Color MenuStripGradientEnd { get { return _owner.BackColor; } }
            public override Color ToolStripDropDownBackground { get { return _owner.BackColor; } }
            public override Color ImageMarginGradientBegin { get { return _owner.BackColor; } }
            public override Color ImageMarginGradientMiddle { get { return _owner.BackColor; } }
            public override Color ImageMarginGradientEnd { get { return _owner.BackColor; } }
            public override Color MenuItemSelectedGradientBegin { get { return MenuItemSelected; } }
            public override Color MenuItemSelectedGradientEnd { get { return MenuItemSelected; } }
            public override Color MenuItemPressedGradientBegin { get { return MenuItemSelected; } }
            public override Color MenuItemPressedGradientMiddle { get { return MenuItemSelected; } }
            public override Color MenuItemPressedGradientEnd { get { return MenuItemSelected; } }
            public override Color SeparatorDark { get { return _owner.ForeColor; } }
            public override Color SeparatorLight { get { return _owner.ForeColor; } }

        }

        // Custom Renderer to apply the ColorTable and handle item text rendering
        public class ThemeManagerRenderer : ToolStripProfessionalRenderer
        {
            public ThemeManagerRenderer(ProfessionalColorTable table) : base(table) { }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = (e.Item.Selected) ? VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey) : VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
                base.OnRenderItemText(e);
            }
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = (e.Item.Selected) ? VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey) : VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
                base.OnRenderArrow(e);
            }
        }
        public void RegisterImage(Image i)
        {
            if (Images == null)
            {
                Images = new List<Image>();
            }
            Images.Add(i);
        }

        public void RegisterImageList(ImageList i)
        {
            if (ImageLists == null)
            {
                ImageLists = new List<ImageList>();
            }
            ImageLists.Add(i);
        }

       public Color BackColor { get; private set; }
       public Color ForeColor { get; private set; }
       public void SetControlColors()//P4VsProviderService.ColorCollection colors)
        {
#if !VS2015
            object s5 =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(IVsUIShell));
            IVsUIShell5 shell5 = s5 as IVsUIShell5;
#endif
            ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);

            if ((Images != null) && (Images.Count > 0))
            {
                for (int idx = 0; idx < Images.Count; idx++)
                {
                    string imgKey = string.Format("{0}.Images[{1}]", ImageCacheKey, idx);
                    Images[idx] = InvertImage(imgKey,shell5,Images[idx], Color.Transparent, (uint)BackColor.ToArgb());
                }
            }
            if ((ImageLists != null) && (ImageLists.Count > 0))
            {
                for (int idx = 0; idx < ImageLists.Count; idx++)
                {
                    string imgKey = string.Format("{0}.ImageLists[{1}]", ImageCacheKey, idx);
                    InvertImageList(imgKey,shell5, ImageLists[idx], Color.Transparent, (uint)BackColor.ToArgb());
                }
            }
            SetControlColors(Controls, ImageCacheKey);
        }

        // Initialize ThemeManager for the given control and apply colors immediately
        public static void SetTheme(Control control)
        {
            if (control == null)
            {
                return;
            }
            // Use control name as key or a generic one
            using (ThemeManager mgr = new ThemeManager(control.Controls, control.GetType().Name))
            {
                mgr.SetControlColors();
                control.BackColor = mgr.BackColor;
                control.ForeColor = mgr.ForeColor;
            }
        }

        //#if VS2012
        //        public Color SelectedItemActiveBackColor = VSColorTheme.GetThemedColor(TreeViewColors.SelectedItemActiveColorKey);
        //        public Color SelectedItemActiveForeColor = VSColorTheme.GetThemedColor(TreeViewColors.SelectedItemActiveTextColorKey);
        //        public Color SelectedItemInactiveBackColor = VSColorTheme.GetThemedColor(TreeViewColors.SelectedItemInactiveColorKey);
        //        public Color SelectedItemInactiveForeColor = VSColorTheme.GetThemedColor(TreeViewColors.SelectedItemInactiveTextColorKey);
        //        public Color DisabledItemForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.SystemGrayTextColorKey);
        //#endif


        public void SetControlColors(Control.ControlCollection Controls, string ImageCacheKey)
        {
#if !VS2015
            object s5 =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(IVsUIShell));
            IVsUIShell5 shell5 = s5 as IVsUIShell5;
#endif
            if ((Controls == null) || (Controls.Count <= 0))
            {
                return;
            }

            foreach (Control c in Controls)
            {
                // General context menu handling for any control
                if (c.ContextMenuStrip != null)
                {
                   RegisterContextMenu(c.ContextMenuStrip);
                }

                // Register context menu for ListView column headers if it's a ListView.
                if (c is ListView)
                {
                    try
                    {
                        var prop = c.GetType().GetProperty("ColumnContextMenu");
                        if (prop != null)
                        {
                            var menu = prop.GetValue(c, null) as ContextMenuStrip;
                            RegisterContextMenu(menu);
                        }
                    }
                    catch { }
                }

                string ImageCacheSubkey = string.Concat(ImageCacheKey, ".", c.Name);
                //if (c is SplitContainer)
                //{
                //    SplitContainer s = c as SplitContainer;
                //    s.BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.EnvironmentBackgroundColorKey);
                //}
                //else if (c is Panel)
                if (c is Panel)
                {
                    Panel p = c as Panel;

                    SetControlColors(p.Controls, ImageCacheSubkey);

                    p.ForeColor = ForeColor;
                    p.BackColor = BackColor;
                }
                else if (c is Button)
                {
                    Button b = c as Button;

                    b.ForeColor = ForeColor;
                    b.BackColor = BackColor;
                    b.FlatAppearance.BorderColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBorderColorKey);

                    //b.UseVisualStyleBackColor = false;
#if VS2015
                    b.Image = InvertImage(ImageCacheSubkey, b.Image, (uint)BackColor.ToArgb());
                    InvertImageList(ImageCacheSubkey + ".ImageList", b.ImageList, (uint)BackColor.ToArgb());
#else
                    if (shell5 != null)
                    {
                        if (b.ImageList != null)
                        {
                            InvertImageList(ImageCacheSubkey + ".ImageList", shell5, b.ImageList, Color.Transparent, (uint)BackColor.ToArgb());
                        }
                        else if (b.Image != null)
                        {
                            b.Image = InvertImage(ImageCacheSubkey, shell5, b.Image, Color.Transparent, (uint)BackColor.ToArgb());
                        }
                    }
#endif
                }
                else if (c is ListBox)
                {
                    ListBox lb = c as ListBox;
                    lb.ForeColor = ForeColor;
                    lb.BackColor = BackColor;
                    lb.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is ProgressBar)
                {
                    //ProgressBar pb = c as ProgressBar;
                    //pb.ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWin);
                    //pb.BackColor = BackColor;
                }
                else if (c is ListView)
                {
                    ListView lv = c as ListView;

                    lv.ForeColor = ForeColor;
                    lv.BackColor = BackColor;
                    lv.BorderStyle = BorderStyle.FixedSingle;
#if !VS2015
                    if (shell5 != null)
                    {
                        InvertImageList(ImageCacheSubkey + ".SmallImageList", shell5, lv.SmallImageList, Color.Transparent, (uint)BackColor.ToArgb());
#else
                        InvertImageList(ImageCacheSubkey + ".SmallImageList", lv.SmallImageList, (uint)BackColor.ToArgb());
#endif
                        if (lv is TreeListView)
                        {
                            TreeListView tlv = lv as TreeListView;

#if VS2015
                            InvertImageList(ImageCacheSubkey + ".classicStateImageList", tlv.classicStateImageList, (uint)BackColor.ToArgb());
                            InvertImageList(ImageCacheSubkey + ".LeftImageList", tlv.LeftImageList, (uint)BackColor.ToArgb());
                            InvertImageList(ImageCacheSubkey + ".RightImageList", tlv.RightImageList, (uint)BackColor.ToArgb());
                            InvertImageList(ImageCacheSubkey + ".checkboxImageList", tlv.checkboxImageList, (uint)BackColor.ToArgb());
#else
                            InvertImageList(ImageCacheSubkey + ".classicStateImageList", shell5, tlv.classicStateImageList, Color.Transparent, (uint)BackColor.ToArgb());
                            InvertImageList(ImageCacheSubkey + ".LeftImageList", shell5, tlv.LeftImageList, Color.Transparent, (uint)BackColor.ToArgb());
                            InvertImageList(ImageCacheSubkey + ".RightImageList", shell5, tlv.RightImageList, Color.Transparent, (uint)BackColor.ToArgb());
                            InvertImageList(ImageCacheSubkey + ".checkboxImageList", shell5, tlv.checkboxImageList, Color.Transparent, (uint)BackColor.ToArgb());

                        }
#endif
                    }
                }
                else if (c is TreeView)
                {
                    // Apply theme colors to TreeView
                    TreeView tv = c as TreeView;
                    tv.ForeColor = ForeColor;
                    tv.BackColor = BackColor;
                    tv.BorderStyle = BorderStyle.FixedSingle;
#if !VS2015
                    if (shell5 != null)
                    {
                        InvertImageList(ImageCacheSubkey + ".ImageList", shell5, tv.ImageList, Color.Transparent, (uint)BackColor.ToArgb());
                    }
#else
                    InvertImageList(ImageCacheSubkey + ".ImageList", tv.ImageList, (uint)BackColor.ToArgb());
#endif
                }
                else if (c is ComboBox)
                {
                    ComboBox cb = c as ComboBox;
                    cb.ForeColor = ForeColor;
                    cb.BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);
                    cb.FlatStyle = FlatStyle.Flat;
                }
                else if (c is GroupBox)
                {
                    GroupBox gb = c as GroupBox;
                    gb.ForeColor = ForeColor;
                    gb.BackColor = BackColor;

                    SetControlColors(gb.Controls, ImageCacheSubkey);
                }
                else if (c is CheckBox)
                {
                    CheckBox cb = c as CheckBox;
                    cb.ForeColor = ForeColor;
                    cb.BackColor = BackColor;
                    //cb.UseVisualStyleBackColor = false;
                }
                else if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    rb.ForeColor = ForeColor;
                    rb.BackColor = BackColor;
                    rb.UseVisualStyleBackColor = false;
                }
                else if (c is Label)
                {
                    Label l = c as Label;
                    l.ForeColor = ForeColor;
                    l.BackColor = BackColor;
                }
                else if (c is TextBox)
                {
                    TextBox tb = c as TextBox;
                    tb.ForeColor = ForeColor;
                    tb.BackColor = BackColor;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is RichTextBox)
                {
                    // Apply theme colors to RichTextBox
                    RichTextBox tb = c as RichTextBox;
                    tb.ForeColor = ForeColor;
                    tb.BackColor = BackColor;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is DataGridView)
                {
                    DataGridView dgv = c as DataGridView;
                    dgv.BorderStyle = BorderStyle.FixedSingle;
                    dgv.BackgroundColor = BackColor;
                    dgv.GridColor = ForeColor;
                    dgv.DefaultCellStyle.BackColor = BackColor;
                    dgv.DefaultCellStyle.ForeColor = ForeColor;
                    dgv.DefaultCellStyle.SelectionBackColor = VSColorTheme.GetThemedColor(EnvironmentColors.SystemHighlightColorKey);
                    dgv.DefaultCellStyle.SelectionForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.SystemHighlightTextColorKey);
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = BackColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = ForeColor;
                    dgv.RowHeadersDefaultCellStyle.BackColor = BackColor;
                    dgv.RowHeadersDefaultCellStyle.ForeColor = ForeColor;
                    dgv.EnableHeadersVisualStyles = false;
                }
                else if (c is ContainerControl)
                {
                    ContainerControl s = c as ContainerControl;

                    SetControlColors(s.Controls, ImageCacheSubkey);
                }
                else
                {
#if DEBUG
                    Type t = c.GetType();

                    System.Diagnostics.Trace.WriteLine(string.Format("Unhandled type: {0}", t.ToString()));
#endif
                }
            }
        }

#if VS2015
        private void InvertImageList(string imageListCacheKey, ImageList list, uint backgroundColor)
#else
        private Image InvertImage(string imageListCacheKey, IVsUIShell5 shell5,
            Image i, Color transparentColor, uint backgroundColor)
#endif
        {
            if (i == null)
            {
                return null;
            }
            if (ImageCache.ContainsKey(imageListCacheKey) == false)
            {
                // new image, so cache it
                ImageCache.Add(imageListCacheKey, i);
            }
            else
            {
                // get the origonal image out of the cache
                i = ImageCache[imageListCacheKey];
            }
#if VS2015
            Bitmap ii = GetInvertedBitmap((Bitmap)i, (uint)BackColor.ToArgb());
#else
            Bitmap ii = GetInvertedBitmap(shell5, (Bitmap)i, Color.Transparent, backgroundColor);
#endif
            return ii;
        }

#if VS2015
        private void InvertImageList(string imageListCacheKey, ImageList list, uint backgroundColor)
#else
        private void InvertImageList(string imageListCacheKey, IVsUIShell5 shell5,
            ImageList list, Color transparentColor, uint backgroundColor)
#endif
        {
            if (list != null)
            {
                List<Image> il = null;

                if (ImageListCache.ContainsKey(imageListCacheKey) == false)
                {
                    // haven't converted this image list yet, so make a copy of the images in the list
                    // to use if we need to adjust the images for another theme change.
                    il = new List<Image>();

                    for (int idx = 0; idx < list.Images.Count; idx++)
                    {
                        il.Add(list.Images[idx]);
                    }
                    ImageListCache.Add(imageListCacheKey, il);
                }
                else
                {
                    il = ImageListCache[imageListCacheKey];
                }
                for (int idx = 0; idx < list.Images.Count; idx++)
                {
                    Image i = il[idx];
                    if (i != null)
                    {
#if VS2015
                        Bitmap ii = GetInvertedBitmap((Bitmap)i, backgroundColor);
#else
                        Bitmap ii = GetInvertedBitmap(shell5, (Bitmap)i, Color.Transparent, backgroundColor);
#endif
                        list.Images[idx] = ii;
                    }
                }
            }
        }

#if VS2015
        private static Bitmap GetInvertedBitmap(Bitmap inputBitmap, uint backgroundColor)
#else
        private static Bitmap GetInvertedBitmap(Microsoft.VisualStudio.Shell.Interop.IVsUIShell5 shell5,
            Bitmap inputBitmap, Color transparentColor, uint backgroundColor)
#endif
        {
            Bitmap outputBitmap = null;
            try
            {
#if VS2015
                outputBitmap = ImageThemingUtilities.GetThemedBitmap(inputBitmap, backgroundColor);
#else
                byte[] outputBytes;
                Rectangle rect;
                System.Drawing.Imaging.BitmapData bitmapData;
                IntPtr sourcePointer;
                int length;

                outputBitmap = new Bitmap(inputBitmap);

                outputBitmap.MakeTransparent(transparentColor);

                rect = new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height);

                bitmapData = outputBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, outputBitmap.PixelFormat);

                sourcePointer = bitmapData.Scan0;

                length = (Math.Abs(bitmapData.Stride) * outputBitmap.Height);

                outputBytes = new byte[length];

                Marshal.Copy(sourcePointer, outputBytes, 0, length);

                shell5.ThemeDIBits( (UInt32)outputBytes.Length, outputBytes, (UInt32)outputBitmap.Width,
                                   (UInt32)outputBitmap.Height, true, backgroundColor);

                Marshal.Copy(outputBytes, 0, sourcePointer, length);

                outputBitmap.UnlockBits(bitmapData);
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return outputBitmap;

        }

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (ColorsChangeDelegate != null)
                {
                    P4VsProvider.Instance.SccService.SetControlColors -= ColorsChangeDelegate;
                }
            }
        }
    }
}