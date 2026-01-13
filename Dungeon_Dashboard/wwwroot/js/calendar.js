document.addEventListener('DOMContentLoaded', function () {
    function clearModalForm() {
        $('#newTitle').val('');
        $('#newDescription').val('');
        $('#newDateTime').val('');
        $('#newLocation').val('');
    }

    const calendarEl = document.getElementById('calendar');
    const calendar = new FullCalendar.Calendar(calendarEl, {
        timezone: 'local',
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

            var eventDate = info.event.start;
            var year = eventDate.getFullYear();
            var month = String(eventDate.getMonth() + 1).padStart(2, '0');
            var day = String(eventDate.getDate()).padStart(2, '0');
            var hours = String(eventDate.getHours()).padStart(2, '0');
            var minutes = String(eventDate.getMinutes()).padStart(2, '0');

            $('#date').val(`${year}-${month}-${day}`);
            $('#time').val(`${hours}:${minutes}`);

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
    
    $('.btn-close-modal').on('click', function () {
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
            location: $("#newLocation").val(),
            start: $("#newDate").val() + "T" + $("#newTime").val() + ":00"
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
                $("#btn-close-modal").click();
                console.log($(".close"));
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