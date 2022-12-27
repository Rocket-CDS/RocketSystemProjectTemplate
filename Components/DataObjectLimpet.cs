﻿using DNNrocketAPI.Components;
using Newtonsoft.Json.Linq;
using Rocket.AppThemes.Components;
using RocketPortal.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace RocketSystemProjectTemplate.Components
{
    public class DataObjectLimpet
    {
        private Dictionary<string, object> _dataObjects;
        private Dictionary<string, string> _passSettings;
        public DataObjectLimpet(int portalid, string moduleRef, SessionParams sessionParams, bool editMode = true)
        {
            var cultureCode = sessionParams.CultureCodeEdit;
            if (!editMode) cultureCode = sessionParams.CultureCode;
            Populate(portalid, moduleRef, cultureCode, sessionParams.ModuleId, sessionParams.TabId);
        }
        public DataObjectLimpet(int portalid, string moduleRef, string cultureCode, int moduleId, int tabId)
        {
            Populate(portalid, moduleRef,  cultureCode, moduleId, tabId);
        }
        public void Populate(int portalid, string moduleRef, string cultureCode, int moduleId, int tabId)
        {
            _passSettings = new Dictionary<string, string>();
            _dataObjects = new Dictionary<string, object>();

            SetDataObject("appthemesystem", new AppThemeSystemLimpet(portalid, SystemKey));
            SetDataObject("portalcontent", new PortalContentLimpet(portalid, cultureCode));
            SetDataObject("portaldata", new PortalLimpet(portalid));
            SetDataObject("systemdata", new SystemLimpet(SystemKey));
            SetDataObject("appthemeprojects", new AppThemeProjectLimpet());
            SetDataObject("remotemodule", new ModuleContentLimpet(portalid, moduleRef));
        }
        public void SetDataObject(String key, object value)
        {
            if (_dataObjects.ContainsKey(key)) _dataObjects.Remove(key);
            _dataObjects.Add(key, value);

            if (key == "modulesettings") // load appTheme if we has settings in ModuleSettings
            {
                if (ModuleSettings.HasProject)
                {
                    SetDataObject("appthemedatalist", new AppThemeDataList(ModuleSettings.ProjectName, SystemKey));
                    if (ModuleSettings.HasAppThemeAdmin)
                    {
                        SetDataObject("appthemeadmin", new AppThemeLimpet(ModuleSettings.PortalId, ModuleSettings.AppThemeAdminFolder, ModuleSettings.AppThemeAdminVersion, ModuleSettings.ProjectName));
                    }
                    if (ModuleSettings.HasAppThemeView)
                        SetDataObject("appthemeview", new AppThemeLimpet(ModuleSettings.PortalId, ModuleSettings.AppThemeViewFolder, ModuleSettings.AppThemeViewVersion, ModuleSettings.ProjectName));
                    else
                        SetDataObject("appthemeview", new AppThemeLimpet(ModuleSettings.PortalId, ModuleSettings.AppThemeAdminFolder, ModuleSettings.AppThemeAdminVersion, ModuleSettings.ProjectName));
                }
            }
        }
        public object GetDataObject(String key)
        {
            if (_dataObjects != null && _dataObjects.ContainsKey(key)) return _dataObjects[key];
            return null;
        }
        public void SetSetting(string key, string value)
        {
            if (_passSettings.ContainsKey(key)) _passSettings.Remove(key);
            _passSettings.Add(key, value);
        }
        public string GetSetting(string key)
        {
            if (!_passSettings.ContainsKey(key)) return "";
            return _passSettings[key];
        }
        public List<SimplisityRecord> GetAppThemeProjects()
        {
            return AppThemeProjects.List;
        }
        public string SystemKey { get { return "rocketsystemprojecttemplate"; } }
        public int PortalId { get { return PortalData.PortalId; } }
        public Dictionary<string, object> DataObjects { get { return _dataObjects; } }
        public ModuleContentLimpet ModuleSettings { get { return (ModuleContentLimpet)GetDataObject("modulesettings"); } }
        public AppThemeSystemLimpet AppThemeSystem { get { return (AppThemeSystemLimpet)GetDataObject("appthemesystem"); } }
        public PortalContentLimpet PortalContent { get { return (PortalContentLimpet)GetDataObject("portalcontent"); } }
        public AppThemeLimpet AppThemeView { get { return (AppThemeLimpet)GetDataObject("appthemeview"); } set { SetDataObject("appthemeview", value); } }
        public AppThemeLimpet AppThemeAdmin { get { return (AppThemeLimpet)GetDataObject("appthemeadmin"); } set { SetDataObject("appthemeadmin", value); } }
        public PortalLimpet PortalData { get { return (PortalLimpet)GetDataObject("portaldata"); } }
        public SystemLimpet SystemData { get { return (SystemLimpet)GetDataObject("systemdata"); } }
        public AppThemeProjectLimpet AppThemeProjects { get { return (AppThemeProjectLimpet)GetDataObject("appthemeprojects"); } }
        public Dictionary<string, string> Settings { get { return _passSettings; } }

    }
}
