using DNNrocketAPI.Components;
using Rocket.AppThemes.Components;
using RocketPortal.Components;
using RocketSystemProjectTemplate.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketSystemProjectTemplate.API
{
    public partial class StartConnect : DNNrocketAPI.APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private SessionParams _sessionParams;
        private UserParams _userParams;
        private AppThemeLimpet _appTheme;
        private AppThemeSystemLimpet _appThemeSystem;
        private PortalContentLimpet _portalContent;
        private PortalLimpet _portalData;
        private RemoteModule _remoteModule;
        private string _dataRef;
        private string _moduleRef;
        private string _projectName;
        private Dictionary<string, object> _dataObjects;
        private AppThemeProjectLimpet _appThemeProjectData;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.
            var storeParamCmd = paramCmd;

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var rtnDic = new Dictionary<string, object>();

            switch (paramCmd)
            {
                case "rocketsystem_edit":
                    strOut = RocketSystem();
                    break;
                case "rocketsystem_init":
                    strOut = RocketSystemInit();
                    break;
                case "rocketsystem_delete":
                    strOut = RocketSystemDelete();
                    break;
                case "rocketsystem_adminpanel":
                    strOut = AdminPanel();
                    break;
                case "rocketsystem_adminpanelheader":
                    strOut = AdminPanelHeader();
                    break;
                case "rocketsystem_save":
                    strOut = RocketSystemSave();
                    break;
                case "rocketsystem_login":
                    _userParams.TrackClear(_systemData.SystemKey);
                    strOut = ReloadPage();
                    break;



                case "rocketsystemprojecttemplate_adminpanel":
                    strOut = AdminPanel();
                    break;
                case "rocketsystemprojecttemplate_adminpanelheader":
                    strOut = AdminPanelHeader();
                    break;
                case "rocketsystemprojecttemplate_admin":
                    strOut = GetDashboard();
                    break;


                case "invalidcommand":
                    strOut = "INVALID COMMAND: " + storeParamCmd;
                    break;

            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;

        }
        private string GetSystemTemplate(string templateName)
        {
            var razorTempl = _appTheme.GetTemplate(templateName);
            if (razorTempl == "") razorTempl = _appThemeSystem.GetTemplate(templateName);
            return razorTempl;
        }
        private string RocketSystemSave()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid"); // we may have passed selection
            if (portalId >= 0)
            {
                _portalContent.Save(_postInfo);
                _portalData.Record.SetXmlProperty("genxml/systems/" + _systemData.SystemKey + "setup", "True");
                _portalData.Update();
                return RocketSystem();
            }
            return "Invalid PortalId";
        }
        private String RocketSystem()
        {
            var razorTempl = GetSystemTemplate("RocketSystem.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalContent, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private String RocketSystemInit()
        {
            var newportalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/newportalid");
            if (newportalId > 0)
            {
                _portalContent = new PortalContentLimpet(newportalId, _sessionParams.CultureCodeEdit);
                _portalContent.Active = true;
                _portalContent.Validate();
                _portalContent.Update();
            }
            return "";
        }
        private String RocketSystemDelete()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId > 0)
            {
                _portalContent = new PortalContentLimpet(portalId, _sessionParams.CultureCodeEdit);
                _portalContent.Delete();
            }
            return "";
        }
        private string AdminPanel()
        {
            var razorTempl = GetSystemTemplate("AdminPanel.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalContent, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string AdminPanelHeader()
        {
            var razorTempl = GetSystemTemplate("AdminPanelHeader.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalContent, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string ReloadPage()
        {
            // user does not have access, logoff
            UserUtils.SignOut();

            var portalAppThemeSystem = new AppThemeDNNrocketLimpet("rocketportal");
            var razorTempl = portalAppThemeSystem.GetTemplate("Reload.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string GetDashboard()
        {
            var razorTempl = GetSystemTemplate("Dashboard.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalContent, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemData = new SystemLimpet(systemInfo.GetXmlProperty("genxml/systemkey"));
            _rocketInterface = new RocketInterface(interfaceInfo);
            _sessionParams = new SessionParams(_paramInfo);
            _userParams = new UserParams(_sessionParams.BrowserSessionId);
            _passSettings = new Dictionary<string, string>();
            _appThemeProjectData = new AppThemeProjectLimpet();
            _moduleRef = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            if (_moduleRef == "") _moduleRef = _paramInfo.GetXmlProperty("genxml/remote/moduleref");

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid >= 0 && PortalUtils.GetPortalId() == 0)
            {
                _remoteModule = new RemoteModule(portalid, _moduleRef);
                _portalContent = new PortalContentLimpet(portalid, _sessionParams.CultureCodeEdit); // Portal 0 is admin, editing portal setup
            }
            else
            {
                _remoteModule = new RemoteModule(PortalUtils.GetPortalId(), _moduleRef);
                _portalContent = new PortalContentLimpet(PortalUtils.GetPortalId(), _sessionParams.CultureCodeEdit);
                if (!_portalContent.Active) return "";
            }
            _portalData = new PortalLimpet(_portalContent.PortalId);
            _appThemeSystem = new AppThemeSystemLimpet(_portalContent.PortalId, _systemData.SystemKey);
            _dataRef = _remoteModule.DataRef;

            if (_dataRef == "")
            {
                // If we are editing from the AdminPanel, we will not have a moduleRef, only a dataref.
                _dataRef = _paramInfo.GetXmlProperty("genxml/hidden/dataref");
                if (_dataRef == "") _dataRef = _paramInfo.GetXmlProperty("genxml/remote/dataref");
            }
            _projectName = _remoteModule.ProjectName;
            if (_projectName == "") _projectName = _appThemeProjectData.DefaultProjectName();

            _appTheme = new AppThemeLimpet(_portalContent.PortalId, _remoteModule.AppThemeViewFolder, _remoteModule.AppThemeViewVersion, _projectName);

            var securityData = new SecurityLimpet(_portalContent.PortalId, _systemData.SystemKey, _rocketInterface, -1, -1);

            _dataObjects = new Dictionary<string, object>();
            _dataObjects.Add("remotemodule", _remoteModule);
            _dataObjects.Add("apptheme", _appTheme);
            _dataObjects.Add("appthemesystem", _appThemeSystem);
            _dataObjects.Add("portalcontent", _portalContent);
            _dataObjects.Add("portaldata", _portalData);
            _dataObjects.Add("securitydata", securityData);
            _dataObjects.Add("systemdata", _systemData);

            if (paramCmd.StartsWith("remote_"))
            {
                var sk = _paramInfo.GetXmlProperty("genxml/remote/securitykeyedit");
                if (!UserUtils.IsEditor() && _portalData.SecurityKeyEdit != sk) paramCmd = "";
            }
            else
            {
                paramCmd = securityData.HasSecurityAccess(paramCmd, "rocketsystem_login");
            }

            return paramCmd;
        }
    }

}
