$(document).ready(function () {
    loadRegisteredUsersRadialChart();
});

function loadRegisteredUsersRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetBookingPieChartData", // URL pro Registered Users
        type: "GET",
        dataType: "json",
        success: function (data) {
            
            loadPieChart("customerBookingsPieChart", data);
            $(".chart-spinner").hide();
        }
    });
}

function loadPieChart(id, data) {
    let chartColors = getChartColorsArray(id);
    
    var options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: 'pie',
            width: 380,
        }
    };




    
    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();

}