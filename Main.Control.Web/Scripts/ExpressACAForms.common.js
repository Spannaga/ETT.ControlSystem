
var returnAjax = {
    NONE: 0,
    Success: 1,
    Failure: 2,
    Duplicate: 3,
    ContainsAnotherTable: 4
}

var employeeReturnStatus = {
    OPEN: "OPEN",
    CLOSED: "CLOSED",
}

$(function () {

    $.validator.addMethod(
    "regex",
    function (value, element, regexp) {
        if (regexp.constructor != RegExp)
            regexp = new RegExp(regexp);
        else if (regexp.global)
            regexp.lastIndex = 0;
        return this.optional(element) || regexp.test(value);
    },
    "Please don't enter special character.");

    $.validator.addMethod("USZip", function (value, element) {
        return this.optional(element) || /^\d{5}([\-]\d{4})?$/i.test(value);
    });

    jQuery.validator.addMethod("notequalEIN", function (EIN, element) {

        if (EIN == "00-0000000") {
            return false;
        }
        else {
            return true;
        }
    });


    jQuery.validator.addMethod("notequalPTIN", function (PTIN, element) {

        if (PTIN == "P00000000") {
            return false;
        }
        else {
            return true;
        }
    });

    $.validator.addMethod("ValidEmail", function (value, element) {
        var emailRegex = new RegExp(/^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/);
        return emailRegex.test(value);
    });

    $.validator.addMethod("InvalidAdjSpace", function (BName, element) {
        if ((BName.indexOf("  ") !== -1)) {
            return false;
        }
        else {
            return true;
        }
    });

    $.validator.addMethod("phoneUSP", function (phone_number, element) {
        phone_number = phone_number.replace(/\s+/g, "");
        return this.optional(element) || phone_number.length > 9 &&
    phone_number.match(/^(1-?)?(\([1-9]\d{2}\)|[0-9]\d{2})-?[0-9]\d{2}-?\d{4}$/);
    });

    $.validator.addMethod("RequiredUsingClass", function (value, element) {
        if ($(element).val() != "") {
            return true;
        }
        else {
            return false;
        }
    }, "Required");


    $.validator.addClassRules({
        CheckValidation: {
            RequiredUsingClass: true
        }
    });

    jQuery.validator.addMethod("CardExpiredvalidation", function (value, element) {
        var selmonth = $('#ExpiryMonth').val();
        if ($("#ExpiryMonth").val() != '' && $("#ExpiryYear").val() != '') {
            $('#lblErrorcard').hide();
            $('#lblErrorcard').html('');
            var d = new Date();
            var curmonth = d.getMonth() + 1;
            var curyear = d.getFullYear();
            var selmonth = $('#ExpiryMonth').val();
            var selyear = $('#ExpiryYear').val();
            if ((selyear == curyear && selmonth < curmonth) || selyear < curyear) {
                return false;
            }
            else {
                return true;
            }
        }
        else if ($("#ExpiryMonth").val() == '' && $("#ExpiryYear").val() != '') {
            return true;
        }

        $('#_AddCreditCardProfileForm').valid();
    });

    jQuery.validator.addMethod("CardExpiredMonthvalidation", function (value, element) {

        var selmonth = $('#ExpiryMonth').val();
        if ($("#ExpiryMonth").val() != '' && $("#ExpiryYear").val() != '') {
            $('#lblErrorcard').hide();
            $('#lblErrorcard').html('');
            var d = new Date();
            var curmonth = d.getMonth() + 1;
            var curyear = d.getFullYear();
            var selmonth = $('#ExpiryMonth').val();
            var selyear = $('#ExpiryYear').val();
            if ((selyear == curyear && selmonth < curmonth) || selyear < curyear) {
                return false;
            }
            else {
                return true;
            }
        }
        else if ($("#ExpiryMonth").val() != '' && $("#ExpiryYear").val() == '') {
            return true;
        }

        $('#ProcessCreditCardForm').valid();
    });


    //$('input[datatip], select[datatip]').focusin(function (e) {

    //    $("#FieldHelpTab").trigger('click');
    //    $("#Help-SW div").removeClass("active");

    //    var idname = $(this).attr('datatip') + "-SW";
    //    var elementName = document.getElementById(idname);

    //    $('#Help-SW').scrollTo(elementName, 800);

    //    elementName.classList.add("active");

    //});

    $('input:radio').click(function () {
        var element = "#" + $(this).attr("name") + "-error";
        if ($(element).length > 0) {
            $(element).remove();
        }
    });

    $("#USPSVerifyLink").click(function () {
        var _payerAddr1 = $('#Address1').val();
        var _payerAddr2 = $('#Address2').val();
        var _payerCity = $('#City').val();
        var _payerZipcode = $('#ZipCode').val();
        var _statename = $('#StateName').val();
        if (_payerAddr1 == "" && _payerAddr2 == "" && _payerCity == "" && _payerZipcode == "" && _statename == "") {
            $('#loaderid').hide();
            $('#ErrorDialogu').dialog("open");
        }
        else {
            CallUSPSService();
        }
    });


    $.easing.elasout = function (x, t, b, c, d) {
        var s = 1.70158; var p = 0; var a = c;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (!p) p = d * .3;
        if (a < Math.abs(c)) { a = c; var s = p / 4; }
        else var s = p / (2 * Math.PI) * Math.asin(c / a);
        return a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b;
    };

    // This one is important, many browsers don't reset scroll on refreshes
    // Reset all scrollable panes to (0,0)
    $('div.pane').scrollTo(0);
    // Reset the screen to (0,0)
    $.scrollTo(0);
});

(function ($) {

    $.fn.alphanumeric = function (p) {

        p = $.extend({
            ichars: "!@#$%^&*()+=[]\\\';,/{}|\":<>?~`.- ",
            nchars: "",
            allow: ""
        }, p);

        return this.each
			(
				function () {

				    if (p.nocaps) p.nchars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				    if (p.allcaps) p.nchars += "abcdefghijklmnopqrstuvwxyz";

				    s = p.allow.split('');
				    for (i = 0; i < s.length; i++) if (p.ichars.indexOf(s[i]) != -1) s[i] = "\\" + s[i];
				    p.allow = s.join('|');

				    var reg = new RegExp(p.allow, 'gi');
				    var ch = p.ichars + p.nchars;
				    ch = ch.replace(reg, '');

				    $(this).keypress
						(
							function (e) {
							    if (!e.charCode) {
							        if (e.which == 46) {
							            if (e.srcElement.value.indexOf(".") < 0) {
							                if (e.srcElement.value.length == 0)
							                    e.srcElement.value = "0";
							            }
							            else {
							                e.preventDefault();
							            }
							        }
							        k = String.fromCharCode(e.which);
							    }
							    else {
							        if (e.charCode == 46) {
							            if (e.currentTarget.value.indexOf(".") < 0) {
							                if (e.currentTarget.value.length == 0)
							                    e.currentTarget.value = "0";
							            }
							            else {
							                e.preventDefault();
							            }
							        }
							        k = String.fromCharCode(e.charCode);
							    }

							    if (ch.indexOf(k) != -1) e.preventDefault();
							    if (e.ctrlKey && k == 'v') e.preventDefault();

							}

						);

				    $(this).bind('contextmenu', function () {
				        return false
				    });

				}
			);

    };

    $.fn.numeric = function (p) {

        var az = "abcdefghijklmnopqrstuvwxyz";
        az += az.toUpperCase();

        p = $.extend({
            nchars: az
        }, p);

        return this.each(function () {
            $(this).alphanumeric(p);
        }
);
    };

    $.fn.alpha = function (p) {

        var nm = "1234567890";

        p = $.extend({
            nchars: nm
        }, p);

        return this.each(function () {
            $(this).alphanumeric(p);
        }
		);
    };

})(jQuery);

function ShowPageHelp(pageId, textHelp) {
   
    $("#PageId").val(pageId);
    $.getJSON("/Home/GetPageHelp/", { PageId: pageId }, function (data) {
        if (data != null) {
            var textHelpMessage = data.HelpMessage.replace("@@FormName", textHelp).replace("@@Form", textHelp).replace("@@Form", textHelp);
            $(".filed-text").hide();
            if (data.KnowledgeCenterText != null && data.KnowledgeCenterText != '' && data.KnowledgeCenterText != undefined)
            {
                //$(".filed-text").show();
                $("#knowledgebase").show();
                var textHelpMssg = data.KnowledgeCenterText.replace("@@FormName", textHelp).replace("@@Form", textHelp).replace("@@Form", textHelp);
                //Right widget
                $('#lblRightSideHelp').html(textHelpMssg);
                //$('#lblRightSideFHelp').html(data.FieldHelpText);
            }

            //Floating Help
            if (data.FieldHelpText != null && data.FieldHelpText != '' && data.FieldHelpText != undefined) {
                $('.whatIsThis').show(); $('#supportPageHelp').show();
                $('#lblRightSideHelpMob').html(data.KnowledgeCenterText);
                $('#Help-SW').html(data.FieldHelpText);
                $('#Help-FH').html(data.FieldHelpTextFloat);
            }

            //Support center
            $('#lblPageHelpSC').html(textHelpMessage);
            $('#ifvideoSC').show().attr("src", data.VideoURL);
            $('#lblVideoHelpSC').html(data.VideoHelpMessage);
            $("#supportPageHelp").hide();
            if (textHelpMessage != null && textHelpMessage != '') {
                $('#supportPageHelp').show();
                $('#lblPageHelp').html(textHelpMessage);
            }

            $('#helpIcon').attr('onclick', 'RenderYouTubePopup("' + data.VideoURL + '")');
            $('#PageHelpId').attr('onclick', 'EditPageHelp("' + data.PageId + '")');

            //To add active for the left menu
            ActivateLeftMenu(data.PageId);
        }
    });
}

function ActivateLeftMenu(pageId) {
    $('#Form' + pageId).addClass('active');
    if (pageId!=106) {
        $('#Form1095').addClass('active');
    }
   
    //if ($('#leftMenu-' + pageId).parent().parent().hasClass('submenu')) {
    //    $('#leftMenu-' + pageId).parent().parent().slideDown(); //To open the respective section when a subsection is clicked
    //    $('#leftMenu-' + pageId).parent().parent().parent().find('.mainmenu').addClass('active');//To apply active for main section
    //}
}

function ShowFieldHelp(fieldId) {

    $.getJSON("/Home/FieldHelpJson/", { FieldId: fieldId }, function (data) {
        if (data != null) {
            $('#lblRightSideHelpTitle').show().html("<b>" + data.FieldNameTitle + "</b>");
            $('#lblRightSideHelp').html("<i>" + data.HelpMessage + "</i>");
        }
    });
}

function CloseSC() {
    $("#SupportCenter_Page").hide();
    $("#ifvideo").attr('src', '');
}

function ShowSupportCenter(tabId) {
    $("#SupportCenter_Page").show();
    $("#tab-" + tabId).trigger('click');
}

function PopOutSC() {
    $("#SupportCenter_Page").hide();
    var pid = $("#PageId").val();
    var url = "/Home/_PopOut/" + pid;
    window.open(url, "Mywindow", "location=no,menubar=no,width=582,height=528,fullscreen=no,toolbar=no,resizable=no");
}

function MinimizeSC() {
    $('#supportContent').toggle();
}

function PopInSC() {
    $('#SupportCenter_Page', window.opener.document).show();
    window.close();
}

function ChangeLanguagePopJSON(_language) {
    var isDirty = false;
    if (isDirty) {
        jConfirm("Warning: You Have Unsaved Changes", "You have edited some of the fields. Are you sure you want to leave this page without saving them?",
        function (r) {
            if (r == true) {
                if (_language != null && _language != undefined && _language != '') {
                    _language = _language.trim();
                    $.ajax({
                        url: '/User/ChangeLanguagePreferenceFromSettingPopup',
                        cache: false,
                        data: 'id=' + _language,
                        success: function (data) {
                            if (data) {
                                top.location.href = window.location.href;
                            }
                        }
                    });
                }
            }
        });
    }
    else {
        if (_language != null && _language != undefined && _language != '') {
            _language = _language.trim();
            $.ajax({
                url: '/User/ChangeLanguagePreferenceFromSettingPopup',
                cache: false,
                data: 'id=' + _language,
                success: function (data) {
                    if (data) {
                        top.location.href = window.location.href;
                    }
                }
            });
        }
    }
}

//function ContactUsPopup() {
//    //$.colorbox({ href: '/User/_ContactUs?t=' + Math.random(), height: 600, width: 700, overlayClose: false });
//}

function CancelModal() {
    $('#myModal').modal("hide");
    }

function IsNullOrEmpty(value) {
    if (value != null && value != '' && value != undefined) {
        return false;
    }
    else {
        return true;
    }
}

function AddressIsNotNull(value) {
    return (value != null && value != '' && value != undefined ? ", " + value : "");
}

function getQuerystring(key, default_) {
    if (default_ == null) default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
    var qs = regex.exec(window.location.href);
    if (qs == null)
        return default_;
    else
        return qs[1];
}

function removeURLParam(url, param) {
    var urlparts = url.split('?');
    if (urlparts.length >= 2) {
        var prefix = encodeURIComponent(param) + '=';
        var pars = urlparts[1].split(/[&;]/g);
        for (var i = pars.length; i-- > 0;)
            if (pars[i].indexOf(prefix, 0) == 0)
                pars.splice(i, 1);
        if (pars.length > 0)
            return urlparts[0] + '?' + pars.join('&');
        else
            return urlparts[0];
    }
    else
        return url;
}

function FormatAmount(obj) {
    var _fmtedAmt = CurrencyFormatted($(obj).val());
    $(obj).val(_fmtedAmt);
}

function CurrencyFormatted(amount) {

    amount = parseInt(amount);
    return amount != 0 ? amount.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$&,') : 0;
}


function ApplyPlusMinusStyle() {
    $(".plusMinus").each(function (index, element) {
        var textFieldVal = $(element).text();
        if (textFieldVal != null && textFieldVal != '' && textFieldVal != undefined) {
            var elementVal1 = $.trim($(element).text());
            if (elementVal1.indexOf(')') < 0 && elementVal1.indexOf(',') < 0) {
                if (parseFloat(elementVal1) < 0) {
                    var currFormatted = CurrencyFormatted((GetRoundOffValue(elementVal1)) * -1);
                    if (currFormatted == 0) {
                        $(element).text(currFormatted);
                        $(element).removeClass('Minus');
                        $(element).addClass('Plus');
                    }
                    else {
                        $(element).text('(' + currFormatted + ')');
                        $(element).removeClass('Plus');
                        $(element).addClass('Minus');
                    }

                }
                else {
                    $(element).text(CurrencyFormatted(GetRoundOffValue(elementVal1)));
                    $(element).removeClass('Minus');
                    $(element).addClass('Plus');
                }
            }
        }
    });

    $(".plusMinus").parent().addClass('taR');
}

function GetElementPlusMinusVal(elementVal) {
    if (elementVal != "") {
        elementVal = $.trim(elementVal);

        if (elementVal.indexOf(')') > 0) {
            elementVal = elementVal.replace("(", "");
            elementVal = elementVal.replace(")", "");
            elementVal = "-" + elementVal;
        }

        elementVal = elementVal.replace(",", "");
        elementVal = elementVal.replace(",", "");
        elementVal = elementVal.replace(",", "");
        elementVal = elementVal.replace(",", "");
        elementVal = elementVal.replace(",", "");
    }
    return elementVal;
}

function GetRoundOffValue(valToRoundOff) {
    var splitted = valToRoundOff.split('.');
    var intPart = splitted[0];
    var decimalValue = splitted[1];
    var finalVal = 0;
    if (decimalValue != null && decimalValue != '' && decimalValue != undefined) {
        decimalValue = decimalValue < 10 ? decimalValue + '0' : decimalValue;
        decimalValue = decimalValue.substring(0, 2);
        if (decimalValue < 50) {
            if (intPart == '-0') {
                finalVal = Math.ceil(valToRoundOff);
            }
            else if (intPart >= 0) {
                finalVal = Math.floor(valToRoundOff);
            }
            else {
                finalVal = Math.ceil(valToRoundOff);
            }
        }
        else if (decimalValue >= 50) {
            if (intPart == '-0') {
                finalVal = Math.floor(valToRoundOff);
            }
            else if (intPart >= 0) {
                finalVal = Math.ceil(valToRoundOff);
            }
            else {
                finalVal = Math.floor(valToRoundOff);
            }
        }

        return finalVal;
    }
    else {
        return valToRoundOff;
    }
}


function AmountFormat() {
    $(".Amount").attr("autocomplete", "off");
    $(".Amount").attr('maxlength', '18');
    $(".Amount").addClass('Dollar');

    $(".Amount").keyup(function () {

        // store current positions in variables
        var start = this.selectionStart,
            end = this.selectionEnd;

        var $this = $(this);

        $this.val($this.val().replace(/[^\d.]/g, ''));

        var numsep = $this.val().split('.');

        var outNu = numsep[0].length > 15 ? numsep[0].substring(0, 15) : numsep[0];

        if ($this.val().indexOf('.') != -1) {

            outNu += numsep[1].length > 1 ? '.' + numsep[1].substring(0, 2) : '.' + numsep[1];
        }

        $this.val(outNu);

        // restore from variables...
        this.setSelectionRange(start, end);

    });

    $(".Amount").focus(function () {
        var $this = $(this);
        if (parseFloat($this.val()) == 0) {
            $this.val("");
        }
    });

    $(".Amount").blur(function () {
        var $this = $(this);
        var splitted = $this.val().split('.');
        var decimalValue = splitted[1];
        var idToPlace = $this.attr('id');
        if (decimalValue != null && decimalValue != undefined) {
            if (decimalValue.length == 1) {
                decimalValue = decimalValue < 10 ? decimalValue + '0' : decimalValue;
            }
            if ($this.val().indexOf('.') == 0 && $this.val().length == 1) {
                $this.val("");
            }
            else {
                if (decimalValue < 50) {
                    var flooredVal = Math.floor($this.val());
                    $('#' + idToPlace).val(flooredVal);
                }
                else if (decimalValue >= 50) {
                    var ceiledVal = Math.ceil($this.val());
                    $('#' + idToPlace).val(ceiledVal);
                }
            }
        }
    });

    //Remove Placeholder
    $('input:text').each(function () {
        $(this).focus(function () {
            $(this).data('placeholder', $(this).attr('placeholder'))
            $(this).attr('placeholder', '');
        });
    });
    //Include Placeholder
    $('input:text').each(function () {
        $(this).blur(function () {
            $(this).attr('placeholder', $(this).data('placeholder'));
        });
    });

    //Remove .00
    //$('input:text').each(function () {
    //    var $this = $(this);
    //    var splitted = $this.val().split('.');
    //    var wholeValue = splitted[0];
    //    var idToPlace = $this.attr('id');
    //    if (wholeValue != null && wholeValue != undefined) {
    //        $('#' + idToPlace).val(wholeValue);
    //    }
    //});

    $(".Amount").focus(function () {
        var $this = $(this);
        if (parseFloat($this.val()) == 0) {
            $this.val("");
        }
    });

}

// Numeric only control handler
jQuery.fn.ForceNumericOnly =
function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.charCode || e.keyCode || 0;
            // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
            // home, end, period, and numpad decimal
            return (
                key == 8 ||
                key == 9 ||
                key == 13 ||
                key == 46 ||
                key == 110 ||
                key == 190 ||
                (key >= 35 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
        });
    });
};

function PercentFormat() {

    $(".Percent").attr("autocomplete", "off");
    $(".Percent").attr('maxlength', '6');

    $(".Percent").keyup(function () {

        var $this = $(this);

        $this.val($this.val().replace(/[^\d.]/g, ''));

        var numsep = $this.val().split('.');

        var outNu = numsep[0].length > 3 ? numsep[0].substring(0, 3) : numsep[0];

        if ($this.val().indexOf('.') != -1) {

            outNu += numsep[1].length > 1 ? '.' + numsep[1].substring(0, 2) : '.' + numsep[1];
        }

        $this.val(outNu);

    });

    $(".Percent").focus(function () {
        var $this = $(this);
        if (parseFloat($this.val()) == 0) {
            $this.val("");
        }
    });

}

function NAmountFormat() {
    $(".NAmount").attr("autocomplete", "off");
    $(".NAmount").attr('maxlength', '18');
    $(".NAmount, .NNAmount").addClass('Dollar');

    $(".NAmount").keyup(function () {
        // store current positions in variables
        var start = this.selectionStart,
            end = this.selectionEnd;

        var $this = $(this);
        $this.val($this.val().replace(/[^\d-.]/g, ''));

        var numsep = $this.val().split('-');

        var outNu = numsep[0].length > 15 ? numsep[0].substring(0, 15) : numsep[0];

        if ($this.val().indexOf('-') != -1) {
            if ($this.val().indexOf('-') < 1) {
                outNu += '-' + numsep[1];
            }
        }

        $this.val(outNu);

        this.setSelectionRange(start, end);

    });

    $(".NNAmount").keyup(function () {

        var start = this.selectionStart,
            end = this.selectionEnd;

        var $this = $(this);
        $this.val($this.val().replace(/[^\d-.]/g, ''));

        var numsep = $this.val().split('.');

        var outNu = numsep[0].length > 15 ? numsep[0].substring(0, 15) : numsep[0];

        if ($this.val().indexOf('.') != -1) {
            outNu += numsep[1].length > 1 ? '.' + numsep[1].substring(0, 2) : '.' + numsep[1];
        }

        $this.val(outNu);

        // restore from variables...
        this.setSelectionRange(start, end);

    });

    $(".NAmount, .NNAmount").focus(function () {
        var $this = $(this);
        if (parseFloat($this.val()) == 0) {
            $this.val("");
        }
    });

    $(".NAmount, .NNAmount").blur(function () {
        var $this = $(this);
        var splitted = $this.val().split('.');
        var decimalValue = splitted[1];
        var idToPlace = $this.attr('id');

        if (decimalValue.length == 1) {
            decimalValue = decimalValue < 10 ? decimalValue + '0' : decimalValue;
        }

        if ($this.val().indexOf('.') == 0 && $this.val().length == 1) {
            $this.val("");
        }
        else {
            if ($this.val().indexOf('-') == 0) {
                if (decimalValue >= 50) {
                    var flooredVal = Math.floor($this.val());
                    $('#' + idToPlace).val(flooredVal);
                }
                else if (decimalValue < 50) {
                    var ceiledVal = Math.ceil($this.val());
                    $('#' + idToPlace).val(ceiledVal);
                }
            }
            else {
                if (decimalValue < 50) {
                    var flooredVal = Math.floor($this.val());
                    $('#' + idToPlace).val(flooredVal);
                }
                else if (decimalValue >= 50) {
                    var ceiledVal = Math.ceil($this.val());
                    $('#' + idToPlace).val(ceiledVal);
                }
            }
        }

        if ($this.val().indexOf('-') == 0 && $this.val().length == 1) {
            $this.val("");
        }

    });

    //Remove .00
    //$('input:text').each(function () {
    //    var $this = $(this);
    //    var splitted = $this.val().split('.');
    //    var wholeValue = splitted[0];
    //    var idToPlace = $this.attr('id');
    //    if (wholeValue != null && wholeValue != undefined) {
    //        $('#' + idToPlace).val(wholeValue);
    //    }
    //});
}

function ShowDraftLetter(id) {
    window.open("/Home/ViewDraftLetter/" + id + "/" + false, 'toolbar=no,titlebar=no, directories=no, location=no,status=yes, menubar=no, resizable=yes, scrollbars=yes,width=1010, height=680,left=0,top=0');
    return false;
}

function ShowWMDraftLetter(id) {
    window.open("/Home/ViewDraftLetter/" + id + "/" + true, 'toolbar=no,titlebar=no, directories=no, location=no,status=yes, menubar=no, resizable=yes, scrollbars=yes,width=1010, height=680,left=0,top=0');
    return false;
}

function setYouTubeTitle(youtubeId) {
    $.ajax({
        url: window.location.protocol + '//query.yahooapis.com/v1/public/yql',
        data: {
            q: "select * from json where url ='http://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=" + youtubeId + "&format=json'",
            format: "json"
        },
        dataType: "jsonp",
        success: function (data) {
            if (data && data.query && data.query.results && data.query.results.json) {
                $("#YouTubeTitle").html(data.query.results.json.title);
            }
        }
    });
}

function GetStateCodeByName(name) {
    name = name.trim();
    var spltName = name.split('(');
    var code = spltName != null && spltName[1]!= null ? spltName[1].substr(0, 2): "";
    return code;
}
function TermsPopup() {
    //parent.$.fn.colorbox({ href: '/Home/_Terms/' + '?t=' + Math.random(), height: 450, width: 900, overlayClose: false });
    if (isbrowser_firefox) {
        window.open("http://www.expresstaxzone.com/Home/Terms", "_blank", "toolbar=yes, location=yes, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=no, copyhistory=yes, width=1000, height=600");
    } else {
        window.open("http://www.expresstaxzone.com/Home/Terms/#divTerms", "_blank", "toolbar=yes, location=yes, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=no, copyhistory=yes, width=1000, height=600");
    }
}

function PrivacyPopup() {
    if (isbrowser_firefox) {
        window.open("http://www.expresstaxzone.com/Home/PrivacyPolicy", "_blank", "toolbar=yes, location=yes, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=no, copyhistory=yes, width=1000, height=600");
    } else {
        window.open("http://www.expresstaxzone.com/Home/PrivacyPolicy/#divPrivacy", "_blank", "toolbar=yes, location=yes, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=no, copyhistory=yes, width=1000, height=600");
    }
}

//Animation
function animationClick(element, animation) {
    element = $(element);
    element.click(
      function () {
          element.addClass('animated ' + animation);
          //wait for animation to finish before removing classes
          window.setTimeout(function () {
              element.removeClass('animated ' + animation);
          }, 2000);
      }
    );
};

function ContactUsPopup() {
    SaveScreenShort();
    $.ajax({
        url: "/User/_ContactUs/",
        cache: false,
        success: function (data) {
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });
}

function EmailPopup() {
    SaveScreenShort();
    $.ajax({
        url: "/User/_Email/",
        cache: false,
        success: function (data) {
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });
}

function SaveScreenShort() {
    html2canvas(document.body, {
        onrendered: function (canvas) {
            var image = canvas.toDataURL("image/png");
            image = image.replace('data:image/png;base64,', '');

            $.ajax({
                type: 'POST',
                url: "/User/SaveScreenShort",
                data: '{ "imageData" : "' + image + '" }',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (msg) {
                    //alert('Image saved successfully !');
                }
            });
        }
    });
}

function EditHelp(id) {

    $.ajax({
        url: "/Home/_EditFieldHelp/",
        cache: false,
        data: { id: id },
        success: function (data) {
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });

}

function EditPageHelp(id) {
    $.ajax({
        url: "/Home/_EditPageHelp/",
        cache: false,
        data: { id: id },
        success: function (data) {
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });

}

function ScrollToTop() {
    $('html,body').animate({
        scrollTop: 5
    }, 1000);
}



function SpeakHelp() {
    var helpStr = $("#lblPageHelp").text();
    $("#playBtn-help").hide();
    $("#pauseBtn-help").show();

    $.getJSON("/Home/SpeakHelpJson/", { id: helpStr }, function (data) {
        $("#playBtn-help").show();
        $("#pauseBtn-help").hide();
    });
}

function ShowTermsAndConditions() {
    $.ajax({
        url: "/User/_TermsAndConditions/",
        cache: false,
        success: function (data) {
            $(".modal-dialog").addClass("edit-dialog75");
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });
}

function calculate_time_zone() {
    var rightNow = new Date();
    var jan1 = new Date(rightNow.getFullYear(), 0, 1, 0, 0, 0, 0);  // jan 1st
    var june1 = new Date(rightNow.getFullYear(), 6, 1, 0, 0, 0, 0); // june 1st
    var temp = jan1.toGMTString();
    var jan2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
    temp = june1.toGMTString();
    var june2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
    var std_time_offset = (jan1 - jan2) / (1000 * 60 * 60);
    var daylight_time_offset = (june1 - june2) / (1000 * 60 * 60);
    var dst;
    if (std_time_offset == daylight_time_offset) {
        dst = "0"; // daylight savings time is NOT observed
    } else {
        // positive is southern, negative is northern hemisphere
        var hemisphere = std_time_offset - daylight_time_offset;
        if (hemisphere >= 0)
            std_time_offset = daylight_time_offset;
        dst = "1"; // daylight savings time is observed
    }

    return convert(std_time_offset) + "," + dst;
}

function convert(value) {
    var hours = parseInt(value);
    value -= parseInt(value);
    value *= 60;
    var mins = parseInt(value);
    value -= parseInt(value);
    value *= 60;
    var secs = parseInt(value);
    var display_hours = hours;
    // handle GMT case (00:00)
    if (hours == 0) {
        display_hours = "00";
    } else if (hours > 0) {
        // add a plus sign and perhaps an extra 0
        display_hours = (hours < 10) ? "+0" + hours : "+" + hours;
    } else {
        // add an extra 0 if needed 
        display_hours = (hours > -10) ? "-0" + Math.abs(hours) : hours;
    }

    mins = (mins < 10) ? "0" + mins : mins;
    return display_hours + ":" + mins;
}

function ApplyPlusMinusStyle() {
    $(".plusMinus").each(function (index, element) {
        var textFieldVal = $(element).text();
        if (textFieldVal != null && textFieldVal != '' && textFieldVal != undefined) {
            var elementVal1 = $.trim($(element).text());
            if (elementVal1.indexOf(')') < 0 && elementVal1.indexOf(',') < 0) {
                if (parseFloat(elementVal1) < 0) {
                    var currFormatted = CurrencyFormatted((elementVal1) * -1);
                    if (currFormatted == 0) {
                        $(element).text(currFormatted);
                        $(element).removeClass('Minus');
                        $(element).addClass('Plus');
                    }
                    else {
                        $(element).text('(' + currFormatted + ')');
                        $(element).removeClass('Plus');
                        $(element).addClass('Minus');
                    }

                }
                else {
                    $(element).text(CurrencyFormatted(elementVal1));
                    $(element).removeClass('Minus');
                    $(element).addClass('Plus');
                }
            }
        }
    });
}


function EditPageHelp(id) {

    $.ajax({
        url: "/Home/_EditPageHelp/",
        cache: false,
        data: { id: id },
        success: function (data) {
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });

}

function EditHelp(id) {

    $.ajax({
        url: "/Home/_EditFieldHelp/",
        cache: false,
        data: { id: id },
        success: function (data) {
            $('#ModelBody').html(data);
            $('#myModal').modal("show");
        },
        error: function (err) {
            console.log(err.responseText);
        }
    });

}