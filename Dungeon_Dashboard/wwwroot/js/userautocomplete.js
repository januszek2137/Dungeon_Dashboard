$(function() {
    $("#invite-username").autocomplete({
        source: function(request, response) {
            $.getJSON("api/Users/Lookup", { term: request.term }, response);
        },
        minLength: 2,
        delay: 200,
        select: function(event, ui) {
            $("#invite-username").val(ui.item.label);
            return false;
        }
    });
});