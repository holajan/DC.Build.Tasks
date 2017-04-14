using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DC.Build.Tasks
{
    public class GetCurrentBuildVersion : Task
    {
        [Output]
        public string Version { get; set; }

        public string BaseVersion { get; set; }

        public override bool Execute()
        {
            var originalVersion = System.Version.Parse(this.BaseVersion ?? "1.0.0");

            this.Version = GetCurrentBuildVersionString(originalVersion);

            return true;
        }

        private static string GetCurrentBuildVersionString(Version baseVersion)
        {
            DateTime d = DateTime.Now;
            return new Version(baseVersion.Major, baseVersion.Minor,
                (DateTime.Today - new DateTime(2000, 1, 1)).Days,
                ((int)new TimeSpan(d.Hour, d.Minute, d.Second).TotalSeconds) / 2).ToString();
        }
    }
}
