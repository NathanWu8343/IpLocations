using System.Text.RegularExpressions;

namespace IpLocations.Api.Misc
{
    public static class IpConverter
    {
        public static long IpToInt(string ip)
        {
            //check
            string ipv4Pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            if (!Regex.IsMatch(ip, ipv4Pattern)) return -1;

            var octets = ip.Split('.').Select(long.Parse).ToArray();

            return octets[0] * (256 * 256 * 256)
                        + octets[1] * (256 * 256)
                        + octets[2] * 256
                        + octets[3];
        }
    }
}