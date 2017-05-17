<?xml version="1.0" encoding="utf-8"?>
<html xsl:version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">
  <head>
    <meta charset="UTF-8" />
    <title>
      接口表格
      <xsl:if test="Api/@Tag">
        [标签:<xsl:value-of select="Api/@Tag"/>]
      </xsl:if>
    </title>
    <link href="/Plugin.ApiDoc/resource/api/style.css" rel="stylesheet" />
    <style>
      body {
      padding: 0px;
      }
      article {
      padding: 20px 0 60px;
      margin: 0;
      }
      code {
      margin-right: 10px;
      }
      .entry-content {
      line-height: 1.2;
      }
    </style>
  </head>
  <body class="page page-template-default basic">
    <div id="page">
      <div id="main" class="container">
        <div class="row">
          <div id="primary">
            <article class="post-530 page type-page status-publish hentry">
              <h1 class="entry-title">
                接口表格
                <xsl:if test="Api/@Tag">
                  [标签:<xsl:value-of select="Api/@Tag"/>]
                </xsl:if>
              </h1>

              <div class="entry-content">
                <table>
                  <thead>
                    <tr>
                      <th>编号</th>
                      <th>名称</th>
                    </tr>
                    <tr>
                      <td colspan="2"></td>
                    </tr>
                  </thead>
                  <tbody>
                    <xsl:for-each select="Api/Plugin">
                      <tr>
                        <th colspan="2" style="text-align: center;">
                          <p>
                            <xsl:element name="a">
                              <xsl:attribute name="name">
                                <xsl:text>Plugin:</xsl:text>
                                <xsl:value-of select="@Id"/>
                              </xsl:attribute>
                            </xsl:element>
                            <b>
                              <xsl:value-of select="@Name"/>
                            </b>
                          </p>
                        </th>
                      </tr>
                      <xsl:for-each select="Feature">
                        <tr>
                          <td colspan="2">
                            <p>
                              <xsl:element name="a">
                                <xsl:attribute name="name">
                                  <xsl:text>Feature:</xsl:text>
                                  <xsl:value-of select="../@Id"/>
                                  <xsl:text>/</xsl:text>
                                  <xsl:value-of select="@Id"/>
                                </xsl:attribute>
                              </xsl:element>
                              <b>
                                <xsl:value-of select="@Name"/>
                              </b>
                            </p>
                          </td>
                        </tr>
                        <xsl:for-each select="IMethod">
                          <tr>
                            <td>
                              <xsl:value-of select="@Path"/>
                              <xsl:text>:</xsl:text>
                              <xsl:value-of select="@HttpMethod"/>
                            </td>
                            <td>
                              <xsl:element name="a">
                                <xsl:attribute name="name">
                                  <xsl:text>Method:</xsl:text>
                                  <xsl:value-of select="@Path"/>
                                  <xsl:text>:</xsl:text>
                                  <xsl:value-of select="@HttpMethod"/>
                                </xsl:attribute>
                              </xsl:element>
                              <xsl:value-of select="@Name"/>
                            </td>
                          </tr>
                        </xsl:for-each>
                      </xsl:for-each>
                    </xsl:for-each>
                  </tbody>
                </table>
              </div>
            </article>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>