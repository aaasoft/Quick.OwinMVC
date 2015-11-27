var CommonRender = {
    //包含其他文件
    include : function (config) {
        jQuery(function ($) {
            $.get(config.url, function (html) {
                var srcHtml = $('body').html();
                document.removeChild(document.all[0])
                html = html.replace("#RenderBody()", srcHtml);
                document.write(html);
                document.close();
                if (config.onReady != null)
                    config.onReady();
            });
        });
    },
    //渲染表单
    renderForm : function (data) {
        this.renderForm2({
            element: data.element,
            title: data.title,
            onDataReadyHandler: function (onDataReady) {
                jQuery.get(data.getFieldInfoUrl, function (json) {
                    onDataReady(json);
                })
            },
            submitHandler: function (form) {
                jQuery.post(data.postFormUrl, form.serialize(), function (result) {
                    var message = null;
                    if (result.hasOwnProperty("message"))
                        message = result.message;
                    if (result.code == 0) {
                        bootbox.alert(message != null ? message : "保存成功！", function () {
                            data.successHandler();
                        });
                    } else {
                        data.element.bootstrapValidator('disableSubmitButtons', false);
                        bootbox.alert(message != null ? message : "保存失败！");
                    }
                }).error(function (err) {
                    data.element.bootstrapValidator('disableSubmitButtons', false);
                    bootbox.alert("网络错误！");
                });
            }
        });
    },

    //渲染表单
    renderForm2 : function (data) {
        var renderField = function (field) {
            var html = "";
            html += '<div class="form-group">';
            html += '    <label class="control-label col-xs-12 col-sm-3 no-padding-right">{0}</label>'.format(field.Name);
            html += '    <div class="col-xs-12 col-sm-9">';
            html += '        <div class="clearfix">';

            var subHtml = "";
            if (field.ValueFormatType != null) {
                switch (field.ValueFormatType) {
                    case "DCIM3.Model.Web.ValueFormat.TextboxValueFormat": {
                        if (field.ValueFormatValue.IsPassword)
                            subHtml += '            <input type="password" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        if (field.ValueFormatValue.IsMultiLine)
                            subHtml += '            <textarea name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                    }
                        break;
                    case "DCIM3.Model.Web.ValueFormat.CheckboxValueFormat":
                        subHtml += '            <label><input name="{0}" type="hidden"><input class="ace ace-switch ace-switch-6" type="checkbox"><span class="lbl"></span></label>'.format(field.Key);
                        break;
                    case "DCIM3.Model.Web.ValueFormat.DatePickerValueFormat":
                        subHtml += '            <div class="input-group date {0} col-xs-12 col-sm-8" data-date-format="{1}">'.format(
                            field.ValueFormatValue.IsContainTime ? "form_datetime" : "form_date", field.ValueFormatValue.DateTimeFormat);
                        subHtml += '            <span class="input-group-addon"><span class="glyphicon glyphicon-th"></span></span>';
                        subHtml += '            <input class="form-control" name="{0}" type="text" />'.format(field.Key);
                        subHtml += '            </div>';
                        break;
                    case "DCIM3.Model.Web.ValueFormat.ComboboxValueFormat":
                        if (field.Value == null)
                            field.ValueFormatValue.Values.splice(0, 0, { Key: "", Value: "请选择.." });
                        subHtml += '            <select class="col-xs-12 col-sm-8" name="{0}">'.format(field.Key);
                        for (var i in field.ValueFormatValue.Values) {
                            subHtml += '                <option';
                            var option = field.ValueFormatValue.Values[i];
                            if (field.Value == option.Key)
                                subHtml += ' selected="selected"';
                            subHtml += ' value="{0}">{1}</option>'.format(option.Key, option.Value);
                        }
                        subHtml += '            </select>';
                        break;
                    case "DCIM3.Model.Web.ValueFormat.SelectValueFormat":
                        subHtml += '            <input type="hidden" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        subHtml += '            <input type="text" readonly="readonly" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        if (!field.IsReadOnly)
                            subHtml += '            <a class="btn btn-xs btn-success no-radius"><i class="icon-share-alt bigger-110"></i></a>';
                        subHtml += '            <div class="modal">';
                        break;
                }
            }
            if (subHtml == "")
                subHtml += '            <input type="text" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
            html += subHtml;
            html += '        </div>';
            html += '    </div>';
            html += '</div>';
            html += '<div class="space-2"></div>';

            var element = jQuery(html);
            if (field.IsHidden != null && field.IsHidden) {
                element.find("div").attr("style", "display: none;");
                element.find("label").remove();
            }
            //各种值格式的处理
            if (field.ValueFormatType != null) {
                switch (field.ValueFormatType) {
                    case "DCIM3.Model.Web.ValueFormat.ComboboxValueFormat":
                        if (field.IsReadOnly != null && field.IsReadOnly) {
                            element.find("select").attr("disabled", "disabled");
                        }
                        break;
                    case "DCIM3.Model.Web.ValueFormat.CheckboxValueFormat":
                        element.find("input[type='hidden']").val(field.Value);
                        element.find("input[type='checkbox']").prop("checked", field.Value);
                        element.find("input[type='checkbox']").change(function () {
                            element.find("input[type='hidden']").val($(this).is(':checked'));
                        });
                        break;
                    case "DCIM3.Model.Web.ValueFormat.TextboxValueFormat":
                        if (field.MaxLength != null)
                            element.find("input").attr("maxlength", field.ValueFormatValue.MaxLength);
                        break;
                    case "DCIM3.Model.Web.ValueFormat.DatePickerValueFormat":
                        var e = element.find(field.ValueFormatValue.IsContainTime ? ".form_datetime" : ".form_date");
                        var minView = field.ValueFormatValue.IsContainTime ? 0 : 2;
                        var maxDate = field.ValueFormatValue.MaxDate;
                        var minDate = field.ValueFormatValue.MinDate;
                        e.datetimepicker({ autoclose: true, minView: minView }).on('changeDate', function (e) {
                            if (maxDate) {
                                divElement.find("input[name='" + maxDate + "']").parent().data("datetimepicker").setStartDate(e.date);
                            }
                            if (minDate) {
                                divElement.find("input[name='" + minDate + "']").parent().data("datetimepicker").setEndDate(e.date);
                            }
                            divElement.find("form").data('bootstrapValidator')
                                      .updateStatus($(this).find("input").attr("name"), 'NOT_VALIDATED', null)
                                      .validateField($(this).find("input").attr("name"));
                        });
                        break;
                    case "DCIM3.Model.Web.ValueFormat.SelectValueFormat":
                        if (field.Value != null) {
                            element.find("input[type='hidden']").attr("value", field.Value);
                            $.get("../../base/api/select?action=names&provider={0}&ids={1}".format(field.ValueFormatValue.Provider, field.Value), function (json) {
                                element.find("input[type='text']").val(json.names);
                            });
                        }
                        var elementDiv = element.find(".modal");
                        
                        element.find("a").click(function () {
                            CommonRender.renderSelect({
                                element: elementDiv,
                                title: "请选择",
                                multiSelect: field.ValueFormatValue.MultiSelect,
                                folderSelect: field.ValueFormatValue.FolderSelect,
                                url: "../../base/api/select",
                                provider: field.ValueFormatValue.Provider,
                                id: field.ValueFormatValue.Id,
                                selectedIds: element.find("input[type='hidden']").val(),
                                onSelectHandler: function (data) {
                                    element.find("input[type='hidden']").val(data.ids);
                                    element.find("input[type='text']").val(data.names);
                                    elementDiv.modal("hide");
                                }
                            });
                            elementDiv.modal("show");
                        });
                        break;
                }
            }

            if (field.ValueFormatType == null ||
                   field.ValueFormatType == "DCIM3.Model.Web.ValueFormat.TextboxValueFormat"
                || field.ValueFormatType == "DCIM3.Model.Web.ValueFormat.DatePickerValueFormat"
                ) {
                if (field.Value != null)
                    element.find("input").attr("value", field.Value);
                if (field.Description != null)
                    element.find("input").attr("placeholder", field.Description);
                if (field.IsReadOnly != null && field.IsReadOnly) {
                    element.find("input").attr("readonly", "readonly");
                }
            }
            return element;
        }

        var divElement = $(
    ['<div class="modal-dialog" role="document">',
    '    <form class="form-horizontal">',
    '        <div class="modal-content">',
    '            <div class="modal-header">',
    '                <button type="button" class="close" aria-label="Close"><span aria-hidden="true">&times;</span></button>',
    '                <h4 class="modal-title">{0}</h4>',
    '            </div>',
    '            <div class="modal-body">',
    '                <i class="icon-refresh icon-spin blue"></i>',
    '            </div>',
    '            <div class="modal-footer">',
    '                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="icon-remove"></i>关闭</button>',
    '                <button type="submit" class="btn btn-primary"><i class="icon-save"></i>保存</button>',
    '            </div>',
    '        </div>',
    '    </form>',
    '</div>'].join("").format(data.title));
        data.element.html("");
        data.element.append(divElement);

        //绑定关闭事件
        divElement.find(".close").click(function () {
            data.element.modal("hide");
        });

        data.onDataReadyHandler(function (json) {
            var body = divElement.find(".modal-body");
            body.html("");
            var fieldsStr = "{";
            var isFirst = true;
            var IsDatePicker = false;
            for (var i in json) {

                var fieldInfo = json[i];
                body.append(renderField(fieldInfo));
                var fieldStr = '"{0}": { "validators": {'.format(fieldInfo.Key);
                var isFirstValidator = true;
                if (fieldInfo.NotEmpty != null && fieldInfo.NotEmpty) {
                    if (isFirstValidator)
                        isFirstValidator = false;
                    else
                        fieldStr += ",";
                    fieldStr += '"notEmpty": { "message": "必填！" }';
                }
                if (fieldInfo.Identical != null) {
                    if (isFirstValidator)
                        isFirstValidator = false;
                    else
                        fieldStr += ",";
                    fieldStr += '"identical": {"field": "{0}","message": "输入不一致"}'.format(fieldInfo.Identical);
                }
                if (fieldInfo.ValueFormatType != null) {
                    switch (fieldInfo.ValueFormatType) {
                        case "DCIM3.Model.Web.ValueFormat.TextboxValueFormat":
                            if (fieldInfo.ValueFormatValue.Regex != null) {
                                if (isFirstValidator)
                                    isFirstValidator = false;
                                else
                                    fieldStr += ",";

                                fieldStr += '"regexp": {"regexp":"{0}",'.format(fieldInfo.ValueFormatValue.Regex);
                                fieldStr += '"message": "{0}" } '.format(fieldInfo.Description);
                            }
                            break;
                        case "DCIM3.Model.Web.ValueFormat.DatePickerValueFormat": {
                            IsDatePicker = true;
                            if (isFirstValidator)
                                isFirstValidator = false;
                            else
                                fieldStr += ",";
                            fieldStr += '"date": { "format": "{0}",  "message":"日期格式不正确"}'.format(fieldInfo.ValueFormatValue.DateTimeFormat);
                        }
                            break;
                        case "DCIM3.Model.Web.ValueFormat.CheckboxValueFormat":
                            fieldStr = "";
                            break;
                    }
                }
                if (fieldStr == "")
                    continue;
                fieldStr += '}}';
                if (isFirst)
                    isFirst = false;
                else
                    fieldsStr += ",";
                fieldsStr += fieldStr;
            }
            fieldsStr += "}";

            var fieldsObj = JSON.parse(fieldsStr);
            divElement.find("form").bootstrapValidator({
                message: '此值无效',
                feedbackIcons: {
                    valid: 'glyphicon glyphicon-ok',
                    invalid: 'glyphicon glyphicon-remove',
                    validating: 'glyphicon glyphicon-refresh'
                },
                submitHandler: function (validator, form, submitButton) {
                    data.submitHandler(form);
                },
                fields: fieldsObj
            });
        });
    },

    //渲染表格
    renderTable : function (data) {
        return this.renderTable2({
            element: data.element,
            getListUrl: data.getListUrl,
            pagination: data.pagination,
            otherColumns: data.otherColumns,
            onHeadDataReadyHandler: function (onHeadDataReady) {
                jQuery.get(data.getHeadInfoUrl, function (json) {
                    onHeadDataReady(json);
                });
            }
        });
    },

    //渲染表格
    renderTable2: function (data) {
        data.onHeadDataReadyHandler(function (headData) {
            if (data.hasOwnProperty("otherColumns")
                && data.otherColumns != null) {
                for (var i in data.otherColumns) {
                    headData[headData.length] = data.otherColumns[i];
                }
            }

            if (data.pagination == null || data.pagination) {
                data.element.bootstrapTable({
                    method: 'get',
                    pagination: true,
                    sidePagination: "server",
                    pageSize: 10,
                    pageList: [10, 25, 50, 100],
                    oLanguage: { sZeroRecords: "找不到匹配的数据！" },
                    url: data.getListUrl,
                    columns: headData
                });
            } else {
                data.element.bootstrapTable({
                    method: 'get',
                    oLanguage: { sZeroRecords: "找不到匹配的数据！" },
                    url: data.getListUrl,
                    columns: headData
                });
            }
        });

        return {
            refresh : function () {
                data.element.bootstrapTable('refresh', {
                    url: data.getListUrl
                });
            }
        };
    },

    //渲染权限的树形选择控件
    renderSelect_Permission : function (config) {
        var url = "../../base/api/permission?ownerType=" + config.ownerType + "&ownerId=" + config.ownerId
        return this.renderSelect({
            element: config.element,
            title: "权限设置",
            multiSelect: true,
            folderSelect: false,
            url: url,
            onSelectHandler: function (data) {
                var convertItems = function (items) {
                    var rtnItems = [];
                    for (i in items) {
                        rtnItems.push({
                            Clazz: items[i].clazz,
                            Id: items[i].id
                        });
                    }
                    return JSON.stringify(rtnItems);
                }
                jQuery.post(url, { add: convertItems(data.addItems), del: convertItems(data.delItems) }, function (json) {
                    bootbox.alert(json.msg, function () {
                        config.element.modal("hide");
                    });
                }).error(function (err) {
                    bootbox.alert(err.msg);
                });
            }
        });
    },

    //渲染树形选择控件
    renderSelect : function (data) {
        if (data.selectedIds != null) {
            var selectedIds = data.selectedIds.split(",");
            data.onNewItemArrivedHandler = function (items, callback) {
                for (var i in items) {
                    var item = items[i];
                    if (selectedIds.indexOf(item.id) >= 0)
                        item.selected = true;
                }
                callback(items);
            }
        }

        var divElement = $(['<div class="modal-dialog" role="document">',
'    <div class="modal-content">',
'        <div class="modal-header">',
'                <button type="button" class="close" aria-hidden="true">',
'                    <span class="white">×</span>',
'                </button>',
'                <h4 class="modal-title">' + data.title + '</h4>',
'        </div>',
'        <div class="modal-body">',
'            <div class="widget-main padding-8">',
'                <div class="tree"></div>',
'            </div>',
'        </div>',
'        <div class="modal-footer no-margin-top">',
'            <a class="btn btn-sm btn-primary pull-left">',
'                <i class="icon-save"></i>',
'                选择',
'            </a>',
'        </div>',
'    </div>',
'</div>'].join(""));
        data.element.html("");
        data.element.append(divElement);

        var allItems = [];
        var beforeSelectedIds = [];
        var parentItem = null;

        //定义树形控件要用到的类
        var DataSourceTree = function (config) {
            this._color = config.color;
            this._callback = config.callback;
        }
        DataSourceTree.prototype.data = function (item, callback) {
            var self = this;
            this._callback(item, function (array) {
                var obj = new Object();
                for (i in array) {
                    var row = array[i];
                    row["icon-class"] = self._color;
                    if (row.selected) {
                        row["additionalParameters"] = { "item-selected": true };
                        beforeSelectedIds.push(row.id);
                    }
                    obj[row.id] = row;
                    allItems[row.id] = row;
                }
                callback({ data: obj });
            });
        };

        //初始化数据源
        var dataSourceTree = new DataSourceTree({
            color: "blue",
            callback: function (item, callback) {
                var url = data.url;
                if (url.indexOf("?") < 0)
                    url = url + "?action=nodes";
                else
                    url = url + "&action=nodes";
                //获取根结点
                if (!("name" in item) && !("type" in item)) {
                    if (data.provider != null)
                        url += '&provider=' + data.provider;
                    if (data.clazz != null)
                        url += '&clazz=' + data.clazz;
                    if (data.id != null)
                        url += '&id=' + data.id;
                    jQuery.get(url, function (json) {
                        if (data.onNewItemArrivedHandler == null)
                            callback(json);
                        else {
                            data.onNewItemArrivedHandler(json, function (items) {
                                callback(items);
                            });
                        }
                    });
                    return;
                }
                url = url + '&provider={0}&clazz={1}&id={2}'.format(item.provider, item.clazz, item.id);
                //获取子结点
                parentItem = item;
                jQuery.get(url, function (json) {
                    if (data.onNewItemArrivedHandler == null)
                        callback(json);
                    else {
                        data.onNewItemArrivedHandler(json, function (items) {
                            if (data.folderSelect != null && data.folderSelect && parentItem.type == "folder") {
                                var newItem = jQuery.extend({}, parentItem);
                                newItem.type = "item";
                                newItem.sourceName = newItem.name;
                                newItem.name = '<i class="icon-arrow-up green"></i>';
                                items.splice(0, 0, newItem);
                            }
                            callback(items);
                        });
                    }
                });
            }
        });
        //绑定关闭事件
        divElement.find(".close").click(function () {
            data.element.modal("hide");
        });

        var treeElement = divElement.find(".tree");
        //加载树    
        treeElement.ace_tree({
            dataSource: dataSourceTree,
            multiSelect: data.multiSelect,
            loadingHTML: '<div class="tree-loading"><i class="icon-refresh icon-spin blue"></i></div>',
            'selectable': true,
            'selected-icon': 'icon-ok',
            'unselected-icon': 'icon-remove'
        });
        //选择按钮点击时
        divElement.find(".btn-primary").click(function () {
            var selectedItems = treeElement.tree('selectedItems');

            var ids = [];
            var names = [];
            for (var i in selectedItems) {
                var item = selectedItems[i];
                ids.push(item.id);
                if (item.sourceName != null)
                    item.name = item.sourceName;
                names.push(item.name);
            }

            var addItems = []
            var delItems = []
            //查找新增的        
            for (i in selectedItems) {
                var selectedItem = selectedItems[i];
                if (beforeSelectedIds.indexOf(selectedItem.id) < 0)
                    addItems.push(selectedItem);
            }
            //查找删除的
            for (i in beforeSelectedIds) {
                var id = beforeSelectedIds[i];
                if (ids.indexOf(id) < 0)
                    delItems.push(allItems[id]);
            }

            data.onSelectHandler({
                selectedItems: selectedItems,
                ids: ids.join(','),
                names: names.join(','),
                addItems: addItems,
                delItems: delItems
            });
        });
    }
};