using System;

namespace CodeLensOopProviderVsix
{
    class GuidAndCmdID
    {
        public const string PackageGuidString = "9a7d41ec-0f34-460a-9499-8356b559ff89";
        public const string PackageCmdSetGuidString = "f3cb9f10-281b-444f-a14e-de5de36177cd";

        public static readonly Guid guidPackage = new Guid(PackageGuidString);
        public static readonly Guid guidCmdSet = new Guid(PackageCmdSetGuidString);

        public const uint cmdidNavigateToChangelist = 0x0100;
        public const uint cmdidShowHistory = 0x1005;
        public const uint cmdidTimeLapseView = 0x1010;
    }
}
