$(document).ready(function () {
    var form = $("#checkoutForm");
    
    function checkDelCost() {
        if ($("#SameDeliveryAddress").is(":checked")) {
            $("#ShippingState").val($("#State").val());
        }
        var city = $("#ShippingState").val();
        $("#DeliveryMethodId_1").prop("checked", true);
        $("#express").hide();
        $.post("/Checkout/CheckDelCost", { dest: city },
            function (data) {
                if (data != null) {
                    $("#deliveryDays").text("| "+data.Days);
                    $("#deliveryCost").text("Rp " + data.Cost);
                    if (data.CostExpress != 0)
                    {
                        $("#deliveryDaysExpress").text("| " + data.DaysExpress);
                        $("#deliveryCostExpress").text("Rp " + data.CostExpress);
                        $("#express").show();
                    }
                    
                }
            });
    }

    function finalDeliveryCost()
    {
        if ($("#SameDeliveryAddress").is(":checked")) {
            $("#ShippingState").val($("#State").val());
        }
        var city = $("#ShippingState").val();
        var delID = $("input[name=DeliveryMethodId]:checked").val();

        $.post("/Checkout/CheckFinalDelCost", { dest: city, deliveryMethodId :delID },
            function (data) {
                if (data != null) {
                    $(".deliveryCost2").text(data.Cost);

                }
            });

    }
    checkDelCost();
    function sameDelAddress() {
        $("#ShippingState").val($("#State").val());
        checkDelCost();
        $("#shippingMethod").css("width", 500);
        $("#shippingDetails").hide();
    }

    function diffDelAddress() {
        $("#ShippingAddress").val("");
        
        
        $("#shippingMethod").css("width", 250);
        $("#shippingDetails").show();
    }

    function duplicateDeliveryAddress() {
        $("#ShippingFirstName").val($("#FirstName").val());
        $("#ShippingLastName").val($("#LastName").val());
        $("#ShippingPhone").val($("#Phone").val());
        $("#ShippingAddress").val($("#Address").val());
        $("#ShippingState").val($("#State").val());
        $("#ShippingCity").val($("#City").val());
        $("#ShippingPostalCode").val($("#PostalCode").val());
    }

    $("#ShippingState").change(function () {
        var state = $(this).val();
        if (state != 0) {
            $.post("/Checkout/GetCities", { state: state },
            function (data) {
                if (data != null) {
                    var output = "";
                    for (var i = 0; i < data.length; i++) {
                        output += '<option value=\"' + data[i] + '\">' + data[i] + '</option>';
                    }
                    $("#ShippingCity").empty().append(output);
                }
            });
            checkDelCost();
        } 
        else {
            $("#ShippingCity").empty().append("<option>--Select City--</option>");
        }
    });

    $("#State").change(function () {
        var state = $(this).val();
        if (state != 0) {
            $.post("/Checkout/GetCities", { state: state },
            function (data) {
                if (data != null) {

                    var output = "";
                    for (var i = 0; i < data.length; i++) {
                        output += '<option value=\"' + data[i] + '\">' + data[i] + '</option>';
                    }
                    $("#City").empty().append(output);
                }
            });

            if ($("#SameDeliveryAddress").is(":checked")) {
                $("#ShippingState").val(state);
                checkDelCost();
            }
        }
        else {
            $("#City").empty().append("<option>--Select City--</option>");
        }
        
    });

    $("#City").change(function () {

        var city = $(this).val();

        if ($("#SameDeliveryAddress").is(":checked")) {
            $("#ShippingState").val(city);
        }
    });

    $("#SameDeliveryAddress").change(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            sameDelAddress();
        } else {
            diffDelAddress();
        }
    });

    $("#checkPromo").click(function (e) {
        e.preventDefault();
        var codee = $("#Code").val();
        if (!$("#Code").is(":disabled")) {
            $.post("/Checkout/CheckPromo", { code: codee },
            function (data) {
                if (data == "1") {

                    $("#Code").attr("disabled", true);
                    $("#PromoCode").val($("#Code").val());
                    $("#PromoActivated").prop("checked", true);
                    $("#PromoActivated").val(true);

                    $("#promoResult").text("Promo Activated!");
                    $(this).prop("disabled", true);
                }
                else {
                    $("#promoResult").text("Wrong Code!");
                }
            }).fail(function () {
                $("#promoResult").text("Wrong Code!");
            });
        }
        
    });


    $("#return").click(function () {
        $("#reviewOrder").hide();
        $("#preCheckout").show();
    });

    $("#preCheckoutSubmit").click(function (e) {
        e.preventDefault();
        form.removeAttr("novalidate");
        if ($("#SameDeliveryAddress").is(":checked")) {
            duplicateDeliveryAddress();
        }
        if ($("#checkoutForm").valid()) {
            var state = $("#ShippingState").val();
            
            var promocode = $("#PromoActivated").val() ? $("#PromoCode").val() : "";
            var delID = $("input[name=DeliveryMethodId]:checked").val();
            $.post("/Checkout/GetTotalPrice", { dest: state, deliveryMethodId : delID, code : promocode },
                function (data) {
                    $("#totalPrice").text(data.TotalPrice);
                    $("#promoDiscount").text(data.PromoDiscount);
                    finalDeliveryCost();
                    $("#preCheckout").hide();
                    $("#reviewOrder").show();
                });
            
        }
    });

    $("#tnc-checkout").on("change", function () {
        $("#confirm-order").prop("disabled", function (e, v) { return !v; });
    });

    $("#confirm-order").click(function () {
        if ($(this).is(":disabled")) {
            alert("Please aggree to the Terms And Condition before placing your order");
        }
        
    });
    
})