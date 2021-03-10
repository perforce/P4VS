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
 * Name		: Guids.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: List of guids exposed by P4VS
 *
 ******************************************************************************/

using System;

namespace Perforce.P4VS_StartPage
{
	/// <summary>
	/// This class is used only to expose the list of Guids used by this package.
	/// This list of guids must match the set of Guids used inside the VSCT file.
	/// </summary>
	public static class GuidList
	{
	// Now define the list of guids as public static members.
   
		// Unique ID of the source control provider; this is also used as the command UI context to show/hide the package UI
		public static readonly Guid guidP4VsProvider = new Guid("{FDA934F4-0492-4F67-A6EB-CBE0953649F0}");
		// The guid of the source control provider service (implementing IVsP4VsProvider interface)
		public static readonly Guid guidP4VsProviderService = new Guid("{93C6B80C-A9E4-4F63-A605-51E7FCB9F906}");
		
#if !VS2012
        // The guid of the source control provider package (implementing IVsPackage interface)
		public static readonly Guid guidP4VsProviderPkg = new Guid("{BBBB4F8F-5EDA-4623-8BAC-644EC6501F97}");
#else //VS2012
        // The guid of the source control provider package (implementing IVsPackage interface)
        public static readonly Guid guidP4VsProviderPkg = new Guid("{8D316614-311A-48F4-85F7-DF7020F62357}");
#endif
    // Other guids for menus and commands
		public static readonly Guid guidP4VsProviderCmdSet = new Guid("{4C690E93-4B58-4866-B02C-6A9E5F0BD0F2}");
	};
}
