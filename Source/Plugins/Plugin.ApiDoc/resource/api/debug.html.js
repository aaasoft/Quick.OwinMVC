//字符串格式化扩展
String.prototype.format = function () {
    var result = this;
    if (arguments.length == 0)
        return null;
    for (var i = 0; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i) + '\\}', 'gm');
        result = result.replace(re, arguments[i]);
    }
    return result;
};

//获取查询参数
function getQueryParams(url) {
    var match,
        pl = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query = window.location.search.substring(1);
    if (url) query = url.substr(url.indexOf("?") + 1)

    var urlParams = {};
    while (match = search.exec(query))
        urlParams[decode(match[1])] = decode(match[2]);
    return urlParams;
}

var Debug_Main = {
    renderForm: function (data) {
        var renderField = function (field) {
            var html = "";
            html += '<div class="form-group">';
            var fieldName = field.Name;
            if (field.NotEmpty)
                fieldName += '  <b style="color:red">*</b>';
            html += '    <label class="control-label col-xs-12 col-sm-3 no-padding-right">{0}</label>'.format(fieldName);
            html += '    <div class="col-xs-12 col-sm-9">';
            html += '        <div class="clearfix">';

            var subHtml = "";
            if (field.ValueFormatType != null) {
                switch (field.ValueFormatType) {
                    case "TextboxValueFormat": {
                        if (field.ValueFormatValue.IsPassword)
                            subHtml += '            <input type="password" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        else if (field.ValueFormatValue.IsMultiLine)
                            subHtml += '            <textarea name="{0}" rows="4" class="col-xs-12 col-sm-8" ></textarea>'.format(field.Key);
                        else
                            subHtml += '            <input type="text" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                    }
                        break;
                    case "CheckboxValueFormat":
                        subHtml += '            <label><input name="{0}" type="hidden"><input class="ace ace-switch ace-switch-6" type="checkbox"><span class="lbl"></span></label>'.format(field.Key);
                        break;
                    case "DatePickerValueFormat":
                        subHtml += '            <div class="input-group date {0} col-xs-12 col-sm-8" data-date-format="{1}">'.format(
                            field.ValueFormatValue.IsContainTime ? "form_datetime" : "form_date", field.ValueFormatValue.DateTimeFormat);
                        subHtml += '            <span class="input-group-addon"><span class="glyphicon glyphicon-th"></span></span>';
                        subHtml += '            <input class="form-control" name="{0}" type="text" />'.format(field.Key);
                        subHtml += '            </div>';
                        break;
                    case "ComboboxValueFormat":
                        if (field.ValueFormatValue.Values == null)
                            field.ValueFormatValue.Values = [];
                        if (field.Value == null)
                            field.ValueFormatValue.Values.splice(0, 0, { Key: "", Value: "请选择.." });
                        subHtml += '            <select class="col-xs-12 col-sm-8" name="{0}">'.format(field.Key);
                        for (var i = 0; i < field.ValueFormatValue.Values.length; i++) {
                            subHtml += '                <option';
                            var option = field.ValueFormatValue.Values[i];
                            if (field.Value == option.Key)
                                subHtml += ' selected="selected"';
                            subHtml += ' value="{0}">{1}</option>'.format(option.Key, option.Value);
                        }
                        subHtml += '            </select>';
                        break;
                    case "SelectValueFormat":
                        subHtml += '            <input type="hidden" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        subHtml += '            <input type="text" name="{0}Name" readonly="readonly" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        if (!field.IsReadOnly)
                            subHtml += '            <a class="btn btn-xs btn-success no-radius"><i class="icon-share-alt bigger-110"></i></a>';
                        subHtml += '            <div class="modal">';
                        break;
                    case "FileValueFormat":
                        subHtml += '  <div class="input-group">'
                        subHtml += '            <input type="text" name="{0}" readonly="readonly" class="form-control" />'.format(field.Key);
                        if (!field.IsReadOnly)
                            subHtml += '  </div>'
                        break;
                    case "FileUploadFormat":
                        subHtml += '            <input type="file" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
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
                    case "ComboboxValueFormat":
                        if (field.IsReadOnly != null && field.IsReadOnly) {
                            element.find("select").attr("disabled", "disabled");
                        }
                        break;
                    case "CheckboxValueFormat":
                        element.find("input[type='hidden']").val(field.Value);
                        element.find("input[type='checkbox']").prop("checked", field.Value);
                        element.find("input[type='checkbox']").change(function () {
                            element.find("input[type='hidden']").val($(this).is(':checked'));
                        });
                        break;
                    case "TextboxValueFormat":
                        if (field.MaxLength != null)
                            element.find("input").attr("maxlength", field.ValueFormatValue.MaxLength);
                        break;
                    case "DatePickerValueFormat":
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
                    case "SelectValueFormat":
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
                                group: field.ValueFormatValue.Group,
                                leaf: field.ValueFormatValue.Leaf,
                                filter: field.ValueFormatValue.Filter,
                                provider: field.ValueFormatValue.Provider,
                                id: field.ValueFormatValue.Id,
                                selectedIds: element.find("input[type='hidden']").val(),
                                onSelectHandler: function (data) {
                                    element.find("input[type='hidden']").val(data.ids);
                                    element.find("input[type='text']").val(data.names);
                                    elementDiv.modal("hide");
                                    divElement.find("form").data('bootstrapValidator')
                                        .updateStatus(element.find("input[type='text']").attr("name"), 'NOT_VALIDATED', null)
                                        .validateField(element.find("input[type='text']").attr("name"));
                                }
                            });
                            elementDiv.modal("show");
                        });
                        break;
                    case "FileValueFormat":
                        element.find("input[type='text']").val(field.Value);
                        element.click(function () {
                            CommonRender.renderSelectFile({
                                title: "请选择",
                                url: "/SvnManage/api/select",
                                fileType: field.ValueFormatValue.FileType,
                                folderName: field.ValueFormatValue.FolderName,
                                selectedIds: field.Value,
                                onSelectHandler: function (data) {
                                    element.find("input[type='text']").val("");
                                    if (data.path)
                                        element.find("input[type='text']").val("/{0}".format(data.path));
                                    top.layer.close(data.index)
                                }
                            });
                        });
                        break;
                    case "FileUploadFormat":
                        break;
                }
            }

            if (field.ValueFormatType == null ||
                field.ValueFormatType == "TextboxValueFormat"
                || field.ValueFormatType == "DatePickerValueFormat"
            ) {
                if (field.ValueFormatValue != null && field.Value != null) {
                    if (field.ValueFormatValue.IsMultiLine)
                        element.find("textarea").html(field.Value);
                }
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
        var divElement = $([
            '<div class="modal-dialog" role="document">',
            '    <form class="form-horizontal">',
            '            <div class="modal-body">',
            '                <i class="icon-refresh icon-spin blue"></i>',
            '            </div>',
            '            <div class="modal-footer">',
            '                <button type="submit" class="btn btn-primary"><i class="icon-save"></i>{0}</button>',
            '            </div>',
            '    </form>',
            '   </div>'
        ].join("").format(data.button_OK));

        data.element.html("");
        data.element.append(divElement);
        //绑定字段
        var body = divElement.find(".modal-body");
        body.html("");
        var fieldsStr = "{";
        var isFirst = true;
        var IsDatePicker = false;
        for (var i = 0; i < data.fields.length; i++) {
            var fieldInfo = data.fields[i];
            body.append(renderField(fieldInfo));
            var fieldStr = '"{0}": { "validators": {'.format(fieldInfo.Key);
            if (fieldInfo.ValueFormatType != null) {
                if (fieldInfo.ValueFormatType == "SelectValueFormat")
                    fieldStr = '"{0}Name": { "validators": {'.format(fieldInfo.Key);
            }

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
                    case "TextboxValueFormat":
                        if (fieldInfo.ValueFormatValue.Regex != null) {
                            if (isFirstValidator)
                                isFirstValidator = false;
                            else
                                fieldStr += ",";

                            fieldStr += '"regexp": {"regexp":"{0}",'.format(fieldInfo.ValueFormatValue.Regex);
                            fieldStr += '"message": "{0}" } '.format(fieldInfo.Description);
                        }
                        break;
                    case "DatePickerValueFormat": {
                        IsDatePicker = true;
                        if (isFirstValidator)
                            isFirstValidator = false;
                        else
                            fieldStr += ",";
                        fieldStr += '"date": { "format": "{0}",  "message":"日期格式不正确"}'.format(fieldInfo.ValueFormatValue.DateTimeFormat);
                    }
                        break;
                    case "CheckboxValueFormat":
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
    },
    renderPureForm: function (data) {
        /*
<div class="form-group">
    <label class="control-label col-xs-2 text-right" for="Name">名称:</label>
    <div class="col-xs-7" style="padding-left:0px">
        <input type="text" name="Name" id="Name" data-validate="validate[required]" class="form-control" />
    </div>
</div>
<div class="space-2"></div>
        */
        var renderField = function (field) {
            var html = "";
            html += '<div class="form-group">';
            var fieldName = field.Name;
            html += '    <label class="control-label col-xs-2 text-right">{0}</label>'.format(fieldName);
            html += '    <div class="col-xs-7" style="padding-left:0px">';
            html += '        <div class="clearfix">';

            var subHtml = "";
            if (field.ValueFormatType != null) {
                switch (field.ValueFormatType) {
                    case "TextboxValueFormat": {
                        if (field.ValueFormatValue.IsPassword)
                            subHtml += '            <input type="password" name="{0}" class="form-control" />'.format(field.Key);
                        else if (field.ValueFormatValue.IsMultiLine)
                            subHtml += '            <textarea name="{0}" rows="4" class="form-control" ></textarea>'.format(field.Key);
                        else
                            subHtml += '            <input type="text" name="{0}" class="form-control" />'.format(field.Key);
                    }
                        break;
                    case "CheckboxValueFormat":
                        subHtml += '            <label><input name="{0}" type="hidden"><input class="ace ace-switch ace-switch-6" type="checkbox"><span class="lbl"></span></label>'.format(field.Key);
                        break;
                    case "DatePickerValueFormat":
                        subHtml += '            <div class="input-group date {0} col-xs-12 col-sm-8" data-date-format="{1}">'.format(
                            field.ValueFormatValue.IsContainTime ? "form_datetime" : "form_date", field.ValueFormatValue.DateTimeFormat);
                        subHtml += '            <span class="input-group-addon"><span class="glyphicon glyphicon-th"></span></span>';
                        subHtml += '            <input class="form-control" name="{0}" type="text" />'.format(field.Key);
                        subHtml += '            </div>';
                        break;
                    case "ComboboxValueFormat":
                        if (field.ValueFormatValue.Values == null)
                            field.ValueFormatValue.Values = [];
                        if (field.Value == null)
                            field.ValueFormatValue.Values.splice(0, 0, { Key: "", Value: "请选择.." });
                        subHtml += '            <select class="form-control" name="{0}">'.format(field.Key);
                        for (var i = 0; i < field.ValueFormatValue.Values.length; i++) {
                            subHtml += '                <option';
                            var option = field.ValueFormatValue.Values[i];
                            if (field.Value == option.Key)
                                subHtml += ' selected="selected"';
                            subHtml += ' value="{0}">{1}</option>'.format(option.Key, option.Value);
                        }
                        subHtml += '            </select>';
                        break;
                    case "SelectValueFormat":
                        subHtml += '            <input type="hidden" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        subHtml += '            <input type="text" name="{0}Name" readonly="readonly" class="form-control" />'.format(field.Key);
                        if (!field.IsReadOnly)
                            subHtml += '            <a class="btn btn-xs btn-success no-radius"><i class="icon-share-alt bigger-110"></i></a>';
                        subHtml += '            <div class="modal">';
                        break;
                    case "FileValueFormat":
                        subHtml += '  <div class="input-group">'
                        subHtml += '            <input type="text" name="{0}" readonly="readonly" class="form-control" />'.format(field.Key);
                        if (!field.IsReadOnly)
                            subHtml += '  </div>'
                        break;
                    case "FileUploadFormat":
                        subHtml += '            <input type="file" name="{0}" class="col-xs-12 col-sm-8" />'.format(field.Key);
                        break;
                }
            }
            if (subHtml == "")
                subHtml += '            <input type="text" name="{0}" class="form-control" />'.format(field.Key);
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
                    case "ComboboxValueFormat":
                        if (field.IsReadOnly != null && field.IsReadOnly) {
                            element.find("select").attr("disabled", "disabled");
                        }
                        break;
                    case "CheckboxValueFormat":
                        element.find("input[type='hidden']").val(field.Value);
                        element.find("input[type='checkbox']").prop("checked", field.Value);
                        element.find("input[type='checkbox']").change(function () {
                            element.find("input[type='hidden']").val($(this).is(':checked'));
                        });
                        break;
                    case "TextboxValueFormat":
                        if (field.MaxLength != null)
                            element.find("input").attr("maxlength", field.ValueFormatValue.MaxLength);
                        break;
                    case "DatePickerValueFormat":
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
                    case "SelectValueFormat":
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
                                group: field.ValueFormatValue.Group,
                                leaf: field.ValueFormatValue.Leaf,
                                filter: field.ValueFormatValue.Filter,
                                provider: field.ValueFormatValue.Provider,
                                id: field.ValueFormatValue.Id,
                                selectedIds: element.find("input[type='hidden']").val(),
                                onSelectHandler: function (data) {
                                    element.find("input[type='hidden']").val(data.ids);
                                    element.find("input[type='text']").val(data.names);
                                    elementDiv.modal("hide");
                                    divElement.find("form").data('bootstrapValidator')
                                        .updateStatus(element.find("input[type='text']").attr("name"), 'NOT_VALIDATED', null)
                                        .validateField(element.find("input[type='text']").attr("name"));
                                }
                            });
                            elementDiv.modal("show");
                        });
                        break;
                    case "FileValueFormat":
                        element.find("input[type='text']").val(field.Value);
                        element.click(function () {
                            CommonRender.renderSelectFile({
                                title: "请选择",
                                url: "/SvnManage/api/select",
                                fileType: field.ValueFormatValue.FileType,
                                folderName: field.ValueFormatValue.FolderName,
                                selectedIds: field.Value,
                                onSelectHandler: function (data) {
                                    element.find("input[type='text']").val("");
                                    if (data.path)
                                        element.find("input[type='text']").val("/{0}".format(data.path));
                                    top.layer.close(data.index)
                                }
                            });
                        });
                        break;
                    case "FileUploadFormat":
                        break;
                }
            }

            if (field.ValueFormatType == null ||
                field.ValueFormatType == "TextboxValueFormat"
                || field.ValueFormatType == "DatePickerValueFormat"
            ) {
                if (field.ValueFormatValue != null && field.Value != null) {
                    if (field.ValueFormatValue.IsMultiLine)
                        element.find("textarea").html(field.Value);
                }
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

        data.element.html("");
        //绑定字段
        var fieldsStr = "{";
        var isFirst = true;
        var IsDatePicker = false;
        for (var i = 0; i < data.fields.length; i++) {
            var fieldInfo = data.fields[i];
            data.element.append(renderField(fieldInfo));
            var fieldStr = '"{0}": { "validators": {'.format(fieldInfo.Key);
            if (fieldInfo.ValueFormatType != null) {
                if (fieldInfo.ValueFormatType == "SelectValueFormat")
                    fieldStr = '"{0}Name": { "validators": {'.format(fieldInfo.Key);
            }

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
                    case "TextboxValueFormat":
                        if (fieldInfo.ValueFormatValue.Regex != null) {
                            if (isFirstValidator)
                                isFirstValidator = false;
                            else
                                fieldStr += ",";

                            fieldStr += '"regexp": {"regexp":"{0}",'.format(fieldInfo.ValueFormatValue.Regex);
                            fieldStr += '"message": "{0}" } '.format(fieldInfo.Description);
                        }
                        break;
                    case "DatePickerValueFormat": {
                        IsDatePicker = true;
                        if (isFirstValidator)
                            isFirstValidator = false;
                        else
                            fieldStr += ",";
                        fieldStr += '"date": { "format": "{0}",  "message":"日期格式不正确"}'.format(fieldInfo.ValueFormatValue.DateTimeFormat);
                    }
                        break;
                    case "CheckboxValueFormat":
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

        //var fieldsObj = JSON.parse(fieldsStr);
        //data.element.bootstrapValidator({
        //    message: '此值无效',
        //    feedbackIcons: {
        //        valid: 'glyphicon glyphicon-ok',
        //        invalid: 'glyphicon glyphicon-remove',
        //        validating: 'glyphicon glyphicon-refresh'
        //    },
        //    //submitHandler: function (validator, form, submitButton) {
        //    //    data.submitHandler(form);
        //    //},
        //    fields: fieldsObj
        //});
    }
};