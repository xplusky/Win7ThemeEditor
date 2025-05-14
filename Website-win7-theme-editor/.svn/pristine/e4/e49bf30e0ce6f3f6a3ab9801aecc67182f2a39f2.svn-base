/// <reference path="scripts/jquery-1.8.2.js" />
$(document).ready(function () {
    $('.blueberry').blueberry();
    $("#downloadlink").click(function() {
        $.get("func.php?arg=incrdowncount");
    });
    $.get("func.php?arg=getdowncount", function (data) {
        $("#downcount").append(data);
    });
});
