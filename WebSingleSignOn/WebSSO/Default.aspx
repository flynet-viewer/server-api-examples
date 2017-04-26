<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebSSO.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="LblUsername" runat="server" Text="Username:" />
            <asp:TextBox ID="TxtBoxUsername" runat="server" />
        </div>
        <div>
            <asp:Label ID="LblPassword" runat="server" Text="Password:" />
            <asp:TextBox ID="TxtBoxPassword" runat="server" TextMode="Password" />
        </div>
        <div>
            <asp:Button ID="BtnLogin" runat="server" Text="Login" OnClick="BtnLogin_Click" />
        </div>
        <div>
            <asp:Label ID="LblErrorMessage" runat="server" ForeColor="Red" />
        </div>
    </form>
</body>
</html>
