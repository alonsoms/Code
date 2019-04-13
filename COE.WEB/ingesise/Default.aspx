<%@ Page Title="" Language="VB" MasterPageFile="~/Content/MasterPage.Master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="UI_ingesise" Culture="es-PE" UICulture="es-PE" %>
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

   <%-- <form id="profileForm" runat="server" >
         <div class="jumbotron">
             <table>
                 <tr>
                     <td style="vertical-align:text-top;">
                         <img src="Logo.jpg" />
                     </td>
                 </tr>
             </table>
            
        <h3>Comprobantes Electrónicos</h3>
            <h4>
            Estimado usuario, para consultar su comprobante de pago (Facturas, Boleta de Venta, Nota de credito y Nota de debito) es necesario ingresar los siguientes datos solicitados por la SUNAT.
            </h4>
             <br />
        
              <table>
                <tr class="spaceUnder">
                    
                    <td>&nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td rowspan="6" style="vertical-align:text-top;" >
                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            
                   </td>
                </tr>
                <tr class="spaceUnder">
                    <td style="vertical-align:text-top;">
                       <dx:ASPxLabel ID="lblTipo" runat="server" Text=" Tipo de comprobante" Font-Size="Medium"></dx:ASPxLabel>
                    </td>
                    <td>&nbsp;&nbsp;</td>
                    <td>
                        <dx:ASPxComboBox ID="cboDocumentos2" runat="server" SelectedIndex="0" width="180px" style="width: 190px;height: 20px;border: 1px solid #3366FF;border-left: 4px solid #3366FF;" >
                            <Items>
                                <dx:ListEditItem Text="FACTURA" Value="01" />
                                <dx:ListEditItem Text="BOLETA DE VENTA" Value="03" />
                                <dx:ListEditItem Text="NOTA DE CREDITO" Value="07" />
                                <dx:ListEditItem Text="NOTA DE DEBITO" Value="08" />
                            </Items>
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                      
                    </td>
                </tr>
                <tr class="spaceUnder">
                    <td style="vertical-align:text-top;">
                          <dx:ASPxLabel ID="lblSerNum" runat="server" Text="Serie y número" Font-Size="Medium"></dx:ASPxLabel>
                    </td>
                      <td>&nbsp;</td>
                    <td>
                        <dx:ASPxTextBox ID="txtSerDoc" runat="server"  style="width: 190px;height: 20px;border: 1px solid #3366FF;border-left: 4px solid #3366FF;"  MaxLength="13">
                            <ValidationSettings ValidationGroup="GrupoValidacion" ErrorText="Dato es requerido" Display="Dynamic" SetFocusOnError="True">
                                <RequiredField IsRequired="True" ErrorText="" />
                                <ErrorFrameStyle Font-Size="Small">
                                </ErrorFrameStyle>
                                <RegularExpression ValidationExpression="^[F,B]{1}\d{3}(-\d{8})$" ErrorText="Formato de serie y numero" />
                            </ValidationSettings>
                        </dx:ASPxTextBox>
                        (Ej: B001-00012345)
                    </td>
                    <td>
                   
                    </td>
                </tr>

                <tr class="spaceUnder">
                    <td style="vertical-align:text-top;">
                       <dx:ASPxLabel ID="lblFecha" runat="server" Text="Fecha de emisión" Font-Size="Small"></dx:ASPxLabel>
                    </td>
                      <td>&nbsp;</td>
                    <td>
                        <dx:ASPxDateEdit ID="ASPxDateEdit1" runat="server" class="form-control" style="width: 190px;height: 20px;border: 1px solid #3366FF;border-left: 4px solid #3366FF;" Font-Size="Medium" >
                            <ValidationSettings ValidationGroup="GrupoValidacion" >
                                <RequiredField IsRequired="True" />
                            </ValidationSettings>
                        </dx:ASPxDateEdit>
                        
                    </td>
                    <td>
                       
                    </td>
                </tr>

                <tr class="spaceUnder">
                    <td style="vertical-align:text-top;">
                        <dx:ASPxLabel ID="lblMonto" runat="server" Text="Monto total" ></dx:ASPxLabel>
                    </td>
                      <td>&nbsp;</td>
                    <td>
                        <dx:ASPxSpinEdit ID="txtMonTot2" runat="server"  Number="0"  DecimalPlaces="2" DisplayFormatString="{0:N}" MaxValue="999999999" MinValue="1" width="180px" MaxLength="12" style="width: 190px;height: 20px;border: 1px solid #3366FF;border-left: 4px solid #3366FF;" >
                            <ValidationSettings ErrorText="Escriba el monto total" ValidationGroup="GrupoValidacion" >
                                <ErrorFrameStyle Font-Size="Small">
                                </ErrorFrameStyle>
                                <RequiredField IsRequired="True" ErrorText="Dato es requerido" />
                            </ValidationSettings>
                            <SpinButtons ShowIncrementButtons="false" />
                        </dx:ASPxSpinEdit>
                          (Ej: 125.05) 
                    </td>
                    <td>
                        
                    </td>
                </tr>
                <tr class="spaceUnder">
                    <td style="vertical-align: text-top;">Código de seguridad</td>
                      <td>&nbsp;</td>
                    <td colspan="2">
                        <dx:ASPxCaptcha ID="ASPxCaptcha1" runat="server" CharacterSet="123456789" CodeLength="4">
                        <ValidationSettings ErrorDisplayMode="ImageWithText" ValidationGroup="GrupoValidacion">
                            <ErrorFrameStyle Font-Size="Small">
                            </ErrorFrameStyle>
                            <RequiredField ErrorText="Dato es requerido" IsRequired="True" />
                        </ValidationSettings>
                        <TextBox LabelText="Ingrese el código de seguridad" Position="Bottom" />
                        <ChallengeImage ForegroundColor="#000000"></ChallengeImage>
                        </dx:ASPxCaptcha>
                    </td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxButton ID="btnBuscar" runat="server" Text="Consultar" Width="100px" >
                        </dx:ASPxButton>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
      </div>
   </form>--%>
</asp:Content>

