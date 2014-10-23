<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="i[.='typedef']">
    <typedef>
      <xsl:call-template name="parse-type">
        <xsl:with-param name="node" select="following::node"/>
      </xsl:call-template>
    </typedef>
  </xsl:template>

  <xsl:template name="parse-type">
    <xsl:choose>
      <xsl:when test=""
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
