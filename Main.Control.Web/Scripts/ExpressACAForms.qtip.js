$(function () {
    $('.steqtip').qtip({
        content: {
            text: function (event, api) {
                $.ajax({
                    url: "/Home/FieldHelpJson/",
                    data: { FieldId: $(this).attr('qtipid') },
                    cache: false,
                    success: function (data) {
                        api.set('content.text', data.HelpMessage)
                        api.set('content.title', data.FieldNameTitle)
                    },
                    error: function (err) {
                        api.set('content.text', status + ': ' + err)
                    }
                });

                return 'Loading...';
            }, button: true
        },
        position: {
            adjust: {
                mouse: false,
                scroll: false
            },
            target: 'mouse'
        },
        //show: {
        //    effect: function () {
        //        $(this).show('slide', 500);
        //    }
        //},
        //hide: {
        //    event: false,
        //    inactive: 30000,
        //    effect: function () {
        //        $(this).hide('puff', 500);
        //    }
        //}
    });
});



