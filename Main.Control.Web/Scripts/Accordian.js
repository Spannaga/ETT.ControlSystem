
$(function () {
    $('.fa-chevron-down').click(function (e) {
        $this = $(this),
        $next = $this.next();

        $(this).parent().next().slideToggle(800, function() {
            $(this).parent().toggleClass('open', $(this).is(':visible'));
        });
    });
});
