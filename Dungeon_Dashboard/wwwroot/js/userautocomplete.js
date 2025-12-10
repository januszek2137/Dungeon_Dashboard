$(function() {
    $("#invite-username").autocomplete({
        source: function(request, response) {
            $.getJSON("api/Users/Lookup", { term: request.term }, response);
        },
        minLength: 2,
        delay: 200,
        appendTo: "#autocomplete-wrapper",
        open: function() {
            var input = $(this);
            var widget = input.autocomplete("widget");

            // Match width to input
            widget.css({
                "width": input.outerWidth() + "px",
                "max-width": input.outerWidth() + "px"
            });
        },
        select: function(event, ui) {
            $("#invite-username").val(ui.item.label);
            return false;
        }
    });
});