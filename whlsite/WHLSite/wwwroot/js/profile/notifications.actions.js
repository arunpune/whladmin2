$(function () {

  $('.whl-action-notification-action').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const data = {
      "NotificationId": $(this).data('notificationid'),
      "Action": $(this).data('notificationaction')
    }
    $.post(updateNotificationUrl, data)
      .then(function (r) {
        window.location.reload();
      })
      .fail(function (error) {
        console.error(error);
      });
  });

});