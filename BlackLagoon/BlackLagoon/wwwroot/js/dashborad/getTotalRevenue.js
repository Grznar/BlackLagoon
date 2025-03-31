$(document).ready(function () {
    loadRevenueRadialChart();
});

function loadRevenueRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetRevenue", // URL pro Revenue
        type: "GET",
        dataType: "json",
        success: function (data) {
            $("#spanTotalRevenueCount").html(data.totalCount);
            let sectionCurrentCount = $("<span></span>");
            if (data.hasRatioIncreased) {
                sectionCurrentCount.addClass("text-success me-1")
                    .html('<i class="bi bi-arrow-up-right-circle me-1"></i><span>' + data.countInCurrentMotnth + '</span>');
            } else {
                sectionCurrentCount.addClass("text-danger me-1")
                    .html('<i class="bi bi-arrow-down-right-circle me-1"></i><span>' + data.countInCurrentMotnth + '</span>');
            }
            $("#sectionRevenueCount").html(sectionCurrentCount).append(" since last month");
            loadRadialBarChart("totalRevenueRadialChart", data);
            $(".chart-spinner").hide();
        }
    });
}
