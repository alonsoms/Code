<%@ Page Title="" Language="VB" MasterPageFile="~/Content/MasterPage.Master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="UI_texavil" %>
<%@ Register assembly="DevExpress.Web.v17.2, Version=17.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<%@ Register Src="~/Content/ConsultaCE.ascx" TagPrefix="uc1" TagName="ConsultaCE" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../Scripts/jquery-1.9.1.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/jquery.mask.min.js"></script>
    <style type="text/css">
        .control_container .dxeBase, .control_container .dxeEditArea
        {
            font-family: Arial;
                       /*font-weight: bold;*/
            width: 170px;
            color: #333333;
            display: inline;
            float: left;
            margin-left: 10px;
            vertical-align: middle;
            display:block;width:100%;height:26px;padding:6px 12px;font-size:14px;line-height:1.42857143;color:#555;background-color:#fff;background-image:none;border:1px solid #ccc;border-radius:4px;-webkit-box-shadow:inset 0 1px 1px rgba(0,0,0,.075);box-shadow:inset 0 1px 1px rgba(0,0,0,.075);-webkit-transition:border-color ease-in-out .15s,-webkit-box-shadow ease-in-out .15s;-o-transition:border-color ease-in-out .15s,box-shadow ease-in-out .15s;transition:border-color ease-in-out .15s,box-shadow ease-in-out .15s
        }
        
        .control_container:focus{border-color:#66afe9;outline:0;-webkit-box-shadow:inset 0 1px 1px rgba(0,0,0,.075),0 0 8px rgba(102,175,233,.6);box-shadow:inset 0 1px 1px rgba(0,0,0,.075),0 0 8px rgba(102,175,233,.6)}
        tr.spaceUnder > td
            {
              padding-bottom: 1em;
            }

   </style> 
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ConsultaCE runat="server" ID="ConsultaCE" />
</asp:Content>

