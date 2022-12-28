using DNNrocketAPI.Components;
using Rocket.AppThemes.Components;
using RocketPortal.Components;
using RocketSystemProjectTemplate.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace RocketSystemProjectTemplate.API
{
    public partial class StartConnect : DNNrocketAPI.APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private SessionParams _sessionParams;
        private DataObjectLimpet _dataObject;
        private string _moduleRef;
        private int _moduleId;
        private int _tabId;

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
                    strOut = ReloadPage();
                    break;



                case "rocketsystemprojecttemplate_adminpanel":
                    strOut = AdminPanel();
                    break;
                case "rocketsystemprojecttemplate_adminpanelheader":
                    strOut = AdminPanelHeader();
                    break;
                case "rocketsystemprojecttemplate_admin":
                    strOut = RenderSystemTemplate("Dashboard.cshtml");
                    break;


                case "invalidcommand":
                    strOut = "INVALID COMMAND: " + storeParamCmd;
                    break;

            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;

        }
        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid == 0) portalid = PortalUtils.GetCurrentPortalId();

            _rocketInterface = new RocketInterface(interfaceInfo);
            _sessionParams = new SessionParams(_paramInfo);
            _moduleRef = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            _tabId = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid");
            _moduleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

            _dataObject = new DataObjectLimpet(portalid, _sessionParams.ModuleRef, _sessionParams);
            if (paramCmd.StartsWith("rocketsystem_") && UserUtils.IsSuperUser()) return paramCmd;
            if (_dataObject.PortalContent.PortalId != 0 && !_dataObject.PortalContent.Active) return "";
            if (paramCmd.StartsWith("remote_public")) return paramCmd;

            if (paramCmd.StartsWith("remote_"))
            {
                var sk = _paramInfo.GetXmlProperty("genxml/remote/securitykeyedit");
                if (!UserUtils.IsEditor() && _dataObject.PortalData.SecurityKeyEdit != sk) paramCmd = "";
            }
            else
            {
                var securityData = new SecurityLimpet(_dataObject.PortalId, _dataObject.SystemKey, _rocketInterface, _sessionParams.TabId, _sessionParams.ModuleId);
                paramCmd = securityData.HasSecurityAccess(paramCmd, "rocketsystem_login");
            }

            return paramCmd;
        }
    }

}
