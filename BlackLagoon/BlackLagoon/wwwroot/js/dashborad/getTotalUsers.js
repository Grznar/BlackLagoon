$(document).ready(function () {
    loadRegisteredUsersRadialChart();
});

function loadRegisteredUsersRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetRegisteredUsersRadialChartData", // URL pro Registered Users
        type: "GET",
        dataType: "json",
        success: function (data) {
            $("#spanTotalUserCount").html(data.totalCount);
            let sectionCurrentCount = $("<span></span>");
            if (data.hasRatioIncreased) {
                sectionCurrentCount.addClass("text-success me-1")
                    .html('<i class="bi bi-arrow-up-right-circle me-1"></i><span>' + data.countInCurrentMotnth + '</span>');
            } else {
                sectionCurrentCount.addClass("text-danger me-1")
                    .html('<i class="bi bi-arrow-down-right-circle me-1"></i><span>' + data.countInCurrentMotnth + '</span>');
            }
            $("#sectionUserCount").html(sectionCurrentCount).append(" since last month");
            loadRadialBarChart("totalUserRadialChart", data);
            $(".chart-spinner").hide();
        }
    });
}
