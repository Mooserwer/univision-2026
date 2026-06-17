$(document).ready(function ($) {
    Site.run(), $(".list-group-item").appear(), $(".list-group-item").not(":appeared").each(function () {
        var $item = $(this);
        $item.addClass("list-group-item-invisible"),  $item.addClass("invisible")
    }), $(document).on("appear", ".list-group-item.list-group-item-invisible", function (e) {
        var $item = $(this);
        $item.removeClass("list-group-item-invisible"), $item.removeClass("invisible").addClass("animation-scale-up")
    })
})