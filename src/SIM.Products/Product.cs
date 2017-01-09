﻿namespace SIM.Products
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Xml;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using Ionic.Zip;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #endregion

  [Serializable]
  public class Product : IXmlSerializable
  {
    #region Constants

    public const string ManifestPrefix = "/manifest/";

    private const string ProductNamePattern = @"([a-zA-Z][a-zA-Z\d\-\s\._]*[a-zA-Z0-9])";

    private const string ProductRevisionPattern = @"(\d\d\d\d\d\d[\d\w\s_\-\!\(\)]*)"; // @"(\d\d\d\d\d\d)";

    private const string ProductVersionPattern = @"(\d\.\d(\.?\d?))";

    #endregion

    #region Fields

    public static readonly XmlDocumentEx EmptyManifest = XmlDocumentEx.LoadXml("<manifest version=\"1.4\" />");

    public static readonly string ProductFileNamePattern = ProductHelper.Settings.CoreProductNamePattern.Value.EmptyToNull() ?? ProductNamePattern + @"[\s]?[\-_]?[\s]?" + ProductVersionPattern + @"[\s\-]*(rev\.|build)[\s]*" + ProductRevisionPattern + @"(.zip)?$";

    public static readonly Regex ProductRegex = new Regex(ProductFileNamePattern, RegexOptions.IgnoreCase);

    public static readonly Product Undefined = new Product()
    {
      Name = "Undefined", 
      OriginalName = "Undefined", 
      PackagePath = string.Empty, 
      IsStandalone = true, 
      Version = string.Empty, 
      ShortName = "undefined", 
      Revision = string.Empty
    };

    private bool? isPackage;
    private bool? isStandalone;
    private XmlDocument manifest;
    private string name;
    private string packagePath;
    private Dictionary<bool, string> readme;
    private string shortName;
    private string shortVersion;
    private int? sortOrder;

    #endregion

    #region Public properties

    public bool SkipPostActions
    {
      get
      {
        XmlDocument document = this.Manifest;
        if (document == null)
        {
          return false;
        }

        XmlElement install = document.SelectSingleElement(ManifestPrefix + "package/install/postStepActions");
        if (install != null)
        {
          var skip = install.GetAttribute("skipStandard");
          if (!string.IsNullOrEmpty(skip) && skip.EqualsIgnoreCase("true"))
          {
            return true;
          }
        }

        return false;
      }
    }

    public virtual int SortOrder
    {
      get
      {
        return this.sortOrder ?? (int)(this.sortOrder = int.Parse(this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/sortOrder")).With(m => m.InnerText.EmptyToNull()) ?? "0"));
      }
    }

    #endregion

    #region Properties

    // Used when computing default standalone instance name
    #region Fields

    public static readonly XmlDocumentEx ArchiveManifest = XmlDocumentEx.LoadXml(@"<manifest version=""1.4"">
  <archive>
    <install>
      <actions>
        <extract />
      </actions>
    </install>
  </archive>
</manifest>");

    public static readonly XmlDocumentEx PackageManifest = XmlDocumentEx.LoadXml(@"<manifest version=""1.4"">
  <package />
</manifest>");
    private bool? isArchive;
    private string searchToken;
    public static readonly IServiceClient Service = new ServiceClient();

    [CanBeNull]
    private IRelease release;

    private bool unknownRelease;

    #endregion

    #region Public properties

    [NotNull]
    public string DefaultFolderName
    {
      get
      {
        return this.FormatString(ProductHelper.Settings.CoreProductRootFolderNamePattern.Value);
      }
    }

    [NotNull]
    public string DefaultInstanceName
    {
      get
      {
        return this.FormatString(ProductHelper.Settings.CoreInstallInstanceNamePattern.Value);
      }
    }

    public bool IsArchive
    {
      get
      {
        return (bool)(this.isArchive ??
                      (
                        this.isArchive =
                          !string.IsNullOrEmpty(this.PackagePath) && (this.Manifest.With(x => x.SelectSingleElement(ManifestPrefix + "archive")) != null || !this.IsPackage && !this.IsStandalone)
                        ));
      }
    }

    public bool IsPackage
    {
      get
      {
        return (bool)(this.isPackage ?? (this.isPackage = GetIsPackage(this.PackagePath, this.Manifest)));
      }
    }

    public bool IsStandalone
    {
      get
      {
        return (bool)(this.isStandalone ?? (this.isStandalone = this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "standalone")) != null));
      }

      set
      {
        this.isStandalone = value;
      }
    }

    [CanBeNull]
    public string Label
    {
      get
      {
        return this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/label").With(x => x.InnerText)) ?? this.Release.With(x => x.Label);
      }
    }

    [CanBeNull]
    public IRelease Release
    {
      get
      {
        var release = this.release;
        if (release != null)
        {
          return release;
        }

        if (this.unknownRelease || this.Name != "Sitecore CMS")
        {
          return null;
        }

        release = Service.GetVersions("Sitecore CMS").FirstOrDefault(z => z.MajorMinor == this.Version).Releases.TryGetValue(this.Revision);

        if (release == null)
        {
          this.unknownRelease = true;

          return null;
        }

        this.release = release;

        return release;
      }
    }

    [CanBeNull]
    public XmlDocument Manifest
    {
      get
      {
        if (this.manifest == null)
        {
          var packageFile = this.PackagePath;
          if (string.IsNullOrEmpty(packageFile))
          {
            return EmptyManifest;
          }

          // get from cache
          const string cacheName = "manifest";
          var text = CacheManager.GetEntry(cacheName, packageFile);
          if (text != null)
          {
            this.manifest = XmlDocumentEx.LoadXml(text);
            return this.manifest;
          }

          this.manifest = ManifestHelper.Compute(packageFile, this.OriginalName);

          // put to cache
          CacheManager.SetEntry(cacheName, packageFile, this.manifest.OuterXml);
        }

        return this.manifest;
      }
    }

    [NotNull]
    public string Name
    {
      get
      {
        return this.name ?? (this.name = this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/name")).With(x => x.InnerText.EmptyToNull()) ?? this.NormalizeName(this.OriginalName));
      }

      set
      {
        this.name = this.NormalizeName(value);
      }
    }

    public string OriginalName { get; set; }

    [NotNull]
    public string PackagePath
    {
      get
      {
        return this.packagePath;
      }

      set
      {
        this.packagePath = value;
        this.isPackage = GetIsPackage(value, this.Manifest);
      }
    }

    public Dictionary<bool, string> Readme
    {
      get
      {
        if (this.readme == null)
        {
          return this.readme = this.GetReadme();
        }

        return this.readme;
      }
    }

    [NotNull]
    public string Revision { get; set; }

    [NotNull]
    [UsedImplicitly]
    public string RevisionAndLabel
    {
      get
      {
        var label = this.Label;
        if (!string.IsNullOrEmpty(label))
        {
          return this.Revision + " - " + label;
        }

        return this.Revision;
      }
    }

    public string SearchToken
    {
      get
      {
        return this.searchToken ?? (this.searchToken = this.ShortName + this.shortVersion + this.ToString());
      }
    }

    [NotNull]
    public string ShortName
    {
      get
      {
        return this.shortName ??
               (this.shortName = this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/shortName")).With(m => m.InnerText.EmptyToNull()) ??
                                 this.Name.Split(' ')
                                   .Aggregate(string.Empty, 
                                     (current, word) =>
                                       current +
                                       (word.Length > 0 ? word[0].ToString(CultureInfo.InvariantCulture) : string.Empty))
                                   .ToLower());
      }

      set
      {
        this.shortName = value;
      }
    }

    [NotNull]
    public string ShortVersion
    {
      get
      {
        return this.shortVersion ?? (this.shortVersion = this.Version.Replace(".", string.Empty));
      }
    }

    [NotNull]
    public string Version { get; set; }

    [NotNull]
    public string VersionAndRevision
    {
      get
      {
        const string Pattern = "{0} rev. {1}";
        var longVersion = this.Version.Length == "7.0".Length ? this.Version + ".0" : this.Version;
        return Pattern.FormatWith(longVersion, this.Revision).TrimEnd(" rev. ");
      }
    }

    [NotNull]
    public string UpdateOrRevision
    {
      get
      {
        var label = this.Label;
        var revision = "rev" + this.Revision;
        if (string.IsNullOrEmpty(label))
        {
          return revision;
        }

        if (label.EqualsIgnoreCase("initial release"))
        {
          return "u0";
        }

        var pos = label.Length - 1;
        if (!char.IsDigit(label[pos]))
        {
          Log.Warn(string.Format("Strange label: " + label));

          return revision;
        }

        while (pos >= 0 && char.IsDigit(label[pos]))
        {
          pos--;
        }
        pos++;

        var num = int.Parse(label.Substring(pos));
        if (label.StartsWith("update-", StringComparison.OrdinalIgnoreCase))
        {
          return "u" + num;
        }

        if (label.StartsWith("service pack-", StringComparison.OrdinalIgnoreCase))
        {
          return "sp" + num;
        }

        Log.Warn(string.Format("Strange label: " + label));

        return revision;
      }
    }

    #endregion

    #region Private methods

    private string NormalizeName(string name)
    {
      var result = string.Empty;
      var words = name.Split();
      foreach (var word in words)
      {
        if (!string.IsNullOrEmpty(word))
        {
          result += " " + char.ToUpperInvariant(word[0]) + (word.Length > 1 ? word.Substring(1) : string.Empty);
        }
      }

      return result.TrimStart();
    }

    #endregion

    #endregion

    #region Public Methods

    #region Public properties

    [NotNull]
    public string DisplayName
    {
      get
      {
        const string Pattern = "{0} {1} rev. {2}";
        return Pattern.FormatWith(this.Name, this.Version, this.Revision).TrimEnd(" rev. ").TrimEnd(' ');
      }
    }

    public int Update
    {
      get
      {
        var version = Service.GetVersions("Sitecore CMS").FirstOrDefault(x => x.MajorMinor.StartsWith(this.Version) || this.Version.StartsWith(x.MajorMinor));
        var releases = version.Releases.Values.OrderBy(x => x.Version.Revision).ToArray();
        for (int i = 0; i < releases.Length; i++)
        {
          if (Revision.Equals(releases[i].Version.Revision))
          {
            return i;
          }
        }

        return 0;
      }
    }

    #endregion

    #region Public methods

    [NotNull]
    public static Product Parse([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      Product product;
      return TryParse(path, out product) && product != null ? product : Undefined;
    }

    public static bool TryParse([NotNull] string packagePath, [CanBeNull] out Product product)
    {
      Assert.ArgumentNotNull(packagePath, nameof(packagePath));

      product = null;
      Match match = ProductRegex.Match(packagePath);
      if (!match.Success)
      {
        return false;
      }

      return TryParse(packagePath, match, out product);
    }

    public bool IsMatchRequirements([NotNull] Product instanceProduct)
    {
      Assert.ArgumentNotNull(instanceProduct, nameof(instanceProduct));

      // !ProductHelper.Settings.InstallModulesCheckRequirements.Value&& 
      if (!this.Name.EqualsIgnoreCase("Sitecore Analytics"))
      {
        return true;
      }

      foreach (XmlElement product in this.Manifest.SelectElements(ManifestPrefix + "*/requirements/product"))
      {
        IEnumerable<XmlElement> rules = product.ChildNodes.OfType<XmlElement>().ToArray();
        foreach (IGrouping<string, XmlElement> group in rules.GroupBy(ch => ch.Name.ToLower()))
        {
          var key = group.Key;
          switch (key)
          {
            case "version":
              if (rules.Where(r => r.Name.ToLower() == key).All(s => !this.VersionMatch(instanceProduct, s.GetAttribute("value"), s)))
              {
                return false;
              }

              break;
            case "revision":
              if (rules.Where(r => r.Name.ToLower() == key).All(s => !this.RevisionMatch(instanceProduct, s.GetAttribute("value"))))
              {
                return false;
              }

              break;
          }
        }
      }

      return true;
    }

    public override string ToString()
    {
      return this.DisplayName;
    }

    #endregion

    #region Private methods

    private Dictionary<bool, string> GetReadme()
    {
      var readmeNode = this.Manifest.GetElementsByTagName("readme")[0];
      if (readmeNode != null && !string.IsNullOrEmpty(readmeNode.InnerText))
      {
        return new Dictionary<bool, string>
        {
          {
            true, readmeNode.InnerText
          }
        };
      }

      var tempExtractFolderPath = Path.Combine(Directory.GetParent(this.PackagePath).FullName, "TempExtract");
      if (!Directory.Exists(tempExtractFolderPath))
      {
        Directory.CreateDirectory(tempExtractFolderPath);
      }

      using (var zip = ZipFile.Read(this.PackagePath))
      {
        ZipEntry readmeEntry;

        var packageEntry = zip["package.zip"];

        if (packageEntry != null)
        {
          packageEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);

          using (var packageZip = ZipFile.Read(Path.Combine(tempExtractFolderPath, "package.zip")))
          {
            readmeEntry = packageZip["metadata/sc_readme.txt"];
            if (readmeEntry != null)
            {
              readmeEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);
            }
          }
        }
        else
        {
          readmeEntry = zip["metadata/sc_readme.txt"];
          if (readmeEntry != null)
          {
            readmeEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);
          }
        }
      }

      string readmeText;

      var path = Path.Combine(tempExtractFolderPath, "metadata", "sc_readme.txt");
      try
      {
        readmeText = File.ReadAllText(path);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"An error occurred during extracting readme text from {path}");
        readmeText = string.Empty;
      }

      Directory.Delete(tempExtractFolderPath, true);
      return new Dictionary<bool, string>
      {
        {
          false, readmeText
        }
      };
    }

    #endregion

    #endregion

    #region Methods

    private static bool TryParse([NotNull] string packagePath, [NotNull] Match match, [CanBeNull] out Product product)
    {
      Assert.ArgumentNotNull(packagePath, nameof(packagePath));
      Assert.ArgumentNotNull(match, nameof(match));

      product = null;
      if (!FileSystem.FileSystem.Local.File.Exists(packagePath))
      {
        packagePath = null;
      }

      var originalName = match.Groups[1].Value;
      var name = originalName;
      string shortName = null;
      var version = match.Groups[2].Value;
      var revision = match.Groups[5].Value;

      if (name.EqualsIgnoreCase("sitecore") && version[0] == '5')
      {
        return false;
      }

      ;

      product = ProductManager.Products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && p.OriginalName.Equals(originalName) && p.ShortName.EqualsIgnoreCase(shortName) && p.Revision.EqualsIgnoreCase(revision))
                ?? new Product
                {
                  OriginalName = originalName, 
                  PackagePath = packagePath, 
                  Version = version, 
                  Revision = revision
                };

      return true;
    }

    [NotNull]
    private string FormatString([NotNull] string pattern)
    {
      Assert.ArgumentNotNull(pattern, nameof(pattern));

      return pattern.Replace("{ShortName}", this.ShortName).Replace("{Name}", this.Name).Replace("{ShortVersion}", this.ShortVersion).Replace("{Version}", this.Version).Replace("{Revision}", this.Revision)
        .Replace("{UpdateOrRevision}", this.UpdateOrRevision);
    }

    private bool RevisionMatch([NotNull] Product instanceProduct, [NotNull] string revision)
    {
      Assert.ArgumentNotNull(instanceProduct, nameof(instanceProduct));
      Assert.ArgumentNotNull(revision, nameof(revision));

      if (string.IsNullOrEmpty(revision))
      {
        revision = this.Revision;
      }

      var instanceRevision = instanceProduct.Revision;
      if (instanceRevision == revision || string.IsNullOrEmpty(instanceRevision))
      {
        return true;
      }

      return false;
    }

    private bool VersionMatch([NotNull] Product instanceProduct, [NotNull] string version, [NotNull] XmlElement versionRule)
    {
      Assert.ArgumentNotNull(instanceProduct, nameof(instanceProduct));
      Assert.ArgumentNotNull(version, nameof(version));
      Assert.ArgumentNotNull(versionRule, nameof(versionRule));

      if (string.IsNullOrEmpty(version))
      {
        version = this.Version;
      }

      var instanceVersion = instanceProduct.Version;
      if (instanceVersion == version)
      {
        var rules = versionRule.SelectElements("revision").ToArray();
        if (rules.Length == 0)
        {
          return true;
        }

        if (rules.Any(s => this.RevisionMatch(instanceProduct, s.GetAttribute("value"))))
        {
          return true;
        }
      }

      return false;
    }

    #endregion

    #region Public methods

    public static Product GetFilePackageProduct(string packagePath)
    {
      return FileSystem.FileSystem.Local.File.Exists(packagePath) ? new Product
      {
        PackagePath = packagePath, 
        IsStandalone = false, 
        Name = Path.GetFileName(packagePath)
      } : null;
    }

    #endregion

    #region Private methods

    private static bool GetIsPackage(string packagePath, XmlDocument manifest)
    {
      if (string.IsNullOrEmpty(packagePath))
      {
        return false;
      }

      const string cacheName = "IsPackage";
      var path = packagePath.ToLowerInvariant();
      using (new ProfileSection("Is it package or not"))
      {
        ProfileSection.Argument("packagePath", packagePath);
        ProfileSection.Argument("manifest", manifest);

        try
        {
          // cache            
          var entry = CacheManager.GetEntry(cacheName, path);
          if (entry != null)
          {
            var result = entry.EqualsIgnoreCase("true");

            return ProfileSection.Result(result);
          }

          if (
            manifest.With(
              x =>
                x.SelectSingleElement(ManifestPrefix + "standalone") ?? x.SelectSingleElement(ManifestPrefix + "archive")) !=
            null)
          {
            CacheManager.SetEntry(cacheName, path, "false");
            return ProfileSection.Result(false);
          }

          if (
            manifest.With(
              x =>
                x.SelectSingleElement(ManifestPrefix + "package")) !=
            null)
          {
            CacheManager.SetEntry(cacheName, path, "true");

            return ProfileSection.Result(true);
          }

          CacheManager.SetEntry(cacheName, path, "false");

          return ProfileSection.Result(false);
        }
        catch (Exception e)
        {
          Log.Warn(string.Format("Detecting if the '{0}' file is a Sitecore Package failed with exception.", path, e));
          CacheManager.SetEntry(cacheName, path, "false");

          return ProfileSection.Result(false);
        }
      }
    }

    XmlSchema IXmlSerializable.GetSchema()
    {
      throw new NotImplementedException();
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      foreach (var property in this.GetType().GetProperties())
      {
        object value = property.GetValue(this, new object[0]);
        var xml = value as XmlDocument;
        if (xml != null)
        {
          writer.WriteNode(new XmlNodeReader(XmlDocumentEx.LoadXml("<Manifest>" + xml.OuterXml + "</Manifest>")), false);
          continue;
        }

        writer.WriteElementString(property.Name, string.Empty, (value ?? string.Empty).ToString());
      }
    }

    #endregion

    /// <summary>
    /// Force manifest to be reloaded from disk, to reset variable replacements.
    /// </summary>
    public void ResetManifest()
    {
      this.manifest = null;
    }
  }
}