let charts = {};

const vesselTypeColorMap = {
    'WIG': 'rgba(255, 99, 132, 0.7)',
    'Fishing': 'rgba(54, 162, 235, 0.7)',
    'HSC': 'rgba(255, 206, 86, 0.7)',
    'Other': 'rgba(75, 192, 192, 0.7)',
    'Passenger': 'rgba(153, 102, 255, 0.7)',
    'Cargo': 'rgba(255, 159, 64, 0.7)',
    'Tanker': 'rgba(199, 199, 199, 0.7)',
    'Sailing': 'rgba(100, 100, 255, 0.7)',
    'Tug': 'rgba(255, 100, 100, 0.7)'
};

function getVesselTypeColor(vesselType) {
    return vesselTypeColorMap[vesselType] || `rgba(${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, 0.7)`;
}

export function createOrUpdatePolarAreaChart(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error(`Canvas with id ${canvasId} not found.`);
        return;
    }

    const backgroundColors = labels.map(label => getVesselTypeColor(label));

    if (charts[canvasId]) {
        charts[canvasId].data.labels = labels;
        charts[canvasId].data.datasets[0].data = data;
        charts[canvasId].data.datasets[0].backgroundColor = backgroundColors;
        charts[canvasId].update();
    } else {
        const chartData = {
            labels: labels,
            datasets: [{
                label: 'Vessel Count by Type',
                data: data,
                backgroundColor: backgroundColors,
                borderWidth: 1
            }]
        };

        charts[canvasId] = new Chart(ctx, {
            type: 'polarArea',
            data: chartData,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        pointLabels: {
                            display: true,
                            centerPointLabels: true,
                            font: {
                                size: 14
                            }
                        }
                    }
                },
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Vessel Count by Type'
                    }
                },
                animation: {
                    duration: 100, // Faster animation
                    easing: 'easeOutQuart',
                    animateRotate: false,
                    animateScale: true
                }
            }
        });
    }
}

export function destroyChart(canvasId) {
    if (charts[canvasId]) {
        charts[canvasId].destroy();
        delete charts[canvasId];
    }
}
