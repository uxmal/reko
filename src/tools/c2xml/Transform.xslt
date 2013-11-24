<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

  <xsl:template match="typedef">
    <xsl:element name="typedef">
      <xsl:variable name="pos">
      <xsl:call-template name="typedef-core">
        <xsl:with-param name="item" select="$pos[1]"/>
      </xsl:call-template>
      </xsl:variable>
    </xsl:element>
  </xsl:template>

  <xsl:template name="typedef-core">
    <xsl:param name="item"></xsl:param>
    HELLO!
    <xsl:value-of select="$item"/>
  </xsl:template>
</xsl:stylesheet>
