
function clearSearch() {
    $("#searchResult").hide();
    $("#searchResult").empty();
    
}

function search(str) {
    if (str.length == 0) {
        clearSearch();
    }
    $.post("/Store/SearchBox", { searchString: str },
            function (data) {

                if (data.length > 0) {
                    $("#searchResult").empty();
                    for (var i = 0; i < data.length; i++) {
                        $("#searchResult").append("<li><a href='/Store/Details/" + data[i].ProductId + "'>" + data[i].Name + "</a></li>");
                        $("#searchResult").show();
                    }
                }
                else {
                    clearSearch();
                }
            });

        
    
}



$(document).ready(function () {

    $("#searchBox").keyup(function (e) {

        var key = $(this).val();
        if (e.which == 13 && key != "") {

            location.href = "/Store/ProductIndex/?src=" + key;
        }else {
            search(key);
        }
        
    });

    $("#searchDiv").mouseleave(function () {
        clearSearch();
    });

    $("#searchBox").mouseenter(function () {
        if ($(this).is(":focus")) {
            search($(this).val());
        }
    });

    $("#searchBox").click(function () {
        if ($(this).is(":focus")) {
            search($(this).val());
        }
    });

    $('a[href="' + this.location.pathname + '"]').parent().addClass('active');
    

    
    

})


    
