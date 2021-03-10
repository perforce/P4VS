/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Globalization;
using MsVsShell = Microsoft.VisualStudio.Shell;

namespace Perforce.P4VS
{
	/// <summary>
	/// This attribute registers the source control provider.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class ProvideSourceControlProvider : MsVsShell.RegistrationAttribute
	{
		private string _regName = null;
		private string _uiName = null;
		
		/// <summary>
		/// </summary>
		public ProvideSourceControlProvider(string regName, string uiName)
		{
			_regName = regName;
			_uiName = uiName;
		}

		/// <summary>
		/// Get the friendly name of the provider (written in registry)
		/// </summary>
		public string RegName
		{
			get { return _regName; }
		}

		/// <summary>
		/// Get the unique guid identifying the provider
		/// </summary>
		public Guid RegGuid
		{
			get { return GuidList.guidP4VsProvider; }
		}

		/// <summary>
		/// Get the UI name of the provider (string resource ID)
		/// </summary>
		public string UIName
		{
			get { return _uiName; }
		}

		/// <summary>
		/// Get the package containing the UI name of the provider
		/// </summary>
		public Guid UINamePkg
		{
			get { return GuidList.guidP4VsProviderPkg; }
		}

		/// <summary>
		/// Get the guid of the provider's service
		/// </summary>
		public Guid P4VsProviderService
		{
			get { return GuidList.guidP4VsProviderService; }
		}

		/// <summary>
		///     Called to register this attribute with the given context.  The context
		///     contains the location where the registration information should be placed.
		///     It also contains other information such as the type being registered and path information.
		/// </summary>
		public override void Register(RegistrationContext context)
		{
			// Write to the context's log what we are about to do
			context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "P4VsProvider:\t\t{0}\n", RegName));

			// Declare the source control provider, its name, the provider's service 
			// and additionally the packages implementing this provider
			using (Key P4VsProviders = context.CreateKey("SourceControlProviders"))
			{
				using (Key P4VsProviderKey = P4VsProviders.CreateSubkey(RegGuid.ToString("B")))
				{
					P4VsProviderKey.SetValue("", RegName);
					P4VsProviderKey.SetValue("Service", P4VsProviderService.ToString("B"));

					using (Key P4VsProviderNameKey = P4VsProviderKey.CreateSubkey("Name"))
					{
						P4VsProviderNameKey.SetValue("", UIName);
						P4VsProviderNameKey.SetValue("Package", UINamePkg.ToString("B"));

						P4VsProviderNameKey.Close();
					}

					// Additionally, you can create a "Packages" subkey where you can enumerate the dll
					// that are used by the source control provider, something like "Package1"="P4VsProvider.dll"
					// but this is not a requirement.
					P4VsProviderKey.Close();
				}

				P4VsProviders.Close();
			}
		}

		/// <summary>
		/// Unregister the source control provider
		/// </summary>
		/// <param name="context"></param>
		public override void Unregister(RegistrationContext context)
		{
			context.RemoveKey("SourceControlProviders\\" + GuidList.guidP4VsProviderPkg.ToString("B"));
		}
	}
}
