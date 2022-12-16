$(function () {

    //Credit card validator position
    $("#CardNumber").validateCreditCard(function (result) {
        if (result != null && result.card_type != null) {
            if (result.card_type.name == "visa") {
                $("#CardNumber").css({ 'background-position': '2px -145px, 230px -53px', 'background-size': '120px 361px, 120px 361px' });
                if (result.length_valid && result.luhn_valid) {
                    $("#CardNumber").css({ 'background-position': '2px -145px, 230px -75px', 'background-size': '120px 361px, 120px 361px' });
                    $("#CardType").val("Visa");
                }
            }
            else if (result.card_type.name == "amex") {
                $("#CardNumber").css({ 'background-position': '2px -335px, 230px -53px', 'background-size': '120px 361px, 120px 361px' });
                if (result.length_valid && result.luhn_valid) {
                    $("#CardNumber").css({ 'background-position': '2px -335px, 230px -75px', 'background-size': '120px 361px, 120px 361px' });
                    $("#CardType").val("Amex");
                }
            }
            else if (result.card_type.name == "mastercard") {
                $("#CardNumber").css({ 'background-position': '2px -222px, 230px -53px', 'background-size': '120px 361px, 120px 361px' });
                if (result.length_valid && result.luhn_valid) {
                    $("#CardNumber").css({ 'background-position': '2px -222px, 230px -75px', 'background-size': '120px 361px, 120px 361px' });
                    $("#CardType").val("MasterCard");
                }
            }
            else if (result.card_type.name == "discover") {
                $("#CardNumber").css({ 'background-position': '2px -297px, 230px -53px', 'background-size': '120px 361px, 120px 361px' });
                if (result.length_valid && result.luhn_valid) {
                    $("#CardNumber").css({ 'background-position': '2px -297px, 230px -75px', 'background-size': '120px 361px, 120px 361px' });
                    $("#CardType").val("Discover");
                }
            }
            else {
                $("#CardType").val("Visa");
                $("#CardNumber").append('<label for="CardNumber" class="error" style="display: inline-block;"><label>Invalid Card Number</label></label>');
            }
            $('#hdnValidCCard').val(true);
        }
        else {//if (result == null || result.card_type == null) {
            $("#CardNumber").css({ 'background-position': '2px -109px, 230px -53px', 'background-size': '120px 361px, 120px 361px' });
            $('#hdnValidCCard').val(false);
            $("#CardType").val("Visa");
        }
    });

    $("#CardNumber").numeric();

    if ($(window).width() > 480) {
        $(".CC").focusin(function () {
            var imgId = $(this).attr('id');
            $("#CC_" + imgId).show();
        });

        $(".CC").focusout(function () {
            var imgId = $(this).attr('id');
            $("#CC_" + imgId).hide();
        });
    }
});