namespace BlinkJenkinsStatus
{
    public class JenkinsProperties
    {
        public enum JenkinsStatus
        {
            Unchecked,
            BuildError,
            BuildOK,
            BuildMixed
        }
    }
}