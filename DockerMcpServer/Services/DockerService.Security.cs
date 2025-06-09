using Docker.DotNet.Models;

namespace DockerMcpServer.Services;

/// <summary>
/// Security-related operations for DockerService
/// </summary>
public partial class DockerService
{
    #region Security Operations

    /// <summary>
    /// Validates and parses security options
    /// </summary>
    public static List<string> ValidateSecurityOptions(List<string>? securityOpts)
    {
        if (securityOpts == null || securityOpts.Count == 0)
            return [];

        var validatedOptions = new List<string>();
        var allowedPrefixes = new[]
        {
            "apparmor:",
            "seccomp:",
            "label:",
            "no-new-privileges",
            "systempaths=",
            "proc-opts="
        };

        foreach (var opt in securityOpts)
        {
            if (string.IsNullOrWhiteSpace(opt))
                continue;

            var isValid = allowedPrefixes.Any(prefix => opt.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            
            if (isValid)
            {
                validatedOptions.Add(opt);
            }
            else
            {
                throw new ArgumentException($"Invalid security option: {opt}. Allowed prefixes: {string.Join(", ", allowedPrefixes)}");
            }
        }

        return validatedOptions;
    }

    /// <summary>
    /// Validates and parses capability additions
    /// </summary>
    public static List<string> ValidateCapabilities(List<string>? capabilities)
    {
        if (capabilities == null || capabilities.Count == 0)
            return [];

        var validatedCaps = new List<string>();
        var knownCapabilities = GetKnownCapabilities();

        foreach (var cap in capabilities)
        {
            if (string.IsNullOrWhiteSpace(cap))
                continue;

            var normalizedCap = cap.ToUpperInvariant();
            if (!normalizedCap.StartsWith("CAP_"))
                normalizedCap = "CAP_" + normalizedCap;

            if (knownCapabilities.Contains(normalizedCap) || normalizedCap == "ALL")
            {
                validatedCaps.Add(normalizedCap);
            }
            else
            {
                throw new ArgumentException($"Unknown capability: {cap}. Use 'ALL' or one of the known Linux capabilities.");
            }
        }

        return validatedCaps;
    }

    /// <summary>
    /// Gets list of known Linux capabilities
    /// </summary>
    public static HashSet<string> GetKnownCapabilities()
    {
        return
        [
            "CAP_CHOWN",
            "CAP_DAC_OVERRIDE",
            "CAP_DAC_READ_SEARCH",
            "CAP_FOWNER",
            "CAP_FSETID",
            "CAP_KILL",
            "CAP_SETGID",
            "CAP_SETUID",
            "CAP_SETPCAP",
            "CAP_LINUX_IMMUTABLE",
            "CAP_NET_BIND_SERVICE",
            "CAP_NET_BROADCAST",
            "CAP_NET_ADMIN",
            "CAP_NET_RAW",
            "CAP_IPC_LOCK",
            "CAP_IPC_OWNER",
            "CAP_SYS_MODULE",
            "CAP_SYS_RAWIO",
            "CAP_SYS_CHROOT",
            "CAP_SYS_PTRACE",
            "CAP_SYS_PACCT",
            "CAP_SYS_ADMIN",
            "CAP_SYS_BOOT",
            "CAP_SYS_NICE",
            "CAP_SYS_RESOURCE",
            "CAP_SYS_TIME",
            "CAP_SYS_TTY_CONFIG",
            "CAP_MKNOD",
            "CAP_LEASE",
            "CAP_AUDIT_WRITE",
            "CAP_AUDIT_CONTROL",
            "CAP_SETFCAP",
            "CAP_MAC_OVERRIDE",
            "CAP_MAC_ADMIN",
            "CAP_SYSLOG",
            "CAP_WAKE_ALARM",
            "CAP_BLOCK_SUSPEND",
            "CAP_AUDIT_READ"
        ];
    }

    /// <summary>
    /// Creates a secure container configuration
    /// </summary>
    public static HostConfig CreateSecureHostConfig(
        bool readOnly = true,
        bool noNewPrivileges = true,
        List<string>? capDrop = null,
        List<string>? capAdd = null,
        List<string>? securityOpts = null)
    {
        var hostConfig = new HostConfig
        {
            ReadonlyRootfs = readOnly,
            Privileged = false
        };

        // Default security configuration
        var defaultCapDrop = capDrop ?? ["ALL"];
        var defaultSecurityOpts = securityOpts ?? [];

        if (noNewPrivileges)
        {
            defaultSecurityOpts.Add("no-new-privileges");
        }

        hostConfig.CapDrop = ValidateCapabilities(defaultCapDrop);
        hostConfig.CapAdd = ValidateCapabilities(capAdd);
        hostConfig.SecurityOpt = ValidateSecurityOptions(defaultSecurityOpts);

        return hostConfig;
    }

    /// <summary>
    /// Validates user specification for security
    /// </summary>
    public static string ValidateUserSpec(string? userSpec)
    {
        if (string.IsNullOrWhiteSpace(userSpec))
            return "1000:1000"; // Default non-root user

        // Prevent running as root
        if (userSpec == "root" || userSpec == "0" || userSpec.StartsWith("0:"))
        {
            throw new ArgumentException("Running containers as root is not recommended for security reasons. Use a non-root user.");
        }

        // Validate format: user, user:group, uid, uid:gid
        var parts = userSpec.Split(':');
        if (parts.Length > 2)
        {
            throw new ArgumentException("Invalid user specification format. Use 'user', 'user:group', 'uid', or 'uid:gid'.");
        }

        return userSpec;
    }

    /// <summary>
    /// Creates security-focused tmpfs mounts
    /// </summary>
    public static Dictionary<string, string> CreateSecureTmpfsMounts()
    {
        return new Dictionary<string, string>
        {
            ["/tmp"] = "rw,noexec,nosuid,size=100m",
            ["/var/tmp"] = "rw,noexec,nosuid,size=50m",
            ["/run"] = "rw,noexec,nosuid,size=50m"
        };
    }

    /// <summary>
    /// Validates and creates secure ulimits
    /// </summary>
    public static List<Ulimit> CreateSecureUlimits(Dictionary<string, (long soft, long hard)>? customLimits = null)
    {
        var defaultLimits = new Dictionary<string, (long soft, long hard)>
        {
            ["nofile"] = (1024, 4096),      // Open files
            ["nproc"] = (100, 200),         // Number of processes
            ["fsize"] = (100 * 1024 * 1024, 200 * 1024 * 1024), // File size (100MB/200MB)
            ["memlock"] = (64 * 1024, 64 * 1024), // Memory lock (64KB)
            ["stack"] = (8 * 1024 * 1024, 8 * 1024 * 1024)  // Stack size (8MB)
        };

        // Merge custom limits with defaults
        if (customLimits != null)
        {
            foreach (var limit in customLimits)
            {
                defaultLimits[limit.Key] = limit.Value;
            }
        }

        return defaultLimits.Select(kvp => new Ulimit
        {
            Name = kvp.Key,
            Soft = kvp.Value.soft,
            Hard = kvp.Value.hard
        }).ToList();
    }

    /// <summary>
    /// Validates device mappings for security
    /// </summary>
    public static List<DeviceMapping> ValidateDeviceMappings(List<string>? devices)
    {
        if (devices == null || devices.Count == 0)
            return [];

        var deviceMappings = new List<DeviceMapping>();
        var dangerousDevices = new[] { "/dev/mem", "/dev/kmem", "/dev/raw", "/dev/disk" };

        foreach (var device in devices)
        {
            var parts = device.Split(':');
            if (parts.Length < 2)
                throw new ArgumentException($"Invalid device mapping format: {device}. Use '/host/device:/container/device[:permissions]'");

            var hostDevice = parts[0];
            var containerDevice = parts[1];
            var permissions = parts.Length > 2 ? parts[2] : "rwm";

            // Check for dangerous devices
            if (dangerousDevices.Any(dangerous => hostDevice.StartsWith(dangerous)))
            {
                throw new ArgumentException($"Device {hostDevice} is potentially dangerous and not allowed.");
            }

            // Validate permissions
            if (!IsValidDevicePermissions(permissions))
            {
                throw new ArgumentException($"Invalid device permissions: {permissions}. Use combination of 'r', 'w', 'm'.");
            }

            deviceMappings.Add(new DeviceMapping
            {
                PathOnHost = hostDevice,
                PathInContainer = containerDevice,
                CgroupPermissions = permissions
            });
        }

        return deviceMappings;
    }

    /// <summary>
    /// Validates device permissions string
    /// </summary>
    private static bool IsValidDevicePermissions(string permissions)
    {
        if (string.IsNullOrWhiteSpace(permissions))
            return false;

        var validChars = new[] { 'r', 'w', 'm' };
        return permissions.All(c => validChars.Contains(c)) && permissions.Distinct().Count() == permissions.Length;
    }

    /// <summary>
    /// Creates secure AppArmor profile options
    /// </summary>
    public static List<string> CreateAppArmorProfile(string? profileName = null)
    {
        var securityOpts = new List<string>();
        
        if (!string.IsNullOrEmpty(profileName))
        {
            securityOpts.Add($"apparmor:{profileName}");
        }
        else
        {
            // Use Docker's default AppArmor profile
            securityOpts.Add("apparmor:docker-default");
        }

        return securityOpts;
    }

    /// <summary>
    /// Creates secure seccomp profile options
    /// </summary>
    public static List<string> CreateSeccompProfile(string? profilePath = null)
    {
        var securityOpts = new List<string>();
        
        if (!string.IsNullOrEmpty(profilePath))
        {
            if (File.Exists(profilePath))
            {
                securityOpts.Add($"seccomp:{profilePath}");
            }
            else
            {
                throw new FileNotFoundException($"Seccomp profile not found: {profilePath}");
            }
        }
        else
        {
            // Use Docker's default seccomp profile
            securityOpts.Add("seccomp:docker-default");
        }

        return securityOpts;
    }

    #endregion
}
