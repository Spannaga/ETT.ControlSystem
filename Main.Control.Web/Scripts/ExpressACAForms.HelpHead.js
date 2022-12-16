$(function () {
    /** Uses jQuery Touch Punch hack for mobile support
    ** http://touchpunch.furf.com/
    **/

    $('.chat-head').draggable({
        axis: "y",
        opacity: 0.8,
        scroll: false,
        start: function (event, ui) {
            ui.helper.bind("click.prevent",
            function (event) { event.preventDefault(); });
        },
        stop: function (event, ui) {
            setTimeout(function () {
                ui.helper.unbind("click.prevent");
            }, 300);
        }
    });

    $('.chat-head').click(function (e) {

        $('#HelpModelBody').height(600);
       
        if (!$('#myHelpModal').is(':visible')) {
            $('#myHelpModal').modal("show");
        }
        else {
            $('#myHelpModal').modal("hide");
        }
      
    });

});