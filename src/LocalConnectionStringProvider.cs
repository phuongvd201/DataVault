using System.Collections.Generic;
using System.Linq;

using DataVault.Common.Extensions;
using DataVault.Internal;

using Renci.SshNet;

namespace DataVault
{
    public class LocalConnectionStringProvider : IConnectionStringProvider
    {
        private const string SshHost = ".";
        private const string SshUsername = ".";
        private const string SshPrivateKeyFileName = ".";

        private const string DvHost = ".";
        private const int DvPort = 3306;
        private const string DvUser = "master";
        private const string DvPassword = ".";
        private const string DvDatabase = ".";

        private const string BoundHost = "127.0.0.1";
        private const int BoundPort = 9002;

        private static bool openedSsh;
        private static readonly object LockObject = new object();

        public string GetConnectionString()
        {
            return GetConnectionString(DvDatabase);
        }

        public string GetConnectionString(string databaseName)
        {
            if (!openedSsh)
            {
                ConnectSsh();
            }

            return new Dictionary<string, string>
                {

                    { "server", BoundHost },
                    { "database", databaseName },
                    { "UID", DvUser },
                    { "password", DvPassword },
                    { "port", BoundPort.ToString() },
                }
                .Select(x => $"{x.Key}={x.Value}")
                .JoinNotEmpty(";");
        }

        private static void ConnectSsh()
        {
            var connectionInfo = new ConnectionInfo(
                SshHost,
                SshUsername,
                new PrivateKeyAuthenticationMethod(SshUsername, new PrivateKeyFile(SshPrivateKeyFileName)));

            var sshClient = new SshClient(connectionInfo);

            sshClient.Connect();

            ForwardedPortLocal portFwd = new ForwardedPortLocal(BoundHost, BoundPort, DvHost, DvPort);

            if (!sshClient.ForwardedPorts.Contains(portFwd))
            {
                sshClient.AddForwardedPort(portFwd);
                portFwd.Start();
            }

            lock (LockObject)
            {
                openedSsh = true;
            }
        }
    }
}