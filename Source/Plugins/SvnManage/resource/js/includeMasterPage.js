function loadJavascript(url) {
    var js = document.createElement("script");
    js.type = "text/javascript";
    js.src = url;
    document.body.appendChild(js);
}
loadJavascript("/base/resource/js/jquery-1.10.2.min.js");
loadJavascript("/base/resource/js/template.js");

window.onload = function (e) {
    //移除footer元素
    $("footer").remove();
    //取出meta中的数据
    var masterPage = $("meta[name='masterPage']").attr('content');
    var title = $("meta[name='title']").attr('content');
    var scripts = $("meta[name='scripts']").attr('content');
    //移除meta元素
    $("meta").remove();

    $.get(masterPage, function (source) {
        template.config("escape", false);

        var headHTML = $("head")[0].innerHTML;
        if (scripts) {
            scripts = scripts.split(';');
            for (var i in scripts)
                headHTML = '<script src="' + scripts[i] + '"></script>' + headHTML;
        }
        var render = template.compile(source);
        var html = render({
            title: title,
            body: $("body")[0].innerHTML,
            script: headHTML
        });
        document.removeChild(document.all[0]);
        document.write(html);
        document.close();
    });
}