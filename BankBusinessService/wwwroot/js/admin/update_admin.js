// Changes the admin profile display based on current database data.
function updateAdminProfileDisplay() {
    $.ajax({
        url: `/api/BProfile/retrieve/admin`,
        method: 'GET',
        success: function (data) {
            $('#profile-name').text(data.name);
            $('#profile-email').attr('href', 'mailto:' + data.email).text(data.email);
            $('#profile-phone').text(data.phone);

            $('#updated-name').text(data.name);
            $('#updated-email').text(data.email);
            $('#updated-phone').text(data.phone);

            retrieveProfilesList();
        },
        error: function (error) {
            console.error('Error fetching profile:', error);
        }
    });

}