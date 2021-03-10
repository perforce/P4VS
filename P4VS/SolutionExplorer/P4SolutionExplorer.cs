/*******************************************************************************

Copyright (c) 2011 Perforce Software, Inc.  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1.  Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.

2.  Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL PERFORCE SOFTWARE, INC. BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*******************************************************************************/

/*******************************************************************************
 * Name		: P4SolutionExplorer.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Subclasses the Solution Explorer Window to allow all scc state 
 * glyphs to be customized
 *
 ******************************************************************************/

using System;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
//using System.ComponentModel.Composition;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace Perforce.P4VS
{
	/// <summary>
	/// Override the WndPrc of the the Solution explorer so that we can set our own image list. 
	/// </summary>
    class NativeTreeView : NativeWindow
    {
        IntPtr hWnd = IntPtr.Zero;
        IntPtr hImgList = IntPtr.Zero;
        IntPtr hFont = IntPtr.Zero;

        public NativeTreeView(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentNullException("hwnd");

            hWnd = hwnd;
        }

        public bool OverrideImages
        {
            get { return hWnd != IntPtr.Zero; }
            set
            {
                if ((Handle != IntPtr.Zero) == value)
                    return;

                if (value)
                    this.AssignHandle(hWnd);
                else
                    this.ReleaseHandle();
            }
        }

        public IntPtr ImageListPtr
        {
            get
            {
                hImgList = (IntPtr)SendMessage(hWnd, TVM_GETIMAGELIST, TVSIL_STATE, IntPtr.Zero);
                return hImgList;
            }
            set
            {
                hImgList = value;
                SendMessage(hWnd, TVM_SETIMAGELIST, TVSIL_STATE, value);
            }
        }

        [DebuggerNonUserCode]
        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            if ((msg.Msg == (int)TVM_SETIMAGELIST) && (msg.WParam == TVSIL_STATE))
            {
                return;
            }

			try
			{
				base.WndProc(ref msg);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
        }

        public bool IsValid
        {
            get { return true; }
        }

        //message constants
        public const uint TVM_SETIMAGELIST = 0x1109;
        public const uint TVM_GETIMAGELIST = 4360;
        public const uint LVM_SETIMAGELIST = 0x1003;

        public static readonly IntPtr TVSIL_STATE = (IntPtr)0x2;
        public static readonly IntPtr LVSIL_NORMAL = (IntPtr)0x0;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr afterChild, string className,
            string windowName);

    }

    class P4SolutionExplorer : IVsWindowFrameNotify, IVsWindowFrameNotify2, IVsWindowFrameNotify3, IDisposable
    {
        IntPtr hOldImageList;

        NativeTreeView SEViewControl;

        IVsWindowFrame SolutionExplorer;
        IVsWindowFrame2 SolutionExplorer2;
        IVsUIHierarchyWindow Tree;

        uint Cookie;

        const string GENERICPANE = "GenericPane";
        const string VSAUTOHIDE = "VsAutoHide";
        const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        const string TREEVIEW = "SysTreeView32";
        const string VBFLOATINGPALETTE = "VBFloatingPalette";


        public void Capture()
        {
            if (SEViewControl != null && SEViewControl.IsValid)
                return;

            EnvDTE.Window window;
            try
            {
                SolutionExplorer = FindSolutionExplorerFrame();

                window = VsShellUtilities.GetWindowObject(SolutionExplorer);
            }
            catch (COMException)
            {
                // for VS2010 - WPF Shell compatibility (we cannot find the solution explorer frame there)
                return;
            }

            string expCaption = window.Caption;

            IntPtr handle = IntPtr.Zero;

            if (window.HWnd != 0)
            {
                // We've got the parent
                handle = (IntPtr)window.HWnd;
            }
            else
            {
                EnvDTE.Window hostWindow = window.LinkedWindowFrame;

                if (hostWindow != null)
                    handle = FindSolutionExplorerWnd((IntPtr)hostWindow.HWnd, expCaption);

                if (handle == IntPtr.Zero)
                {
                    hostWindow = window.DTE.MainWindow;

                    if (hostWindow != null)
                        handle = FindSolutionExplorerWnd((IntPtr)hostWindow.HWnd, expCaption);
                }

                if (handle == IntPtr.Zero)
                    handle = FindInVBFloatingPalettes(expCaption);
            }

            if (handle == IntPtr.Zero)
                return; // Not found :(

            IntPtr uiHierarchy = NativeTreeView.FindWindowEx(handle, IntPtr.Zero,
                UIHIERARCHY, null);
            IntPtr treeHwnd = NativeTreeView.FindWindowEx(uiHierarchy, IntPtr.Zero, TREEVIEW,
                null);

            if (treeHwnd == IntPtr.Zero)
                return;

            SEViewControl = new NativeTreeView(treeHwnd);

            SetGlyphs();
        }

        private IntPtr FindSolutionExplorerWnd(IntPtr hparent, string wndCaption)
        {
            // is it directly under the parent?
            IntPtr hSEWnd = NativeTreeView.FindWindowEx(hparent, IntPtr.Zero, GENERICPANE, wndCaption);
            if (hSEWnd != IntPtr.Zero)
                return hSEWnd;

            IntPtr win = NativeTreeView.FindWindowEx(hparent, IntPtr.Zero, null, null);
            while (win != IntPtr.Zero)
            {
                hSEWnd = FindSolutionExplorerWnd(win, wndCaption);
                if (hSEWnd != IntPtr.Zero)
                {
                    return hSEWnd;
                }
                win = NativeTreeView.FindWindowEx(hparent, win, null, null);
            }

            return IntPtr.Zero;
        }

        private IntPtr FindInVBFloatingPalettes(string wndCaption)
        {
            IntPtr hFloatingPalette = NativeTreeView.FindWindowEx(IntPtr.Zero, IntPtr.Zero, VBFLOATINGPALETTE, null);
            while (hFloatingPalette != IntPtr.Zero)
            {
                IntPtr hSEWnd = FindSolutionExplorerWnd(hFloatingPalette, wndCaption);
                if (hSEWnd != IntPtr.Zero)
                {
                    return hSEWnd;
                }
                hFloatingPalette = NativeTreeView.FindWindowEx(IntPtr.Zero, hFloatingPalette, VBFLOATINGPALETTE, null);
            }
            return IntPtr.Zero;
        }

        //[Import]
        //System.IServiceProvider ServiceManager = null;

        public IVsWindowFrame FindSolutionExplorerFrame()
        {
            IVsUIShell uiShell = (IVsUIShell)P4VsProvider.Service(typeof(SVsUIShell));
            return FindSolutionExplorerFrame(uiShell);
        }

        public IVsWindowFrame FindSolutionExplorerFrame(IVsUIShell uiShell)
        {
            if (SolutionExplorer == null)
            {
                //IVsUIShell shell = GetService<IVsUIShell>(typeof(SVsUIShell));
                //IVsUIShell uiShell = (IVsUIShell)cmp.GetService(typeof(SVsUIShell));

                Debug.Assert(uiShell != null); // Must be true

                if (uiShell != null)
                {
                    IVsWindowFrame solutionExplorer;
                    Guid solutionExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);

                    Marshal.ThrowExceptionForHR(uiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref solutionExplorerGuid, out solutionExplorer));

                    if (solutionExplorer != null)
                    {
                        SolutionExplorer = solutionExplorer;
                        IVsWindowFrame2 solutionExplorer2 = solutionExplorer as IVsWindowFrame2;

                        if (solutionExplorer2 != null)
                        {
                            uint cookie;
                            Marshal.ThrowExceptionForHR(solutionExplorer2.Advise(this, out cookie));
                            Cookie = cookie;
                            SolutionExplorer2 = solutionExplorer2;
                        }
                    }
                }
            }
            return SolutionExplorer;
        }

        public IVsUIHierarchyWindow FindHierarchyWindow()
        {
            if (Tree == null)
            {
                IVsWindowFrame frame = SolutionExplorer;

                if (frame != null)
                {
                    object pvar = null;
                    Marshal.ThrowExceptionForHR(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out pvar));

                    Tree = pvar as IVsUIHierarchyWindow;
                }
            }
            return Tree;
        }

        static ImageList customGlyphs = null;
		static ImageList customGlyphsShortList = null;
		static ImageList customGlyphsLongList = null;

		public static ImageList CustomGlyphs
		{
			get
			{
				if (customGlyphs == null)
				{
					customGlyphs = new ImageList();
					customGlyphs.ColorDepth = ColorDepth.Depth32Bit;
					customGlyphs.ImageSize = new System.Drawing.Size(7, 16);
					customGlyphs.TransparentColor = System.Drawing.Color.FromArgb(200, 191, 231);

					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_00);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_01);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_02);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_03);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_04);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_05);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_06);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_07);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_08);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_09);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_10);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_11);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_12);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_13);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_14);
					customGlyphs.Images.Add(SolutionExplorerResourses.Glyph_15);
				}
				return customGlyphs;
			}
		}
		public static ImageList CustomGlyphsShortList
		{
			get
			{
				if (customGlyphsShortList == null)
				{
					customGlyphsShortList = new ImageList();
					customGlyphsShortList.ColorDepth = ColorDepth.Depth32Bit;
					customGlyphsShortList.ImageSize = new System.Drawing.Size(7, 16);
					customGlyphsShortList.TransparentColor = System.Drawing.Color.FromArgb(200, 191, 231);

					customGlyphsShortList.Images.Add(SolutionExplorerResourses.Glyph_12);
					customGlyphsShortList.Images.Add(SolutionExplorerResourses.Glyph_13);
					customGlyphsShortList.Images.Add(SolutionExplorerResourses.Glyph_14);
					customGlyphsShortList.Images.Add(SolutionExplorerResourses.Glyph_15);
				}
				return customGlyphsShortList;
			}
		}
		public static ImageList CustomGlyphsLongList
		{
			get
			{
				if (customGlyphsLongList == null)
				{
					customGlyphsLongList = new ImageList();
					customGlyphsLongList.ColorDepth = ColorDepth.Depth32Bit;
					customGlyphsLongList.ImageSize = new System.Drawing.Size(7, 16);
					customGlyphsLongList.TransparentColor = System.Drawing.Color.FromArgb(200, 191, 231);

					// first add the custom glyphs to use for the slots 12-15
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_12);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_13);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_14);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_15);

					// then add all 16 for solution navigator and other possible newer users of
					// the glyph service and don't trim the index to 4 bits.
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_01);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_02);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_03);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_04);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_05);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_06);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_07);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_08);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_09);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_10);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_11);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_12);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_13);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_14);
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_15);

#if VS2010 || VS2012
					// Add the extended glyphs that can only be displayed in Solution 
					//	Navigator (Solution Explore in VS 21012).
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//16 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//17 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_18);	//18 Self checkout, locked
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//19 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_20);	//20 Self checkout, stale, locked
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_21);	//21 Ignored File
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_22);	//22 Other checkout, stale, locked
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_23);	//23 Ignored File
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//24 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//25 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//26 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//27 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_28);	//28 Other checkout, locked
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//29 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//30 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//31 Not Yet Used
					// Add the second tier of extended glyphs that can only be displayed in Solution 
					//	Navigator (Solution Explore in VS 21012).
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//32 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_33);	//33 Integrated
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//34 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_35);	//35 Integrate, stale
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//36 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_37);	//37 Status Not Loaded (lazy Load)
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_38);	//38 Integrated, Other checkout
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//39 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//40 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//41 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//42 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//43 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_44);	//44 Integrated, Other checkout, stale
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//45 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//46 Not Yet Used
					customGlyphsLongList.Images.Add(SolutionExplorerResourses.Glyph_00);	//47 Not Yet Used
#endif
				}
				return customGlyphsLongList;
			}
		}

        void SetGlyphs()
        {
            if (SEViewControl == null)
                return; // Nothing to do

            // store the original image list (check that we're not storing our own statusImageList
            if (CustomGlyphs.Handle != SEViewControl.ImageListPtr)
                hOldImageList = SEViewControl.ImageListPtr;

            // and assign the status image list to the tree
            SEViewControl.ImageListPtr = CustomGlyphs.Handle;
            SEViewControl.OverrideImages = true;
        }

        void RestoreGlyphs()
        {
            if (SEViewControl == null)
                return; // Nothing to do

            // if someone wants VSS images now, let them.
            SEViewControl.OverrideImages = false;

            if (hOldImageList != IntPtr.Zero)
            {
                SEViewControl.ImageListPtr = hOldImageList;
                hOldImageList = IntPtr.Zero;
            }
        }


        #region IVsWindowFrameNotify3 Members

        public int OnClose(ref uint pgrfSaveOptions)
        {
            Dispose();
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            Capture();
            return VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
			Capture();
			return VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
			Capture();
			return VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
			Capture();
			return VSConstants.S_OK;
        }

        #endregion

        #region IVsWindowFrameNotify Members

        public int OnDockableChange(int fDockable)
        {
			Capture();
			return VSConstants.S_OK;
        }

        public int OnMove()
        {
			Capture();
			return VSConstants.S_OK;
        }

        public int OnSize()
        {
			Capture();
			return VSConstants.S_OK;
        }

        #endregion

        #region IVsWindowFrameNotify2 Members

        int IVsWindowFrameNotify2.OnClose(ref uint pgrfSaveOptions)
        {
            Dispose();
            return VSConstants.S_OK;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
			RestoreGlyphs();

			if (customGlyphs != null)
			{
				customGlyphs.Dispose();
				customGlyphs = null;
			}
			if (customGlyphsShortList != null)
			{
				customGlyphsShortList.Dispose();
				customGlyphsShortList = null;
			}
			if (customGlyphsLongList != null)
			{
				customGlyphsLongList.Dispose();
				customGlyphsLongList = null;
			}
			if (SEViewControl != null)
			{
				SEViewControl.OverrideImages = false;
				SEViewControl = null;
			}
			SolutionExplorer = null;
			Tree = null;
        }

        #endregion
    }
}
