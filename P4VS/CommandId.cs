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
 * Name		: CommandId.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Command ids for commands defined in the vcst file.
 *
 ******************************************************************************/

using System;

namespace Perforce.P4VS
{
	/// <summary>
	/// This class is used to expose the list of the IDs of the commands implemented
	/// by the client package. This list of IDs must match the set of IDs defined inside the
	/// VSCT file.
	/// </summary>
	public static class CommandId
	{
		// Define the list a set of public static members.

		// Define the list of menus (these include toolbars)
		public const int imnuFileSourceControlMenu		= 0x200;
		public const int imnuToolWindowToolbarMenu      = 0x201;
		public const int imnuManageMenu					= 0x202;
		public const int imnuRevisionsMenu				= 0x203;
		public const int imnuCopyMergeMenu				= 0x204;
		public const int imnuViewsMenu					= 0x205;
		public const int imnuDiffMenu					= 0x206;


		// View Menu
		// 100-109
		public const int icmdViewWorkspaceToolWindow	= 0x100;
		public const int icmdViewHistoryToolWindow		= 0x101;
		public const int icmdViewJobsToolWindow			= 0x102;
//      public const int icmdToolWindowToolbarCommand   = 0x103;
//      public const int icmdViewP4ToolWindow           = 0x104;
		public const int icmdViewSubmittedChangelistsToolWindow    = 0x105;
		public const int icmdViewPendingChangelistsToolWindow      = 0x106;
		public const int icmdViewLabelsToolWindow       = 0x107;
		//public const int icmdViewSwarmToolWindow        = 0x108;
        public const int icmdViewStreamsToolWindow		= 0x109;
		//public const int icmdViewReviewsToolWindow        = 0x10A;

		// File Menu
		// 110-119
		public const int icmdFileOpenInDepot			= 0x110;
		public const int icmdOpenConnection				= 0x111;
		public const int icmdCloseConnection			= 0x112;

		// Help Menu 
		// 120-129 -->
		public const int icmdP4VSHelp					= 0x120;
        public const int icmdP4VSSystemInfo             = 0x122;

        // Define the list of menus related to launching P4V
        // 140 - 149
        public const int icmdP4V						= 0x140;
		public const int icmdTimeLapse					= 0x141;
		public const int icmdRevGraph					= 0x142;
		public const int icmdStreamGraph				= 0x143;

		// Context Menu Items 
		// 150-199
        public const int icmdScmRefresh = 0x150;
		public const int icmdAddToSourceControl			= 0x151;
		public const int icmdCheckout					= 0x152;
        public const int icmdCheckoutProject			= 0x153;
        public const int icmdCheckoutSolution			= 0x154;
		public const int icmdCheckin                    = 0x155;
//		public const int icmdUseSccOffline				= 0x156;
		public const int icmdRevert						= 0x157;
		public const int icmdRevertUnchanged			= 0x158;
		public const int icmdLock						= 0x159;
		public const int icmdUnlock						= 0x160;
		public const int icmdChangeFileType				= 0x161;
		public const int icmdMoveToChangelist			= 0x162;
//		public const int icmdMarkForDelete				= 0x162;
		public const int icmdSyncHead					= 0x163;
        public const int icmdSync						= 0x164;
		public const int icmdDiffVsHave					= 0x165;
		public const int icmdDiffVsAny					= 0x166;
		public const int icmdAddToIgnoreList			= 0x167;
//		public const int icmdAddProjectToSCC			= 0x167;
		public const int icmdShowHistory				= 0x168;
		public const int icmdShelve						= 0x169;
		public const int icmdScmAttributes				= 0x170;
        public const int icmdScmCopy					= 0x171;
		public const int icmdScmMerge                   = 0x172;
		public const int icmdResolve					= 0x173;
		public const int icmdRemoveFromIgnoreList		= 0x174;
		public const int icmdEditIgnoreList				= 0x175;
        public const int icmdReconcile                  = 0x176;
        public const int icmdPublish                    = 0x177;

        // Connection Toolbar commands
        public const int cmdidConnectionDropDownCombo				= 0x220;
		public const int cmdidConnectionDropDownComboGetList		= 0x221;
		public const int cmdidActiveChangelistDropDownCombo			= 0x222;
		public const int cmdidActiveChangelistDropDownComboGetList	= 0x223;
		public const int cmdidCancelActiveCommand					= 0x224;
		public const int cmdidCurrentStream							= 0x225;

		// Define the list of icons (use decimal numbers here, to match the resource IDs)
		public const int iiconProductIcon               = 400;

		// Define the list of bitmaps (use decimal numbers here, to match the resource IDs)
		public const int ibmpToolbarMenusImages         = 500;
		public const int ibmpToolWindowsImages			= 508;
		public const int ibmpHistoryToolWindowsImage	= 502;
        public const int ibmpWorkspacesToolWindowsImage = 503;
        public const int ibmpJobsToolWindowsImage		= 504;
        public const int ibmpPendingToolWindowsImage	= 505;
        public const int ibmpSubmittedToolWindowsImage	= 506;
        public const int ibmpLabelsToolWindowsImage		= 510;

		// Glyph indexes in the bitmap used for toolwindows (ibmpToolWindowsImages)
		public const int iconHistory      = 0;
		public const int iconWorkspaces = 1;
		public const int iconPending = 2;
		public const int iconSubmitted = 3;
		public const int iconJobs = 4;
		public const int iconlabels = 5;

	}
}
