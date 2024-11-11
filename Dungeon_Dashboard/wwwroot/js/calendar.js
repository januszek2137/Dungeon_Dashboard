document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        eventSources: [
            {
                url: '/api/eventmodels',
                failure: function () {
                    alert('There was an error while fetching events!');
                }
            }
        ],
        eventClick: function (info) {
            // Fill details modal with event data
            $('#title').val(info.event.title);
            $('#description').val(info.event.extendedProps.description);
            $('#date').val(info.event.start.toISOString().substring(0, 10));
            $('#time').val(info.event.start.toISOString().substring(11, 16));
            $('#location').val(info.event.extendedProps.location);
            // Open details modal
            $('#detailsModal').modal('show');
        }
    });
    calendar.render();
});

$(document).ready(function () {
    $("#openModalButton").on('click', function () {
        clearModalForm(); // Clear form before opening new event modal
        $("#newEventModal").modal('show');
    });

    $('.close').on('click', function () {
        $('#detailsModal').modal('hide');
        $('#newEventModal').modal('hide');
        clearModalForm(); // Clear form when closing any modal
    });

    $("#newEventForm").on('submit', function (e) {
        e.preventDefault();
        console.log("Submitting form");

        var eventData = {
            title: $("#newTitle").val(),
            description: $("#newDescription").val(),
            date: $("#newDate").val(),
            time: $("#newTime").val(),
            location: $("#newLocation").val()
        };

        console.log("Event data prepared", eventData);

        $.ajax({
            url: '/api/eventmodels',
            type: 'POST',
            data: JSON.stringify(eventData),
            contentType: 'application/json',
            success: function (data) {
                console.log("Success response received");
                $("#newEventModal").modal("hide");
                calendar.getEventSources().forEach(function (source) {
                    source.refetch();
                });
                clearModalForm();
            },
            error: function (xhr, status, error) {
                console.error("Error saving event", xhr, status, error);
            }
        });


        function clearModalForm() {
            $('#newTitle').val('');
            $('#newDescription').val('');
            $('#newDate').val('');
            $('#newTime').val('');
            $('#newLocation').val('');
        }
    });
});