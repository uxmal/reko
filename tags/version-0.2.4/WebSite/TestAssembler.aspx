<%@ Page language="c#" Codebehind="TestAssembler.aspx.cs" AutoEventWireup="false" Inherits="Revenge.WebSite.TestAssembler" %>
<%@ Register tagprefix="r" namespace="Revenge.WebSite" assembly="revenge" %>
<%@ Register tagprefix="r" tagname="navbar" src="navbar.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
	<title>TestAssembler</title>
	<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" Content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="revenge.css" rel="stylesheet" type="text/css">
  </head>
  <body>
	<form id="Form1" method="post" runat="server">
	  <r:navbar runat="server" id=Navbar1 /><br>
	  Enter your assembler language code (or pick one of the preset examples). Then try decompiling it!
	  <p>
	  Samples:<br>	  
	  <asp:DropDownList ID="ddlSamples" Runat=server width="528px" autopostback=True></asp:DropDownList><br>
	  <asp:TextBox ID="txtAssembler" Runat="server" TextMode=MultiLine width="528px" rows="20"/><br>
	  <asp:Button ID="btnDecompile" Runat="server" text="Decompile"></asp:Button><br>
	  <br>
	  Assembler output:<br>
	  <pre><asp:literal ID="plcOutput" Runat="server"/></pre>
	  Decompiled output:<br>
	  <pre><asp:literal id="plcDecompiled" runat="server"/></pre>
	</form>
  </body>
</html>
