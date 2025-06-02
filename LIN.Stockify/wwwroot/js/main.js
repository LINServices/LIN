function backLast() {
    history.back();
}
function forceClick(id) {

    const control = document.getElementById(id);
    control.click();
}

function showToast(id) {
    const toastEl = document.getElementById(id);
    toastEl.classList.remove('hidden');
    toastEl.classList.add('flex');

    setTimeout(() => {
        toastEl.classList.add('hidden');
        toastEl.classList.remove('flex');
    }, 5000);
}


function loadChart(dates, numbers) {
    let options = {
        chart: {
            height: "100%",
            maxWidth: "100%",
            type: "area",
            fontFamily: "Inter, sans-serif",
            dropShadow: {
                enabled: false,
            },
            toolbar: {
                show: false,
            },
        },
        tooltip: {
            enabled: true,
            x: {
                show: false,
            },
        },
        fill: {
            type: "gradient",
            gradient: {
                opacityFrom: 0.55,
                opacityTo: 0,
                shade: "#6e964c",
                gradientToColors: ["#6e964c"],
            },
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            width: 6,
        },
        grid: {
            show: false,
            strokeDashArray: 4,
            padding: {
                left: 2,
                right: 2,
                top: 0
            },
        },
        series: [
            {
                name: "Ganancias",
                data: numbers,
                color: "#6e964c",
            },
        ],
        xaxis: {
            categories: dates,
            labels: {
                show: false,
            },
            axisBorder: {
                show: false,
            },
            axisTicks: {
                show: false,
            },
        },
        yaxis: {
            show: false,
        },
    }

    if (document.getElementById("area-chart") && typeof ApexCharts !== 'undefined') {
        document.getElementById("area-chart").innerHTML = "";
        const chart = new ApexCharts(document.getElementById("area-chart"), options);
        chart.render();
    }
}
