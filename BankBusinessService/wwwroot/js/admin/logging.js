// Function to retrieve activity logs from the server
function retrieveActivityLogs() {
    $.ajax({
        url: `/api/blog/all`,
        method: 'GET',
        success: function (data) {
            displayActivityLogs(data); // Display the logs on the page
        },
        error: function (error) {
            console.error('Error fetching activity logs:', error);
            alert('Error fetching activity logs. Please try again.');
        }
    });
}

// Function to display activity logs dynamically
function displayActivityLogs(logs) {
    let logsTable = $('#activityLogsTable tbody');
    logsTable.empty();

    logs.forEach(log => {
        // Ensure the timestamp is formatted correctly for display
        const formattedTimestamp = log.timestamp ? new Date(log.timestamp).toLocaleString('en-GB') : 'N/A';

        logsTable.append(`
                    <tr>
                        <td>${log.username}</td>
                        <td>${log.action}</td>
                        <td>${log.details}</td>
                        <td>${formattedTimestamp}</td>
                    </tr>
                `);
    });
}