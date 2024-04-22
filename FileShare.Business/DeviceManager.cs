using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using FileShare.Business.Abstraction;
using FileShare.Business.Constants;

namespace FileShare.Business;

public class DeviceManager:IDeviceManager
{
    public List<string> GetLocalDeviceIPs(string? subnetMask = default)
    {
        string IP = GetLocalIPAddress();
        int[] loopCounts = LoopCounts(subnetMask);
        for (int i = 0; i < loopCounts[0]; i++)
        {
            string subIP = IP[..IP.LastIndexOf('.')];
            string lastOcta = subIP[(subIP.LastIndexOf('.') + 1)..];
            lastOcta = (int.Parse(lastOcta) + i).ToString();
            subIP = subIP[..(subIP.LastIndexOf('.') + 1)];
            subIP += lastOcta + ".";
            for (int j = 1; j < loopCounts[1]; j++)
            {
                string fullIP = subIP + j;
            }
        }
        
        return new List<string>();
    }

    public string? GetSubnetMask()
    {
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        
        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.Name == "en0")
            {
                IPInterfaceProperties properties = networkInterface.GetIPProperties();
                
                IPv4InterfaceProperties ipv4Properties = properties.GetIPv4Properties();
                if (ipv4Properties != null)
                {
                    foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.IPv4Mask.ToString();
                        }
                    }
                }
            }
        }
        
        throw new NetworkInformationException();
    }
    
    static string GetLocalIPAddress()
    {
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        
        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                var ipProperties = networkInterface.GetIPProperties();
                var ipAddress = ipProperties.UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                if (ipAddress != null)
                {
                    return ipAddress.Address.ToString();
                }
            }
        }

        throw new Exception($"{ErrorMessages.LocalIPNotFound} \n {ErrorMessages.DNSInnerExceptionMessage}");
    }

    private int[] LoopCounts(string? subnetMask)
    {
        int[] loopCounts = new int[2];
        if (subnetMask == null)
        {
            loopCounts[0] = 1;
            loopCounts[1] = 256;
        }
        else
        {
            string[] subnetMaskSectors = subnetMask.Split('.');
            for (int i = 2; i < subnetMaskSectors.Length; i++)
            {
                loopCounts[i - 2] = 256 - int.Parse(subnetMaskSectors[i]);
            }
        }

        return loopCounts;
    }
}