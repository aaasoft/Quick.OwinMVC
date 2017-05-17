<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" encoding="utf-8" indent="yes" />
  <xsl:template match="Api">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta charset="UTF-8" />
        <title>
          接口文档<xsl:if test="@Tag">
            [标签:<xsl:value-of select="@Tag"/>]
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

          .plain_pre {
            color: #666;
            font-family: "Helvetica Neue", Helvetica, sans-serif;
            border: none;
            margin: 0px;
            padding: 0px;
            font-size: 14px;
            line-height: 1.2;
          }

          h2.api_index {
            color: white;
            background: #49C895;
            margin-left: -30px;
            border-left: 50px solid #299480;
            padding-left: 0;
            margin-bottom: 20px;
          }

          a.debug-a{
            color:white;
          }
          a.debug-a:hover{
            color: green;
          }
        </style>
        <script src="/Plugin.ApiDoc/resource/jquery-1.12.4/jquery.min.js" type="text/javascript">_</script>
        <script src="/Plugin.ApiDoc/resource/layer-3.0.3/layer.js" type="text/javascript">_</script>
        <script type="text/javascript">
          function invokeApi(id, name) {
            layer.open({
                type: 2,
                title: name,
                shadeClose: true,
                shade: false,
                //moveType: 1,
                maxmin: true, //开启最大化最小化按钮
                area: ['640px', '480px'],
                content: '/Plugin.ApiDoc/resource/api/debug.html?id=' + id,
                zIndex: layer.zIndex,
                success: function(layero){
                layer.setTop(layero); //重点2
              }
            });
          }
        </script>
      </head>
      <body class="page page-template-default basic">
        <div id="page">
          <div id="main" class="container">
            <div class="row">
              <div id="primary">
                <div style="float: right;">
                  <code>
                    <a href="/api/Api/Doc">全部接口文档</a>
                  </code>
                  <code>
                    <a href="/api/Api/Doc?Tag=external">对外接口文档</a>
                  </code>
                  <code>
                    <a href="/api/Api/Table">全部接口表格</a>
                  </code>
                  <code>
                    <a href="/api/Api/Table?Tag=external">对外接口表格</a>
                  </code>
                </div>
                <article class="post-530 page type-page status-publish hentry">
                  <h1 class="entry-title">
                    接口文档
                    <xsl:if test="@Tag">
                      [标签:<xsl:value-of select="@Tag"/>]
                    </xsl:if>
                  </h1>
                  <div class="h5">
                      <h2 class="api_index">目录</h2>
                      <ul>
                        <xsl:for-each select="Plugin">
                          <li>
                            <xsl:element name="a">
                              <xsl:attribute name="href">
                                <xsl:text>#Plugin:</xsl:text>
                                <xsl:value-of select="@Id"/>
                              </xsl:attribute>
                              <xsl:value-of select="@Name"/>
                            </xsl:element>
                            <ul>
                              <xsl:for-each select="Feature">
                                <li>
                                  <xsl:element name="a">
                                    <xsl:attribute name="href">
                                      <xsl:text>#Feature:</xsl:text>
                                      <xsl:value-of select="../@Id"/>
                                      <xsl:text>/</xsl:text>
                                      <xsl:value-of select="@Id"/>
                                    </xsl:attribute>
                                    <xsl:value-of select="@Name"/>
                                  </xsl:element>
                                  <ul>
                                    <xsl:for-each select="IMethod">
                                      <li>
                                        <xsl:element name="a">
                                          <xsl:attribute name="href">
                                            <xsl:text>#Method:</xsl:text>
                                            <xsl:value-of select="@Path"/>
                                            <xsl:text>:</xsl:text>
                                            <xsl:value-of select="@HttpMethod"/>
                                          </xsl:attribute>
                                          <xsl:value-of select="@Name"/>
                                        </xsl:element>
                                        <xsl:element name="a">
                                          <xsl:attribute name="href">
                                            <xsl:text>javascript:invokeApi(</xsl:text>
                                            <xsl:text>'</xsl:text>
                                            <xsl:value-of select="@Path"/>
                                            <xsl:text>:</xsl:text>
                                            <xsl:value-of select="@HttpMethod"/>
                                            <xsl:text>'</xsl:text>
                                            <xsl:text>,</xsl:text>
                                            <xsl:text>'</xsl:text>
                                            <xsl:value-of select="@Name"/>
                                            <xsl:text>'</xsl:text>
                                            <xsl:text>)</xsl:text>
                                          </xsl:attribute>
                                          <xsl:attribute name="class">
                                            <xsl:text>debug-a</xsl:text>
                                          </xsl:attribute>
                                          <xsl:text>➤</xsl:text>
                                        </xsl:element>
                                      </li>
                                    </xsl:for-each>
                                  </ul>
                                </li>
                              </xsl:for-each>
                            </ul>
                          </li>
                        </xsl:for-each>
                      </ul>
                  </div>

                  <div class="entry-content">
                    <xsl:for-each select="Plugin">
                      <h2>
                        <xsl:element name="a">
                          <xsl:attribute name="name">
                            <xsl:text>Plugin:</xsl:text>
                            <xsl:value-of select="@Id"/>
                          </xsl:attribute>
                        </xsl:element>
                        <xsl:value-of select="@Name"/>
                      </h2>
                      <div>
                        <xsl:for-each select="Feature">
                          <h3>
                            <xsl:element name="a">
                              <xsl:attribute name="name">
                                <xsl:text>Feature:</xsl:text>
                                <xsl:value-of select="../@Id"/>
                                <xsl:text>/</xsl:text>
                                <xsl:value-of select="@Id"/>
                              </xsl:attribute>
                            </xsl:element>
                            <xsl:value-of select="@Name"/>
                          </h3>
                          <div>
                            <xsl:for-each select="IMethod">
                              <h4>
                                <xsl:element name="a">
                                  <xsl:attribute name="name">
                                    <xsl:text>Method:</xsl:text>
                                    <xsl:value-of select="@Path"/>
                                    <xsl:text>:</xsl:text>
                                    <xsl:value-of select="@HttpMethod"/>
                                  </xsl:attribute>
                                </xsl:element>
                                <xsl:value-of select="@Name"/>
                              </h4>
                              <p style="padding-left: 0px;">
                                <xsl:if test="Tags">
                                  <img style="display: inline;box-shadow: none;border-style: none; margin: 0px 10px 0px 0px;padding:0px" src="/Plugin.ApiDoc/resource/api/tags.png"></img>
                                  <xsl:for-each select="Tags/Tag">
                                    <code>
                                      <xsl:element name="a">
                                        <xsl:attribute name="target">
                                          <xsl:text>_blank</xsl:text>
                                        </xsl:attribute>
                                        <xsl:attribute name="href">
                                          <xsl:text>?Tag=</xsl:text>
                                          <xsl:value-of select="@Name"/>
                                        </xsl:attribute>
                                        <xsl:value-of select="@Name"/>
                                      </xsl:element>
                                    </code>
                                  </xsl:for-each>
                                </xsl:if>
                              </p>
                              <h5>基本信息</h5>
                              <table>
                                <xsl:if test="@Description!=''">
                                  <tr>
                                    <th style="width: 200px;">描述</th>
                                    <td>
                                      <xsl:choose>
                                        <xsl:when test="contains(@Description, '&#10;')">
                                          <pre>
                                            <xsl:value-of select="@Description"/>
                                          </pre>
                                        </xsl:when>
                                        <xsl:otherwise>
                                          <xsl:value-of select="@Description"/>
                                        </xsl:otherwise>
                                      </xsl:choose>
                                    </td>
                                  </tr>
                                </xsl:if>
                                <tr>
                                  <th style="width: 200px;">地址</th>
                                  <td>
                                    <xsl:value-of select="@Path"/>
                                  </td>
                                </tr>
                                <tr>
                                  <th style="width: 200px;">请求方式</th>
                                  <td>
                                    <xsl:element name="a">
                                      <xsl:attribute name="href">
                                        <xsl:text>javascript:invokeApi(</xsl:text>
                                        <xsl:text>'</xsl:text>
                                        <xsl:value-of select="@Path"/>
                                        <xsl:text>:</xsl:text>
                                        <xsl:value-of select="@HttpMethod"/>
                                        <xsl:text>'</xsl:text>
                                        <xsl:text>,</xsl:text>
                                        <xsl:text>'</xsl:text>
                                        <xsl:value-of select="@Name"/>
                                        <xsl:text>'</xsl:text>
                                        <xsl:text>)</xsl:text>
                                      </xsl:attribute>
                                      <xsl:value-of select="@HttpMethod"/>
                                    </xsl:element>
                                  </td>
                                </tr>
                                <xsl:if test="Permissions">
                                  <tr>
                                    <th style="width: 200px;">权限</th>
                                    <td>
                                      <xsl:for-each select="Permissions/Permission">
                                        <code>
                                          <xsl:value-of select="@Name"/>
                                        </code>
                                      </xsl:for-each>
                                    </td>
                                  </tr>
                                </xsl:if>
                              </table>
                              <h5>参数列表</h5>
                              <xsl:choose>
                                <xsl:when test="(Parameters) and (count(Parameters/FormFieldInfo) > 0)">
                                  <table>
                                    <tr>
                                      <th>参数</th>
                                      <th>类型</th>
                                      <th>名称</th>
                                      <th>描述</th>
                                    </tr>
                                    <xsl:for-each select="Parameters/FormFieldInfo">
                                      <tr>
                                        <td>
                                          <xsl:value-of select="@Key"/>
                                        </td>
                                        <td>
                                          <xsl:value-of select="@Type"/>
                                        </td>
                                        <td>
                                          <xsl:value-of select="@Name"/>
                                        </td>
                                        <td>
                                          <pre class="plain_pre">
                                            <xsl:choose>
                                              <xsl:when test="@NotEmpty = 'True'">必需。</xsl:when>
                                              <xsl:otherwise>可选。</xsl:otherwise>
                                            </xsl:choose>
                                            <xsl:value-of select="@Description"/>
                                          </pre>
                                        </td>
                                      </tr>
                                    </xsl:for-each>
                                  </table>
                                </xsl:when>
                                <xsl:otherwise>
                                  <p>
                                    <code>无</code>
                                  </p>
                                </xsl:otherwise>
                              </xsl:choose>
                              <xsl:if test="@InvokeExample != ''">
                                <h5>调用示例</h5>
                                <pre>
                                  <xsl:value-of select="@InvokeExample"/>
                                </pre>
                              </xsl:if>

                              <xsl:if test="@ReturnValueExample != ''">
                                <h5>返回值</h5>
                                <pre>
                                  <xsl:value-of select="@ReturnValueExample"/>
                                </pre>
                              </xsl:if>
                            </xsl:for-each>
                          </div>
                        </xsl:for-each>
                      </div>
                    </xsl:for-each>
                  </div>
                </article>
              </div>
            </div>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>