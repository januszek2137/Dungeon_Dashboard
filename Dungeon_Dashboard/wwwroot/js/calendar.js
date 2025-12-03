document.addEventListener('DOMContentLoaded', function () {
    function clearModalForm() {
        $('#newTitle').val('');
        $('#newDescription').val('');
        $('#newDateTime').val('');
        $('#newLocation').val('');
    }

    const calendarEl = document.getElementById('calendar');
    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        eventSources: [
            {
                url: '/api/eventmodels',
                failure: function (error) {
                    console.error('Failed to load events:', error);
                    alert('There was an error while fetching events!');
                },
                success: function(data) {
                    console.log('✅ Events loaded:', data);
                }
            }
        ],
        eventClick: function (info) {
            $('#title').val(info.event.title);
            $('#description').val(info.event.extendedProps.description);
            const datetime = info.event.start.toISOString().substring(0, 16);
            $('#datetime').val(datetime);
            $('#location').val(info.event.extendedProps.location);
            $('#detailsModal').modal('show');
        },
        eventDidMount: function(info) {
            console.log('Event rendered:', info.event.title);
        }
    });
    calendar.render();

    $("button[data-target='#newEventModal']").on('click', function () {
        clearModalForm();
        $("#newEventModal").modal('show');
    });

    $('.close').on('click', function () {
        $('#detailsModal').modal('hide');
        $('#newEventModal').modal('hide');
        clearModalForm();
    });

    $("#newEventForm").on('submit', function (e) {
        e.preventDefault();
        console.log("📝 Submitting form");

        var datetimeValue = $("#newDateTime").val();
        console.log("📅 DateTime from input:", datetimeValue);

        if (!datetimeValue) {
            alert("Please select date and time!");
            return;
        }

        var eventData = {
            title: $("#newTitle").val(),
            description: $("#newDescription").val(),
            start: datetimeValue,  // ✅ zmienione z dateTime na start
            location: $("#newLocation").val()
        };

        console.log("📤 Sending to API:", JSON.stringify(eventData, null, 2));

        $.ajax({
            url: '/api/eventmodels',
            type: 'POST',
            data: JSON.stringify(eventData),
            contentType: 'application/json',
            success: function (data) {
                console.log("✅ Success response:", data);

                // Dodaj event do kalendarza
                calendar.addEvent({
                    id: data.id,
                    title: data.title,
                    start: data.start,
                    extendedProps: {
                        description: data.description,
                        location: data.location
                    }
                });

                $("#newEventModal").modal('hide');
                clearModalForm();

                alert("Event added successfully!");
            },
            error: function (xhr, status, error) {
                console.error("❌ Error details:", {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    response: xhr.responseText
                });
                alert("Error saving event: " + (xhr.responseJSON?.error || error));
            }
        });
    });
});