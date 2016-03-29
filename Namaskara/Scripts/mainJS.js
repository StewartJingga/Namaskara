
function search(str) {
    $("#result").html("");

    if (str.length > 0) {

        $.post("/Store/SearchBox", { searchString: str },
            function (data) {

                if (data != null) {

                    for (var i = 0; i < data.length; i++) {
                        $("#result").append("<a href='/Store/Details/"+ data[i].ProductId +"'>" + data[i].Name + "</a>");
                    }
                }
            });
    }
}

$(document).ready(function (e) {

    //$("#searchBox").blur(function () {
    //    $("#result").html("");
        
    //})




});
    
