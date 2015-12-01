function loadJavascript(url) {
    var js = document.createElement("script");
    js.type = "text/javascript";
    js.src = url;
    document.body.appendChild(js);
}
loadJavascript("../../base/resource/js/jquery-1.10.2.min.js");
loadJavascript("../../base/resource/js/template.js");

window.onload = function (e) {
    $("footer").remove();
    var masterPage = $("meta[name='masterPage']").attr('content');
    var title = $("meta[name='title']").attr('content');
    $.get(masterPage, function (source) {
        template.config("escape", false);

        var render = template.compile(source);
        var html = render({
            title: title,
            body: $("body")[0].innerHTML,
            script: $("head")[0].innerHTML
        });
        document.removeChild(document.all[0]);
        document.write(html);
        document.close();
    });
}