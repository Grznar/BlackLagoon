let dataTable;
$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get("status");
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/booking/getall?status="+status
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "name", "width": "15%" },
            { "data": "phone", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "status", "width": "10%" },           // Booking status
            { "data": "checkInDate", "width": "10%" },
            { "data": "nights", "width": "10%" },
            { "data": "totalCost", "width": "10%" },
            { "data": "status", "width": "10%" },      // Payment status
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Booking/bookingDetails?bookingId=${data}" class="btn btn-outline-warning mx-2 text-white" style="cursor:pointer; width:100px;">
                                <i class="fas fa-edit"></i> Details
                            </a>
                        </div>
                    `;
                },
                "width": "10%"
            }
        ]
    });
}
