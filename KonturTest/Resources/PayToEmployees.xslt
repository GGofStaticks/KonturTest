<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" indent="yes"/>

  <xsl:key name="employees-by-name" match="item" use="concat(@name, '|', @surname)"/>

  <xsl:template match="/Pay">
    <Employees>
      <xsl:for-each select="//item[generate-id() = generate-id(key('employees-by-name', concat(@name, '|', @surname))[1])]">
        <Employee name="{@name}" surname="{@surname}">
          <xsl:for-each select="key('employees-by-name', concat(@name, '|', @surname))">
            <salary amount="{@amount}">
              <xsl:attribute name="mount">
                <xsl:choose>
                  <xsl:when test="local-name(..) != 'Pay'">
                    <xsl:value-of select="local-name(..)"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="@mount"/>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </salary>
          </xsl:for-each>
        </Employee>
      </xsl:for-each>
    </Employees>
  </xsl:template>
</xsl:stylesheet>
