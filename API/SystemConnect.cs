﻿using DNNrocketAPI.Components;
using Rocket.AppThemes.Components;
using RocketSystemProjectTemplate.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace RocketSystemProjectTemplate.API
{
    public partial class StartConnect
    {

        private string RenderSystemTemplate(string templateName)
        {
            var razorTempl = _dataObject.AppThemeSystem.GetTemplate(templateName);
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        private string RocketSystemSave()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid"); // we may have passed selection
            if (portalId >= 0)
            {
                _dataObject.PortalContent.Save(_postInfo);
                _dataObject.PortalData.Record.SetXmlProperty("genxml/systems/" + _dataObject.SystemKey + "setup", "True");
                _dataObject.PortalData.Update();
                return RocketSystem();
            }
            return "Invalid PortalId";
        }
        private String RocketSystem()
        {
            return RenderSystemTemplate("RocketSystem.cshtml");
        }
        private String RocketSystemInit()
        {
            var newportalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/newportalid");
            if (newportalId > 0)
            {
                var portalContent = new PortalContentLimpet(newportalId, _sessionParams.CultureCodeEdit);
                portalContent.Validate();
                portalContent.Active = true;
                portalContent.Update();
                _dataObject.SetDataObject("portalcontent", portalContent);
            }
            return "";
        }
        private String RocketSystemDelete()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId > 0)
            {
                _dataObject.PortalContent.Delete();
            }
            return "";
        }
        private string AdminPanel()
        {
            return RenderSystemTemplate("AdminPanel.cshtml");
        }
        private string AdminPanelHeader()
        {
            return RenderSystemTemplate("AdminPanelHeader.cshtml");
        }
        private string ReloadPage()
        {
            // user does not have access, logoff
            UserUtils.SignOut();
            return RenderSystemTemplate("Reload.cshtml");
        }
        private string MessageDisplay(string msgKey)
        {
            _dataObject.SetSetting("msgkey", msgKey);
            return RenderSystemTemplate("MessageDisplay.cshtml");
        }



    }
}

