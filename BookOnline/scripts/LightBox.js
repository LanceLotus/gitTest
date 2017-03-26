function ShowLightBox() {
    $("body").append("<div id='MaskDiv'></div>");
    $("body").append("<div id='ContentDiv'></div>");
    $("#MaskDiv").css({
        "position":"absolute",
        "left":"0px",
        "top":"0px",
        "background-color":"black",
        "z-index":"2",
        "opacity":"0.7",
        "display":"none"
    });
    $("#ContentDiv").css({
        "position": "absolute",
        "left": "0px",
        "top": "0px",
        "width":"400px",
        "height":"300px",
        "background-color":"white",
        "z-index":"3",
        "display":"none"
    });

    $("#ContentDiv").append("<p style='text-align:right'><span id='CloseLink'>關閉</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</p>")
    $("#CloseLink").css({
        "font-weight": "bold",
        "cursor": "pointer",
        "color":"blue",
        "text-decoration":"underline"
    });

    var ww = $(document).width();
    var hh = $(document).height();
    $("#MaskDiv").css({ "width": ww, "height": hh });
    $("#ContentDiv").css({ "left": (ww - 400) / 2, "top": 300 });    

    $("#MaskDiv").show();
    $("#ContentDiv").show();
    $("#CloseLink").click(function () {
        $("#MaskDiv").remove();
        $("#ContentDiv").remove();
    });
}