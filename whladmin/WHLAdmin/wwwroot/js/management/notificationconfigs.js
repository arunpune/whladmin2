$(function () {

  $('#notifications-tab-content li:first-child button').tab('show');

  $('.whl-action-add-notification').on('click', function (e) {
    e.preventDefault();
    $.get(notificationConfigAddUrl)
      .done(function (response) {
        $('#whl-title-notification-action').html('Add Notification');
        $('#hidNotificationAction').val('ADD');
        $('#notificationEditorModal').find('div.modal-body').html(response);
        $('#notificationEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-notification').on('click', function (e) {
    e.preventDefault();
    const notificationId = $(e.currentTarget).data('notificationid');
    $.get(notificationConfigEditUrl + '?notificationId=' + notificationId)
      .done(function (response) {
        $('#whl-title-notification-action').html('Edit Notification');
        $('#hidNotificationAction').val('EDIT');
        $('#notificationEditorModal').find('div.modal-body').html(response);
        $('#notificationEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-notification').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidNotificationAction').val();
    const notificationTitle = $('#Title').val();
    const url = a === 'EDIT' ? notificationConfigEditUrl : notificationConfigAddUrl;
    const data = {
      NotificationId: $('#NotificationId').val(),
      CategoryCd: $('#CategoryCd').val(),
      Title: $('#Title').val(),
      Text: $('#Text').val(),
      FrequencyCd: $('#FrequencyCd').val(),
      FrequencyInterval: $('#FrequencyInterval').val(),
      NotificationList: $('#NotificationList').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#notificationEditorModal').find('div.modal-body').html('');
        $('#notificationEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Notification ' + notificationTitle + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save notification information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-notification').on('click', function (e) {
    e.preventDefault();
    const notificationId = $(e.currentTarget).data('notificationid');
    const notificationTitle = $(e.currentTarget).data('notificationtitle');
    if (confirm('Are you sure you want to delete notification - ' + notificationTitle + '?')) {
      $.post(notificationConfigDeleteUrl + '?notificationId=' + notificationId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Notification ' + faqTitle + ' deleted, refreshing page.');
        });
    }
  });

});